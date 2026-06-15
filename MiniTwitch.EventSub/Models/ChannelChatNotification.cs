using System.Text.Json.Serialization;
using MiniTwitch.EventSub.Internal;

namespace MiniTwitch.EventSub.Models;

/// <summary>
/// A notification when an event that appears in chat occurs, such as someone subscribing to the channel or a subscription is gifted
/// </summary>
[EventSubEvent("channel.chat.notification", "1")]
public partial struct ChannelChatNotification
{
    /// <summary>The broadcaster user ID</summary>
    [JsonPropertyName("broadcaster_user_id")]
    public partial long BroadcasterId { get; }
    /// <summary>The broadcaster login</summary>
    [JsonPropertyName("broadcaster_user_login"), Intern]
    public partial string BroadcasterUsername { get; }
    /// <summary>The broadcaster display name</summary>
    [JsonPropertyName("broadcaster_user_name"), Intern]
    public partial string BroadcasterDisplayName { get; }
    /// <summary>The user ID of the user that sent the message</summary>
    [JsonPropertyName("chatter_user_id")]
    public partial long ChatterId { get; }
    /// <summary>The user login of the user that sent the message</summary>
    [JsonPropertyName("chatter_user_login")]
    public partial string ChatterUsername { get; }
    /// <summary>The user display name of the user that sent the message</summary>
    [JsonPropertyName("chatter_user_name")]
    public partial string ChatterDisplayName { get; }
    /// <summary>Whether or not the chatter is anonymous</summary>
    public partial bool ChatterIsAnonymous { get; }
    /// <summary>The color of the user's name in the chat room</summary>
    public partial string Color { get; }
    /// <summary>Chat badges for the chatter</summary>
    public partial Badge[] Badges { get; }
    /// <summary>The message Twitch shows in the chat room for this notice</summary>
    public partial string SystemMessage { get; }
    /// <summary>A UUID that identifies the message</summary>
    public partial string MessageId { get; }
    /// <summary>The structured chat message</summary>
    public partial NotificationMessage Message { get; }

    /// <summary>The type of notice</summary>
    [Intern]
    public partial string NoticeType { get; }

    /// <summary>Information about the sub event. Null if <c>notice_type</c> is not <c>sub</c></summary>
    public partial SubNotice? Sub { get; }
    /// <summary>Information about the resub event. Null if <c>notice_type</c> is not <c>resub</c></summary>
    public partial ResubNotice? Resub { get; }
    /// <summary>Information about the gift sub event. Null if <c>notice_type</c> is not <c>sub_gift</c></summary>
    public partial SubGiftNotice? SubGift { get; }
    /// <summary>Information about the community gift sub event. Null if <c>notice_type</c> is not <c>community_sub_gift</c></summary>
    public partial CommunitySubGiftNotice? CommunitySubGift { get; }
    /// <summary>Information about the community gift paid upgrade event. Null if <c>notice_type</c> is not <c>gift_paid_upgrade</c></summary>
    public partial GiftPaidUpgradeNotice? GiftPaidUpgrade { get; }
    /// <summary>Information about the Prime gift paid upgrade event. Null if <c>notice_type</c> is not <c>prime_paid_upgrade</c></summary>
    public partial PrimePaidUpgradeNotice? PrimePaidUpgrade { get; }
    /// <summary>Information about the pay it forward event. Null if <c>notice_type</c> is not <c>pay_it_forward</c></summary>
    public partial PayItForwardNotice? PayItForward { get; }
    /// <summary>Information about the raid event. Null if <c>notice_type</c> is not <c>raid</c></summary>
    public partial RaidNotice? Raid { get; }
    /// <summary>Returns an empty payload if <c>notice_type</c> is not <c>unraid</c>, otherwise returns null</summary>
    public partial UnraidNotice? Unraid { get; }
    /// <summary>Information about the announcement event. Null if <c>notice_type</c> is not <c>announcement</c></summary>
    public partial AnnouncementNotice? Announcement { get; }
    /// <summary>Information about the Bits badge tier event. Null if <c>notice_type</c> is not <c>bits_badge_tier</c></summary>
    public partial BitsBadgeTierNotice? BitsBadgeTier { get; }
    /// <summary>Information about the charity donation event. Null if <c>notice_type</c> is not <c>charity_donation</c></summary>
    public partial CharityDonationNotice? CharityDonation { get; }
    /// <summary>Information about the Watch Streak event. Null if <c>notice_type</c> is not <c>watch_streak</c></summary>
    public partial WatchStreakNotice? WatchStreak { get; }
    /// <summary>Information about the modiversary event. Null if <c>notice_type</c> is not <c>modiversary</c></summary>
    public partial ModiversaryNotice? Modiversary { get; }

