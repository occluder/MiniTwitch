using System.Text.Json.Serialization;
using MiniTwitch.EventSub.Internal;

namespace MiniTwitch.EventSub.Models;

/// <summary>
/// A notification when a viewer has redeemed an automatic channel points reward
/// </summary>
[EventSubEvent("channel.channel_points_automatic_reward_redemption.add", "1")]
public partial struct ChannelPointsAutomaticRewardRedemptionAdd
{
    /// <summary>The ID of the channel where the reward was redeemed</summary>
    [JsonPropertyName("broadcaster_user_id")]
    public partial long BroadcasterId { get; }
    /// <summary>The login of the channel where the reward was redeemed</summary>
    [JsonPropertyName("broadcaster_user_login"), Intern]
    public partial string BroadcasterUsername { get; }
    /// <summary>The display name of the channel where the reward was redeemed</summary>
    [JsonPropertyName("broadcaster_user_name"), Intern]
    public partial string BroadcasterDisplayName { get; }
    /// <summary>The ID of the redeeming user</summary>
    public partial long UserId { get; }
    /// <summary>The login of the redeeming user</summary>
    [JsonPropertyName("user_login")]
    public partial string Username { get; }
    /// <summary>The display name of the redeeming user</summary>
    [JsonPropertyName("user_name")]
    public partial string UserDisplayName { get; }
    /// <summary>The ID of the Redemption</summary>
    public partial string Id { get; }
    /// <summary>An object that contains the reward information</summary>
    public partial RewardInfo Reward { get; }
    /// <summary>An object that contains the user message and emote information</summary>
    public partial RedemptionMessage Message { get; }
    /// <summary>A string that the user entered if the reward requires input</summary>
    public partial string? UserInput { get; }
    /// <summary>The timestamp of when the reward was redeemed</summary>
    public partial DateTimeOffset RedeemedAt { get; }

    /// <summary>Automatic reward information</summary>
    [EventProperty]
    public partial struct RewardInfo
    {
        /// <summary>The type of reward</summary>
        public partial string Type { get; }
        /// <summary>The reward cost</summary>
        public partial int Cost { get; }
        /// <summary>Optional. Emote that was unlocked</summary>
        public partial UnlockedEmote? UnlockedEmote { get; }
    }

    /// <summary>Emote that was unlocked by the reward</summary>
    [EventProperty]
    public partial struct UnlockedEmote
    {
        /// <summary>The emote ID</summary>
        public partial string Id { get; }
        /// <summary>The human readable emote token</summary>
        public partial string Name { get; }
    }

    /// <summary>The user message and emote information</summary>
    [EventProperty]
    public partial struct RedemptionMessage
    {
        /// <summary>The text of the chat message</summary>
        public partial string Text { get; }
        /// <summary>An array that includes the emote ID and start and end positions</summary>
        public partial EmotePosition[] Emotes { get; }
    }

    /// <summary>An emote within the message</summary>
    [EventProperty]
    public partial struct EmotePosition
    {
        /// <summary>The emote ID</summary>
        public partial string Id { get; }
        /// <summary>The index of where the Emote starts in the text</summary>
        public partial int Begin { get; }
        /// <summary>The index of where the Emote ends in the text</summary>
        public partial int End { get; }
    }
}
