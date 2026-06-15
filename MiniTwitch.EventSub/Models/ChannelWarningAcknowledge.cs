using System.Text.Json.Serialization;
using MiniTwitch.EventSub.Internal;

namespace MiniTwitch.EventSub.Models;

/// <summary>
/// A notification when a warning is acknowledged by a user
/// </summary>
[EventSubEvent("channel.warning.acknowledge", "1")]
public partial struct ChannelWarningAcknowledge
{
    /// <summary>The user ID of the broadcaster</summary>
    [JsonPropertyName("broadcaster_user_id")]
    public partial long BroadcasterId { get; }
    /// <summary>The login of the broadcaster</summary>
    [JsonPropertyName("broadcaster_user_login"), Intern]
    public partial string BroadcasterUsername { get; }
    /// <summary>The user name of the broadcaster</summary>
    [JsonPropertyName("broadcaster_user_name"), Intern]
    public partial string BroadcasterDisplayName { get; }
    /// <summary>The ID of the user that has acknowledged their warning</summary>
    public partial long UserId { get; }
    /// <summary>The login of the user that has acknowledged their warning</summary>
    [JsonPropertyName("user_login")]
    public partial string Username { get; }
    /// <summary>The user name of the user that has acknowledged their warning</summary>
    [JsonPropertyName("user_name")]
    public partial string UserDisplayName { get; }
}
