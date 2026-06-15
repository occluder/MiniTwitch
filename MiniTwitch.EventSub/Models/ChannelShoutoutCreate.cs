using System.Text.Json.Serialization;
using MiniTwitch.EventSub.Internal;

namespace MiniTwitch.EventSub.Models;

/// <summary>
/// A notification when the specified broadcaster sends a Shoutout
/// </summary>
[EventSubEvent("channel.shoutout.create", "1")]
public partial struct ChannelShoutoutCreate
{
    /// <summary>An ID that identifies the broadcaster that sent the Shoutout</summary>
    [JsonPropertyName("broadcaster_user_id")]
    public partial long BroadcasterId { get; }
    /// <summary>The broadcaster's login name</summary>
    [JsonPropertyName("broadcaster_user_login"), Intern]
    public partial string BroadcasterUsername { get; }
    /// <summary>The broadcaster's display name</summary>
    [JsonPropertyName("broadcaster_user_name"), Intern]
    public partial string BroadcasterDisplayName { get; }
    /// <summary>An ID that identifies the broadcaster that received the Shoutout</summary>
    [JsonPropertyName("to_broadcaster_user_id")]
    public partial long ToBroadcasterId { get; }
    /// <summary>The broadcaster's login name</summary>
    [JsonPropertyName("to_broadcaster_user_login"), Intern]
    public partial string ToBroadcasterUsername { get; }
    /// <summary>The broadcaster's display name</summary>
    [JsonPropertyName("to_broadcaster_user_name"), Intern]
    public partial string ToBroadcasterDisplayName { get; }
    /// <summary>An ID that identifies the moderator that sent the Shoutout</summary>
    [JsonPropertyName("moderator_user_id")]
    public partial long ModeratorId { get; }
    /// <summary>The moderator's login name</summary>
    [JsonPropertyName("moderator_user_login")]
    public partial string ModeratorUsername { get; }
    /// <summary>The moderator's display name</summary>
    [JsonPropertyName("moderator_user_name")]
    public partial string ModeratorDisplayName { get; }
    /// <summary>The number of users that were watching the broadcaster's stream at the time of the Shoutout</summary>
    public partial int ViewerCount { get; }
    /// <summary>The UTC timestamp of when the moderator sent the Shoutout</summary>
    public partial DateTimeOffset StartedAt { get; }
    /// <summary>The UTC timestamp of when the broadcaster may send a Shoutout to a different broadcaster</summary>
    public partial DateTimeOffset CooldownEndsAt { get; }
    /// <summary>The UTC timestamp of when the broadcaster may send another Shoutout to the broadcaster in <c>ToBroadcasterId</c></summary>
    public partial DateTimeOffset TargetCooldownEndsAt { get; }
}