    /// <summary>The broadcaster user ID of the channel the message was sent from. Null when not in a shared chat session</summary>
    [JsonPropertyName("source_broadcaster_user_id")]
    public partial long? SourceBroadcasterId { get; }
    /// <summary>The user name of the broadcaster of the channel the message was sent from. Null when not in a shared chat session</summary>
    [JsonPropertyName("source_broadcaster_user_name")]
    public partial string? SourceBroadcasterDisplayName { get; }
    /// <summary>The login of the broadcaster of the channel the message was sent from. Null when not in a shared chat session</summary>
    [JsonPropertyName("source_broadcaster_user_login")]
    public partial string? SourceBroadcasterUsername { get; }
    /// <summary>The UUID that identifies the source message from the channel the message was sent from. Null when not in a shared chat session</summary>
    public partial string? SourceMessageId { get; }
    /// <summary>The list of chat badges for the chatter in the channel the message was sent from. Null when not in a shared chat session</summary>
    public partial Badge[]? SourceBadges { get; }
    /// <summary>Whether the notification is only sent to the source channel. Null if not in a shared chat session</summary>
    public partial bool? IsSourceOnly { get; }

    /// <summary>Information about the <c>shared_chat_sub</c> event. Null if <c>notice_type</c> is not <c>shared_chat_sub</c></summary>
    public partial SubNotice? SharedChatSub { get; }
    /// <summary>Information about the <c>shared_chat_resub</c> event. Null if <c>notice_type</c> is not <c>shared_chat_resub</c></summary>
    public partial ResubNotice? SharedChatResub { get; }
    /// <summary>Information about the <c>shared_chat_sub_gift</c> event. Null if <c>notice_type</c> is not <c>shared_chat_sub_gift</c></summary>
    public partial SubGiftNotice? SharedChatSubGift { get; }
    /// <summary>Information about the <c>shared_chat_community_sub_gift</c> event. Null if <c>notice_type</c> is not <c>shared_chat_community_sub_gift</c></summary>
    public partial CommunitySubGiftNotice? SharedChatCommunitySubGift { get; }
    /// <summary>Information about the <c>shared_chat_gift_paid_upgrade</c> event. Null if <c>notice_type</c> is not <c>shared_chat_gift_paid_upgrade</c></summary>
    public partial GiftPaidUpgradeNotice? SharedChatGiftPaidUpgrade { get; }
    /// <summary>Information about the <c>shared_chat_prime_paid_upgrade</c> event. Null if <c>notice_type</c> is not <c>shared_chat_prime_paid_upgrade</c></summary>
    public partial PrimePaidUpgradeNotice? SharedChatPrimePaidUpgrade { get; }
    /// <summary>Information about the <c>shared_chat_pay_it_forward</c> event. Null if <c>notice_type</c> is not <c>shared_chat_pay_it_forward</c></summary>
    public partial PayItForwardNotice? SharedChatPayItForward { get; }
    /// <summary>Information about the <c>shared_chat_raid</c> event. Null if <c>notice_type</c> is not <c>shared_chat_raid</c></summary>
    public partial RaidNotice? SharedChatRaid { get; }
    /// <summary>Information about the <c>shared_chat_announcement</c> event. Null if <c>notice_type</c> is not <c>shared_chat_announcement</c></summary>
    public partial AnnouncementNotice? SharedChatAnnouncement { get; }
    /// <summary>Information about the <c>shared_chat_modiversary</c> event. Null if <c>notice_type</c> is not <c>shared_chat_modiversary</c></summary>
    public partial ModiversaryNotice? SharedChatModiversary { get; }

