using System.Buffers;
using System.Net.WebSockets;
using System.Text;
using Microsoft.Extensions.Logging;

namespace MiniTwitch.Common;
public sealed class WebSocketClient(TimeSpan reconnectionDelay) : IAsyncDisposable
{
    #region Properties
    public bool IsConnected => _client is not null && _client.State == WebSocketState.Open;
    #endregion

    #region Events
    public event Func<Task> OnConnect = default!;
    public event Func<Task> OnReconnect = default!;
    public event Func<Task> OnDisconnect = default!;
    public event Action<ReadOnlyMemory<byte>> OnData = default!;
    public event Action<LogLevel, string, object[]> OnLog = default!;
    public event Action<Exception, string, object[]> OnLogEx = default!;
    #endregion

    #region Fields
    private readonly SemaphoreSlim _reconnectionLock = new(0);
    private readonly SemaphoreSlim _receiveLock = new(1);
    private readonly SemaphoreSlim _sendLock = new(1);
    private readonly TimeSpan _reconnectDelay = reconnectionDelay;
    private readonly IByteMemoryConsumer? _consumer;
    private CancellationTokenSource _cts = new();
    private ClientWebSocket _client = new();
    private Uri _uri = default!;
    private Task _receiveTask = default!;
    private bool _reconnecting = false;

    #endregion


    public WebSocketClient(TimeSpan reconnectionDelay, short _ = 4096) : this(reconnectionDelay)
    {
    }

    public WebSocketClient(TimeSpan reconnectionDelay, IByteMemoryConsumer consumer) : this(reconnectionDelay)
    {
        _consumer = consumer;
    }

    #region Controls
    public async Task Start(Uri uri, CancellationToken cancellationToken = default)
    {
        // The ClientWebSocket cannot be reused once it is aborted
        if (_client.State == WebSocketState.Aborted)
        {
            Log(LogLevel.Error, "Cannot start WebSocket in aborted state");
            return;
        }

        // Set URI for reconnects
        _uri = uri;
        Log(LogLevel.Trace, "Connecting to {Uri} ...", uri);

        // Try connecting
        try
        {
            await _client.ConnectAsync(uri, cancellationToken);
        }
        catch (Exception ex)
        {
            LogException(ex, "WebSocket connection failed!");
            return;
        }

        // Make sure _cts isn't cancelled
        if (_cts.IsCancellationRequested)
            _cts = new();

        // Start receiving data
        _receiveTask = Task.Factory.StartNew(Receive, TaskCreationOptions.LongRunning);
        await Task.Delay(250, cancellationToken);

        // Invoke OnConnect if this is the first time connecting
        if (this.IsConnected)
        {
            if (!_reconnecting)
                await OnConnect.Invoke();
            else // This is a reconnect. So we want to invoke OnReconnect instead, which happens in Restart()
                _ = _reconnectionLock.Release();
        }
    }

    private async Task Stop(CancellationToken cancellationToken = default)
    {
        WebSocketCloseStatus status = _client.CloseStatus ?? WebSocketCloseStatus.NormalClosure;
        string? description = _client.CloseStatusDescription;

        await _client.CloseAsync(status, description, cancellationToken);
        //await _client.CloseOutputAsync(status, description, cancellationToken);
    }

    public async Task Disconnect(CancellationToken cancellationToken = default)
    {
        _cts.Cancel();
        // Need to be connected to do this
        if (this.IsConnected)
            await Stop(cancellationToken);

        _client.Dispose();
        await OnDisconnect.Invoke();
        if (_receiveTask.IsFaulted || _receiveTask.IsCanceled)
            _receiveTask.Dispose();
    }

