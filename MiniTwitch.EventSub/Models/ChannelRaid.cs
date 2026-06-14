using System.Text.Json.Serialization;
using MiniTwitch.EventSub.Internal;

namespace MiniTwitch.EventSub.Models;

/// <summary>
/// A notification when a broadcaster raids another broadcaster's channel
/// </summary>
[EventSubEvent("channel.raid", "1")]
public partial struct ChannelRaid
{
    /// <summary>The broadcaster ID that created the raid</summary>
    [JsonPropertyName("from_broadcaster_user_id")]
    public partial long FromBroadcasterId { get; }
    /// <summary>The broadcaster login that created the raid</summary>
    [JsonPropertyName("from_broadcaster_user_login"), Intern]
    public partial string FromBroadcasterUsername { get; }
    /// <summary>The broadcaster display name that created the raid</summary>
    [JsonPropertyName("from_broadcaster_user_name"), Intern]
    public partial string FromBroadcasterDisplayName { get; }
    /// <summary>The broadcaster ID that received the raid</summary>
    [JsonPropertyName("to_broadcaster_user_id")]
    public partial long ToBroadcasterId { get; }
    /// <summary>The broadcaster login that received the raid</summary>
    [JsonPropertyName("to_broadcaster_user_login"), Intern]
    public partial string ToBroadcasterUsername { get; }
    /// <summary>The broadcaster display name that received the raid</summary>
    [JsonPropertyName("to_broadcaster_user_name"), Intern]
    public partial string ToBroadcasterDisplayName { get; }
    /// <summary>The number of viewers in the raid</summary>
    public partial int Viewers { get; }
}
