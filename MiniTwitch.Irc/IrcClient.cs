using Microsoft.Extensions.Logging;
using MiniTwitch.Common;
using MiniTwitch.Common.Extensions;
using MiniTwitch.Irc.Enums;
using MiniTwitch.Irc.Interfaces;
using MiniTwitch.Irc.Internal;
using MiniTwitch.Irc.Internal.Enums;
using MiniTwitch.Irc.Internal.Models;
using MiniTwitch.Irc.Models;

namespace MiniTwitch.Irc;

/// <summary>
/// Responsible for all communication with Twitch IRC. Parses and invokes events for IRC messages
/// </summary>
public sealed class IrcClient : IIrcClient, IAsyncDisposable
{
    #region Properties
    /// <summary>
    /// The action to invoke when an exception is caught within an event
    /// </summary>
    public Action<Exception> ExceptionHandler { get; set; } = default!;
    /// <summary>
    /// List of the currently joined channels
    /// <para>Note: The client will attempt to rejoin all channels upon reconnection</para>
    /// </summary>
    public List<IBasicChannel> JoinedChannels { get; } = new();
    /// <summary>
    /// The default logger for <see cref="IrcChannel"/>, only used when <see cref="ILogger"/> is not provided in client options
    /// <para>Can be toggled with <see cref="DefaultMiniTwitchLogger{T}.Enabled"/></para>
    /// </summary>
    public DefaultMiniTwitchLogger<IrcChannel> DefaultLogger { get; } = new();
    /// <summary>
    /// Whether the client is currently connected
    /// </summary>
    public bool IsConnected => _ws.IsConnected;

