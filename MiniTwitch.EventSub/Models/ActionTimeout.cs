using System.Text.Json.Serialization;
using MiniTwitch.EventSub.Internal;

namespace MiniTwitch.EventSub.Models;

/// <summary>Metadata associated with a timeout action</summary>
[EventProperty]
public partial struct ActionTimeout
{
    /// <summary>The ID of the user being timed out</summary>
    public partial long UserId { get; }
    /// <summary>The login of the user being timed out</summary>
    [JsonPropertyName("user_login")]
    public partial string Username { get; }
    /// <summary>The user name of the user being timed out</summary>
    [JsonPropertyName("user_name")]
    public partial string UserDisplayName { get; }
    /// <summary>The reason given for the timeout</summary>
    public partial string? Reason { get; }
    /// <summary>The time at which the timeout ends</summary>
    public partial DateTimeOffset ExpiresAt { get; }
}
