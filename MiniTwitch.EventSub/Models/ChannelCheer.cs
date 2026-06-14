using System.Text.Json.Serialization;
using MiniTwitch.EventSub.Internal;

namespace MiniTwitch.EventSub.Models;

/// <summary>
/// A notification when a user cheers on the specified channel
/// </summary>
[EventSubEvent("channel.cheer", "1")]
public partial struct ChannelCheer
{
    /// <summary>Whether the user cheered anonymously or not</summary>
    public partial bool IsAnonymous { get; }
    /// <summary>
    /// The user ID for the user who cheered on the specified channel.
    /// This is null if <c>IsAnonymous</c> is true
    /// </summary>
    public partial long? UserId { get; }
    /// <summary>
    /// The user login for the user who cheered on the specified channel.
    /// This is null if <c>IsAnonymous</c> is true
    /// </summary>
    [JsonPropertyName("user_login")]
    public partial string? Username { get; }
    /// <summary>
    /// The user display name for the user who cheered on the specified channel.
    /// This is null if <c>IsAnonymous</c> is true
    /// </summary>
    [JsonPropertyName("user_name")]
    public partial string? UserDisplayName { get; }
    /// <summary>The requested broadcaster ID</summary>
    [JsonPropertyName("broadcaster_user_id")]
    public partial long BroadcasterId { get; }
    /// <summary>The requested broadcaster login</summary>
    [JsonPropertyName("broadcaster_user_login"), Intern]
    public partial string BroadcasterUsername { get; }
    /// <summary>The requested broadcaster display name</summary>
    [JsonPropertyName("broadcaster_user_name"), Intern]
    public partial string BroadcasterDisplayName { get; }
    /// <summary>The message sent with the cheer</summary>
    public partial string Message { get; }
    /// <summary>The number of bits cheered</summary>
    public partial int Bits { get; }
}