    internal ClientOptions Options { get; init; }
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
    /// Invoked when a message is received
    /// </summary>
    public event Func<Privmsg, ValueTask> OnMessage = default!;
    /// <summary>
    /// Invoked when a user is about to gift subscription(s)
    /// <para>Provides a count of how many they are gifting (<see cref="IGiftSubNoticeIntro.GiftCount"/>), and how much they have gifted in total (<see cref="IGiftSubNoticeIntro.TotalGiftCount"/>)</para>
    /// </summary>
    public event Func<IGiftSubNoticeIntro, ValueTask> OnGiftedSubNoticeIntro = default!;
    /// <summary>
    /// Invoked when a user is gifted a subscription
    /// </summary>
    public event Func<IGiftSubNotice, ValueTask> OnGiftedSubNotice = default!;
    /// <summary>
    /// Invoked when a user (re)subscribes
    /// </summary>
    public event Func<ISubNotice, ValueTask> OnSubscriptionNotice = default!;
    /// <summary>
    /// Invoked when a raid occurs
    /// </summary>
    public event Func<IRaidNotice, ValueTask> OnRaidNotice = default!;
    /// <summary>
    /// Invoked when a user continues their gifted subscription
    /// <para>Note: <see cref="IPaidUpgradeNotice.GifterUsername"/> and <see cref="IPaidUpgradeNotice.GifterDisplayName"/> will equal to <see cref="string.Empty"/> if the gifted subscription was from an anonymous gifter</para>
    /// </summary>
    public event Func<IPaidUpgradeNotice, ValueTask> OnPaidUpgradeNotice = default!;
    /// <summary>
    /// Invoked when a user converts their subscription plan from Prime to paid (Tier 1, 2 or 3)
    /// </summary>
    public event Func<IPrimeUpgradeNotice, ValueTask> OnPrimeUpgradeNotice = default!;
    /// <summary>
    /// Invoked when a chat announcement is sent
    /// </summary>
    public event Func<IAnnouncementNotice, ValueTask> OnAnnouncement = default!;
    /// <summary>
    /// Invoked when a user gets timed out
    /// </summary>
    public event Func<IUserTimeout, ValueTask> OnUserTimeout = default!;
    /// <summary>
    /// Invoked when a user gets banned
    /// </summary>
    public event Func<IUserBan, ValueTask> OnUserBan = default!;
    /// <summary>
    /// Invoked when a chat gets cleared
    /// </summary>
    public event Func<IChatClear, ValueTask> OnChatClear = default!;
    /// <summary>
    /// Invoked when a message gets deleted
    /// </summary>
    public event Func<Clearmsg, ValueTask> OnMessageDelete = default!;
    /// <summary>
    /// Invoked when a channel is joined
    /// <para>Contains ROOMSTATE information such as <see cref="IrcChannel.FollowerModeEnabled"/>, <see cref="IrcChannel.SubOnlyEnabled"/>, <see cref="IrcChannel.SlowModeDuration"/>...</para>
    /// </summary>
    public event Func<IrcChannel, ValueTask> OnChannelJoin = default!;
    /// <summary>
    /// Invoked when "Emote Only" mode is either activated or deactivated
    /// </summary>
    public event Func<IEmoteOnlyModified, ValueTask> OnEmoteOnlyModified = default!;
    /// <summary>
    /// Invoked when "Followers Only" mode is either activated, modified or deactivated
    /// </summary>
    public event Func<IFollowersOnlyModified, ValueTask> OnFollowerModeModified = default!;
    /// <summary>
    /// Invoked when "R9K"/"Unique" mode is either activated or deactivated
    /// </summary>
    public event Func<IR9KModified, ValueTask> OnUniqueModeModified = default!;
    /// <summary>
    /// Invoked when "Slow" mode is either activated, modified or deactivated
    /// </summary>
    public event Func<ISlowModeModified, ValueTask> OnSlowModeModified = default!;
    /// <summary>
    /// Invoked when "Subscribers Only" mode is either activated or deactivated
    /// </summary>
    public event Func<ISubOnlyModified, ValueTask> OnSubOnlyModified = default!;
    /// <summary>
    /// Invoked when a channel is parted
    /// </summary>
    public event Func<IPartedChannel, ValueTask> OnChannelPart = default!;
    /// <summary>
    /// Invoked when a NOTICE is received
    /// </summary>
    public event Func<Notice, ValueTask> OnNotice = default!;
    /// <summary>
    /// Invoked when a USERSTATE is received
    /// <para>USERSTATEs are received upon initially joining a channel or sending a message</para>
    /// <para> They provide information about you (<seealso cref="Userstate.Self"/>), and your available emote sets (<see cref="Userstate.EmoteSets"/>) in that channel</para>
    /// </summary>
    public event Func<Userstate, ValueTask> OnUserstate = default!;
    /// <summary>
    /// Invoked when a whisper Is received
    /// </summary>
    public event Func<Whisper, ValueTask> OnWhisper = default!;
    /// <summary>
    /// Invoked when a user donates to a charity during a fundraiser
    /// </summary>
    public event Func<ICharityDonation, ValueTask> OnCharityDonation = default!;
    /// <summary>
    /// Occurs when a global user state message is received.
    /// </summary>
    public event Func<GlobalUserstate, ValueTask> OnGlobalUserstate = default!;
    /// <summary>
    /// Invoked when a user reaches a consecutive streams watched milestone
    /// </summary>
    public event Func<IViewerMilestone, ValueTask> OnViewerMilestone = default!;

    // Kept for testing
    internal event Func<ValueTask> OnPing = default!;
    #endregion

    #region Fields
    private static readonly WaitableEvents[] _channelJoinEvents = new[]
    {
        WaitableEvents.JoinedChannel, WaitableEvents.ChannelSuspended
    };
    private readonly AsyncEventCoordinator<WaitableEvents> _coordinator = new();
    private readonly HashSet<string> _moderated = new();
    private readonly RateLimitManager _manager;
    private readonly WebSocketConsumer _consumer;
    private readonly WebSocketClient _ws;
    private Uri _targetUrl = default!;
    private string _loggingHeader = "[MiniTwitch:Irc-default!]";
    private bool _connectInvoked;
    IDisposable? _loggingScope;
    #endregion

    #region Init
    /// <summary>
    /// Creates a new instance of <see cref="IrcClient"/>
    /// </summary>
    public IrcClient(Action<ClientOptions> options)
    {
        ClientOptions clientOptions = new();
        options.Invoke(clientOptions);
        this.Options = clientOptions;
        _consumer = new WebSocketConsumer(client: this);
        _ws = new WebSocketClient(this.Options.ReconnectionDelay, _consumer);
        _manager = new(clientOptions);

        InternalInit();
    }

