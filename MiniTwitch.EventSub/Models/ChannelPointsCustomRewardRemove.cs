using System.Text.Json.Serialization;
using MiniTwitch.EventSub.Internal;

namespace MiniTwitch.EventSub.Models;

/// <summary>
/// A notification when a custom channel points reward has been removed
/// </summary>
[EventSubEvent("channel.channel_points_custom_reward.remove", "1")]
public partial struct ChannelPointsCustomRewardRemove
{
    /// <summary>The reward identifier</summary>
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
    /// <summary>Whether the reward is currently enabled</summary>
    public partial bool IsEnabled { get; }
    /// <summary>Whether the reward is currently paused</summary>
    public partial bool IsPaused { get; }
    /// <summary>Whether the reward is currently in stock</summary>
    public partial bool IsInStock { get; }
    /// <summary>The reward title</summary>
    public partial string Title { get; }
    /// <summary>The reward cost</summary>
    public partial int Cost { get; }
    /// <summary>The reward description</summary>
    public partial string Prompt { get; }
    /// <summary>Whether the viewer needs to enter information when redeeming</summary>
    public partial bool IsUserInputRequired { get; }
    /// <summary>Whether redemptions skip the request queue</summary>
    public partial bool ShouldRedemptionsSkipRequestQueue { get; }
    /// <summary>Whether a maximum per stream is enabled and what the maximum is</summary>
    public partial MaxPerStreamInfo? MaxPerStream { get; }
    /// <summary>Whether a maximum per user per stream is enabled and what the maximum is</summary>
    public partial MaxPerUserPerStreamInfo? MaxPerUserPerStream { get; }
    /// <summary>Custom background color for the reward</summary>
    public partial string BackgroundColor { get; }
    /// <summary>Set of custom images for the reward. Can be null if no images have been uploaded</summary>
    public partial RewardImage? Image { get; }
    /// <summary>Set of default images for the reward</summary>
    public partial RewardImage DefaultImage { get; }
    /// <summary>Whether a cooldown is enabled and what the cooldown is in seconds</summary>
    public partial GlobalCooldownInfo? GlobalCooldown { get; }
    /// <summary>Timestamp of the cooldown expiration. Null if the reward isn't on cooldown</summary>
    public partial DateTimeOffset? CooldownExpiresAt { get; }
    /// <summary>
    /// The number of redemptions redeemed during the current live stream.
    /// Null if the broadcaster's stream isn't live or max_per_stream isn't enabled
    /// </summary>
    public partial int? RedemptionsRedeemedCurrentStream { get; }

}
