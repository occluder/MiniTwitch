using System.Text.Json.Serialization;
using MiniTwitch.EventSub.Internal;

namespace MiniTwitch.EventSub.Models;

/// <summary>
/// A notification when a suspicious user has been updated
/// </summary>
[EventSubEvent("channel.suspicious_user.update", "1")]
public partial struct ChannelSuspiciousUserUpdate
{
    /// <summary>The ID of the channel where the treatment for a suspicious user was updated</summary>
    [JsonPropertyName("broadcaster_user_id")]
    public partial long BroadcasterId { get; }
    /// <summary>The login of the channel where the treatment for a suspicious user was updated</summary>
    [JsonPropertyName("broadcaster_user_login"), Intern]
    public partial string BroadcasterUsername { get; }
    /// <summary>The display name of the channel where the treatment for a suspicious user was updated</summary>
    [JsonPropertyName("broadcaster_user_name"), Intern]
    public partial string BroadcasterDisplayName { get; }
    /// <summary>The ID of the moderator that updated the treatment for a suspicious user</summary>
    [JsonPropertyName("moderator_user_id")]
    public partial long ModeratorId { get; }
    /// <summary>The login of the moderator that updated the treatment for a suspicious user</summary>
    [JsonPropertyName("moderator_user_login")]
    public partial string ModeratorUsername { get; }
    /// <summary>The display name of the moderator that updated the treatment for a suspicious user</summary>
    [JsonPropertyName("moderator_user_name")]
    public partial string ModeratorDisplayName { get; }
    /// <summary>The ID of the suspicious user whose treatment was updated</summary>
    public partial long UserId { get; }
    /// <summary>The login of the suspicious user whose treatment was updated</summary>
    [JsonPropertyName("user_login")]
    public partial string Username { get; }
    /// <summary>The display name of the suspicious user whose treatment was updated</summary>
    [JsonPropertyName("user_name")]
    public partial string UserDisplayName { get; }
    /// <summary>
    /// The status set for the suspicious user.
    /// Possible values: <c>none</c>, <c>active_monitoring</c>, <c>restricted</c>
    /// </summary>
    [Intern]
    public partial string LowTrustStatus { get; }
}