    /// <summary>
    /// Creates a new instance of <see cref="IrcClient"/>
    /// </summary>
    public IrcClient(ClientOptions options)
    {
        this.Options = options;
        _consumer = new WebSocketConsumer(client: this);
        _ws = new WebSocketClient(this.Options.ReconnectionDelay, _consumer);
        _manager = new(options);

        InternalInit();
    }

    private void InternalInit()
    {
        this.Options.CheckCredentials();

        this.ExceptionHandler ??= LogEventException;
        _loggingHeader = "Irc-User=Anonymous";
        if (!this.Options.Anonymous)
        {
            this.Options.OAuth = Utils.CheckToken(this.Options.OAuth);
            _loggingHeader = $"Irc-User={this.Options.Username}";
        }

        _loggingScope = GetLogger().BeginScope(_loggingHeader);

        OnPing += PingReceived;
    }
    #endregion

    #region Connection
    private async Task Login()
    {
        await _ws.SendAsync("CAP REQ :twitch.tv/tags twitch.tv/commands");
        if (this.Options.Anonymous)
        {
            await _ws.SendAsync($"NICK justinfan{Random.Shared.Next(100, 900)}");
            return;
        }

        await _ws.SendAsync($"PASS oauth:{this.Options.OAuth}", this.Options.HideAuthenticationLogs);
        await _ws.SendAsync($"NICK {this.Options.Username}", this.Options.HideAuthenticationLogs);
    }

    /// <summary>
    /// Attempts connection to TMI like <see cref="ConnectAsync(string, CancellationToken)"/>, but connects in a "fire and forget" style
    /// </summary>
    public void Connect(string url = "wss://irc-ws.chat.twitch.tv:443") => ConnectAsync(url).StepOver();

    /// <summary>
    /// Connects to TMI
    /// </summary>
    /// <returns><see langword="true"/> if the connection is successful; Otherwise, after 15 seconds: <see langword="false"/></returns>
    public async Task<bool> ConnectAsync(string url = "wss://irc-ws.chat.twitch.tv:443",
        CancellationToken cancellationToken = default)
    {
        _targetUrl = new(url);
        await _ws.Start(_targetUrl, cancellationToken);
        if (await _coordinator.WaitFor(WaitableEvents.Connected, TimeSpan.FromSeconds(15), cancellationToken))
            return true;

        Log(LogLevel.Critical, "Connection timed out.");
        return false;
    }

    /// <summary>
    /// Disconnects from TMI in a "fire and forget" style
    /// <para>Note: To reconnect, you must use <see cref="ReconnectAsync(CancellationToken)"/></para>
    /// </summary>
    public void Disconnect() => _ws.Disconnect().StepOver();

    /// <summary>
    /// Disconnects from TMI
    /// <para>Note: To reconnect, you must use <see cref="ReconnectAsync(CancellationToken)"/></para>
    /// </summary>
    public Task DisconnectAsync(CancellationToken cancellationToken = default) => _ws.Disconnect(cancellationToken);

    /// <summary>
    /// Disconnects then reconnects to TMI
    /// </summary>
    public Task ReconnectAsync(CancellationToken cancellationToken = default) => _ws.Restart(this.Options.ReconnectionDelay, cancellationToken);

    private async Task WsReconnected()
    {
        await Login();
        TimeSpan joinInterval = this.JoinedChannels.Count >= this.Options.JoinRateLimit
            ? TimeSpan.FromSeconds(this.JoinedChannels.Count * (10 / this.Options.JoinRateLimit))
            : TimeSpan.Zero;

        foreach (string channel in this.JoinedChannels.Select(c => c.Name))
        {
            bool res = await JoinChannel(channel);
            Log(LogLevel.Information, $"{(res ? "Rejoined" : "Failed to rejoin")} channel: #{{Channel}}", channel);
            await Task.Delay(joinInterval);
        }
    }

    private Task WsDisconnected()
    {
        OnDisconnect?.Invoke().StepOver(this.ExceptionHandler);
        return Task.CompletedTask;
    }
    #endregion

