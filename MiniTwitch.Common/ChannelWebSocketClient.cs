using System.Buffers;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Channels;
using Microsoft.Extensions.Logging;

namespace MiniTwitch.Common;

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

    readonly TimeSpan _reconnectDelay;
    readonly SemaphoreSlim _reconnectionLock = new(0);
    readonly ConcurrentDictionary<string, PendingRequest> _pending = new();
    readonly Channel<ConnectionState> _connectionState;
    readonly Channel<byte[]> _broadcast;
    readonly BroadcastReader _broadcastReader;
    readonly Channel<OutboundItem> _outbound;
    readonly FireForgetWriter _writer;
    CancellationTokenSource _cts = new();
    ClientWebSocket _ws = new();
    Uri _uri = default!;
    Channel<byte[]> _rawInbound = default!;
    Task _receiveTask = Task.CompletedTask;
    Task _dispatchTask = Task.CompletedTask;
    Task _sendTask = Task.CompletedTask;
    bool _reconnecting;

    public ChannelWebSocketClient(TimeSpan reconnectionDelay)
    {
        _reconnectDelay = reconnectionDelay;
        _broadcast = Channel.CreateBounded<byte[]>(new BoundedChannelOptions(BroadcastCapacity)
        {
            FullMode = BoundedChannelFullMode.DropOldest,
            SingleWriter = true,
            SingleReader = true,
            AllowSynchronousContinuations = true,
        });
        _broadcastReader = new BroadcastReader(_broadcast.Reader);
        _outbound = Channel.CreateBounded<OutboundItem>(new BoundedChannelOptions(OutboundCapacity)
        {
            FullMode = BoundedChannelFullMode.DropOldest,
            SingleWriter = false,
            SingleReader = true,
        });
        _connectionState = Channel.CreateBounded<ConnectionState>(new BoundedChannelOptions(StateCapacity)
        {
            FullMode = BoundedChannelFullMode.DropOldest,
            SingleWriter = true,
            SingleReader = false,
        });
        _writer = new FireForgetWriter(_outbound.Writer);
        Writer = _writer;
        StateReader = _connectionState.Reader;
        InitRawInbound();
    }

    void InitRawInbound()
    {
        _rawInbound = Channel.CreateBounded<byte[]>(new BoundedChannelOptions(RawInboundCapacity)
        {
            FullMode = BoundedChannelFullMode.DropOldest,
            SingleWriter = true,
            SingleReader = true,
        });
    }

    public async Task Start(Uri uri, CancellationToken ct = default)
    {
        if (_ws.State == WebSocketState.Aborted)
        {
            Log(LogLevel.Error, "Cannot start WebSocket in aborted state");
            return;
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

        if (_cts.IsCancellationRequested)
        {
            _cts = new();
        }

        _receiveTask = Task.Factory.StartNew(ReceiveLoop, _cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        _dispatchTask = Task.Factory.StartNew(DispatchLoop, _cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        _sendTask = Task.Factory.StartNew(SendLoop, _cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        int attempts = 0;
        while (!this.IsConnected)
        {
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

    public async Task Disconnect(CancellationToken ct = default)
    {
        _cts.Cancel();

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
        _broadcast.Writer.TryComplete();
        _outbound.Writer.TryComplete();
        _connectionState.Writer.TryWrite(ConnectionState.Disconnected);
        _connectionState.Writer.TryComplete();
    }

    async Task Restart(TimeSpan delay, CancellationToken ct = default)
    {
        if (_reconnecting)
        {
            return;
        }

        _reconnecting = true;
        _connectionState.Writer.TryWrite(ConnectionState.Reconnecting);

        Log(LogLevel.Critical, "The WebSocket client is restarting in {Delay}", delay);

        _cts.Cancel();

        foreach (var (_, req) in _pending)
        {
            req.Tcs.TrySetCanceled(ct);
        }

        _pending.Clear();

        _rawInbound.Writer.TryComplete();

        _ws.Dispose();
        _ws = new ClientWebSocket();
        InitRawInbound();

        await Task.Delay(_reconnectDelay, ct).ConfigureAwait(false);
        Log(LogLevel.Debug, "Finished waiting for reconnection delay");

        if (_cts.IsCancellationRequested)
        {
            _cts = new();
        }

        await Start(_uri, ct).ConfigureAwait(false);
        Log(LogLevel.Trace, "If the WebSocket doesn't reconnect in 10 seconds you will see a warning");

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
        _pending[nonce] = new PendingRequest(nonceBytes, tcs);

        await _outbound.Writer.WriteAsync(new OutboundItem(data, nonce, tcs), ct).ConfigureAwait(false);

        TimeSpan effectiveTimeout = timeout ?? TimeSpan.FromSeconds(30);
        using var timeoutCts = new CancellationTokenSource(effectiveTimeout);
        using var linked = CancellationTokenSource.CreateLinkedTokenSource(ct, timeoutCts.Token);

        try
        {
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
        using var buffer = MemoryPool<byte>.Shared.Rent(1024 * 32);
        int written = 0;

        while (this.IsConnected && !_cts.IsCancellationRequested)
        {
            try
            {
                ValueWebSocketReceiveResult result = await _ws.ReceiveAsync(buffer.Memory[written..], _cts.Token).ConfigureAwait(false);
                written += result.Count;

                if (result.EndOfMessage)
                {
                    CopyAndEnqueueMessage(buffer.Memory, ref written);
                }
            }
            catch (WebSocketException wse)
            {
                Log(LogLevel.Critical, "An error occurred while receiving data from the WebSocket connection: {Message}", wse.Message);
                break;
            }
            catch (InvalidOperationException)
            {
                Log(LogLevel.Warning, "Tried to receive data, but the WebSocket client is not connected");
                break;
            }
            catch (OperationCanceledException) when (_cts.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                LogException(ex, "Exception caught in data receiver: ");
            }
        }

        if (!_cts.IsCancellationRequested)
        {
            await Restart(_reconnectDelay).ConfigureAwait(false);
        }
    }

    void CopyAndEnqueueMessage(Memory<byte> bufferMemory, ref int written)
    {
        int length = TrimTrailingNewline(bufferMemory.Span, written);
        byte[] copy = ArrayPool<byte>.Shared.Rent(length);
        bufferMemory[..length].CopyTo(copy);

        if (!_rawInbound.Writer.TryWrite(copy))
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
        try
        {
            while (await _rawInbound.Reader.WaitToReadAsync(_cts.Token).ConfigureAwait(false))
            {
                while (_rawInbound.Reader.TryRead(out byte[]? message))
                {
                    bool matched = false;

                    foreach (var (nonce, req) in _pending)
                    {
                        if (MatchNonce(message, req))
                        {
                            byte[] responseCopy = new byte[message.Length];
                            Buffer.BlockCopy(message, 0, responseCopy, 0, message.Length);
                            ArrayPool<byte>.Shared.Return(message);

                            if (req.Tcs.TrySetResult(responseCopy))
                            {
                                _pending.TryRemove(nonce, out _);
                            }

                            matched = true;
                            break;
                        }
                    }

                    if (!matched)
                    {
                        try
                        {
                            await _broadcast.Writer.WriteAsync(message, _cts.Token).ConfigureAwait(false);
                        }
                        catch (OperationCanceledException) when (_cts.IsCancellationRequested)
                        {
                            ArrayPool<byte>.Shared.Return(message);
                            return;
                        }
                        catch (ChannelClosedException)
                        {
                            ArrayPool<byte>.Shared.Return(message);
                            return;
                        }
                    }
                }
            }
        }
        catch (OperationCanceledException) when (_cts.IsCancellationRequested)
        {
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool MatchNonce(byte[] message, PendingRequest req) => message.AsSpan().IndexOf(req.NonceBytes) >= 0;

    async Task SendLoop()
    {
        try
        {
            while (await _outbound.Reader.WaitToReadAsync(_cts.Token).ConfigureAwait(false))
            {
                while (_outbound.Reader.TryRead(out OutboundItem item))
                {
                    if (!this.IsConnected)
                    {
                        break;
                    }

                    try
                    {
                        await _ws.SendAsync(item.Data, WebSocketMessageType.Text, true, _cts.Token).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        LogException(ex, "Exception caught whilst trying to send data.");
                    }
                }
            }
        }
        catch (OperationCanceledException) when (_cts.IsCancellationRequested)
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

    public async ValueTask DisposeAsync()
    {
        if (this.IsConnected)
        {
            await Disconnect().ConfigureAwait(false);
            return;
        }

        Log(LogLevel.Debug, "Disposed {Name}", nameof(ChannelWebSocketClient));
    }
}

public enum ConnectionState
{
    Disconnected,
    Connected,
    Reconnecting
}

internal sealed class BroadcastReader(ChannelReader<byte[]> inner) : ChannelReader<ReadOnlyMemory<byte>>
{
    public override Task Completion => inner.Completion;

    byte[]? _previous;

    public override bool TryRead(out ReadOnlyMemory<byte> item)
    {
        ReturnPrevious();
        if (inner.TryRead(out byte[]? arr))
        {
            _previous = arr;
            item = arr;
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
        byte[] arr = await inner.ReadAsync(ct).ConfigureAwait(false);
        _previous = arr;
        return arr;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void ReturnPrevious()
    {
        if (_previous is not null)
        {
            ArrayPool<byte>.Shared.Return(_previous);
            _previous = null;
        }
    }
}
