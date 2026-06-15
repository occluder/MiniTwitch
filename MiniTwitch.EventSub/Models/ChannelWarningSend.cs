using System.Text.Json.Serialization;
using MiniTwitch.EventSub.Internal;

namespace MiniTwitch.EventSub.Models;

/// <summary>
/// A notification when a warning is sent to a user
/// </summary>
[EventSubEvent("channel.warning.send", "1")]
public partial struct ChannelWarningSend
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
    /// <summary>The user ID of the moderator who sent the warning</summary>
    [JsonPropertyName("moderator_user_id")]
    public partial long ModeratorId { get; }
    /// <summary>The login of the moderator</summary>
    [JsonPropertyName("moderator_user_login")]
    public partial string ModeratorUsername { get; }
    /// <summary>The user name of the moderator</summary>
    [JsonPropertyName("moderator_user_name")]
    public partial string ModeratorDisplayName { get; }
    /// <summary>The ID of the user being warned</summary>
    public partial long UserId { get; }
    /// <summary>The login of the user being warned</summary>
    [JsonPropertyName("user_login")]
    public partial string Username { get; }
    /// <summary>The user name of the user being warned</summary>
    [JsonPropertyName("user_name")]
    public partial string UserDisplayName { get; }
    /// <summary>The reason given for the warning</summary>
    public partial string? Reason { get; }
    /// <summary>The chat rules cited for the warning</summary>
    public partial string[]? ChatRulesCited { get; }
}
