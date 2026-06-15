using System.Text.Json.Serialization;
using MiniTwitch.EventSub.Internal;

namespace MiniTwitch.EventSub.Models;

/// <summary>A broadcaster participating in a shared Hype Train</summary>
[EventProperty]
public partial struct SharedTrainParticipant
{
    /// <summary>The ID of the broadcaster participating in the shared Hype Train</summary>
    [JsonPropertyName("broadcaster_user_id")]
    public partial long BroadcasterId { get; }
    /// <summary>The login of the broadcaster participating in the shared Hype Train</summary>
    [JsonPropertyName("broadcaster_user_login"), Intern]
    public partial string BroadcasterUsername { get; }
    /// <summary>The display name of the broadcaster participating in the shared Hype Train</summary>
    [JsonPropertyName("broadcaster_user_name"), Intern]
    public partial string BroadcasterDisplayName { get; }
}
