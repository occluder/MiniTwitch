using System.Text.Json.Serialization;
using MiniTwitch.EventSub.Internal;

namespace MiniTwitch.EventSub.Models;

/// <summary>
/// A notification when the specified broadcaster starts a stream
/// </summary>
[EventSubEvent("stream.online", "1")]
public partial struct StreamOnline
{
    /// <summary>The id of the stream</summary>
    public partial string Id { get; }
    /// <summary>The broadcaster's user id</summary>
    [JsonPropertyName("broadcaster_user_id")]
    public partial long BroadcasterId { get; }
    /// <summary>The broadcaster's user login</summary>
    [JsonPropertyName("broadcaster_user_login"), Intern]
    public partial string BroadcasterUsername { get; }
    /// <summary>The broadcaster's user display name</summary>
    [JsonPropertyName("broadcaster_user_name"), Intern]
    public partial string BroadcasterDisplayName { get; }
    /// <summary>
    /// The stream type. Valid values: <c>live</c>, <c>playlist</c>, <c>watch_party</c>, <c>premiere</c>, <c>rerun</c>
    /// </summary>
    [Intern]
    public partial string Type { get; }
    /// <summary>The timestamp at which the stream went online</summary>
    public partial DateTimeOffset StartedAt { get; }
}
