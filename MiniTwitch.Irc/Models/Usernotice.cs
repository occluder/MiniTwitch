using System.Drawing;
using System.Text;
using MiniTwitch.Common.Extensions;
using MiniTwitch.Irc.Enums;
using MiniTwitch.Irc.Interfaces;
using MiniTwitch.Irc.Internal.Enums;
using MiniTwitch.Irc.Internal.Models;
using MiniTwitch.Irc.Internal.Parsing;

namespace MiniTwitch.Irc.Models;

/// <summary>
/// Represents the USERNOTICE command
/// <para>Twitch docs: <see href="https://dev.twitch.tv/docs/irc/commands/#usernotice"/>, <see href="https://dev.twitch.tv/docs/irc/tags/#usernotice-tags"/></para>
/// </summary>
public readonly struct Usernotice : IGiftSubNoticeIntro, IAnnouncementNotice, IPaidUpgradeNotice,
    ISubNotice, IGiftSubNotice, IRaidNotice, IPrimeUpgradeNotice, IEquatable<Usernotice>,
    ICharityDonation, IViewerMilestone
{
    private const string VIP_ROLE = "vip/1";

    /// <inheritdoc/>
    public MessageAuthor Author { get; init; }
    /// <inheritdoc/>
    public IGiftSubRecipient Recipient { get; init; }
    /// <inheritdoc/>
    public IBasicChannel Channel { get; init; }
    /// <inheritdoc/>
    public SubPlan SubPlan { get; init; }
    /// <inheritdoc/>
    public AnnouncementColor Color { get; init; }
    /// <inheritdoc/>
    public string Emotes { get; init; }
    /// <inheritdoc/>
    public string Flags { get; init; }
    /// <inheritdoc/>
    public string Id { get; init; }
    /// <inheritdoc/>
    public string SubPlanName { get; init; }
    /// <inheritdoc/>
    public string SystemMessage { get; init; }
    /// <inheritdoc/>
    public string Message { get; init; }
    /// <inheritdoc/>
    public string GifterUsername { get; init; }
    /// <inheritdoc/>
    public string GifterDisplayName { get; init; }
    /// <inheritdoc/>
    public string CharityName { get; init; }
    /// <inheritdoc/>
    public double DonationAmount { get; init; }
    /// <inheritdoc/>
    public CurrencyCode DonationCurrency { get; init; }
    /// <inheritdoc/>
    public int CumulativeMonths { get; init; }
    /// <inheritdoc/>
    public int Months { get; init; }
    /// <inheritdoc/>
    public int MonthStreak { get; init; }
    /// <inheritdoc/>
    public int GiftedMonths { get; init; }
    /// <inheritdoc/>
    public int GiftCount { get; init; }
    /// <inheritdoc/>
    public int TotalGiftCount { get; init; }
    /// <inheritdoc/>
    public int ViewerCount { get; init; }
    /// <inheritdoc/>
    public bool ShouldShareStreak { get; init; }
    /// <inheritdoc/>
    public ulong CommunityGiftId { get; init; }
    /// <summary>
    /// Source information about the message.
    /// <para>Only populated if <see cref="NoticeSource.HasSource"/> is <see langword="true"/></para>
    /// </summary>
    public NoticeSource Source { get; init; }
    /// <summary>
    /// Information about the channel's goal.
    /// <para>Check <see cref="ChannelGoal.HasGoal"/> to see if there is a goal</para>
    /// </summary>
    public ChannelGoal ChannelGoal { get; init; }
    /// <inheritdoc/>
    public int ConsecutiveStreamsWatched { get; init; }
    /// <inheritdoc/>
    public int Reward { get; init; }
    /// <inheritdoc/>
    public string Category { get; init; }

    /// <inheritdoc/>
    public long TmiSentTs { get; init; } = default;
    /// <inheritdoc/>
    public DateTimeOffset SentTimestamp => DateTimeOffset.FromUnixTimeMilliseconds(this.TmiSentTs);

    internal UsernoticeType MsgId { get; init; } = UsernoticeType.Unknown;

    internal Usernotice(ref IrcMessage message)
    {
        // Author
        bool isMod = false;
        Color colorCode = default;
        string badges = string.Empty;
        long userId = 0;
        UserType userType = UserType.None;
        string badgeInfo = string.Empty;
        bool isSubscriber = false;
        string displayName = string.Empty;
        string username = string.Empty;
        bool isTurbo = false;

        // Recipient
        string recipientDisplayName = string.Empty;
        string recipientUsername = string.Empty;
        long recipientId = 0;

        // Channel
        string channelName = message.GetChannel();
        long channelId = 0;

        SubPlan subPlan = SubPlan.None;
        AnnouncementColor color = AnnouncementColor.Unknown;
        string emotes = string.Empty;
        string flags = string.Empty;
        string id = string.Empty;
        string subPlanName = string.Empty;
        string systemMessage = string.Empty;
        string content = string.Empty;
        string gifterUsername = string.Empty;
        string gifterDisplayName = string.Empty;
        int cumulativeMonths = 0;
        int months = 0;
        int monthStreak = 0;
        int giftedMonths = 0;
        int giftCount = 0;
        int totalGiftCount = 0;
        int viewerCount = 0;
        bool shouldShareStreak = false;
        ulong communityGiftId = 0;

        string charityName = string.Empty;
        int donationAmount = 0;
        int donationExponent = 0;
        double actualDonationAmount = 0;
        CurrencyCode donationCurrency = CurrencyCode.None;

        // NoticeSource
        string sourceBadgeInfo = string.Empty;
        string sourceBadges = string.Empty;
        string sourceId = string.Empty;
        long sourceRoomId = 0;
        UsernoticeType srcMsgId = UsernoticeType.Unknown;

        // ChannelGoal
        string goalDescription = string.Empty;
        int goalTarget = 0;
        int goalCurrent = 0;
        int goalUser = 0;

        // for msg-id=viewermilestone
        int consecutiveStreamsWatched = 0;
        int reward = 0;
        string category = string.Empty;

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
                    isMod = TagHelper.GetBool(tagValue);
                    break;

                //flags
                case (int)Tags.Flags when tagKey.SequenceEqual("flags"u8):
                    flags = TagHelper.GetString(tagValue);
                    break;

                //login
                case (int)Tags.Login when tagKey.SequenceEqual("login"u8):
                    username = TagHelper.GetString(tagValue);
                    break;

                //color
                case (int)Tags.Color when tagKey.SequenceEqual("color"u8):
                    colorCode = TagHelper.GetColor(tagValue);
                    break;

                //turbo 
                case (int)Tags.Turbo when tagKey.SequenceEqual("turbo"u8):
                    isTurbo = TagHelper.GetBool(tagValue);
                    break;

                //msg-id
                case (int)Tags.MsgId when tagKey.SequenceEqual("msg-id"u8):
                    this.MsgId = tagValue.Length switch
                    {
                        (int)UsernoticeType.Sub when tagValue.SequenceEqual("sub"u8) => UsernoticeType.Sub,
                        (int)UsernoticeType.Raid when tagValue.SequenceEqual("raid"u8) => UsernoticeType.Raid,
                        (int)UsernoticeType.Resub when tagValue.SequenceEqual("resub"u8) => UsernoticeType.Resub,
                        (int)UsernoticeType.Unraid when tagValue.SequenceEqual("unraid"u8) => UsernoticeType.Unraid,
                        (int)UsernoticeType.Subgift when tagValue.SequenceEqual("subgift"u8) => UsernoticeType.Subgift,
                        (int)UsernoticeType.Announcement when tagValue.SequenceEqual("announcement"u8) => UsernoticeType.Announcement,
                        (int)UsernoticeType.BitsBadgeTier when tagValue.SequenceEqual("bitsbadgetier"u8) => UsernoticeType.BitsBadgeTier,
                        (int)UsernoticeType.SubMysteryGift when tagValue.SequenceEqual("submysterygift"u8) => UsernoticeType.SubMysteryGift,
                        (int)UsernoticeType.GiftPaidUpgrade when tagValue.SequenceEqual("giftpaidupgrade"u8) => UsernoticeType.GiftPaidUpgrade,
                        15 when tagValue.SequenceEqual("charitydonation"u8) => UsernoticeType.CharityDonation,
                        15 when tagValue.SequenceEqual("viewermilestone"u8) => UsernoticeType.ViewerMilestone,
                        (int)UsernoticeType.PrimePaidUpgrade when tagValue.SequenceEqual("primepaidupgrade"u8) => UsernoticeType.PrimePaidUpgrade,
                        (int)UsernoticeType.StandardPayForward when tagValue.SequenceEqual("standardpayforward"u8) => UsernoticeType.StandardPayForward,
                        (int)UsernoticeType.AnonGiftPaidUpgrade when tagValue.SequenceEqual("anongiftpaidupgrade"u8) => UsernoticeType.AnonGiftPaidUpgrade,
                        16 when tagValue.SequenceEqual("sharedchatnotice"u8) => UsernoticeType.SharedChatNotice,
                        _ => UsernoticeType.Unknown,
                    };
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
                    userId = TagHelper.GetLong(tagValue);
                    break;

                //user-type
                case (int)Tags.UserType when tagValue.Length > 0 && tagKey.SequenceEqual("user-type"u8):
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

                //system-msg
                case (int)Tags.SystemMsg when tagKey.SequenceEqual("system-msg"u8):
                    systemMessage = TagHelper.GetString(tagValue, unescape: true);
                    break;

                //subscriber
                case (int)Tags.Subscriber when tagKey.SequenceEqual("subscriber"u8):
                    isSubscriber = TagHelper.GetBool(tagValue);
                    break;

                //tmi-sent-ts
                case (int)Tags.TmiSentTs when tagKey.SequenceEqual("tmi-sent-ts"u8):
                    this.TmiSentTs = TagHelper.GetLong(tagValue);
                    break;

                //display-name
                case (int)Tags.DisplayName when tagKey.SequenceEqual("display-name"u8):
                    displayName = TagHelper.GetString(tagValue);
                    break;

                //msg-param-color
                case (int)Tags.MsgParamColor when tagKey.SequenceEqual("msg-param-color"u8):
                    color = (AnnouncementColor)tagValue.Sum();
                    break;

                //msg-param-value
                case (int)Tags.MsgParamValue when tagKey.SequenceEqual("msg-param-value"u8):
                    // This might be used in the future for other notice types
                    switch (MsgId)
                    {
                        case UsernoticeType.ViewerMilestone:
                            consecutiveStreamsWatched = TagHelper.GetInt(tagValue);
                            break;
                    }
                    break;

                //msg-param-months
                case (int)Tags.MsgParamMonths when tagKey.SequenceEqual("msg-param-months"u8):
                    months = TagHelper.GetInt(tagValue);
                    break;

                //msg-param-sub-plan
                case (int)Tags.MsgParamSubPlan when tagKey.SequenceEqual("msg-param-sub-plan"u8):
                    subPlan = (SubPlan)tagValue.Sum();
                    break;

                //msg-param-category
                case (int)Tags.MsgParamCategory when tagKey.SequenceEqual("msg-param-category"u8):
                    category = TagHelper.GetString(tagValue, intern: true);
                    break;

                //msg-param-copoReward
                case (int)Tags.MsgParamCopoReward when tagKey.SequenceEqual("msg-param-copoReward"u8):
                    reward = TagHelper.GetInt(tagValue);
                    break;

                //msg-param-sender-name
                case (int)Tags.MsgParamSenderName when tagKey.SequenceEqual("msg-param-sender-name"u8):
                    gifterDisplayName = TagHelper.GetString(tagValue);
                    break;

                //msg-param-gift-months
                case (int)Tags.MsgParamGiftMonths when tagKey.SequenceEqual("msg-param-gift-months"u8):
                    giftedMonths = TagHelper.GetInt(tagValue);
                    break;

                //msg-param-viewerCount
                case (int)Tags.MsgParamViewerCount when tagKey.SequenceEqual("msg-param-viewerCount"u8):
                    viewerCount = TagHelper.GetInt(tagValue);
                    break;

                //msg-param-recipient-id
                case (int)Tags.MsgParamRecipientId when tagKey.SequenceEqual("msg-param-recipient-id"u8):
                    recipientId = TagHelper.GetLong(tagValue);
                    break;

                //msg-param-sender-login
                case (int)Tags.MsgParamSenderLogin when tagKey.SequenceEqual("msg-param-sender-login"u8):
                    gifterUsername = TagHelper.GetString(tagValue);
                    break;

                //msg-param-sender-count
                case (int)Tags.MsgParamSenderCount when tagKey.SequenceEqual("msg-param-sender-count"u8):
                    totalGiftCount = TagHelper.GetInt(tagValue);
                    break;

                //msg-param-sub-plan-name
                case (int)Tags.MsgParamSubPlanName when tagKey.SequenceEqual("msg-param-sub-plan-name"u8):
                    subPlanName = TagHelper.GetString(tagValue, true, true);
                    break;

                //msg-param-streak-months
                case (int)Tags.MsgParamStreakMonths when tagKey.SequenceEqual("msg-param-streak-months"u8):
                    monthStreak = TagHelper.GetInt(tagValue);
                    break;

                //msg-param-mass-gift-count
                case (int)Tags.MsgParamMassGiftCount when tagKey.SequenceEqual("msg-param-mass-gift-count"u8):
                    giftCount = TagHelper.GetInt(tagValue);
                    break;

                //msg-param-community-gift-id
                case (int)Tags.MsgParamCommunityGiftId when tagKey.SequenceEqual("msg-param-community-gift-id"u8):
                    communityGiftId = TagHelper.GetULong(tagValue);
                    break;

                //msg-param-cumulative-months
                case (int)Tags.MsgParamCumulativeMonths when tagKey.SequenceEqual("msg-param-cumulative-months"u8):
                    cumulativeMonths = TagHelper.GetInt(tagValue);
                    break;

                //msg-param-recipient-user-name
                case (int)Tags.MsgParamRecipientUserName when tagKey.SequenceEqual("msg-param-recipient-user-name"u8):
                    recipientUsername = TagHelper.GetString(tagValue);
                    break;

                //msg-param-should-share-streak
                case (int)Tags.MsgParamShouldShareStreak when tagKey.SequenceEqual("msg-param-should-share-streak"u8):
                    shouldShareStreak = TagHelper.GetBool(tagValue);
                    break;

                //msg-param-recipient-display-name
                case (int)Tags.MsgParamRecipientDisplayName when tagKey.SequenceEqual("msg-param-recipient-display-name"u8):
                    recipientDisplayName = TagHelper.GetString(tagValue);
                    break;

                //msg-param-charity-name
                case (int)Tags.MsgParamCharityName when tagKey.SequenceEqual("msg-param-charity-name"u8):
                    charityName = TagHelper.GetString(tagValue, intern: true, unescape: true);
                    break;

                //msg-param-donation-amount
                case (int)Tags.MsgParamDonationAmount when tagKey.SequenceEqual("msg-param-donation-amount"u8):
                    donationAmount = TagHelper.GetInt(tagValue);
                    break;

                //msg-param-exponent
                case (int)Tags.MsgParamExponent when tagKey.SequenceEqual("msg-param-exponent"u8):
                    donationExponent = TagHelper.GetInt(tagValue);
                    break;

                //msg-param-donation-currency
                case (int)Tags.MsgParamDonationCurrency when tagKey.SequenceEqual("msg-param-donation-currency"u8):
                    donationCurrency = TagHelper.GetEnum<CurrencyCode>(tagValue);
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

                //source-msg-id
                case (int)Tags.SourceMsgId when tagKey.SequenceEqual("source-msg-id"u8):
                    srcMsgId = tagValue.Length switch
                    {
                        (int)UsernoticeType.Sub when tagValue.SequenceEqual("sub"u8) => UsernoticeType.Sub,
                        (int)UsernoticeType.Raid when tagValue.SequenceEqual("raid"u8) => UsernoticeType.Raid,
                        (int)UsernoticeType.Resub when tagValue.SequenceEqual("resub"u8) => UsernoticeType.Resub,
                        (int)UsernoticeType.Unraid when tagValue.SequenceEqual("unraid"u8) => UsernoticeType.Unraid,
                        (int)UsernoticeType.Subgift when tagValue.SequenceEqual("subgift"u8) => UsernoticeType.Subgift,
                        (int)UsernoticeType.Announcement when tagValue.SequenceEqual("announcement"u8) => UsernoticeType.Announcement,
                        (int)UsernoticeType.BitsBadgeTier when tagValue.SequenceEqual("bitsbadgetier"u8) => UsernoticeType.BitsBadgeTier,
                        (int)UsernoticeType.SubMysteryGift when tagValue.SequenceEqual("submysterygift"u8) => UsernoticeType.SubMysteryGift,
                        (int)UsernoticeType.GiftPaidUpgrade when tagValue.SequenceEqual("giftpaidupgrade"u8) => UsernoticeType.GiftPaidUpgrade,
                        15 when tagValue.SequenceEqual("charitydonation"u8) => UsernoticeType.CharityDonation,
                        (int)UsernoticeType.PrimePaidUpgrade when tagValue.SequenceEqual("primepaidupgrade"u8) => UsernoticeType.PrimePaidUpgrade,
                        (int)UsernoticeType.StandardPayForward when tagValue.SequenceEqual("standardpayforward"u8) => UsernoticeType.StandardPayForward,
                        (int)UsernoticeType.AnonGiftPaidUpgrade when tagValue.SequenceEqual("anongiftpaidupgrade"u8) => UsernoticeType.AnonGiftPaidUpgrade,
                        16 when tagValue.SequenceEqual("sharedchatnotice"u8) => UsernoticeType.SharedChatNotice,
                        _ => UsernoticeType.Unknown,
                    };
                    break;

                //msg-param-goal-description
                case (int)Tags.MsgParamGoalDescription when tagKey.SequenceEqual("msg-param-goal-description"u8):
                    goalDescription = TagHelper.GetString(tagValue, intern: true, unescape: true);
                    break;

                //msg-param-goal-target-contributions
                case (int)Tags.MsgParamGoalTargetContributions when tagKey.SequenceEqual("msg-param-goal-target-contributions"u8):
                    goalTarget = TagHelper.GetInt(tagValue);
                    break;

                //msg-param-goal-current-contributions
                case (int)Tags.MsgParamGoalCurrentContributions when tagKey.SequenceEqual("msg-param-goal-current-contributions"u8):
                    goalCurrent = TagHelper.GetInt(tagValue);
                    break;

                //msg-param-goal-user-contributions
                case (int)Tags.MsgParamGoalUserContributions when tagKey.SequenceEqual("msg-param-goal-user-contributions"u8):
                    goalUser = TagHelper.GetInt(tagValue);
                    break;
            }
        }

        switch (this.MsgId == UsernoticeType.SharedChatNotice ? srcMsgId : this.MsgId)
        {

            case UsernoticeType.Sub or UsernoticeType.Resub or UsernoticeType.Announcement:
                content = message.HasMessageContent ? message.GetContent().Content : string.Empty;
                break;

            case UsernoticeType.CharityDonation:
                actualDonationAmount = donationAmount * Math.Pow(10, -donationExponent);
                break;
        }

        this.Author = new MessageAuthor()
        {
            BadgeInfo = badgeInfo,
            Badges = badges,
            ChatColor = colorCode,
            DisplayName = displayName,
            Id = userId,
            IsMod = isMod,
            IsSubscriber = isSubscriber,
            Type = userType,
            Name = username,
            IsTurbo = isTurbo,
            IsVip = badges.Contains(VIP_ROLE)
        };
        this.Channel = new IrcChannel()
        {
            Name = channelName,
            Id = channelId
        };
        this.Recipient = new MessageAuthor()
        {
            DisplayName = recipientDisplayName,
            Name = recipientUsername,
            Id = recipientId
        };
        this.SubPlan = subPlan;
        this.Color = color;
        this.Emotes = emotes;
        this.Flags = flags;
        this.Id = id;
        this.SubPlanName = subPlanName;
        this.SystemMessage = systemMessage;
        this.Message = content;
        this.GifterUsername = gifterUsername;
        this.GifterDisplayName = gifterDisplayName;
        this.CumulativeMonths = cumulativeMonths;
        this.Months = months;
        this.MonthStreak = monthStreak;
        this.GiftedMonths = giftedMonths;
        this.GiftCount = giftCount;
        this.TotalGiftCount = totalGiftCount;
        this.ViewerCount = viewerCount;
        this.ShouldShareStreak = shouldShareStreak;
        this.CharityName = charityName;
        this.DonationAmount = actualDonationAmount;
        this.DonationCurrency = donationCurrency;
        this.CommunityGiftId = communityGiftId;
        this.Source = new NoticeSource()
        {
            BadgeInfo = sourceBadgeInfo,
            Badges = sourceBadges,
            ChannelId = sourceRoomId,
            Id = sourceId,
            MsgId = srcMsgId,
        };
        this.ChannelGoal = new ChannelGoal()
        {
            Description = goalDescription,
            TargetContributions = goalTarget,
            CurrentContributions = goalCurrent,
            UserContribution = goalUser
        };
        this.ConsecutiveStreamsWatched = consecutiveStreamsWatched;
        this.Reward = reward;
        this.Category = category;
    }

    /// <summary>
    /// Construct a <see cref="Usernotice"/> from a string. Useful for testing
    /// </summary>
    /// <param name="rawData">The raw IRC message</param>
    /// <returns><see cref="Usernotice"/> with the related data</returns>
    public static Usernotice Construct(string rawData)
    {
        ReadOnlyMemory<byte> memory = new(Encoding.UTF8.GetBytes(rawData));
        var message = new IrcMessage(memory);
        return new(ref message);
    }

#pragma warning disable CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).
    /// <inheritdoc/>
    public bool Equals(Usernotice other) => this.Id == other.Id;
    /// <inheritdoc/>
    public override bool Equals(object obj) => obj is Usernotice && Equals((Usernotice)obj);
#pragma warning restore CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).
    /// <inheritdoc/>
    public static bool operator ==(Usernotice left, Usernotice right) => left.Equals(right);
    /// <inheritdoc/>
    public static bool operator !=(Usernotice left, Usernotice right) => !(left == right);
    /// <inheritdoc/>
    public override int GetHashCode()
    {
        var code = new HashCode();
        code.Add(this.Id);
        code.Add(this.MsgId);
        return code.ToHashCode();
    }
}