    public async Task Restart(TimeSpan delay, CancellationToken cancellationToken = default)
    {
        // Sometimes the method gets invoked twice in a short span of time
        // The whole purpose of this _reconnecting field is to stop that
        if (_reconnecting)
            return;

        _reconnecting = true;

        Log(LogLevel.Critical, "The WebSocket client is restarting in {Delay}", delay);
        // Attempt to disconnect
        await Disconnect(cancellationToken);

        // Re-instantiate the client
        _client = new ClientWebSocket();

        // Wait the delay
        await Task.Delay(_reconnectDelay, cancellationToken);
        Log(LogLevel.Debug, "Finished waiting for reconnection delay");

        // Attempt to start the client again
        await Start(_uri, cancellationToken);
        Log(LogLevel.Trace, "If the WebSocket doesn't reconnect in 10 seconds you will see a warning");

        // Keep trying until you reconnect
        if (!await _reconnectionLock.WaitAsync(TimeSpan.FromSeconds(10), cancellationToken))
        {
            Log(LogLevel.Warning, "WebSocket reconnect failed. Retrying...");
            _reconnecting = false; // Set to false otherwise the method returns early
            await Restart(_reconnectDelay, cancellationToken);
            return;
        }

        Log(LogLevel.Information, "Successfully reconnected!");
        _reconnecting = false;
        await OnReconnect.Invoke();
    }
    #endregion

    #region Communication
    private async Task Receive()
    {
        var buffer = MemoryPool<byte>.Shared.Rent(1024 * 32);
        int written = 0;
        while (this.IsConnected)
        {
            try
            {
                await _receiveLock.WaitAsync(_cts.Token);
                var receiveTask = _client.ReceiveAsync(buffer.Memory[written..], _cts.Token);
                ValueWebSocketReceiveResult result = receiveTask.IsCompleted ? receiveTask.Result : await receiveTask;
                written += result.Count;
                if (result.EndOfMessage)
                {
                    OnData?.Invoke(buffer.Memory[..written]);
                    _consumer?.Consume(buffer.Memory[..written]);
                    written = 0;
                }

            }
            catch (WebSocketException wse)
            {
                Log(LogLevel.Critical, "An error occurred while receiving data from the WebSocket connection: {Message}",
                    wse.Message);
                break;
            }
            catch (InvalidOperationException)
            {
                Log(LogLevel.Warning, "Tried to receive data, but the WebSocket client is not connected");
                break;
            }
            catch (TaskCanceledException) when (_cts.IsCancellationRequested)
            {
                // Break loop to exit method
                break;
            }
            catch (Exception ex)
            {
                LogException(ex, "Exception caught in data receiver: ");
            }
            finally
            {
                _ = _receiveLock.Release();
            }
        }

        buffer.Dispose();
        // Don't restart if it's a user disconnect
        if (!_cts.IsCancellationRequested)
        {
            await Restart(_reconnectDelay);
        }
    }

    public async ValueTask SendAsync(string data, bool sensitive = false, CancellationToken cancellationToken = default)
    {
        var (written, bytes) = StringToBytes(data);
        if (!await _sendLock.WaitAsync(TimeSpan.FromSeconds(10), cancellationToken).ConfigureAwait(false))
        {
            Log(LogLevel.Warning, "{Method} timed out after 10 seconds.", nameof(SendAsync));
            return;
        }
        else if (!this.IsConnected)
        {
            Log(LogLevel.Warning, "Cannot send data in non-connect state. ({State})", _client.State);
        }

        if (!sensitive)
            Log(LogLevel.Debug, "Sending data: {Message}", data);

        try
        {
            await _client!.SendAsync(new ReadOnlyMemory<byte>(bytes)[..written], WebSocketMessageType.Text, true, cancellationToken);
        }
        catch (Exception ex)
        {
            LogException(ex, "Exception caught whilst trying to send data.");
        }
        finally
        {
            BytePool.Return(bytes, true);
            _ = _sendLock.Release();
        }
    }
    #endregion

    #region Helpers
    private void Log(LogLevel level, string template, params object[] properties) => OnLog?.Invoke(level, template, properties);

    private void LogException(Exception ex, string template, params object[] properties) => OnLogEx?.Invoke(ex, template, properties);

    private static (int, byte[]) StringToBytes(string s)
    {
        ReadOnlySpan<char> chars = s;
        byte[] bytes = BytePool.Rent(chars.Length * sizeof(char));
        int written = Encoding.UTF8.GetBytes(chars, bytes.AsSpan());
        return (written, bytes);
    }
    #endregion

    public async ValueTask DisposeAsync()
    {
        if (this.IsConnected)
        {
            await Disconnect();
            return;
        }

        await OnDisconnect.Invoke();
        Log(LogLevel.Debug, "Disposed {Name}", nameof(WebSocketClient));
    }
}
