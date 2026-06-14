using System.Text.Json.Serialization;
using MiniTwitch.EventSub.Internal;

namespace MiniTwitch.EventSub.Models;

/// <summary>
/// A notification when a viewer has redeemed an automatic channel points reward (V2)
/// </summary>
[EventSubEvent("channel.channel_points_automatic_reward_redemption.add", "2")]
public partial struct ChannelPointsAutomaticRewardRedemptionAddV2
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
    public partial RewardInfoV2 Reward { get; }
    /// <summary>An object that contains the user message and fragment information</summary>
    public partial RedemptionMessageV2 Message { get; }
    /// <summary>The timestamp of when the reward was redeemed</summary>
    public partial DateTimeOffset RedeemedAt { get; }

    /// <summary>Automatic reward information</summary>
    [EventProperty]
    public partial struct RewardInfoV2
    {
        /// <summary>The type of reward</summary>
        public partial string Type { get; }
        /// <summary>The reward cost in channel points</summary>
        [JsonPropertyName("channel_points")]
        public partial int ChannelPoints { get; }
        /// <summary>Optional. Emote associated with the reward</summary>
        public partial RewardEmote? Emote { get; }
    }

    /// <summary>Emote associated with the reward</summary>
    [EventProperty]
    public partial struct RewardEmote
    {
        /// <summary>The emote ID</summary>
        public partial string Id { get; }
    }

    /// <summary>The user message and fragment information</summary>
    [EventProperty]
    public partial struct RedemptionMessageV2
    {
        /// <summary>The text of the chat message</summary>
        public partial string Text { get; }
        /// <summary>An array of message fragments</summary>
        public partial MessageFragmentV2[] Fragments { get; }
    }

    /// <summary>A message fragment</summary>
    [EventProperty]
    public partial struct MessageFragmentV2
    {
        /// <summary>The type of fragment</summary>
        public partial string Type { get; }
        /// <summary>The text of the fragment</summary>
        public partial string Text { get; }
        /// <summary>Optional. Metadata pertaining to the emote</summary>
        public partial FragmentEmote? Emote { get; }
    }

    /// <summary>Emote within a fragment</summary>
    [EventProperty]
    public partial struct FragmentEmote
    {
        /// <summary>The emote ID</summary>
        public partial string Id { get; }
    }
}
