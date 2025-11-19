using MiniTwitch.Irc.Models;

namespace MiniTwitch.Irc.Interfaces;

/// <summary>
/// Defines the behavior of the Twitch IRC client.
/// </summary>
public interface IIrcClient
{
    /// <inheritdoc cref="IrcClient.OnConnect"/>
    event Func<ValueTask> OnConnect;
    /// <inheritdoc cref="IrcClient.OnReconnect"/>
    event Func<ValueTask> OnReconnect;
    /// <inheritdoc cref="IrcClient.OnDisconnect"/>
    event Func<ValueTask> OnDisconnect;
    /// <inheritdoc cref="IrcClient.OnMessage"/>
    event Func<Privmsg, ValueTask> OnMessage;
    /// <inheritdoc cref="IrcClient.OnGiftedSubNoticeIntro"/>
    event Func<IGiftSubNoticeIntro, ValueTask> OnGiftedSubNoticeIntro;
    /// <inheritdoc cref="IrcClient.OnGiftedSubNotice"/>
    event Func<IGiftSubNotice, ValueTask> OnGiftedSubNotice;
    /// <inheritdoc cref="IrcClient.OnSubscriptionNotice"/>
    event Func<ISubNotice, ValueTask> OnSubscriptionNotice;
    /// <inheritdoc cref="IrcClient.OnRaidNotice"/>
    event Func<IRaidNotice, ValueTask> OnRaidNotice;
    /// <inheritdoc cref="IrcClient.OnPaidUpgradeNotice"/>
    event Func<IPaidUpgradeNotice, ValueTask> OnPaidUpgradeNotice;
    /// <inheritdoc cref="IrcClient.OnPrimeUpgradeNotice"/>
    event Func<IPrimeUpgradeNotice, ValueTask> OnPrimeUpgradeNotice;
    /// <inheritdoc cref="IrcClient.OnAnnouncement"/>
    event Func<IAnnouncementNotice, ValueTask> OnAnnouncement;
    /// <inheritdoc cref="IrcClient.OnUserTimeout"/>
    event Func<IUserTimeout, ValueTask> OnUserTimeout;
    /// <inheritdoc cref="IrcClient.OnUserBan"/>
    event Func<IUserBan, ValueTask> OnUserBan;
    /// <inheritdoc cref="IrcClient.OnChatClear"/>
    event Func<IChatClear, ValueTask> OnChatClear;
    /// <inheritdoc cref="IrcClient.OnMessageDelete"/>
    event Func<Clearmsg, ValueTask> OnMessageDelete;
    /// <inheritdoc cref="IrcClient.OnChannelJoin"/>
    event Func<IrcChannel, ValueTask> OnChannelJoin;
    /// <inheritdoc cref="IrcClient.OnEmoteOnlyModified"/>
    event Func<IEmoteOnlyModified, ValueTask> OnEmoteOnlyModified;
    /// <inheritdoc cref="IrcClient.OnFollowerModeModified"/>
    event Func<IFollowersOnlyModified, ValueTask> OnFollowerModeModified;
    /// <inheritdoc cref="IrcClient.OnUniqueModeModified"/>
    event Func<IR9KModified, ValueTask> OnUniqueModeModified;
    /// <inheritdoc cref="IrcClient.OnSlowModeModified"/>
    event Func<ISlowModeModified, ValueTask> OnSlowModeModified;
    /// <inheritdoc cref="IrcClient.OnSubOnlyModified"/>
    event Func<ISubOnlyModified, ValueTask> OnSubOnlyModified;
    /// <inheritdoc cref="IrcClient.OnChannelPart"/>
    event Func<IPartedChannel, ValueTask> OnChannelPart;
    /// <inheritdoc cref="IrcClient.OnNotice"/>
    event Func<Notice, ValueTask> OnNotice;
    /// <inheritdoc cref="IrcClient.OnUserstate"/>
    event Func<Userstate, ValueTask> OnUserstate;
    /// <inheritdoc cref="IrcClient.OnWhisper"/>
    event Func<Whisper, ValueTask> OnWhisper;
    /// <inheritdoc cref="IrcClient.OnCharityDonation"/>
    event Func<ICharityDonation, ValueTask> OnCharityDonation;
    /// <inheritdoc cref="IrcClient.OnGlobalUserstate"/>
    event Func<GlobalUserstate, ValueTask> OnGlobalUserstate;
    /// <inheritdoc cref="IrcClient.OnViewerMilestone"/>
    event Func<IViewerMilestone, ValueTask> OnViewerMilestone;
    /// <inheritdoc cref="IrcClient.Connect"/>
    void Connect(string url = "wss://irc-ws.chat.twitch.tv:443");
    /// <inheritdoc cref="IrcClient.ConnectAsync"/>
    Task<bool> ConnectAsync(string url = "wss://irc-ws.chat.twitch.tv:443", CancellationToken cancellationToken = default);
    /// <inheritdoc cref="IrcClient.Disconnect"/>
    void Disconnect();
    /// <inheritdoc cref="IrcClient.DisconnectAsync"/>
    Task DisconnectAsync(CancellationToken cancellationToken = default);
    /// <inheritdoc cref="IrcClient.ReconnectAsync"/>
    Task ReconnectAsync(CancellationToken cancellationToken = default);
    /// <inheritdoc cref="IrcClient.JoinChannel(IBasicChannel, CancellationToken)"/>
    Task PartChannel(IBasicChannel channel, CancellationToken cancellationToken = default);
    /// <inheritdoc cref="IrcClient.JoinChannel(string, CancellationToken)"/>
    Task<bool> JoinChannel(string channel, CancellationToken cancellationToken = default);
    /// <inheritdoc cref="IrcClient.JoinChannels(IEnumerable{string}, CancellationToken)"/>
    Task<bool> JoinChannels(IEnumerable<string> channels, CancellationToken cancellationToken = default);
    /// <inheritdoc cref="IrcClient.JoinChannels(IEnumerable{IBasicChannel}, CancellationToken)"/>
    Task PartChannel(string channel, CancellationToken cancellationToken = default);
    /// <inheritdoc cref="IrcClient.JoinChannels(IEnumerable{IBasicChannel}, CancellationToken)"/>
    ValueTask ReplyTo(Privmsg parentMessage, string message, bool action = false, bool replyInThread = false, CancellationToken cancellationToken = default);
    /// <inheritdoc cref="IrcClient.ReplyTo(string, string, string, bool, CancellationToken)"/>
    ValueTask ReplyTo(string messageId, string channel, string reply, bool action = false, CancellationToken cancellationToken = default);
    /// <inheritdoc cref="IrcClient.SendRaw(string, CancellationToken)"/>
    ValueTask SendRaw(string message, CancellationToken cancellationToken = default);
}
