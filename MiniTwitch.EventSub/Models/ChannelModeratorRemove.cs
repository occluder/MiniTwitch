using System.Text.Json.Serialization;
using MiniTwitch.EventSub.Internal;

namespace MiniTwitch.EventSub.Models;

/// <summary>
/// A notification when a user has moderator privileges removed on a specified channel
/// </summary>
[EventSubEvent("channel.moderator.remove", "1")]
public partial struct ChannelModeratorRemove
{
    /// <summary>The requested broadcaster ID</summary>
    [JsonPropertyName("broadcaster_user_id")]
    public partial long BroadcasterId { get; }
    /// <summary>The requested broadcaster login</summary>
    [JsonPropertyName("broadcaster_user_login"), Intern]
    public partial string BroadcasterUsername { get; }
    /// <summary>The requested broadcaster display name</summary>
    [JsonPropertyName("broadcaster_user_name"), Intern]
    public partial string BroadcasterDisplayName { get; }
    /// <summary>The user ID of the removed moderator</summary>
    public partial long UserId { get; }
    /// <summary>The user login of the removed moderator</summary>
    [JsonPropertyName("user_login")]
    public partial string Username { get; }
    /// <summary>The display name of the removed moderator</summary>
    [JsonPropertyName("user_name")]
    public partial string UserDisplayName { get; }
}
