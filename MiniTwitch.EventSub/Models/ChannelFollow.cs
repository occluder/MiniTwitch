using System.Text.Json.Serialization;
using MiniTwitch.EventSub.Internal;

namespace MiniTwitch.EventSub.Models;

/// <summary>
/// A notification when a specified channel receives a follow
/// </summary>
[EventSubEvent("channel.follow", "2")]
public partial struct ChannelFollow
{
    /// <summary>The user ID for the user now following the specified channel</summary>
    public partial long UserId { get; }
    /// <summary>The user login for the user now following the specified channel</summary>
    [JsonPropertyName("user_login")]
    public partial string Username { get; }
    /// <summary>The user display name for the user now following the specified channel</summary>
    [JsonPropertyName("user_name")]
    public partial string UserDisplayName { get; }
    /// <summary>The requested broadcaster ID</summary>
    [JsonPropertyName("broadcaster_user_id")]
    public partial long BroadcasterId { get; }
    /// <summary>The requested broadcaster login</summary>
    [JsonPropertyName("broadcaster_user_login"), Intern]
    public partial string BroadcasterUsername { get; }
    /// <summary>The requested broadcaster display name</summary>
    [JsonPropertyName("broadcaster_user_name"), Intern]
    public partial string BroadcasterDisplayName { get; }
    /// <summary>RFC3339 timestamp of when the follow occurred</summary>
    public partial DateTimeOffset FollowedAt { get; }
}
