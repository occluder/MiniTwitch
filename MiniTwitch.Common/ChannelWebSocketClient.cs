using System.Buffers;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Channels;
using Microsoft.Extensions.Logging;

namespace MiniTwitch.Common;

/// <summary>
/// A pooled array together with the number of valid bytes it contains.
/// <see cref="ArrayPool{T}"/> may return an array larger than requested, so the
/// logical length has to travel with the buffer. Using <c>byte[].Length</c>
/// instead would expose stale data left in the tail of the pooled array.
/// </summary>
readonly struct PooledBuffer(byte[] buffer, int length)
{
    public readonly byte[] Buffer = buffer;
    public readonly int Length = length;
    public ReadOnlyMemory<byte> Memory => Buffer.AsMemory(0, Length);
}

public sealed class ChannelWebSocketClient : IAsyncDisposable
{
    readonly struct OutboundItem(ReadOnlyMemory<byte> data, string? nonce, TaskCompletionSource<byte[]>? tcs)
    {
        public readonly ReadOnlyMemory<byte> Data = data;
        public readonly string? Nonce = nonce;
        public readonly TaskCompletionSource<byte[]>? Tcs = tcs;
    }

    readonly struct PendingRequest(byte[] nonceBytes, TaskCompletionSource<byte[]> tcs)
    {
        public readonly byte[] NonceBytes = nonceBytes;
        public readonly TaskCompletionSource<byte[]> Tcs = tcs;
    }

    public bool IsConnected => _ws is not null && _ws.State == WebSocketState.Open;
    public ChannelReader<ReadOnlyMemory<byte>> InboundReader => _broadcastReader;
    public ChannelWriter<ReadOnlyMemory<byte>> Writer { get; }
    public ChannelReader<ConnectionState> StateReader { get; }

    public event Action<LogLevel, string, object[]>? OnLog;
    public event Action<Exception, string, object[]>? OnLogEx;

    const int BroadcastCapacity = 256;
    const int OutboundCapacity = 256;
    const int StateCapacity = 1;
    const int RawInboundCapacity = 64;
    const int InitialReceiveBufferSize = 1024 * 64;
    const int MaxReceiveBufferSize = 1024 * 1024 * 4;

    readonly TimeSpan _reconnectDelay;
    readonly SemaphoreSlim _reconnectionLock = new(0);
    readonly ConcurrentDictionary<string, PendingRequest> _pending = new();
    readonly Channel<ConnectionState> _connectionState;
    readonly Channel<PooledBuffer> _broadcast;
    readonly BroadcastReader _broadcastReader;
    readonly Channel<OutboundItem> _outbound;
    readonly FireForgetWriter _writer;
    readonly object _generationLock = new();
    CancellationTokenSource _cts = new();
    ClientWebSocket _ws = new();
    Uri _uri = default!;
    Channel<PooledBuffer> _rawInbound = default!;
    Task _receiveTask = Task.CompletedTask;
    Task _dispatchTask = Task.CompletedTask;
    Task _sendTask = Task.CompletedTask;
    bool _reconnecting;
    volatile bool _disconnected;
    volatile bool _disposed;

