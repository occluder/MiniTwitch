using System.Text.Json.Serialization;
using MiniTwitch.EventSub.Internal;

namespace MiniTwitch.EventSub.Models;

/// <summary>
/// A notification when a redemption of a channel points custom reward has been updated
/// </summary>
[EventSubEvent("channel.channel_points_custom_reward_redemption.update", "1")]
public partial struct ChannelPointsCustomRewardRedemptionUpdate
{
    /// <summary>The redemption identifier</summary>
    public partial string Id { get; }
    /// <summary>The requested broadcaster ID</summary>
    [JsonPropertyName("broadcaster_user_id")]
    public partial long BroadcasterId { get; }
    /// <summary>The requested broadcaster login</summary>
    [JsonPropertyName("broadcaster_user_login"), Intern]
    public partial string BroadcasterUsername { get; }
    /// <summary>The requested broadcaster display name</summary>
    [JsonPropertyName("broadcaster_user_name"), Intern]
    public partial string BroadcasterDisplayName { get; }
    /// <summary>User ID of the user that redeemed the reward</summary>
    public partial long UserId { get; }
    /// <summary>Login of the user that redeemed the reward</summary>
    [JsonPropertyName("user_login")]
    public partial string Username { get; }
    /// <summary>Display name of the user that redeemed the reward</summary>
    [JsonPropertyName("user_name")]
    public partial string UserDisplayName { get; }
    /// <summary>The user input provided. Empty string if not provided</summary>
    public partial string UserInput { get; }
    /// <summary>
    /// Defaults to <c>unfulfilled</c>.
    /// Possible values: <c>unknown</c>, <c>unfulfilled</c>, <c>fulfilled</c>, <c>canceled</c>
    /// </summary>
    public partial string Status { get; }
    /// <summary>Basic information about the reward that was redeemed</summary>
    public partial RedemptionReward Reward { get; }
    /// <summary>RFC3339 timestamp of when the reward was redeemed</summary>
    public partial DateTimeOffset RedeemedAt { get; }

    /// <summary>Basic information about the reward that was redeemed</summary>
    [EventProperty]
    public partial struct RedemptionReward
    {
        /// <summary>The reward identifier</summary>
        public partial string Id { get; }
        /// <summary>The reward title</summary>
        public partial string Title { get; }
        /// <summary>The reward cost</summary>
        public partial int Cost { get; }
        /// <summary>The reward description</summary>
        public partial string Prompt { get; }
    }
}
