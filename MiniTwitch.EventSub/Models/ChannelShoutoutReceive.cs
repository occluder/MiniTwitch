using System.Text.Json.Serialization;
using MiniTwitch.EventSub.Internal;

namespace MiniTwitch.EventSub.Models;

/// <summary>
/// A notification when the specified broadcaster receives a Shoutout
/// </summary>
[EventSubEvent("channel.shoutout.receive", "1")]
public partial struct ChannelShoutoutReceive
{
    /// <summary>An ID that identifies the broadcaster that received the Shoutout</summary>
    [JsonPropertyName("broadcaster_user_id")]
    public partial long BroadcasterId { get; }
    /// <summary>The broadcaster's login name</summary>
    [JsonPropertyName("broadcaster_user_login"), Intern]
    public partial string BroadcasterUsername { get; }
    /// <summary>The broadcaster's display name</summary>
    [JsonPropertyName("broadcaster_user_name"), Intern]
    public partial string BroadcasterDisplayName { get; }
    /// <summary>An ID that identifies the broadcaster that sent the Shoutout</summary>
    [JsonPropertyName("from_broadcaster_user_id")]
    public partial long FromBroadcasterId { get; }
    /// <summary>The broadcaster's login name</summary>
    [JsonPropertyName("from_broadcaster_user_login"), Intern]
    public partial string FromBroadcasterUsername { get; }
    /// <summary>The broadcaster's display name</summary>
    [JsonPropertyName("from_broadcaster_user_name"), Intern]
    public partial string FromBroadcasterDisplayName { get; }
    /// <summary>The number of users that were watching the from-broadcaster's stream at the time of the Shoutout</summary>
    public partial int ViewerCount { get; }
    /// <summary>The UTC timestamp of when the moderator sent the Shoutout</summary>
    public partial DateTimeOffset StartedAt { get; }
}
