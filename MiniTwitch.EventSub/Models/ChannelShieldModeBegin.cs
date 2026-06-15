using System.Text.Json.Serialization;
using MiniTwitch.EventSub.Internal;

namespace MiniTwitch.EventSub.Models;

/// <summary>
/// A notification when the broadcaster activates Shield Mode
/// </summary>
[EventSubEvent("channel.shield_mode.begin", "1")]
public partial struct ChannelShieldModeBegin
{
    /// <summary>An ID that identifies the broadcaster whose Shield Mode status was updated</summary>
    [JsonPropertyName("broadcaster_user_id")]
    public partial long BroadcasterId { get; }
    /// <summary>The broadcaster's login name</summary>
    [JsonPropertyName("broadcaster_user_login"), Intern]
    public partial string BroadcasterUsername { get; }
    /// <summary>The broadcaster's display name</summary>
    [JsonPropertyName("broadcaster_user_name"), Intern]
    public partial string BroadcasterDisplayName { get; }
    /// <summary>An ID that identifies the moderator that updated the Shield Mode's status</summary>
    [JsonPropertyName("moderator_user_id")]
    public partial long ModeratorId { get; }
    /// <summary>The moderator's login name</summary>
    [JsonPropertyName("moderator_user_login")]
    public partial string ModeratorUsername { get; }
    /// <summary>The moderator's display name</summary>
    [JsonPropertyName("moderator_user_name")]
    public partial string ModeratorDisplayName { get; }
    /// <summary>The UTC timestamp of when the moderator activated Shield Mode</summary>
    public partial DateTimeOffset StartedAt { get; }
}
