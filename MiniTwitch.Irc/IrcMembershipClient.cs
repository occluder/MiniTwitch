using Microsoft.Extensions.Logging;
using MiniTwitch.Common;
using MiniTwitch.Common.Extensions;
using MiniTwitch.Irc.Interfaces;
using MiniTwitch.Irc.Internal;
using MiniTwitch.Irc.Internal.Enums;
using MiniTwitch.Irc.Internal.Models;
using MiniTwitch.Irc.Models;

namespace MiniTwitch.Irc;

/// <summary>
/// Listens only to user JOINs or PARTs from channels. Most other IRC messages are ignored 
/// </summary>
public sealed class IrcMembershipClient : IAsyncDisposable
{
    #region Constants
    private const string CONN_URL = "wss://irc-ws.chat.twitch.tv:443";
    private const string LOG_HEADER = "Irc-Membership";
    #endregion

    #region Properties
    /// <summary>
    /// The action to invoke when an exception is caught within an event
    /// </summary>
    public Action<Exception> ExceptionHandler { get; set; } = default!;
    /// <summary>
    /// The default logger for <see cref="IrcChannel"/>, only used when <see cref="ILogger"/> is not provided in client options
    /// <para>Can be toggled with <see cref="DefaultMiniTwitchLogger{T}.Enabled"/></para>
    /// </summary>
    public DefaultMiniTwitchLogger<IrcMembershipClient> DefaultLogger { get; } = new();
    /// <summary>
    /// Whether the client is currently connected
    /// </summary>
    public bool IsConnected => _ws.IsConnected;
    #endregion

    #region Events
    /// <summary>
    /// Invoked upon connecting to TMI
    /// <para>Note: This is only invoked once. Following connections to TMI will invoke <see cref="OnReconnect"/></para>
    /// </summary>
    public event Func<ValueTask> OnConnect = default!;
    /// <summary>
    /// Invoked upon reconnecting to TMI
    /// </summary>
    public event Func<ValueTask> OnReconnect = default!;
    /// <summary>
    /// Invoked upon disconnection from TMI
    /// </summary>
    public event Func<ValueTask> OnDisconnect = default!;
    /// <summary>
    /// Invoked when a user joins a channel
    /// </summary>
    public event Func<MembershipArgs, ValueTask> OnUserJoin = default!;
    /// <summary>
    /// Invoked when a user parts a channel
    /// </summary>
    public event Func<MembershipArgs, ValueTask> OnUserPart = default!;

    internal event Func<ValueTask> OnPing = default!;
    #endregion

    #region Fields
    private readonly SemaphoreSlim _connectionWaiter = new(0);
    private readonly SemaphoreSlim _joinChannelWaiter = new(0);
    private readonly IMembershipClientOptions _options;
    private readonly WebSocketClient _ws;
    private readonly List<string> _joinedChannels = new();
    private readonly RateLimitManager _manager;
    private bool _connectInvoked;
    IDisposable? _loggingScope;
    #endregion

    #region Init
    /// <summary>
    /// Creates a new instance of <see cref="IrcMembershipClient"/>
    /// </summary>
    public IrcMembershipClient(Action<IMembershipClientOptions> options)
    {
        var clientOptions = new ClientOptions();
        options(clientOptions);
        _options = clientOptions;
        _ws = new(_options.ReconnectionDelay, 8192);
        _manager = new(clientOptions);
        _loggingScope = GetLogger().BeginScope(LOG_HEADER);

        InternalInt();
    }

    private void InternalInt()
    {
        this.ExceptionHandler ??= LogEventException;

        OnPing += Ping;
        _ws.OnDisconnect += OnWsDisconnect;
        _ws.OnConnect += Login;
        _ws.OnReconnect += OnWsReconnect;
        _ws.OnData += Parse;
        _ws.OnLog += Log;
        _ws.OnLogEx += LogException;
    }
    #endregion

    #region Connection
    private async Task Login()
    {
        await _ws.SendAsync("CAP REQ :twitch.tv/membership twitch.tv/commands");
        await _ws.SendAsync($"NICK justinfan{Random.Shared.Next(100, 900)}");
    }

    /// <summary>
    /// Attempts connection to TMI like <see cref="ConnectAsync(CancellationToken)"/>, but connects in a "fire and forget" style
    /// </summary>
    public void Connect() => ConnectAsync().StepOver();

    /// <summary>
    /// Connects to TMI
    /// </summary>
    /// <returns><see langword="true"/> if the connection is successful; Otherwise, after 15 seconds: <see langword="false"/></returns>
    public async Task<bool> ConnectAsync(CancellationToken cancellationToken = default)
    {
        Uri uri = new(CONN_URL);

        await _ws.Start(uri, cancellationToken);

        if (await _connectionWaiter.WaitAsync(TimeSpan.FromSeconds(15), cancellationToken).ConfigureAwait(false))
            return true;

        Log(LogLevel.Critical, "Connection timed out.");
        return false;
    }

    /// <summary>
    /// Disconnects from TMI in a "fire and forget" style
    /// </summary>
    public void Disconnect() => _ws.Disconnect().StepOver();

    /// <summary>
    /// Disconnects from TMI
    /// </summary>
    public Task DisconnectAsync(CancellationToken cancellationToken = default) => _ws.Disconnect(cancellationToken);

    /// <summary>
    /// Disconnects then reconnects to TMI
    /// </summary>
    public Task ReconnectAsync(CancellationToken cancellationToken = default) => _ws.Restart(_options.ReconnectionDelay, cancellationToken);

    private async Task OnWsReconnect()
    {
        await Login();
        TimeSpan joinInterval = _joinedChannels.Count >= _options.JoinRateLimit
            ? TimeSpan.FromSeconds(_joinedChannels.Count * (10 / _options.JoinRateLimit))
            : TimeSpan.Zero;

        foreach (string channel in _joinedChannels)
        {
            await JoinChannel(channel);
            await Task.Delay(joinInterval);
        }
    }