    /// <summary>The structured chat message</summary>
    [EventProperty]
    public partial struct NotificationMessage
    {
        /// <summary>The chat message in plain text</summary>
        public partial string Text { get; }
        /// <summary>Ordered list of chat message fragments</summary>
        public partial MessageFragment[] Fragments { get; }
    }

    /// <summary>Information about a subscription</summary>
    [EventProperty]
    public partial struct SubNotice
    {
        /// <summary>The tier of the subscription. Possible values: <c>1000</c>, <c>2000</c>, <c>3000</c></summary>
        public partial string SubTier { get; }
        /// <summary>Indicates if the subscription was obtained through Amazon Prime</summary>
        public partial bool IsPrime { get; }
        /// <summary>The number of months the subscription is for</summary>
        public partial int DurationMonths { get; }
    }

    /// <summary>Information about a resubscription</summary>
    [EventProperty]
    public partial struct ResubNotice
    {
        /// <summary>The total number of months the user has subscribed</summary>
        public partial int CumulativeMonths { get; }
        /// <summary>The number of months the subscription is for</summary>
        public partial int DurationMonths { get; }
        /// <summary>The total number of consecutive months the user has subscribed</summary>
        public partial int? StreakMonths { get; }
        /// <summary>The tier of the subscription. Possible values: <c>1000</c>, <c>2000</c>, <c>3000</c></summary>
        public partial string SubTier { get; }
        /// <summary>Indicates if the subscription was obtained through Amazon Prime</summary>
        public partial bool IsPrime { get; }
        /// <summary>Whether or not the resub was a result of a gift</summary>
        public partial bool IsGift { get; }
        /// <summary>Whether or not the gift was anonymous</summary>
        public partial bool? GifterIsAnonymous { get; }
        /// <summary>The user ID of the subscription gifter. Null if anonymous</summary>
        public partial long? GifterUserId { get; }
        /// <summary>The user name of the subscription gifter. Null if anonymous</summary>
        [JsonPropertyName("gifter_user_name")]
        public partial string? GifterDisplayName { get; }
        /// <summary>The user login of the subscription gifter. Null if anonymous</summary>
        [JsonPropertyName("gifter_user_login")]
        public partial string? GifterUsername { get; }
    }

    /// <summary>Information about a gift subscription</summary>
    [EventProperty]
    public partial struct SubGiftNotice
    {
        /// <summary>The number of months the subscription is for</summary>
        public partial int DurationMonths { get; }
        /// <summary>The amount of gifts the gifter has given in this channel. Null if anonymous</summary>
        public partial int? CumulativeTotal { get; }
        /// <summary>The user ID of the subscription gift recipient</summary>
        public partial long RecipientUserId { get; }
        /// <summary>The user name of the subscription gift recipient</summary>
        [JsonPropertyName("recipient_user_name")]
        public partial string RecipientDisplayName { get; }
        /// <summary>The user login of the subscription gift recipient</summary>
        [JsonPropertyName("recipient_user_login")]
        public partial string RecipientUsername { get; }
        /// <summary>The tier of the subscription. Possible values: <c>1000</c>, <c>2000</c>, <c>3000</c></summary>
        public partial string SubTier { get; }
        /// <summary>The ID of the associated community gift. Null if not associated with a community gift</summary>
        public partial string? CommunityGiftId { get; }
    }

    /// <summary>Information about a community gift subscription</summary>
    [EventProperty]
    public partial struct CommunitySubGiftNotice
    {
        /// <summary>The ID of the associated community gift</summary>
        public partial string Id { get; }
        /// <summary>Number of subscriptions being gifted</summary>
        public partial int Total { get; }
        /// <summary>The tier of the subscription. Possible values: <c>1000</c>, <c>2000</c>, <c>3000</c></summary>
        public partial string SubTier { get; }
        /// <summary>The amount of gifts the gifter has given in this channel. Null if anonymous</summary>
        public partial int? CumulativeTotal { get; }
    }

