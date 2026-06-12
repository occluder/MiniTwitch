using System.Text.Json.Serialization;
using MiniTwitch.EventSub.Internal;

namespace MiniTwitch.EventSub.Models;

/// <summary>
/// A notification when a viewer is timed out or banned from the specified channel
/// </summary>
[EventSubEvent("channel.ban", "1")]
public partial struct ChannelBan
{
    /// <summary>The user ID for the user who was banned on the specified channel</summary>
    public partial long UserId { get; }
    /// <summary>The user login for the user who was banned on the specified channel</summary>
    [JsonPropertyName("user_login")]
    public partial string Username { get; }
    /// <summary>The user display name for the user who was banned on the specified channel</summary>
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
    /// <summary>The user ID of the issuer of the ban</summary>
    [JsonPropertyName("moderator_user_id")]
    public partial long ModeratorId { get; }
    /// <summary>The user login of the issuer of the ban</summary>
    [JsonPropertyName("moderator_user_login")]
    public partial string ModeratorUsername { get; }
    /// <summary>The user name of the issuer of the ban</summary>
    [JsonPropertyName("moderator_user_name")]
    public partial string ModeratorDisplayName { get; }
    /// <summary>The reason behind the ban</summary>
    public partial string Reason { get; }
    /// <summary>The UTC date and time of when the user was banned or put in a timeout</summary>
    public partial DateTimeOffset BannedAt { get; }
    /// <summary>
    /// The UTC date and time of when the timeout ends.
    /// Is <c>null</c> if the user was banned instead of put in a timeout
    /// </summary>
    public partial DateTimeOffset? EndsAt { get; }
    /// <summary>
    /// Indicates whether the ban is permanent (<c>true</c>) or a timeout (<c>false</c>).
    /// If <c>true</c>, <c>EndsAt</c> will be null
    /// </summary>
    public partial bool IsPermanent { get; }
}
