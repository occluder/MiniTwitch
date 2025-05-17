using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using MiniTwitch.Common;
using MiniTwitch.Common.Extensions;
using MiniTwitch.PubSub.Enums;
using MiniTwitch.PubSub.Interfaces;
using MiniTwitch.PubSub.Internal;
using MiniTwitch.PubSub.Internal.Enums;
using MiniTwitch.PubSub.Internal.Models;
using MiniTwitch.PubSub.Internal.Parsing;
using MiniTwitch.PubSub.Models;
using MiniTwitch.PubSub.Payloads;

namespace MiniTwitch.PubSub;

/// <summary>
/// Responsible for communication with the Twitch PubSub services.
/// </summary>
public sealed class PubSubClient : IAsyncDisposable
{
    #region Properties
    /// <summary>
    /// The action that is invoked when an exception is thrown in events
    /// </summary>
    public Action<Exception> ExceptionHandler { get; set; } = default!;
    /// <summary>
    /// A collection of topics that are actively being listened to by this client
    /// </summary>
    public IReadOnlyCollection<Topic> ActiveTopics => _ws.IsConnected ? _topics : Array.Empty<Topic>();
    /// <summary>
    /// The default logger for <see cref="PubSubClient"/>, only used when <see cref="ILogger"/> is not provided in the constructor
    /// <para>Can be toggled with <see cref="DefaultMiniTwitchLogger{T}.Enabled"/></para>
    /// </summary>
    public DefaultMiniTwitchLogger<PubSubClient> DefaultLogger { get; } = new();
    /// <summary>
    /// Whether the client is currently connected
    /// </summary>
    public bool IsConnected => _ws.IsConnected;
    #endregion