    private Task OnWsDisconnect()
    {
        OnDisconnect?.Invoke().StepOver(this.ExceptionHandler);
        return Task.CompletedTask;
    }
    #endregion

    #region Communication
    private async ValueTask Ping()
    {
        Log(LogLevel.Debug, "PING received. Sending PONG");
        await _ws.SendAsync("PONG");
    }
    #endregion

    #region Channels
    /// <summary>
    /// Used for joining a channel
    /// </summary>
    /// <param name="channel">Username of the channel to join</param>
    /// <param name="cancellationToken">A cancellation token to stop further execution of asynchronous actions</param>
    /// <returns><see langword="true"/> if the join is successful; Otherwise, after 10 seconds: <see langword="false"/></returns>
    public async Task JoinChannel(string channel, CancellationToken cancellationToken = default)
    {
        if (!_ws.IsConnected)
        {
            Log(LogLevel.Error, "Failed to join channel {Channel}:  Not connected.", channel);
            return;
        }

        if (!_manager.CanJoin())
        {
            Log(LogLevel.Warning, "Waiting to join #{Channel}: Configured ratelimit of {Rate} joins/10s is hit", channel, _options.JoinRateLimit);
            await Task.Delay(TimeSpan.FromSeconds(30 / _options.JoinRateLimit), cancellationToken);
            await JoinChannel(channel, cancellationToken);
            return;
        }

        await _ws.SendAsync($"JOIN #{channel}", cancellationToken: cancellationToken);
        _joinedChannels.Add(channel);
    }

    /// <summary>
    /// Used for joining multiple channels
    /// </summary>
    /// <param name="channels">Usernames of channels to join</param>
    /// <param name="cancellationToken">A cancellation token to stop further execution of asynchronous actions</param>
    /// <returns><see langword="true"/> if all joins are successful; Otherwise, <see langword="false"/></returns>
    public async Task JoinChannels(IEnumerable<string> channels, CancellationToken cancellationToken = default)
    {
        if (!_ws.IsConnected)
        {
            Log(LogLevel.Error, "Failed to join channels {Channels}:  Not connected.", string.Join(',', channels));
            return;
        }

        foreach (string channel in channels)
        {
            await JoinChannel(channel, cancellationToken);
        }
    }

    /// <summary>
    /// Used for leaving/parting a joined channel
    /// </summary>
    /// <param name="channel">name of the channel to part</param>
    /// <param name="cancellationToken">A cancellation token to stop further execution of asynchronous actions</param>
    public Task PartChannel(string channel, CancellationToken cancellationToken = default)
    {
        if (!_ws.IsConnected)
        {
            Log(LogLevel.Error, "Failed to part channel {Channel}: Not connected.", channel);
            return Task.CompletedTask;
        }

        if (_joinedChannels.Remove(channel))
            Log(LogLevel.Debug, "Removed #{Channel} from joined channels list.", channel);

        return _ws.SendAsync($"PART #{channel}", cancellationToken: cancellationToken).AsTask();
    }
    #endregion

    #region Parsing
    internal void Parse(ReadOnlyMemory<byte> data)
    {
        IrcMessage message = new(data);
        HandleMessage(message);
        if (message.IsMultipleMessages)
        {
            Parse(data[message.NextMessageStartIndex..]);
        }
    }

    private void HandleMessage(IrcMessage message)
    {
        switch (message.Command)
        {
            case IrcCommand.Connected:
                if (_connectionWaiter.CurrentCount == 0)
                    _ = _connectionWaiter.Release();

                if (_connectInvoked)
                {
                    OnReconnect?.Invoke().StepOver(this.ExceptionHandler);
                    break;
                }

                _connectInvoked = true;
                OnConnect?.Invoke().StepOver(this.ExceptionHandler);
                break;

            case IrcCommand.RECONNECT:
                Log(LogLevel.Information, "Twitch servers requested a reconnection. Reconnecting ...");
                _ws.Restart(_options.ReconnectionDelay).StepOver();
                OnReconnect?.Invoke().StepOver(this.ExceptionHandler);
                break;

            case IrcCommand.PING:
                OnPing?.Invoke().StepOver(this.ExceptionHandler);
                break;

            case IrcCommand.JOIN:
                MembershipArgs joinArgs = new()
                {
                    User = new MessageAuthor()
                    {
                        Name = message.GetUsername()
                    },
                    Channel = new IrcChannel()
                    {
                        Name = message.GetChannel()
                    }
                };
                OnUserJoin?.Invoke(joinArgs).StepOver(this.ExceptionHandler);
                break;

            case IrcCommand.PART:
                MembershipArgs partArgs = new()
                {
                    User = new MessageAuthor()
                    {
                        Name = message.GetUsername()
                    },
                    Channel = new IrcChannel()
                    {
                        Name = message.GetChannel()
                    }
                };
                OnUserPart?.Invoke(partArgs).StepOver(this.ExceptionHandler);
                break;
        }
    }
    #endregion

    #region Utils
    private ILogger GetLogger() => _options.Logger ?? this.DefaultLogger;
    private void LogEventException(Exception ex) => LogException(ex, "🚨 Exception caught in an event:");
    private void Log(LogLevel level, string template, params object[] properties) => GetLogger().Log(level, template, properties);
    private void LogException(Exception ex, string template, params object[] properties) => GetLogger().LogError(ex, template, properties);
    #endregion

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        await _ws.DisposeAsync();
        _joinedChannels.Clear();
        _connectionWaiter.Dispose();
        _joinChannelWaiter.Dispose();
        _loggingScope?.Dispose();
    }
}
