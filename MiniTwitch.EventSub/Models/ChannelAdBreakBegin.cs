using System.Text.Json.Serialization;
using MiniTwitch.EventSub.Internal;

namespace MiniTwitch.EventSub.Models;

/// <summary>
/// A notification when a user runs a midroll commercial break,
/// either manually or automatically via ads manager
/// </summary>
[EventSubEvent("channel.ad_break.begin", "1")]
public partial struct ChannelAdBreakBegin
{
    /// <summary>Length in seconds of the mid-roll ad break requested</summary>
    public partial int DurationSeconds { get; }
    /// <summary>The UTC timestamp of when the ad break began</summary>
    public partial DateTimeOffset StartedAt { get; }
    /// <summary>Indicates if the ad was automatically scheduled via Ads Manager</summary>
    public partial bool IsAutomatic { get; }
    /// <summary>The broadcaster's user ID for the channel the ad was run on</summary>
    [JsonPropertyName("broadcaster_user_id")]
    public partial long BroadcasterId { get; }
    /// <summary>The broadcaster's user login for the channel the ad was run on</summary>
    [JsonPropertyName("broadcaster_user_login"), Intern]
    public partial string BroadcasterUsername { get; }
    /// <summary>The broadcaster's user display name for the channel the ad was run on</summary>
    [JsonPropertyName("broadcaster_user_name"), Intern]
    public partial string BroadcasterDisplayName { get; }
    /// <summary>The ID of the user that requested the ad</summary>
    [JsonPropertyName("requester_user_id")]
    public partial long RequesterId { get; }
    /// <summary>The login of the user that requested the ad</summary>
    [JsonPropertyName("requester_user_login")]
    public partial string RequesterUsername { get; }
    /// <summary>The display name of the user that requested the ad</summary>
    [JsonPropertyName("requester_user_name")]
    public partial string RequesterDisplayName { get; }
}