    #region Events
    /// <summary>
    /// Invoked upon connecting to the PubSub service
    /// <para>Note: This is only invoked once. Following connections to PubSub will invoke <see cref="OnReconnect"/></para>
    /// </summary>
    public event Func<ValueTask> OnConnect = default!;
    /// <summary>
    /// Invoked upon reconnecting to PubSub
    /// </summary>
    public event Func<ValueTask> OnReconnect = default!;
    /// <summary>
    /// Invoked upon disconnecting from PubSub
    /// </summary>
    public event Func<ValueTask> OnDisconnect = default!;
    /// <summary>
    /// Invoked when a prediction is started
    /// <para>Requires topic: <see cref="Topics.ChannelPredictions(long, string?)"/></para>
    /// </summary>
    public event Func<ChannelId, IPredictionStarted, ValueTask> OnPredictionStarted = default!;
    /// <summary>
    /// Invoked every second when a prediction is active, giving updated information about the prediction as more users participate
    /// <para>Requires topic: <see cref="Topics.ChannelPredictions(long, string?)"/></para>
    /// </summary>
    public event Func<ChannelId, IPredictionUpdate, ValueTask> OnPredictionUpdate = default!;
    /// <summary>
    /// Invoked when the period for users to predict has ended
    /// <para>Requires topic: <see cref="Topics.ChannelPredictions(long, string?)"/></para>
    /// </summary>
    public event Func<ChannelId, IPredictionWindowClosed, ValueTask> OnPredictionWindowClosed = default!;
    /// <summary>
    /// Invoked when a prediction is ended, an outcome was chosen to be the winner
    /// <para>See <see cref="IPredictionEnded.WinningOutcomeId"/> for chosen outcome</para>
    /// <para>Requires topic: <see cref="Topics.ChannelPredictions(long, string?)"/></para>
    /// </summary>
    public event Func<ChannelId, IPredictionEnded, ValueTask> OnPredictionEnded = default!;
    /// <summary>
    /// Invoked when a moderator manually closes the prediction window for users to participate
    /// <para>Requires topic: <see cref="Topics.ChannelPredictions(long, string?)"/></para>
    /// </summary>
    public event Func<ChannelId, IPredictionLocked, ValueTask> OnPredictionLocked = default!;
    /// <summary>
    /// Invoked when the prediction is cancelled/refunded
    /// <para>Requires topic: <see cref="Topics.ChannelPredictions(long, string?)"/></para>
    /// </summary>
    public event Func<ChannelId, IPredictionCancelled, ValueTask> OnPredictionCancelled = default!;
    /// <summary>
    /// Invoked when the listener is timed out from a channel
    /// <para>Requires topic: <see cref="Topics.ChatroomsUser(long, string?)"/></para>
    /// </summary>
    public event Func<UserId, ITimeOutData, ValueTask> OnTimedOut = default!;
    /// <summary>
    /// Invoked when the listener is untimed out from a channel
    /// <para>Requires topic: <see cref="Topics.ChatroomsUser(long, string?)"/></para>
    /// </summary>
    public event Func<UserId, IUntimeOutData, ValueTask> OnUntimedOut = default!;
    /// <summary>
    /// Invoked when the listener is banned from a channel
    /// <para>Requires topic: <see cref="Topics.ChatroomsUser(long, string?)"/></para>
    /// </summary>
    public event Func<UserId, IBanData, ValueTask> OnBanned = default!;
    /// <summary>
    /// Invoked when the listener is unbanned from a channel
    /// <para>Requires topic: <see cref="Topics.ChatroomsUser(long, string?)"/></para>
    /// </summary>
    public event Func<UserId, IUntimeOutData, ValueTask> OnUnbanned = default!;
    /// <summary>
    /// It is not known what this does
    /// <para>If you know what this does, please open an issue describing how it works</para>
    /// <para>Requires topic: <see cref="Topics.ChatroomsUser(long, string?)"/></para>
    /// </summary>
    public event Func<UserId, IAliasRestrictedUpdate, ValueTask> OnAliasRestrictionUpdate = default!;
    /// <summary>
    /// Invoked when a user cheers with bits
    /// <para>Requires topic: <see cref="Topics.BitsEventsV1(long, string?)"/> or <see cref="Topics.BitsEventsV2(long, string?)"/> (difference between these 2 topics is unknown)</para>
    /// </summary>
    public event Func<ChannelId, BitsEvents, ValueTask> OnBitsEvent = default!;
    /// <summary>
    /// Invoked when a user unlocks a new bits badge
    /// <para>Requires topic: <see cref="Topics.BitsBadgeUnlock(long, string?)"/></para>
    /// </summary>
    public event Func<ChannelId, BitsBadgeUnlock, ValueTask> OnBitsBadgeUnlock = default!;
    /// <summary>
    /// Invoked when a user redeems a channel point reward
    /// <para>Requires topic: <see cref="Topics.ChannelPoints(long, string?)"/> or <see cref="Topics.CommunityChannelPoints(long, string?)"/></para>
    /// </summary>
    public event Func<ChannelId, ChannelPoints, ValueTask> OnChannelPointsRedemption = default!;
    /// <summary>
    /// Invoked when a user subscribes
    /// <para>Requires topic: <see cref="Topics.SubscribeEvents(long, string?)"/></para>
    /// </summary>
    public event Func<ChannelId, ISubEvent, ValueTask> OnSub = default!;
    /// <summary>
    /// Invoked when a user gifts a sub
    /// <para>Requires topic: <see cref="Topics.SubscribeEvents(long, string?)"/></para>
    /// </summary>
    public event Func<ChannelId, ISubGiftEvent, ValueTask> OnSubGift = default!;
    /// <summary>
    /// Invoked when an anonymous user gifts a sub
    /// <para>Requires topic: <see cref="Topics.SubscribeEvents(long, string?)"/></para>
    /// </summary>
    public event Func<ChannelId, IAnonSubGiftEvent, ValueTask> OnAnonSubGift = default!;
    /// <summary>
    /// Invoked when a message is caught by automod
    /// <para>Requires topic: <see cref="Topics.AutomodQueue(long, long, string?)"/></para>
    /// </summary>
    public event Func<UserId, ChannelId, AutoModQueue, ValueTask> OnAutoModMessageCaught = default!;
    /// <summary>
    /// Invoked when a low-trusted chat message is treated
    /// <para>Requires topic: <see cref="Topics.LowTrustUsers(long, long, string?)"/></para>
    /// </summary>
    public event Func<UserId, ChannelId, ILowTrustTreatmentMessage, ValueTask> OnLowTrustTreatmentUpdate = default!;
    /// <summary>
    /// Invoked when a low-trusted chat message is sent
    /// <para>Requires topic: <see cref="Topics.LowTrustUsers(long, long, string?)"/></para>
    /// </summary>
    public event Func<UserId, ChannelId, ILowTrustChatMessage, ValueTask> OnLowTrustChatMessage = default!;
    /// <summary>
    /// Invoked when a user's message held by AutoMod has been approved or denied
    /// <para>Requires topic: <see cref="Topics.ModerationNotifications(long, long, string?)"/></para>
    /// </summary>
    public event Func<UserId, ChannelId, CaughtMessageData, ValueTask> OnModerationNotificationMessage = default!;
    /// <summary>
    /// Invoked when a message is pinned by a moderator
    /// <para>Requires topic: <see cref="Topics.PinnedChatUpdates(long, string?)"/></para>
    /// </summary>
    public event Func<ChannelId, IModPinnedMessage, IModPinnedMessageData, ValueTask> OnModPinnedMessage = default!;
    /// <summary>
    /// Invoked when a message is unpinned
    /// <para>Requires topic: <see cref="Topics.PinnedChatUpdates(long, string?)"/></para>
    /// </summary>
    public event Func<ChannelId, IModUnpinnedMessage, ValueTask> OnModUnpinnedMessage = default!;
    /// <summary>
    /// Invoked when the pin status of a message is updated
    /// <para>Requires topic: <see cref="Topics.PinnedChatUpdates(long, string?)"/></para>
    /// </summary>
    public event Func<ChannelId, IModPinnedMessageDataUpdate, ValueTask> OnModPinnedMessageUpdated = default!;
    /// <summary>
    /// Invoked when a Hype Chat message is pinned
    /// <para>Requires topic: <see cref="Topics.PinnedChatUpdates(long, string?)"/></para>
    /// </summary>
    public event Func<ChannelId, IHypeChatPinnedMessage, IHypeChatPinnedMessageData, ValueTask> OnHypeChatMessagePinned = default!;
    /// <summary>
    /// Invoked when a stream goes online
    /// <para>Requires topic: <see cref="Topics.VideoPlayback(long, string?)"/></para>
    /// </summary>
    public event Func<ChannelId, IStreamUp, ValueTask> OnStreamUp = default!;
    /// <summary>
    /// Invoked every few seconds when a stream is online, returning the updated viewer count
    /// <para>Requires topic: <see cref="Topics.VideoPlayback(long, string?)"/></para>
    /// </summary>
    public event Func<ChannelId, IViewerCountUpdate, ValueTask> OnViewerCountUpdate = default!;
    /// <summary>
    /// Invoked when a stream takes an ad break
    /// <para>Requires topic: <see cref="Topics.VideoPlayback(long, string?)"/></para>
    /// </summary>
    public event Func<ChannelId, ICommercialBreak, ValueTask> OnCommercialBreak = default!;
    /// <summary>
    /// Invoked when a stream goes offline
    /// <para>Requires topic: <see cref="Topics.VideoPlayback(long, string?)"/></para>
    /// </summary>
    public event Func<ChannelId, IStreamDown, ValueTask> OnStreamDown = default!;
    /// <summary>
    /// Invoked when the title of a stream is changed
    /// <para>Requires topic: <see cref="Topics.BroadcastSettingsUpdate(long, string?)"/></para>
    /// </summary>
    public event Func<ChannelId, ITitleChange, ValueTask> OnTitleChange = default!;
    /// <summary>
    /// Invoked when the game of a stream is changed
    /// <para>Requires topic: <see cref="Topics.BroadcastSettingsUpdate(long, string?)"/></para>
    /// </summary>
    public event Func<ChannelId, IGameChange, ValueTask> OnGameChange = default!;
    /// <summary>
    /// Invoked when a user is timed out
    /// <para>Requires topic: <see cref="Topics.ModeratorActions(long, long, string?)"/></para>
    /// </summary>
    public event Func<UserId, ChannelId, IUserTimedOut, ValueTask> OnUserTimedOut = default!;
    /// <summary>
    /// Invoked when a user is banned
    /// <para>Requires topic: <see cref="Topics.ModeratorActions(long, long, string?)"/></para>
    /// </summary>
    public event Func<UserId, ChannelId, IUserBanned, ValueTask> OnUserBanned = default!;
    /// <summary>
    /// Invoked when a user is untimed out
    /// <para>Requires topic: <see cref="Topics.ModeratorActions(long, long, string?)"/></para>
    /// </summary>
    public event Func<UserId, ChannelId, IUserUntimedOut, ValueTask> OnUserUntimedOut = default!;
    /// <summary>
    /// Invoked when a user is unbanned
    /// <para>Requires topic: <see cref="Topics.ModeratorActions(long, long, string?)"/></para>
    /// </summary>
    public event Func<UserId, ChannelId, IUserUnbanned, ValueTask> OnUserUnbanned = default!;
    /// <summary>
    /// Invoked when a poll is created
    /// <para>Requires topic: <see cref="Topics.Polls(long, string?)"/></para>
    /// </summary>
    public event Func<ChannelId, IPollCreated, ValueTask> OnPollCreated = default!;
    /// <summary>
    /// Invoked every second when users participate in a poll
    /// <para>Requires topic: <see cref="Topics.Polls(long, string?)"/></para>
    /// </summary>
    public event Func<ChannelId, IPollUpdated, ValueTask> OnPollUpdate = default!;
    /// <summary>
    /// Invoked when a poll ends
    /// <para>Requires topic: <see cref="Topics.Polls(long, string?)"/></para>
    /// </summary>
    public event Func<ChannelId, IPollCompleted, ValueTask> OnPollCompleted = default!;
    /// <summary>
    /// Invoked when a poll is no longer displayed
    /// <para>Requires topic: <see cref="Topics.Polls(long, string?)"/></para>
    /// </summary>
    public event Func<ChannelId, Poll, ValueTask> OnPollArchived = default!;
    /// <summary>
    /// Invoked when a user follows the channel specified
    /// <para>Requires topic: <see cref="Topics.Following(long, string?)"/></para>
    /// </summary>
    public event Func<ChannelId, Follower, ValueTask> OnFollow = default!;
    /// <summary>
    /// Invoked when a "moments" event happens in a channel
    /// <para>Requires topic: <see cref="Topics.CommunityMoments(long, string?)"/></para>
    /// </summary>
    public event Func<ChannelId, CommunityMoments, ValueTask> OnCommunityMoment = default!;
    #endregion