    #region Communication
    /// <summary>
    /// Send a raw message to TMI
    /// </summary>
    /// <param name="message">The message to send</param>
    /// <param name="cancellationToken">A cancellation token to stop further execution of asynchronous actions</param>
    public async ValueTask SendRaw(string message, CancellationToken cancellationToken = default)
    {
        if (!_ws.IsConnected)
        {
            Log(LogLevel.Error, "Failed to send raw message {Message}: Not connected.", message);
            return;
        }

        await _ws.SendAsync(message, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Sends a chat message
    /// </summary>
    /// <param name="channel">The channel to send the message to</param>
    /// <param name="message">The message to send</param>
    /// <param name="action">Whether to prepend .me to the message</param>
    /// <param name="nonce">Custom nonce to send with the message. Value can't contain spaces</param>
    /// <param name="cancellationToken">A cancellation token to stop further execution of asynchronous actions</param>
    public async ValueTask SendMessage(string channel, string message, bool action = false, string? nonce = null, CancellationToken cancellationToken = default)
    {
        if (!_ws.IsConnected)
        {
            Log(LogLevel.Error, "Failed to send message {Message}: Not connected.", message);
            return;
        }
        else if (this.Options.Anonymous)
        {
            Log(LogLevel.Error, "Failed to send message {Message}: Cannot send message with anonymous account.", message);
            return;
        }

        if (nonce is not null && nonce.Contains(' '))
        {
            Log(LogLevel.Error, "Failed to send message {Message}: Nonce cannot contain spaces.", message);
            return;
        }

        if (!_manager.CanSend(channel, _moderated.Contains(channel)))
        {
            var delay = TimeSpan.FromSeconds(90 / this.Options.MessageRateLimit);
            Log(LogLevel.Debug, "Cannot send message to #{Channel}: Rate limit of {Count} hit. Retrying in {Delay}s",
                channel, this.Options.ModMessageRateLimit, delay.TotalSeconds);
            Log(LogLevel.Warning, "#{Channel}: Your message was not sent yet due to the configured messaging ratelimit (normal: {Normal}/30s, mod: {Mod}/30s)",
                channel, this.Options.MessageRateLimit, this.Options.ModMessageRateLimit);
            await Task.Delay(delay, cancellationToken);
            await SendMessage(channel, message, action, nonce, cancellationToken);
            return;
        }

        string outMsg = $"{(nonce is null ? string.Empty : $"@client-nonce={nonce}")} PRIVMSG #{channel} :{(action ? $".me {message}" : message)}";

        await _ws.SendAsync(outMsg, cancellationToken: cancellationToken);
    }
    /// <summary>
    /// Replies to a chat message
    /// </summary>
    /// <param name="parentMessage">The message to reply to</param>
    /// <param name="message">The message to reply with</param>
    /// <param name="action">Prepend .me to the message</param>
    /// <param name="replyInThread">Prefer replying to the target message in the same thread instead of creating a new one</param>
    /// <param name="cancellationToken">A cancellation token to stop further execution of asynchronous actions</param>
    public async ValueTask ReplyTo(Privmsg parentMessage, string message, bool action = false,
        bool replyInThread = false, CancellationToken cancellationToken = default)
    {
        if (!_ws.IsConnected)
        {
            Log(LogLevel.Error, "Failed to send reply {Message}: Not connected.", message);
            return;
        }
        else if (this.Options.Anonymous)
        {
            Log(LogLevel.Error, "Failed to send reply {Message}: Cannot send message with anonymous account.", message);
            return;
        }

        string channel = parentMessage.Channel.Name;
        if (!_manager.CanSend(channel, _moderated.Contains(channel)))
        {
            var delay = TimeSpan.FromSeconds(90 / this.Options.MessageRateLimit);
            Log(LogLevel.Debug, "Cannot send message to #{Channel}: Rate limit of {Count} hit. Retrying in {Delay}s",
                channel, this.Options.ModMessageRateLimit, delay.TotalSeconds);
            Log(LogLevel.Warning, "#{Channel}: Your message was not sent yet due to the configured messaging ratelimit (normal: {Normal}/30s, mod: {Mod}/30s)",
                channel, this.Options.MessageRateLimit, this.Options.ModMessageRateLimit);

            await Task.Delay(delay, cancellationToken);
            await ReplyTo(parentMessage, message, action, replyInThread, cancellationToken);
            return;
        }

        string target = replyInThread && !string.IsNullOrEmpty(parentMessage.Reply.ParentThreadMessageId)
            ? parentMessage.Reply.ParentThreadMessageId
            : parentMessage.Id;

        string outMsg = $"@reply-parent-msg-id={target} PRIVMSG #{channel} :{(action ? $".me {message}" : message)}";
        await _ws.SendAsync(outMsg, cancellationToken: cancellationToken);
    }
    /// <summary>
    /// Replies to a chat message
    /// </summary>
    /// <param name="messageId">The ID of the message to reply to</param>
    /// <param name="channel">the channel in which that message was sent</param>
    /// <param name="reply">The message to reply with</param>
    /// <param name="action">Prepend .me to the message</param>
    /// <param name="cancellationToken">A cancellation token to stop further execution of asynchronous actions</param>
    /// <returns></returns>
    public async ValueTask ReplyTo(string messageId, string channel, string reply, bool action = false, CancellationToken cancellationToken = default)
    {
        const string privmsg = "PRIVMSG #";
        const string replyTag = "@reply-parent-msg-id=";
        const string act = ".me ";
        if (!_ws.IsConnected)
        {
            Log(LogLevel.Error, "Failed to send reply {Message}: Not connected.", messageId);
            return;
        }
        else if (this.Options.Anonymous)
        {
            Log(LogLevel.Error, "Failed to send reply {Message}: Cannot send message with anonymous account.", messageId);
            return;
        }

        if (!_manager.CanSend(channel, _moderated.Contains(channel)))
        {
            var delay = TimeSpan.FromSeconds(90 / this.Options.MessageRateLimit);
            Log(LogLevel.Debug, "Cannot send message to #{Channel}: Rate limit of {Count} hit. Retrying in {Delay}s",
                channel, this.Options.ModMessageRateLimit, delay.TotalSeconds);
            Log(LogLevel.Warning, "#{Channel}: Your message was not sent yet due to the configured messaging ratelimit (normal: {Normal}/30s, mod: {Mod}/30s)",
                channel, this.Options.MessageRateLimit, this.Options.ModMessageRateLimit);
            await Task.Delay(delay, cancellationToken);
            await ReplyTo(messageId, channel, reply, action, cancellationToken);
            return;
        }

        string outMsg = $"{replyTag}{messageId} {privmsg}{channel} :{(action ? $"{act} {reply}" : reply)}";

        await _ws.SendAsync(outMsg, cancellationToken: cancellationToken);
    }

    private ValueTask PingReceived()
    {
        Log(LogLevel.Debug, "PING received");
        return _ws.SendAsync("PONG");
    }
    #endregion

    #region Channels
    /// <summary>
    /// Used for joining a channel
    /// </summary>
    /// <param name="channel">The channel to join</param>
    /// <param name="cancellationToken">A cancellation token to stop further execution of asynchronous actions</param>
    /// <returns><see langword="true"/> if the join is successful; Otherwise, after 10 seconds: <see langword="false"/></returns>
    public Task<bool> JoinChannel(IBasicChannel channel, CancellationToken cancellationToken = default)
        => JoinChannel(channel.Name, cancellationToken: cancellationToken);

    /// <summary>
    /// Used for joining multiple channels
    /// </summary>
    /// <param name="channels">The channels to join</param>
    /// <param name="cancellationToken">A cancellation token to stop further execution of asynchronous actions</param>
    /// <returns><see langword="true"/> if all joins are successful; Otherwise, <see langword="false"/></returns>
    public Task<bool> JoinChannels(IEnumerable<IBasicChannel> channels, CancellationToken cancellationToken = default)
        => JoinChannels(channels.Select(x => x.Name), cancellationToken: cancellationToken);

    /// <summary>
    /// Used for leaving/parting a joined channel
    /// </summary>
    /// <param name="channel">The channel to part</param>
    /// <param name="cancellationToken">A cancellation token to stop further execution of asynchronous actions</param>
    public Task PartChannel(IBasicChannel channel, CancellationToken cancellationToken = default)
        => PartChannel(channel.Name, cancellationToken: cancellationToken);

    /// <summary>
    /// Used for joining a channel
    /// </summary>
    /// <param name="channel">Username of the channel to join</param>
    /// <param name="cancellationToken">A cancellation token to stop further execution of asynchronous actions</param>
    /// <returns><see langword="true"/> if the join is successful; Otherwise, after 10 seconds: <see langword="false"/></returns>
    public async Task<bool> JoinChannel(string channel, CancellationToken cancellationToken = default)
    {
        if (!_ws.IsConnected)
        {
            Log(LogLevel.Error, "Failed to join channel #{Channel}:  Not connected.", channel);
            return false;
        }

        if (!_manager.CanJoin())
        {
            Log(LogLevel.Warning, "Waiting to join #{Channel}: Configured ratelimit of {Rate} joins/10s is hit", channel, this.Options.JoinRateLimit);
            await Task.Delay(TimeSpan.FromSeconds(30 / this.Options.JoinRateLimit), cancellationToken);
            return await JoinChannel(channel, cancellationToken);
        }

        await _ws.SendAsync($"JOIN #{channel}", cancellationToken: cancellationToken);
        try
        {
            WaitableEvents ev = await _coordinator.WaitForAny(_channelJoinEvents, TimeSpan.FromSeconds(10), cancellationToken);
            return ev == WaitableEvents.JoinedChannel;
        }
        catch
        {
            Log(LogLevel.Error, "Failed to join channel #{Channel}: Timed out. (Channel may have been renamed or deleted)", channel);
            return false;
        }
    }

    /// <summary>
    /// Used for joining multiple channels
    /// </summary>
    /// <param name="channels">Usernames of channels to join</param>
    /// <param name="cancellationToken">A cancellation token to stop further execution of asynchronous actions</param>
    /// <returns><see langword="true"/> if all joins are successful; Otherwise, <see langword="false"/></returns>
    public async Task<bool> JoinChannels(IEnumerable<string> channels, CancellationToken cancellationToken = default)
    {
        if (!_ws.IsConnected)
        {
            Log(LogLevel.Error, "Failed to join channels {Channels}:  Not connected.", string.Join(',', channels));
            return false;
        }

        bool allSuccess = true;
        foreach (string channel in channels)
        {
            if (!await JoinChannel(channel, cancellationToken))
                allSuccess = false;
        }

        return allSuccess;
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
            Log(LogLevel.Error, "Failed to part channel #{Channel}: Not connected.", channel);
            return Task.CompletedTask;
        }

        return _ws.SendAsync($"PART #{channel}", cancellationToken: cancellationToken).AsTask();
    }
    #endregion

    #region Parsing
    internal void Parse(ReadOnlyMemory<byte> data)
    {
        IrcMessage message = new(data);
        HandleMessage(ref message);
        if (message.IsMultipleMessages)
        {
            Parse(data[message.NextMessageStartIndex..]);
        }
    }

    private void HandleMessage(ref IrcMessage message)
    {
        // Return if user decides to ignore the command
        if (this.Options.IgnoreCommands != IgnoreCommand.None && message.Command switch
        {
            IrcCommand.PRIVMSG => this.Options.IgnoreCommands.HasFlag(IgnoreCommand.PRIVMSG),
            IrcCommand.USERNOTICE => this.Options.IgnoreCommands.HasFlag(IgnoreCommand.USERNOTICE),
            IrcCommand.CLEARCHAT => this.Options.IgnoreCommands.HasFlag(IgnoreCommand.CLEARCHAT),
            IrcCommand.CLEARMSG => this.Options.IgnoreCommands.HasFlag(IgnoreCommand.CLEARMSG),
            IrcCommand.WHISPER => this.Options.IgnoreCommands.HasFlag(IgnoreCommand.WHISPER),
            IrcCommand.USERSTATE => this.Options.IgnoreCommands.HasFlag(IgnoreCommand.USERSTATE),
            IrcCommand.JOIN => this.Options.IgnoreCommands.HasFlag(IgnoreCommand.JOIN),
            IrcCommand.PART => this.Options.IgnoreCommands.HasFlag(IgnoreCommand.PART),
            IrcCommand.NOTICE => this.Options.IgnoreCommands.HasFlag(IgnoreCommand.NOTICE),
            IrcCommand.ROOMSTATE => this.Options.IgnoreCommands.HasFlag(IgnoreCommand.ROOMSTATE),
            _ => false
        })
        {
            return;
        }

        switch (message.Command)
        {
            case IrcCommand.PRIVMSG:
                Privmsg ircMessage = new(ref message, this);
                OnMessage?.Invoke(ircMessage).StepOver(this.ExceptionHandler);
                break;

            case IrcCommand.Connected:
                _coordinator.ReleaseIfLocked(WaitableEvents.Connected);

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
                _ws.Restart(this.Options.ReconnectionDelay).StepOver();
                OnReconnect?.Invoke().StepOver(this.ExceptionHandler);
                break;

            case IrcCommand.PING:
                PingReceived().StepOver(this.ExceptionHandler);
                OnPing?.Invoke().StepOver(this.ExceptionHandler);
                break;

            case IrcCommand.USERNOTICE:
                Usernotice usernotice = new(ref message);
                UsernoticeType msgId = usernotice.MsgId == UsernoticeType.SharedChatNotice
                    ? usernotice.Source.MsgId
                    : usernotice.MsgId;

                switch (msgId)
                {
                    case UsernoticeType.Sub
                    or UsernoticeType.Resub:
                        OnSubscriptionNotice?.Invoke(usernotice).StepOver(this.ExceptionHandler);
                        break;

                    case UsernoticeType.Subgift:
                        OnGiftedSubNotice?.Invoke(usernotice).StepOver(this.ExceptionHandler);
                        break;

                    case UsernoticeType.Raid:
                        OnRaidNotice?.Invoke(usernotice).StepOver(this.ExceptionHandler);
                        break;

                    case UsernoticeType.AnonGiftPaidUpgrade
                    or UsernoticeType.GiftPaidUpgrade:
                        OnPaidUpgradeNotice?.Invoke(usernotice).StepOver(this.ExceptionHandler);
                        break;

                    case UsernoticeType.PrimePaidUpgrade:
                        OnPrimeUpgradeNotice?.Invoke(usernotice).StepOver(this.ExceptionHandler);
                        break;

                    case UsernoticeType.Announcement:
                        OnAnnouncement?.Invoke(usernotice).StepOver(this.ExceptionHandler);
                        break;

                    case UsernoticeType.SubMysteryGift:
                        OnGiftedSubNoticeIntro?.Invoke(usernotice).StepOver(this.ExceptionHandler);
                        break;

                    case UsernoticeType.CharityDonation:
                        OnCharityDonation?.Invoke(usernotice).StepOver(this.ExceptionHandler);
                        break;

                    case UsernoticeType.ViewerMilestone:
                        OnViewerMilestone?.Invoke(usernotice).StepOver(this.ExceptionHandler);
                        break;
                }

                break;

            case IrcCommand.CLEARCHAT:
                Clearchat clearchat = new(ref message);
                if (clearchat.IsClearChat)
                    OnChatClear?.Invoke(clearchat).StepOver(this.ExceptionHandler);
                else if (clearchat.IsBan)
                    OnUserBan?.Invoke(clearchat).StepOver(this.ExceptionHandler);
                else
                    OnUserTimeout?.Invoke(clearchat).StepOver(this.ExceptionHandler);

                break;

            case IrcCommand.CLEARMSG:
                Clearmsg clearmsg = new(ref message);
                OnMessageDelete?.Invoke(clearmsg).StepOver(this.ExceptionHandler);
                break;

            case IrcCommand.ROOMSTATE:
                IrcChannel ircChannel = new(ref message);
                if (ircChannel.Roomstate == RoomstateType.All)
                {
                    _coordinator.ReleaseIfLocked(WaitableEvents.JoinedChannel);
                    if (!this.JoinedChannels.Contains(ircChannel))
                    {
                        this.JoinedChannels.Add(ircChannel);
                        Log(LogLevel.Information, "Joined #{Channel}", ircChannel.Name);
                        Log(LogLevel.Debug, "Added #{Channel} to joined channels list.", ircChannel.Name);
                    }

                    OnChannelJoin?.Invoke(ircChannel).StepOver(this.ExceptionHandler);
                }
                else if (ircChannel.Roomstate == RoomstateType.EmoteOnly)
                {
                    OnEmoteOnlyModified?.Invoke(ircChannel).StepOver(this.ExceptionHandler);
                }
                else if (ircChannel.Roomstate == RoomstateType.FollowerOnly)
                {
                    OnFollowerModeModified?.Invoke(ircChannel).StepOver(this.ExceptionHandler);
                }
                else if (ircChannel.Roomstate == RoomstateType.R9K)
                {
                    OnUniqueModeModified?.Invoke(ircChannel).StepOver(this.ExceptionHandler);
                }
                else if (ircChannel.Roomstate == RoomstateType.Slow)
                {
                    OnSlowModeModified?.Invoke(ircChannel).StepOver(this.ExceptionHandler);
                }
                else if (ircChannel.Roomstate == RoomstateType.SubOnly)
                {
                    OnSubOnlyModified?.Invoke(ircChannel).StepOver(this.ExceptionHandler);
                }
                else
                {
                    Log(LogLevel.Warning, "Unknown ROOMSTATE type received.");
                }

                break;

            case IrcCommand.PART:
                IrcChannel channel = new(ref message);
                if (this.JoinedChannels.Remove(channel))
                {
                    Log(LogLevel.Information, "Parted #{Channel}", channel.Name);
                    Log(LogLevel.Debug, "Removed #{Channel} from joined channels list.", channel.Name);

                }

                OnChannelPart?.Invoke(channel).StepOver(this.ExceptionHandler);
                break;

            case IrcCommand.NOTICE:
                Notice notice = new(ref message);
                if (notice.Type == NoticeType.Msg_channel_suspended)
                {
                    Log(LogLevel.Error, "Failed to join #{Channel}: Channel does not exist anymore.", notice.Channel.Name);
                    _coordinator.ReleaseIfLocked(WaitableEvents.ChannelSuspended);
                }
                else if (notice.Type == NoticeType.Bad_auth)
                {
                    Log(LogLevel.Critical, "Authentication failed: {Message}", notice.SystemMessage);
                }

                OnNotice?.Invoke(notice).StepOver(this.ExceptionHandler);
                break;

            case IrcCommand.USERSTATE:
                Userstate state = new(ref message);
                if (state.Self.IsMod && !_moderated.Contains(state.Channel.Name))
                    _ = _moderated.Add(state.Channel.Name);

                OnUserstate?.Invoke(state).StepOver(this.ExceptionHandler);
                break;

            case IrcCommand.GLOBALUSERSTATE:
                GlobalUserstate globalState = new(ref message);
                OnGlobalUserstate?.Invoke(globalState).StepOver(this.ExceptionHandler);
                break;

            case IrcCommand.WHISPER:
                Whisper whisper = new(ref message);
                OnWhisper?.Invoke(whisper).StepOver(this.ExceptionHandler);
                break;
        }
    }
    #endregion

    #region Utils
    private ILogger GetLogger() => this.Options.Logger ?? this.DefaultLogger;

    private void LogEventException(Exception ex) => LogException(ex, "🚨 Exception caught in an event:");

    private void Log(LogLevel level, string template, params object[] properties) => GetLogger().Log(level, template, properties);

    private void LogException(Exception ex, string template, params object[] properties) => GetLogger().LogError(ex, template, properties);
    #endregion

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        await _ws.DisposeAsync();
        this.JoinedChannels.Clear();
        _coordinator.Dispose();
        _moderated.Clear();
        _loggingScope?.Dispose();
    }

    internal class WebSocketConsumer(IrcClient client) : IWebSocketConsumer
    {
        private readonly IrcClient _client = client;

        public void BytesReceived(ReadOnlyMemory<byte> data) => _client.Parse(data);
        public Task ConnectionEstablished() => _client.Login();
        public Task ConnectionReestablished() => _client.WsReconnected();
        public Task Disconnection() => _client.WsDisconnected();
        public void ReadLogMessage(LogLevel level, string message, params object[] args) => _client.Log(level, message, args);
        public void ReadLogException(Exception ex, string message, params object[] args) => _client.LogException(ex, message, args);
    }
}
