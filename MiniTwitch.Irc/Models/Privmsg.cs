using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Text;
using MiniTwitch.Common.Extensions;
using MiniTwitch.Common.Interaction;
using MiniTwitch.Irc.Enums;
using MiniTwitch.Irc.Interfaces;
using MiniTwitch.Irc.Internal.Enums;
using MiniTwitch.Irc.Internal.Models;
using MiniTwitch.Irc.Internal.Parsing;

namespace MiniTwitch.Irc.Models;

/// <summary>
/// Represents a chat message
/// <para>Twitch docs: <see href="https://dev.twitch.tv/docs/irc/tags/#privmsg-tags"/></para>
/// <para>Note: Object can be implicitly converted to <see langword="string"/>, which returns <see cref="Privmsg.Content"/></para>
/// </summary>
public readonly struct Privmsg : IUnixTimestamped, IHelixMessageTarget, IEquatable<Privmsg>
{
    /// <summary>
    /// Author of the message
    /// </summary>
    public MessageAuthor Author { get; }
    /// <summary>
    /// Reply contents of the message
    /// <para>Note: If <see cref="MessageReply.HasContent"/> is <see langword="false"/>, strings are <see cref="string.Empty"/> and numbers are <see langword="0"/></para>
    /// </summary>
    public MessageReply Reply { get; init; }
    /// <summary>
    /// The channel where the message was sent
    /// </summary>
    public IBasicChannel Channel { get; init; }
    /// <summary>
    /// Content of the message
    /// </summary>
    public string Content { get; init; }
    /// <summary>
    /// Emote sets in the content of the message
    /// <para><see cref="string.Empty"/> if there are none</para>
    /// </summary>
    public string Emotes { get; init; }
    /// <summary>
    /// Automod flags in the content of the message
    /// <para><see cref="string.Empty"/> if there are none</para>
    /// </summary>
    public string Flags { get; init; }
    /// <summary>
    /// Unique ID to identify the message
    /// </summary>
    public string Id { get; init; }
    /// <summary>
    /// The amount of bits cheered in the message
    /// <para>Default is 0</para>
    /// </summary>
    public int Bits { get; init; }
    /// <summary>
    /// Client nonce that was sent with the message
    /// <para>Note: Can be <see cref="string.Empty"/></para>
    /// </summary>
    public string Nonce { get; init; }
    /// <summary>
    /// Whether the was the author's first message in the channel
    /// </summary>
    public bool IsFirstMessage { get; init; }
    /// <summary>
    /// Whether the message is an action. Action messages are sent with .me
    /// </summary>
    public bool IsAction { get; init; }
    /// <summary>
    /// Whether the user is a returning chatter
    /// <para>This tag is not documented <see href="https://dev.twitch.tv/docs/irc/tags/#privmsg-tags"/> </para>
    /// </summary>
    public bool IsReturningChatter { get; init; }
    /// <summary>
    /// The ID of the custom reward that was redeemed if the message was sent with a custom reward
    /// </summary>
    public string CustomRewardId { get; init; }
    /// <summary>
    /// Whether the message is highlighted.
    /// </summary>
    public bool IsHighlighted { get; init; }
    /// <summary>
    /// Information about the message animation.
    /// </summary>
    public MessageAnimation Animation { get; init; }
    /// <summary>
    /// Whether the emotes in the message are gigantified
    /// </summary>
    public bool IsGigantifiedEmoteMessage { get; init; }
    /// <summary>
    /// Source information about the message.
    /// <para>Only populated if <see cref="MessageSource.HasSource"/> is <see langword="true"/></para>
    /// </summary>
    public MessageSource Source { get; init; }

    /// <inheritdoc/>
    public long TmiSentTs { get; init; }
    /// <inheritdoc/>
    public DateTimeOffset SentTimestamp => DateTimeOffset.FromUnixTimeMilliseconds(this.TmiSentTs);

    internal IrcClient? SourceClient { get; init; }

    internal Privmsg(ref IrcMessage message, IrcClient? source = null)
    {
        this.SourceClient = source;

        // MessageAuthor
        string badges = string.Empty;
        string badgeInfo = string.Empty;
        Color color = default;
        string displayName = string.Empty;
        string username = message.GetUsername();
        long uid = 0;
        bool mod = false;
        bool sub = false;
        bool turbo = false;
        bool vip = false;
        UserType userType = UserType.None;

        // MessageReply
        string replyMessageId = string.Empty;
        long replyUserId = 0;
        string replyMessageBody = string.Empty;
        string replyUsername = string.Empty;
        string replyDisplayName = string.Empty;
        string threadParentMessageid = string.Empty;
        string threadParentUsername = string.Empty;

        // IBasicChannel
        string channelName = message.GetChannel();
        long channelId = 0;

        (this.Content, this.IsAction) = message.GetContent(maybeAction: true);
        string emotes = string.Empty;
        string flags = string.Empty;
        string id = string.Empty;
        int bits = 0;
        string nonce = string.Empty;
        long tmiSentTs = 0;
        bool firstMsg = false;
        bool returningChatter = false;
        bool highlighted = false;
        string customRewardId = string.Empty;

        // bit stuff
        bool gigantified = false;
        bool animated = false;
        string animation = string.Empty;

        // MessageSource
        string sourceBadgeInfo = string.Empty;
        string sourceBadges = string.Empty;
        string sourceId = string.Empty;
        long sourceRoomId = 0;
        bool sourceOnly = false;

        using IrcTags tags = message.ParseTags();
        foreach (IrcTag tag in tags)
        {
            ReadOnlySpan<byte> tagKey = tag.Key.Span;
            ReadOnlySpan<byte> tagValue = tag.Value.Span;

            switch (tagKey.Length)
            {
                //id
                case (int)Tags.Id when tagKey.SequenceEqual("id"u8):
                    id = TagHelper.GetString(tagValue);
                    break;

                //mod
                case (int)Tags.Mod when tagKey.SequenceEqual("mod"u8):
                    mod = TagHelper.GetBool(tagValue);
                    break;

                //vip
                case (int)Tags.Vip when tagKey.SequenceEqual("vip"u8):
                    vip = true;
                    break;

                //bits
                case (int)Tags.Bits when tagKey.SequenceEqual("bits"u8):
                    bits = TagHelper.GetInt(tagValue);
                    break;

                //flags
                case (int)Tags.Flags when tagKey.SequenceEqual("flags"u8):
                    flags = TagHelper.GetString(tagValue);
                    break;

                //color
                case (int)Tags.Color when tagKey.SequenceEqual("color"u8):
                    color = TagHelper.GetColor(tagValue);
                    break;

                //turbo
                case (int)Tags.Turbo when tagKey.SequenceEqual("turbo"u8):
                    turbo = TagHelper.GetBool(tagValue);
                    break;

                //badges
                case (int)Tags.Badges when tagKey.SequenceEqual("badges"u8):
                    badges = TagHelper.GetString(tagValue, true);
                    break;

                //emotes
                case (int)Tags.Emotes when tagKey.SequenceEqual("emotes"u8):
                    emotes = TagHelper.GetString(tagValue);
                    break;

                //room-id
                case (int)Tags.RoomId when tagKey.SequenceEqual("room-id"u8):
                    channelId = TagHelper.GetLong(tagValue);
                    break;

                //user-id
                case (int)Tags.UserId when tagKey.SequenceEqual("user-id"u8):
                    uid = TagHelper.GetLong(tagValue);
                    break;

                //first-msg
                case (int)Tags.FirstMsg when tagKey.SequenceEqual("first-msg"u8):
                    firstMsg = TagHelper.GetBool(tagValue);
                    break;

                //msg-id
                case (int)Tags.MsgId when tagKey.SequenceEqual("msg-id"u8):
                    switch (tagValue.Sum())
                    {
                        case 2516:
                            gigantified = true;
                            break;

                        case 1939:
                            highlighted = true;
                            break;

                        case 1621:
                            animated = true;
                            break;
                    }
                    break;

                //user-type
                case (int)Tags.UserType when tagKey.SequenceEqual("user-type"u8) && tagValue.Length > 0:
                    userType = tagValue.Length switch
                    {
                        3 when tagValue.SequenceEqual("mod"u8) => UserType.Mod,
                        5 when tagValue.SequenceEqual("admin"u8) => UserType.Admin,
                        5 when tagValue.SequenceEqual("staff"u8) => UserType.Staff,
                        10 when tagValue.SequenceEqual("global_mod"u8) => UserType.GlobalModerator,
                        _ => UserType.None
                    };
                    break;

                //badge-info
                case (int)Tags.BadgeInfo when tagKey.SequenceEqual("badge-info"u8):
                    badgeInfo = TagHelper.GetString(tagValue, true, true);
                    break;

                //subscriber
                case (int)Tags.Subscriber when tagKey.SequenceEqual("subscriber"u8):
                    sub = TagHelper.GetBool(tagValue);
                    break;

                //animation-id
                case (int)Tags.AnimationId when tagKey.SequenceEqual("animation-id"u8):
                    animation = TagHelper.GetString(tagValue, intern: true);
                    break;

                //tmi-sent-ts
                case (int)Tags.TmiSentTs when tagKey.SequenceEqual("tmi-sent-ts"u8):
                    tmiSentTs = TagHelper.GetLong(tagValue);
                    break;

                //client-nonce
                case (int)Tags.ClientNonce when tagKey.SequenceEqual("client-nonce"u8):
                    nonce = TagHelper.GetString(tagValue);
                    break;

                //display-name
                case (int)Tags.DisplayName when tagKey.SequenceEqual("display-name"u8):
                    displayName = TagHelper.GetString(tagValue);
                    break;

                //returning-chatter
                case (int)Tags.ReturningChatter when tagKey.SequenceEqual("returning-chatter"u8):
                    returningChatter = TagHelper.GetBool(tagValue);
                    break;

                //reply-parent-msg-id
                case (int)Tags.ReplyParentMsgId when tagKey.SequenceEqual("reply-parent-msg-id"u8):
                    replyMessageId = TagHelper.GetString(tagValue);
                    break;

                //reply-parent-user-id
                case (int)Tags.ReplyParentUserId when tagKey.SequenceEqual("reply-parent-user-id"u8):
                    replyUserId = TagHelper.GetLong(tagValue);
                    break;

                //reply-parent-msg-body
                case (int)Tags.ReplyParentMsgBody when tagKey.SequenceEqual("reply-parent-msg-body"u8):
                    replyMessageBody = TagHelper.GetString(tagValue, unescape: true);
                    break;

                //reply-parent-user-login
                case (int)Tags.ReplyParentUserLogin when tagKey.SequenceEqual("reply-parent-user-login"u8):
                    replyUsername = TagHelper.GetString(tagValue);
                    break;

                //reply-parent-display-name
                case (int)Tags.ReplyParentDisplayName when tagKey.SequenceEqual("reply-parent-display-name"u8):
                    replyDisplayName = TagHelper.GetString(tagValue);
                    break;

                //reply-thread-parent-msg-id
                case (int)Tags.ReplyThreadParentMsgId when tagKey.SequenceEqual("reply-thread-parent-msg-id"u8):
                    threadParentMessageid = TagHelper.GetString(tagValue);
                    break;

                //reply-thread-parent-user-login
                case (int)Tags.ReplyThreadParentUserLogin when tagKey.SequenceEqual("reply-thread-parent-user-login"u8):
                    threadParentUsername = TagHelper.GetString(tagValue);
                    break;

                //custom-reward-id
                case (int)Tags.CustomRewardId when tagKey.SequenceEqual("custom-reward-id"u8):
                    customRewardId = TagHelper.GetString(tagValue);
                    break;

                //source-badge-info
                case (int)Tags.SourceBadgeInfo when tagKey.SequenceEqual("source-badge-info"u8):
                    sourceBadgeInfo = TagHelper.GetString(tagValue, intern: true, unescape: true);
                    break;

                //source-badges
                case (int)Tags.SourceBadges when tagKey.SequenceEqual("source-badges"u8):
                    sourceBadges = TagHelper.GetString(tagValue, intern: true);
                    break;

                //source-id
                case (int)Tags.SourceId when tagKey.SequenceEqual("source-id"u8):
                    sourceId = TagHelper.GetString(tagValue);
                    break;

                //source-room-id
                case (int)Tags.SourceRoomId when tagKey.SequenceEqual("source-room-id"u8):
                    sourceRoomId = TagHelper.GetLong(tagValue);
                    break;

                //source-only
                case (int)Tags.SourceOnly when tagKey.SequenceEqual("source-only"u8):
                    sourceOnly = TagHelper.GetBool(tagValue);
                    break;
            }
        }

        this.Author = new MessageAuthor()
        {
            BadgeInfo = badgeInfo,
            Badges = badges,
            ChatColor = color,
            DisplayName = displayName,
            Id = uid,
            IsMod = mod,
            IsSubscriber = sub,
            Type = userType,
            Name = username,
            IsTurbo = turbo,
            IsVip = vip
        };
        this.Reply = new MessageReply()
        {
            ParentMessageId = replyMessageId,
            ParentDisplayName = replyDisplayName,
            ParentMessage = replyMessageBody,
            ParentUserId = replyUserId,
            ParentUsername = replyUsername,
            ParentThreadMessageId = threadParentMessageid,
            ParentThreadUsername = threadParentUsername
        };
        this.Channel = new IrcChannel()
        {
            Name = channelName,
            Id = channelId
        };
        this.Emotes = emotes;
        this.Flags = flags;
        this.Id = id;
        this.Bits = bits;
        this.Nonce = nonce;
        this.TmiSentTs = tmiSentTs;
        this.IsFirstMessage = firstMsg;
        this.IsReturningChatter = returningChatter;
        this.CustomRewardId = customRewardId;
        this.IsHighlighted = highlighted;
        this.IsGigantifiedEmoteMessage = gigantified;
        this.Animation = new MessageAnimation()
        {
            IsAnimated = animated,
            AnimationId = animation
        };
        this.Source = new MessageSource()
        {
            BadgeInfo = sourceBadgeInfo,
            Badges = sourceBadges,
            ChannelId = sourceRoomId,
            MessageId = sourceId,
            SourceOnly = sourceOnly
        };
    }

    /// <summary>
    /// Reply to the message
    /// </summary>
    /// <param name="reply">The reply to send</param>
    /// <param name="action">Prepend .me</param>
    /// <param name="replyInThread">Prefer replying to the target message in the same thread instead of creating a new one</param>
    /// <param name="cancellationToken">A cancellation token to stop further execution of asynchronous actions</param>
    public ValueTask ReplyWith(string reply, bool action = false, bool replyInThread = false, CancellationToken cancellationToken = default) =>
        this.SourceClient?.ReplyTo(this, reply, action, replyInThread, cancellationToken) ?? ValueTask.CompletedTask;

    /// <summary>
    /// Construct a message from a string. Useful for testing
    /// </summary>
    /// <param name="rawData">The raw IRC message <para>Example input: @badge-info=subscriber/10;badges=subscriber/6;color=#F2647B;display-name=occluder;emotes=;first-msg=0;flags=;id=1eef01e3-634a-493b-b1a7-4f65040fa986;mod=0;returning-chatter=0;room-id=11148817;subscriber=1;tmi-sent-ts=1679231590118;turbo=0;user-id=783267696;user-type= :occluder!occluder@occluder.tmi.twitch.tv PRIVMSG #pajlada :-tags lol!</para></param>
    /// <returns><see cref="Privmsg"/> with the related data</returns>
    public static Privmsg Construct(string rawData)
    {
        ReadOnlyMemory<byte> memory = new(Encoding.UTF8.GetBytes(rawData));
        var message = new IrcMessage(memory);
        return new(ref message);
    }

    /// <inheritdoc/>
    public bool Equals(Privmsg other) => other.Id == this.Id;

    /// <inheritdoc/>
    public bool Equals(string? other) => this.Content.Equals(other);

    // Don't remove this, compiler wont shut up
#pragma warning disable CS8765
    /// <inheritdoc/>
    public override bool Equals([NotNull] object obj) => obj is Privmsg && Equals((Privmsg)obj);
#pragma warning restore CS8765
    /// <inheritdoc/>
    public static bool operator ==(Privmsg left, Privmsg right) => left.Equals(right);
    /// <inheritdoc/>
    public static bool operator !=(Privmsg left, Privmsg right) => !(left == right);
    /// <inheritdoc/>
    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(this.Id);
        hash.Add(this.Content);
        return hash.ToHashCode();
    }

    /// <inheritdoc/>
    public static implicit operator string(Privmsg message) => message.Content;
}