    #region Fields
    private readonly JsonSerializerOptions _sOptions = new()
    {
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        Converters =
        {
            new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseLower),
        }
    };
    private readonly WaitableEvents[] _topicResponses = new[]
    {
        WaitableEvents.ERR_BADAUTH, WaitableEvents.Success, WaitableEvents.ERR_BADMESSAGE,
        WaitableEvents.ERR_BADTOPIC, WaitableEvents.ERR_SERVER, WaitableEvents.ERR_TOO_MANY_TOPICS
    };
    private readonly AsyncEventCoordinator<WaitableEvents> _coordinator = new();
    private readonly HashSet<Topic> _topics = new(10);
    private readonly MessageTemplates _templates;
    private readonly WebSocketClient _ws;
    private readonly ILogger? _logger;
    private CancellationTokenSource _pingerToken = new();
    private ListenResponse _response = default!;
    private Uri _targetUrl = default!;
    private Task _pinger = default!;
    IDisposable? _loggingScope;
    #endregion

    #region Init
    /// <summary>
    /// Initializes the client with the given parameters
    /// </summary>
    /// <param name="authToken">The default authentication token to use in listen/unlisten requests</param>
    /// <param name="logger">The destination for logs. If none is provided then <see cref="DefaultLogger"/> is used</param>
    /// <param name="identifier">A string used to identify this instance in logging</param>
    public PubSubClient(string authToken, ILogger? logger = null, string? identifier = null)
    {
        _ws = new(TimeSpan.FromMinutes(1));
        _logger = logger;
        _templates = new(authToken);
        _loggingScope = GetLogger().BeginScope(identifier is null ? "PubSub" : $"PubSub-Id={identifier}");

        _ws.OnData += ReceiveData;
        _ws.OnConnect += OnWsConnect;
        _ws.OnReconnect += OnWsReconnect;
        _ws.OnDisconnect += OnWsDisconnect;
        _ws.OnLog += Log;
        _ws.OnLogEx += LogException;
    }
    #endregion

    #region Communication
    private async Task Ping()
    {
        await _ws.SendAsync(_templates.Ping());
        if (!await _coordinator.WaitFor(WaitableEvents.PONG, TimeSpan.FromSeconds(10)))
        {
            Log(LogLevel.Error, "PONG not received within the allowed time. Reconnecting...");
            await ReconnectAsync();
        }
    }

    /// <summary>
    /// Listen to multiple PubSub topics
    /// </summary>
    /// <param name="topics">A collection of the desired topics to listen to</param>
    /// <param name="cancellationToken">A cancellation token to stop further execution of asynchronous actions</param>
    /// <returns>An array of responses for all listen requests</returns>
    public async Task<ListenResponse[]> ListenTo(IEnumerable<Topic> topics, CancellationToken cancellationToken = default)
    {
        IReadOnlyList<Topic> arr = topics is IReadOnlyList<Topic> listImpl ? listImpl : topics.ToArray();
        var results = new ListenResponse[arr.Count];
        for (int i = 0; i < arr.Count && !cancellationToken.IsCancellationRequested; i++)
        {
            results[i] = await ListenTo(arr[i], cancellationToken);
        }

        return results;
    }

    /// <summary>
    /// Listen to a PubSub topic
    /// </summary>
    /// <param name="topic">The desired topic to listen to</param>
    /// <param name="cancellationToken">A cancellation token to stop further execution of asynchronous actions</param>
    /// <returns>A response to the listen request</returns>
    public async Task<ListenResponse> ListenTo(Topic topic, CancellationToken cancellationToken = default)
    {
        if (_topics.Count is >= 90 and < 100)
            Log(LogLevel.Warning, "Approaching maximum allowed topics on a single connection (100)");

        string msg = _templates.Listen(topic);
        await _ws.SendAsync(msg, sensitive: true, cancellationToken: cancellationToken);
        TimeSpan timeout = TimeSpan.FromSeconds(5);
        WaitableEvents returnedEvent;
        try
        {
            returnedEvent = await _coordinator.WaitForAny(_topicResponses, timeout, cancellationToken);
        }
        catch (TimeoutException)
        {
            Log(LogLevel.Error, "Listening to topic '{TopicName}' timed out ({Timeout})", topic.Key, timeout);
            return new() { Error = ResponseError.Response_Timeout, TopicKey = _response.TopicKey };
        }

        if (returnedEvent == WaitableEvents.Success)
            _ = _topics.Add(topic);

        return new() { Error = (ResponseError)returnedEvent, TopicKey = _response.TopicKey };
    }

    /// <summary>
    /// Unlisten to multiple PubSub topics
    /// </summary>
    /// <param name="topics">A collection of the topics to unlisten to</param>
    /// <param name="cancellationToken">A cancellation token to stop further execution of asynchronous actions</param>
    /// <returns>An array of responses for all unlisten requests</returns>
    public async Task<ListenResponse[]> UnlistenTo(IEnumerable<Topic> topics, CancellationToken cancellationToken = default)
    {
        IReadOnlyList<Topic> arr = topics is IReadOnlyList<Topic> listImpl ? listImpl : topics.ToArray();
        var results = new ListenResponse[arr.Count];
        for (int i = 0; i < arr.Count && !cancellationToken.IsCancellationRequested; i++)
        {
            results[i] = await UnlistenTo(arr[i], cancellationToken);
        }

        return results;
    }

    /// <summary>
    /// Unlisten to a PubSub topic
    /// </summary>
    /// <param name="topic">The topic to unlisten to</param>
    /// <param name="cancellationToken">A cancellation token to stop further execution of asynchronous actions</param>
    /// <returns>A response to the unlisten request</returns>
    public async Task<ListenResponse> UnlistenTo(Topic topic, CancellationToken cancellationToken = default)
    {
        if (!_topics.Contains(topic))
        {
            Log(LogLevel.Error, "Cannot unlisten to topic {TopicKey} because the client is not listening to it", topic.Key);
            return new() { Error = ResponseError.None, TopicKey = topic.Key };
        }

        string msg = _templates.Unlisten(topic);
        await _ws.SendAsync(msg, sensitive: true, cancellationToken: cancellationToken);
        TimeSpan timeout = TimeSpan.FromSeconds(5);
        WaitableEvents returnedEvent;
        try
        {
            returnedEvent = await _coordinator.WaitForAny(_topicResponses, timeout, cancellationToken);
        }
        catch (TimeoutException)
        {
            return new() { Error = ResponseError.Response_Timeout, TopicKey = _response.TopicKey };
        }

        if (returnedEvent == WaitableEvents.Success)
            _ = _topics.Remove(topic);

        return new() { Error = (ResponseError)returnedEvent, TopicKey = _response.TopicKey };
    }
    #endregion

    #region Connection
    /// <summary>
    /// Fire-and-forget a connection attempt to the Twitch PubSub service
    /// </summary>
    /// <param name="url">Url of the PubSub service</param>
    public void Connect(string url = "wss://pubsub-edge.twitch.tv") => ConnectAsync(url).StepOver();

    /// <summary>
    /// Attempt to connect to the Twitch PubSub service asynchronously
    /// </summary>
    /// <param name="url">Url of the PubSub service</param>
    /// <param name="cancellationToken">A cancellation token to stop further execution of asynchronous actions</param>
    /// <returns><see langword="true"/> if the connection was established successfully; Otherwise <see langword="false"/></returns>
    public async Task<bool> ConnectAsync(string url = "wss://pubsub-edge.twitch.tv", CancellationToken cancellationToken = default)
    {
        _targetUrl = new(url);
        await _ws.Start(_targetUrl, cancellationToken);
        if (await _coordinator.WaitFor(WaitableEvents.Connected, TimeSpan.FromSeconds(15), cancellationToken))
            return true;

        Log(LogLevel.Critical, "Connection timed out.");
        return false;
    }

    /// <summary>
    /// Fire-and-forget a disconnection attempt to the Twitch PubSub service
    /// <para>Note: To reconnect, you must use <see cref="ReconnectAsync(CancellationToken)"/></para>
    /// </summary>
    public void Disconnect() => _ws.Disconnect().StepOver();

    /// <summary>
    /// Asynchronously disconnect from the Twitch PubSub service
    /// <para>Note: To reconnect, you must use <see cref="ReconnectAsync(CancellationToken)"/></para>
    /// </summary>
    public Task DisconnectAsync(CancellationToken cancellationToken = default) => _ws.Disconnect(cancellationToken);

    /// <summary>
    /// Asynchronously reconnect (disconnect then connect) to the Twitch PubSub service
    /// </summary>
    public Task ReconnectAsync(CancellationToken cancellationToken = default) => _ws.Restart(TimeSpan.FromSeconds(60), cancellationToken);

    private Task OnWsConnect()
    {
        Log(LogLevel.Information, "Connected");
        _coordinator.Release(WaitableEvents.Connected);
        OnConnect?.Invoke().StepOver(GetExceptionHandler());
        _pinger = Task.Factory.StartNew(PingerTask, TaskCreationOptions.LongRunning);
        Log(LogLevel.Trace, "Started PingerTask");
        return Task.CompletedTask;
    }

    private Task OnWsDisconnect()
    {
        Log(LogLevel.Warning, "Disconnected");
        _pingerToken.Cancel();
        OnDisconnect?.Invoke().StepOver(GetExceptionHandler());
        return Task.CompletedTask;
    }

    private async Task OnWsReconnect()
    {
        Log(LogLevel.Information, "Reconnected");
        _pingerToken = new();
        _pinger = Task.Factory.StartNew(PingerTask, TaskCreationOptions.LongRunning);
        foreach (Topic t in _topics)
            _ = await ListenTo(t);

        OnReconnect?.Invoke().StepOver(GetExceptionHandler());
    }

    private async Task PingerTask()
    {
        while (_ws.IsConnected && !_pingerToken.IsCancellationRequested)
        {
            await Ping();
            await Task.Delay(TimeSpan.FromMinutes(4), _pingerToken.Token);
        }
    }
    #endregion

    #region Utils
    private ILogger GetLogger() => _logger ?? DefaultLogger;
    private Action<Exception> GetExceptionHandler() => this.ExceptionHandler ??= LogEventException;

    private void LogEventException(Exception ex) => LogException(ex, "🚨 Exception caught in an event:");

    private void Log(LogLevel level, string template, params object[] properties) => GetLogger().Log(level, template, properties);

    private void LogException(Exception ex, string template, params object[] properties) => GetLogger().LogError(ex, template, properties);
    #endregion

    #region Parsing
    private void ReceiveData(ReadOnlyMemory<byte> data)
    {
        MessageType type = PubSubParsing.ParseType(data.Span);
        switch (type)
        {
            case MessageType.PONG:
                _coordinator.Release(WaitableEvents.PONG);
                break;

            case MessageType.RECONNECT:
                ReconnectAsync().StepOver(GetExceptionHandler());
                break;

            case MessageType.MESSAGE:
            {
                using TopicInfo info = new(data.Span);
                switch (info.Topic)
                {
                    case MessageTopic.ChannelPredictions:
                        var prediction = data.Span.ReadJsonMessage<ChannelPredictions>(options: _sOptions, logger: GetLogger());
                        if (prediction.Type == "event-created")
                        {
                            OnPredictionStarted?.Invoke(info[0], prediction.Data.Event).StepOver(GetExceptionHandler());
                            return;
                        }

                        switch (prediction.Data.Event.Status)
                        {
                            case "ACTIVE":
                                OnPredictionUpdate?.Invoke(info[0], prediction.Data.Event).StepOver(GetExceptionHandler());
                                break;

                            case "LOCKED":
                                if (!prediction.Data.Event.LockedBy.HasValue)
                                {
                                    OnPredictionWindowClosed?.Invoke(info[0], prediction.Data.Event).StepOver(GetExceptionHandler());
                                    return;
                                }

                                OnPredictionLocked?.Invoke(info[0], prediction.Data.Event).StepOver(GetExceptionHandler());
                                break;

                            case "RESOLVED":
                                OnPredictionEnded?.Invoke(info[0], prediction.Data.Event).StepOver(GetExceptionHandler());
                                break;

                            case "CANCELED":
                                OnPredictionCancelled?.Invoke(info[0], prediction.Data.Event).StepOver(GetExceptionHandler());
                                break;
                        }

                        break;

                    case MessageTopic.ChatroomsUser:
                        var chatrooms = data.Span.ReadJsonMessage<ChatroomsUser>(options: _sOptions, logger: GetLogger());
                        switch (chatrooms.Type)
                        {
                            case "user_moderation_action":
                                switch (chatrooms.Data.Action)
                                {
                                    case "timeout":
                                        OnTimedOut?.Invoke(info[0], chatrooms.Data).StepOver(GetExceptionHandler());
                                        break;

                                    case "untimeout":
                                        OnUntimedOut?.Invoke(info[0], chatrooms.Data).StepOver(GetExceptionHandler());
                                        break;

                                    case "ban":
                                        OnBanned?.Invoke(info[0], chatrooms.Data).StepOver(GetExceptionHandler());
                                        break;

                                    case "unban":
                                        OnUnbanned?.Invoke(info[0], chatrooms.Data).StepOver(GetExceptionHandler());
                                        break;
                                }

                                break;

                            case "channel_banned_alias_restriction_update":
                                OnAliasRestrictionUpdate?.Invoke(info[0], chatrooms.Data).StepOver(GetExceptionHandler());
                                break;
                        }

                        break;

                    case MessageTopic.BitsEventsV1 or MessageTopic.BitsEventsV2:
                        var bits = data.Span.ReadJsonMessage<BitsEvents>(options: _sOptions, logger: GetLogger());
                        OnBitsEvent?.Invoke(info[0], bits).StepOver(GetExceptionHandler());
                        break;

                    case MessageTopic.BitsBadgeUnlock:
                        var bitBadgeUnlock = data.Span.ReadJsonMessage<BitsBadgeUnlock>(options: _sOptions, logger: GetLogger());
                        OnBitsBadgeUnlock?.Invoke(info[0], bitBadgeUnlock).StepOver(GetExceptionHandler());
                        break;

                    case MessageTopic.ChannelPoints:
                        var channelPoints = data.Span.ReadJsonMessage<ChannelPoints>(options: _sOptions, logger: GetLogger());
                        OnChannelPointsRedemption?.Invoke(info[0], channelPoints).StepOver(GetExceptionHandler());
                        break;

                    case MessageTopic.SubscribeEvents:
                        var subEvent = data.Span.ReadJsonMessage<SubscribeEvents>(options: _sOptions, logger: GetLogger());
                        switch (subEvent.Context)
                        {
                            case "sub" or "resub":
                                OnSub?.Invoke(info[0], subEvent).StepOver(GetExceptionHandler());
                                break;

                            case "subgift":
                                OnSubGift?.Invoke(info[0], subEvent).StepOver(GetExceptionHandler());
                                break;

                            case "anonsubgift":
                                OnAnonSubGift?.Invoke(info[0], subEvent).StepOver(GetExceptionHandler());
                                break;
                        }
                        break;

                    case MessageTopic.AutomodQueue:
                        var automod = data.Span.ReadJsonMessage<AutoModQueue>(options: _sOptions, logger: GetLogger());
                        OnAutoModMessageCaught?.Invoke(info[0], info[1], automod).StepOver(GetExceptionHandler());
                        break;

                    case MessageTopic.LowTrustUsers:
                        var lowTrust = data.Span.ReadJsonMessage<LowTrustUser>(options: _sOptions, logger: GetLogger());
                        if (lowTrust.IsTreatmentMessage)
                            OnLowTrustTreatmentUpdate?.Invoke(info[0], info[1], lowTrust).StepOver(GetExceptionHandler());
                        else
                            OnLowTrustChatMessage?.Invoke(info[0], info[1], lowTrust).StepOver(GetExceptionHandler());

                        break;

                    case MessageTopic.ModerationNotifications:
                        var notification = data.Span.ReadJsonMessage<ModerationNotificationMessage>(options: _sOptions, logger: GetLogger());
                        OnModerationNotificationMessage?.Invoke(info[0], info[1], notification.Data).StepOver(GetExceptionHandler());
                        break;

                    case MessageTopic.PinnedChatUpdates:
                        var pinUpdate = data.Span.ReadJsonMessage<PinnedChatUpdates>(options: _sOptions, logger: GetLogger());
                        switch (pinUpdate.Type)
                        {
                            case "pin-message":
                                if (pinUpdate.Data.Message.Type == "MOD")
                                    OnModPinnedMessage?.Invoke(info[0], pinUpdate.Data, pinUpdate.Data.Message).StepOver(GetExceptionHandler());
                                else
                                    OnHypeChatMessagePinned?.Invoke(info[0], pinUpdate.Data, pinUpdate.Data.Message).StepOver(GetExceptionHandler());

                                break;

                            case "unpin-message":
                                OnModUnpinnedMessage?.Invoke(info[0], pinUpdate.Data).StepOver(GetExceptionHandler());
                                break;

                            case "update-message":
                                OnModPinnedMessageUpdated?.Invoke(info[0], pinUpdate.Data.Message).StepOver(GetExceptionHandler());
                                break;
                        }

                        break;

                    case MessageTopic.VideoPlayback:
                        var videoPlayback = data.Span.ReadJsonMessage<VideoPlayback>(options: _sOptions, logger: GetLogger());
                        switch (videoPlayback.Type)
                        {
                            case "stream-up":
                                OnStreamUp?.Invoke(info[0], videoPlayback).StepOver(GetExceptionHandler());
                                break;

                            case "viewcount":
                                OnViewerCountUpdate?.Invoke(info[0], videoPlayback).StepOver(GetExceptionHandler());
                                break;

                            case "commercial":
                                OnCommercialBreak?.Invoke(info[0], videoPlayback).StepOver(GetExceptionHandler());
                                break;

                            case "stream-down" or "tos-strike":
                                OnStreamDown?.Invoke(info[0], videoPlayback).StepOver(GetExceptionHandler());
                                break;

                            default:
                                Log(LogLevel.Warning, "unknown VideoPlayBack type: {VideoPlaybackType}", videoPlayback.Type);
                                break;
                        }

                        break;

                    case MessageTopic.BroadcastSettingsUpdate:
                        var settingsUpdate = data.Span.ReadJsonMessage<BroadcastSettingsUpdate>(options: _sOptions, logger: GetLogger());
                        if (settingsUpdate.NewTitle != settingsUpdate.OldTitle)
                            OnTitleChange?.Invoke(info[0], settingsUpdate).StepOver(GetExceptionHandler());

                        if (settingsUpdate.NewGameId != settingsUpdate.OldGameId)
                            OnGameChange?.Invoke(info[0], settingsUpdate).StepOver(GetExceptionHandler());

                        break;

                    case MessageTopic.ModeratorActions:
                        var modAction = data.Span.ReadJsonMessage<ModeratorActions>(options: _sOptions, logger: GetLogger());
                        switch (modAction.Data.ModerationAction)
                        {
                            case "timeout":
                                OnUserTimedOut?.Invoke(info[0], info[1], modAction.Data).StepOver(GetExceptionHandler());
                                break;

                            case "ban":
                                OnUserBanned?.Invoke(info[0], info[1], modAction.Data).StepOver(GetExceptionHandler());
                                break;

                            case "untimeout":
                                OnUserUntimedOut?.Invoke(info[0], info[1], modAction.Data).StepOver(GetExceptionHandler());
                                break;

                            case "unban":
                                OnUserUnbanned?.Invoke(info[0], info[1], modAction.Data).StepOver(GetExceptionHandler());
                                break;

                            default:
                                Log(LogLevel.Warning, "Received unknown moderation action: {ModerationAction}", modAction.Data.ModerationAction);
                                break;
                        }
                        break;

                    case MessageTopic.Polls:
                        var poll = data.Span.ReadJsonMessage<Polls>(options: _sOptions, logger: GetLogger());
                        switch (poll.Type)
                        {
                            case "POLL_CREATE":
                                OnPollCreated?.Invoke(info[0], poll.Data.Poll).StepOver(GetExceptionHandler());
                                break;

                            case "POLL_UPDATE":
                                OnPollUpdate?.Invoke(info[0], poll.Data.Poll).StepOver(GetExceptionHandler());
                                break;

                            case "POLL_COMPLETE":
                                OnPollCompleted?.Invoke(info[0], poll.Data.Poll).StepOver(GetExceptionHandler());
                                break;

                            case "POLL_ARCHIVE":
                                OnPollArchived?.Invoke(info[0], poll.Data.Poll).StepOver(GetExceptionHandler());
                                break;

                            default:
                                Log(LogLevel.Warning, "Unknown poll type: {PollType}", poll.Type);
                                break;
                        }

                        break;

                    case MessageTopic.CommunityChannelPoints:
                        var cChannelPoints = data.Span.ReadJsonMessage<ChannelPoints>(options: _sOptions, logger: GetLogger());
                        OnChannelPointsRedemption?.Invoke(info[0], cChannelPoints).StepOver(GetExceptionHandler());
                        break;

                    case MessageTopic.Following:
                        var follower = data.Span.ReadJsonMessage<Follower>(options: _sOptions, logger: GetLogger());
                        OnFollow?.Invoke(info[0], follower).StepOver(GetExceptionHandler());
                        break;

                    case MessageTopic.CommunityMoments:
                        var moment = data.Span.ReadJsonMessage<CommunityMoments>(options: _sOptions, logger: GetLogger());
                        OnCommunityMoment.Invoke(info[0], moment).StepOver(GetExceptionHandler());
                        break;
                }

                break;
            }

            case MessageType.RESPONSE:
                _response = PubSubParsing.ParseResponse(data.Span);
                _coordinator.Release((WaitableEvents)_response.Error);
                string listenType = _response.TopicKey[0] == '0' ? "listen" : "unlisten";
                string topicKey = _response.TopicKey[2..];
                if (_response.IsSuccess)
                    Log(LogLevel.Information, "Successful {ListenType} to {TopicKey}", listenType, topicKey);
                else
                    Log(LogLevel.Error, "Failed to {ListenType} to {TopicKey}: {ErrorCode}", listenType, topicKey, _response.Error);

                break;
        }
    }
    #endregion

    /// <summary>
    /// Disconnects and disposes of resources used by <see cref="PubSubClient"/>
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        await _ws.DisposeAsync();
        _coordinator.Dispose();
        _pingerToken.Cancel();
        _pingerToken.Dispose();
        _pinger?.Dispose();
        GC.SuppressFinalize(this);
    }
}