    public ChannelWebSocketClient(TimeSpan reconnectionDelay)
    {
        _reconnectDelay = reconnectionDelay;
        // Wait + TryWrite (rather than DropOldest) so that a full channel reports the
        // overflow back to the writer, which can then return the pooled buffer instead
        // of silently leaking it out of the array pool.
        _broadcast = Channel.CreateBounded<PooledBuffer>(new BoundedChannelOptions(BroadcastCapacity)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleWriter = false,
            SingleReader = true,
            AllowSynchronousContinuations = true,
        });
        _broadcastReader = new BroadcastReader(_broadcast.Reader);
        _outbound = Channel.CreateBounded<OutboundItem>(new BoundedChannelOptions(OutboundCapacity)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleWriter = false,
            SingleReader = true,
        });
        _connectionState = Channel.CreateBounded<ConnectionState>(new BoundedChannelOptions(StateCapacity)
        {
            FullMode = BoundedChannelFullMode.DropOldest,
            SingleWriter = false,
            SingleReader = false,
        });
        _writer = new FireForgetWriter(_outbound.Writer);
        Writer = _writer;
        StateReader = _connectionState.Reader;
        InitRawInbound();
    }

    void InitRawInbound()
    {
        _rawInbound = Channel.CreateBounded<PooledBuffer>(new BoundedChannelOptions(RawInboundCapacity)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleWriter = false,
            SingleReader = true,
        });
    }

    /// <summary>
    /// Connects to <paramref name="uri"/> and starts the receive, dispatch and send loops.
    /// May be called again after <see cref="Disconnect"/> to reconnect; the persistent
    /// <see cref="InboundReader"/>, <see cref="StateReader"/> and <see cref="Writer"/> stay
    /// alive across the cycle. Only <see cref="DisposeAsync"/> makes the instance unusable.
    /// </summary>
    public async Task Start(Uri uri, CancellationToken ct = default)
    {
        if (_disposed)
        {
            Log(LogLevel.Warning, "Cannot start a disposed WebSocket client.");
            return;
        }

        // An explicit Start clears a previous Disconnect. The internal reconnect path
        // (Restart) calls StartCore directly so it cannot undo a user Disconnect.
        _disconnected = false;
        await StartCore(uri, ct).ConfigureAwait(false);
    }

    async Task StartCore(Uri uri, CancellationToken ct = default)
    {
        // A ClientWebSocket cannot be reused once it is Closed/Aborted: Dispose leaves it
        // Closed (not Aborted) and a failed connect leaves it Closed too, so a fresh
        // instance is needed to reconnect.
        if (_ws.State is WebSocketState.Closed or WebSocketState.Aborted)
        {
            _ws.Dispose();
            _ws = new ClientWebSocket();
        }

        _uri = uri;
        Log(LogLevel.Debug, "Connecting to {Uri} ...", uri);

        try
        {
            await _ws.ConnectAsync(uri, ct).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            LogException(ex, "WebSocket failed to connect.");
            return;
        }

        // A Disconnect/Dispose may have landed while connecting; it snapshots the
        // generation while holding this same lock, so the check and the spawn must be
        // atomic — otherwise a Disconnect could slip in between and leave the freshly
        // spawned loops running on a token nothing cancels.
        lock (_generationLock)
        {
            if (_disconnected || _disposed)
            {
                return;
            }

            if (_cts.IsCancellationRequested)
            {
                _cts = new();
            }

            // Always start on a fresh raw inbound channel. A completed channel can still
            // hold an undrained message, in which case its Completion stays incomplete;
            // reusing it would leave a new receive loop writing into a closed writer.
            InitRawInbound();

            // StartNew over an async method returns Task<Task>; Unwrap so the stored tasks
            // track the loops themselves (needed to join them on restart/disposal).
            _receiveTask = Task.Factory.StartNew(ReceiveLoop, _cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default).Unwrap();
            _dispatchTask = Task.Factory.StartNew(DispatchLoop, _cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default).Unwrap();
            _sendTask = Task.Factory.StartNew(SendLoop, _cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default).Unwrap();
        }

        int attempts = 0;
        while (!this.IsConnected)
        {
            if (_disconnected || _disposed)
            {
                return;
            }

            if (attempts++ >= 20)
            {
                Log(LogLevel.Error, "WebSocket connection timed out.");
                return;
            }

            await Task.Delay(250, ct).ConfigureAwait(false);
        }

        if (_reconnecting)
        {
            _ = _reconnectionLock.Release();
        }

        _connectionState.Writer.TryWrite(ConnectionState.Connected);
    }

    /// <summary>
    /// Gracefully closes the current connection and stops the loops. The client remains
    /// reusable: call <see cref="Start"/> again to reconnect. The persistent
    /// <see cref="InboundReader"/>, <see cref="StateReader"/> and <see cref="Writer"/> are
    /// left open; use <see cref="DisposeAsync"/> to tear them down for good.
    /// </summary>
    public async Task Disconnect(CancellationToken ct = default)
    {
        _disconnected = true;

        // Capture the generation being stopped under the lock so a concurrent StartCore
        // cannot spawn a replacement we would miss, and so we never await a newer
        // generation's loops, which nothing here cancels.
        Task dispatch;
        Task send;
        CancellationTokenSource cts;
        lock (_generationLock)
        {
            _disconnected = true;
            dispatch = _dispatchTask;
            send = _sendTask;
            cts = _cts;
        }

        cts.Cancel();

        if (this.IsConnected)
        {
            WebSocketCloseStatus status = _ws.CloseStatus ?? WebSocketCloseStatus.NormalClosure;
            string? description = _ws.CloseStatusDescription;
            try
            {
                await _ws.CloseAsync(status, description, ct).ConfigureAwait(false);
            }
            catch
            {
            }
        }

        _ws.Dispose();

        foreach (var (_, req) in _pending)
        {
            req.Tcs.TrySetCanceled(ct);
        }

        _pending.Clear();

        _rawInbound.Writer.TryComplete();

        // The receive loop observes cancellation and exits without restarting (its
        // captured token is cancelled), but the dispatch and send loops must be joined
        // here so a later Start cannot run a second generation against the persistent
        // channels (in particular the single-reader outbound channel).
        try
        {
            await Task.WhenAll(dispatch, send).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception ex)
        {
            LogException(ex, "Dispatch or send loop faulted while disconnecting.");
        }

        _connectionState.Writer.TryWrite(ConnectionState.Disconnected);
    }

    async Task Restart(TimeSpan delay, CancellationToken ct = default)
    {
        // Capture the generation being restarted under the lock so a concurrent Start
        // cannot swap it out from under the join below.
        Task dispatch;
        Task send;
        CancellationTokenSource cts;
        lock (_generationLock)
        {
            if (_reconnecting || _disconnected || _disposed)
            {
                return;
            }

            _reconnecting = true;
            dispatch = _dispatchTask;
            send = _sendTask;
            cts = _cts;
        }

        _connectionState.Writer.TryWrite(ConnectionState.Reconnecting);

        Log(LogLevel.Critical, "The WebSocket client is restarting in {Delay}", delay);

        cts.Cancel();

        foreach (var (_, req) in _pending)
        {
            req.Tcs.TrySetCanceled(ct);
        }

        _pending.Clear();

        // The previous receive loop is the caller, but the dispatch and send loops
        // must be allowed to observe the cancellation and stop before a new
        // generation is started. Otherwise both generations could read from the
        // freshly created raw inbound channel and process the same message twice.
        try
        {
            await Task.WhenAll(dispatch, send).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception ex)
        {
            LogException(ex, "Dispatch or send loop faulted while restarting.");
        }

        _rawInbound.Writer.TryComplete();

        _ws.Dispose();
        _ws = new ClientWebSocket();

        await Task.Delay(_reconnectDelay, ct).ConfigureAwait(false);
        Log(LogLevel.Debug, "Finished waiting for reconnection delay");

        // A Disconnect/Dispose may have happened while waiting; don't resurrect.
        if (_disconnected || _disposed)
        {
            _reconnecting = false;
            return;
        }

        if (_cts.IsCancellationRequested)
        {
            _cts = new();
        }

        await StartCore(_uri, ct).ConfigureAwait(false);
        Log(LogLevel.Trace, "If the WebSocket doesn't reconnect in 10 seconds you will see a warning");

        // StartCore bails (without spawning loops) when a Disconnect/Dispose landed while
        // connecting; don't wait on a reconnection that will not come.
        if (_disconnected || _disposed)
        {
            _reconnecting = false;
            return;
        }

        if (!await _reconnectionLock.WaitAsync(TimeSpan.FromSeconds(10), ct).ConfigureAwait(false))
        {
            Log(LogLevel.Warning, "WebSocket reconnect failed. Retrying...");
            _reconnecting = false;
            await Restart(_reconnectDelay, ct).ConfigureAwait(false);
            return;
        }

        Log(LogLevel.Information, "Successfully reconnected!");
        _reconnecting = false;
        _connectionState.Writer.TryWrite(ConnectionState.Connected);
    }

    public async ValueTask<ReadOnlyMemory<byte>> SendAndWaitAsync(
        ReadOnlyMemory<byte> data,
        string nonce,
        TimeSpan? timeout = null,
        CancellationToken ct = default)
    {
        if (!this.IsConnected)
        {
            Log(LogLevel.Warning, "Cannot send data in non-connected state. ({State})", _ws.State);
            return ReadOnlyMemory<byte>.Empty;
        }

        var tcs = new TaskCompletionSource<byte[]>(TaskCreationOptions.RunContinuationsAsynchronously);
        var nonceBytes = Encoding.UTF8.GetBytes(nonce);

        // Reject a nonce that is already in flight. Overwriting the previous entry
        // would strand its TaskCompletionSource until it timed out.
        if (!_pending.TryAdd(nonce, new PendingRequest(nonceBytes, tcs)))
        {
            throw new InvalidOperationException($"A request with nonce '{nonce}' is already pending.");
        }

        TimeSpan effectiveTimeout = timeout ?? TimeSpan.FromSeconds(30);
        using var timeoutCts = new CancellationTokenSource(effectiveTimeout);
        using var linked = CancellationTokenSource.CreateLinkedTokenSource(ct, timeoutCts.Token);

        try
        {
            await _outbound.Writer.WriteAsync(new OutboundItem(data, nonce, tcs), linked.Token).ConfigureAwait(false);

            byte[] result = await tcs.Task.WaitAsync(linked.Token).ConfigureAwait(false);
            return result;
        }
        catch (OperationCanceledException) when (!ct.IsCancellationRequested)
        {
            if (_pending.TryRemove(nonce, out var req))
            {
                req.Tcs.TrySetCanceled(linked.Token);
            }

            throw new TimeoutException($"Request with nonce '{nonce}' timed out after {effectiveTimeout}.");
        }
        catch (OperationCanceledException)
        {
            _pending.TryRemove(nonce, out _);
            throw;
        }
        catch
        {
            // The enqueue failed (or the response task faulted); don't leave a stale entry behind.
            _pending.TryRemove(nonce, out _);
            throw;
        }
    }

    public async ValueTask<ReadOnlyMemory<byte>> SendAndWaitAsync(
        string data,
        string nonce,
        TimeSpan? timeout = null,
        CancellationToken ct = default)
    {
        return await SendAndWaitAsync(Encoding.UTF8.GetBytes(data).AsMemory(), nonce, timeout, ct).ConfigureAwait(false);
    }

    async Task ReceiveLoop()
    {
        // Capture the generation up front so a Start that replaces _cts/_ws/_rawInbound
        // cannot make this loop keep running or enqueue into a newer channel.
        CancellationTokenSource cts = _cts;
        ChannelWriter<PooledBuffer> inbound = _rawInbound.Writer;
        var ws = _ws;
        byte[] buffer = ArrayPool<byte>.Shared.Rent(InitialReceiveBufferSize);
        int written = 0;

        try
        {
            while (this.IsConnected && !cts.IsCancellationRequested)
            {
                try
                {
                    // A message that outgrows the buffer (ArrayPool may return more than
                    // requested, but a fragment can still fill it) is grown rather than
                    // overflowing the slice and throwing.
                    if (written == buffer.Length)
                    {
                        if (buffer.Length >= MaxReceiveBufferSize)
                        {
                            Log(LogLevel.Error, "Incoming WebSocket message exceeded the {Max} byte limit; reconnecting.", MaxReceiveBufferSize);
                            break;
                        }

                        byte[] grown = ArrayPool<byte>.Shared.Rent(written * 2);
                        buffer.AsSpan(0, written).CopyTo(grown);
                        ArrayPool<byte>.Shared.Return(buffer);
                        buffer = grown;
                    }

                    ValueWebSocketReceiveResult result = await ws.ReceiveAsync(buffer.AsMemory(written), cts.Token).ConfigureAwait(false);
                    written += result.Count;

                    if (result.EndOfMessage)
                    {
                        CopyAndEnqueueMessage(inbound, buffer, ref written);
                    }
                }
                catch (WebSocketException wse)
                {
                    Log(LogLevel.Critical, "An error occurred while receiving data from the WebSocket connection: {Message}", wse.Message);
                    break;
                }
                catch (ObjectDisposedException)
                {
                    // The socket was disposed by a Disconnect/Restart while a receive was pending.
                    break;
                }
                catch (InvalidOperationException)
                {
                    Log(LogLevel.Warning, "Tried to receive data, but the WebSocket client is not connected");
                    break;
                }
                catch (OperationCanceledException) when (cts.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    LogException(ex, "Exception caught in data receiver: ");
                }
            }

            if (!cts.IsCancellationRequested)
            {
                await Restart(_reconnectDelay).ConfigureAwait(false);
            }
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }

    void CopyAndEnqueueMessage(ChannelWriter<PooledBuffer> inbound, byte[] buffer, ref int written)
    {
        int length = TrimTrailingNewline(buffer.AsSpan(0, written), written);
        byte[] copy = ArrayPool<byte>.Shared.Rent(length);
        buffer.AsSpan(0, length).CopyTo(copy);

        if (!inbound.TryWrite(new PooledBuffer(copy, length)))
        {
            ArrayPool<byte>.Shared.Return(copy);
        }

        written = 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static int TrimTrailingNewline(ReadOnlySpan<byte> span, int length)
    {
        const byte lf = (byte)'\n';
        if (length > 0 && span[length - 1] == lf)
        {
            return length - 1;
        }

        return length;
    }

    async Task DispatchLoop()
    {
        CancellationTokenSource cts = _cts;
        var inbound = _rawInbound;
        try
        {
            while (await inbound.Reader.WaitToReadAsync(cts.Token).ConfigureAwait(false))
            {
                while (inbound.Reader.TryRead(out PooledBuffer message))
                {
                    bool matched = false;

                    foreach (var (nonce, req) in _pending)
                    {
                        if (MatchNonce(message.Memory.Span, req.NonceBytes))
                        {
                            byte[] responseCopy = new byte[message.Length];
                            Buffer.BlockCopy(message.Buffer, 0, responseCopy, 0, message.Length);
                            ArrayPool<byte>.Shared.Return(message.Buffer);

                            if (req.Tcs.TrySetResult(responseCopy))
                            {
                                _pending.TryRemove(nonce, out _);
                            }

                            matched = true;
                            break;
                        }
                    }

                    if (!matched && !_broadcast.Writer.TryWrite(message))
                    {
                        // The broadcast channel is full; a blocking write here would stall
                        // the dispatch (and therefore the receive) loop, so drop and reclaim.
                        ArrayPool<byte>.Shared.Return(message.Buffer);
                    }
                }
            }
        }
        catch (OperationCanceledException) when (cts.IsCancellationRequested)
        {
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool MatchNonce(ReadOnlySpan<byte> message, ReadOnlySpan<byte> nonce) => message.IndexOf(nonce) >= 0;

    async Task SendLoop()
    {
        CancellationTokenSource cts = _cts;
        var ws = _ws;
        try
        {
            while (await _outbound.Reader.WaitToReadAsync(cts.Token).ConfigureAwait(false))
            {
                while (_outbound.Reader.TryRead(out OutboundItem item))
                {
                    if (!this.IsConnected)
                    {
                        item.Tcs?.TrySetException(new InvalidOperationException(
                            "Cannot send data while the WebSocket is not connected."));
                        continue;
                    }

                    try
                    {
                        await ws.SendAsync(item.Data, WebSocketMessageType.Text, true, cts.Token).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        item.Tcs?.TrySetException(ex);
                        LogException(ex, "Exception caught whilst trying to send data.");
                    }
                }
            }
        }
        catch (OperationCanceledException) when (cts.IsCancellationRequested)
        {
        }
    }

    sealed class FireForgetWriter(ChannelWriter<OutboundItem> inner) : ChannelWriter<ReadOnlyMemory<byte>>
    {
        public override bool TryWrite(ReadOnlyMemory<byte> item) => inner.TryWrite(new OutboundItem(item, null, null));
        public override ValueTask WriteAsync(ReadOnlyMemory<byte> item, CancellationToken ct = default) => inner.WriteAsync(new OutboundItem(item, null, null), ct);
        public override ValueTask<bool> WaitToWriteAsync(CancellationToken ct) => inner.WaitToWriteAsync(ct);
        public override bool TryComplete(Exception? error = null) => false;
    }

    void Log(LogLevel level, string template, params object[] properties)
        => OnLog?.Invoke(level, template, properties);

    void LogException(Exception ex, string template, params object[] properties)
        => OnLogEx?.Invoke(ex, template, properties);

    /// <summary>
    /// Terminates the client for good. Unlike <see cref="Disconnect"/>, the persistent
    /// channels are completed so consumers of <see cref="InboundReader"/> and
    /// <see cref="StateReader"/> finish, and the instance cannot be started again.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        lock (_generationLock)
        {
            _disposed = true;
        }

        await Disconnect().ConfigureAwait(false);

        _broadcast.Writer.TryComplete();
        _outbound.Writer.TryComplete();
        _connectionState.Writer.TryComplete();

        // Give the loops a moment to observe cancellation and stop using the client.
        await Task.WhenAny(
            Task.WhenAll(_receiveTask, _dispatchTask, _sendTask),
            Task.Delay(TimeSpan.FromSeconds(5))).ConfigureAwait(false);

        Log(LogLevel.Debug, "Disposed {Name}", nameof(ChannelWebSocketClient));
    }
}

public enum ConnectionState
{
    Disconnected,
    Connected,
    Reconnecting
}

internal sealed class BroadcastReader(ChannelReader<PooledBuffer> inner) : ChannelReader<ReadOnlyMemory<byte>>
{
    public override Task Completion => inner.Completion;

    PooledBuffer _previous;

    public override bool TryRead(out ReadOnlyMemory<byte> item)
    {
        ReturnPrevious();
        if (inner.TryRead(out PooledBuffer buffer))
        {
            _previous = buffer;
            item = buffer.Memory;
            return true;
        }

        item = default;
        return false;
    }

    public override ValueTask<bool> WaitToReadAsync(CancellationToken ct = default)
        => inner.WaitToReadAsync(ct);

    public override async ValueTask<ReadOnlyMemory<byte>> ReadAsync(CancellationToken ct = default)
    {
        ReturnPrevious();
        PooledBuffer buffer = await inner.ReadAsync(ct).ConfigureAwait(false);
        _previous = buffer;
        return buffer.Memory;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void ReturnPrevious()
    {
        if (_previous.Buffer is not null)
        {
            ArrayPool<byte>.Shared.Return(_previous.Buffer);
            _previous = default;
        }
    }
}