    /// <summary>Information about a gift paid upgrade</summary>
    [EventProperty]
    public partial struct GiftPaidUpgradeNotice
    {
        /// <summary>Whether the gift was given anonymously</summary>
        public partial bool GifterIsAnonymous { get; }
        /// <summary>The user ID of the user who gifted the subscription. Null if anonymous</summary>
        public partial long? GifterUserId { get; }
        /// <summary>The user name of the user who gifted the subscription. Null if anonymous</summary>
        [JsonPropertyName("gifter_user_name")]
        public partial string? GifterDisplayName { get; }
    }

    /// <summary>Information about a Prime paid upgrade</summary>
    [EventProperty]
    public partial struct PrimePaidUpgradeNotice
    {
        /// <summary>The tier of the subscription. Possible values: <c>1000</c>, <c>2000</c>, <c>3000</c></summary>
        public partial string SubTier { get; }
    }

    /// <summary>Information about a pay it forward event</summary>
    [EventProperty]
    public partial struct PayItForwardNotice
    {
        /// <summary>Whether the gift was given anonymously</summary>
        public partial bool GifterIsAnonymous { get; }
        /// <summary>The user ID of the user who gifted the subscription. Null if anonymous</summary>
        public partial long? GifterUserId { get; }
        /// <summary>The user name of the user who gifted the subscription. Null if anonymous</summary>
        [JsonPropertyName("gifter_user_name")]
        public partial string? GifterDisplayName { get; }
        /// <summary>The user login of the user who gifted the subscription. Null if anonymous</summary>
        [JsonPropertyName("gifter_user_login")]
        public partial string? GifterUsername { get; }
    }

    /// <summary>Information about a raid event</summary>
    [EventProperty]
    public partial struct RaidNotice
    {
        /// <summary>The user ID of the broadcaster raiding this channel</summary>
        public partial long UserId { get; }
        /// <summary>The user name of the broadcaster raiding this channel</summary>
        [JsonPropertyName("user_name")]
        public partial string UserDisplayName { get; }
        /// <summary>The login name of the broadcaster raiding this channel</summary>
        [JsonPropertyName("user_login")]
        public partial string Username { get; }
        /// <summary>The number of viewers raiding this channel</summary>
        public partial int ViewerCount { get; }
        /// <summary>Profile image URL of the broadcaster raiding this channel</summary>
        public partial string ProfileImageUrl { get; }
    }

    /// <summary>Information about an unraid event</summary>
    [EventProperty]
    public partial struct UnraidNotice
    {
    }

    /// <summary>Information about an announcement event</summary>
    [EventProperty]
    public partial struct AnnouncementNotice
    {
        /// <summary>Color of the announcement</summary>
        [Intern]
        public partial string Color { get; }
    }

    /// <summary>Information about a Bits badge tier event</summary>
    [EventProperty]
    public partial struct BitsBadgeTierNotice
    {
        /// <summary>The tier of the Bits badge the user just earned</summary>
        public partial int Tier { get; }
    }

    /// <summary>Information about a charity donation event</summary>
    [EventProperty]
    public partial struct CharityDonationNotice
    {
        /// <summary>Name of the charity</summary>
        [Intern]
        public partial string CharityName { get; }
        /// <summary>An object that contains the amount of money that the user paid</summary>
        public partial CharityAmount Amount { get; }

        /// <summary>An object that contains a monetary amount</summary>
        [EventProperty]
        public partial struct CharityAmount
        {
            /// <summary>The monetary amount in the currency's minor unit</summary>
            public partial int Value { get; }
            /// <summary>The number of decimal places used by the currency</summary>
            public partial int DecimalPlaces { get; }
            /// <summary>The ISO-4217 three-letter currency code</summary>
            [Intern]
            public partial string Currency { get; }
        }
    }

    /// <summary>Information about a Watch Streak event</summary>
    [EventProperty]
    public partial struct WatchStreakNotice
    {
        /// <summary>The number of consecutive broadcasts for which the user has been watching</summary>
        public partial int StreakCount { get; }
        /// <summary>The number of channel points awarded for the Watch Streak milestone</summary>
        public partial int ChannelPointsAwarded { get; }
    }

    /// <summary>Information about a modiversary event</summary>
    [EventProperty]
    public partial struct ModiversaryNotice
    {
        /// <summary>The number of months the user has been a moderator in this channel</summary>
        public partial int Months { get; }
    }
}
