using System.Text.Json.Serialization;
using MiniTwitch.EventSub.Internal;

namespace MiniTwitch.EventSub.Models;

/// <summary>
/// A notification when the active shared chat session the channel is in changes
/// </summary>
[EventSubEvent("channel.shared_chat.update", "1")]
public partial struct ChannelSharedChatUpdate
{
    /// <summary>The unique identifier for the shared chat session</summary>
    public partial string SessionId { get; }
    /// <summary>The User ID of the channel in the subscription condition</summary>
    [JsonPropertyName("broadcaster_user_id")]
    public partial long BroadcasterId { get; }
    /// <summary>The user login of the channel in the subscription condition</summary>
    [JsonPropertyName("broadcaster_user_login"), Intern]
    public partial string BroadcasterUsername { get; }
    /// <summary>The display name of the channel in the subscription condition</summary>
    [JsonPropertyName("broadcaster_user_name"), Intern]
    public partial string BroadcasterDisplayName { get; }
    /// <summary>The User ID of the host channel</summary>
    [JsonPropertyName("host_broadcaster_user_id")]
    public partial long HostBroadcasterId { get; }
    /// <summary>The user login of the host channel</summary>
    [JsonPropertyName("host_broadcaster_user_login"), Intern]
    public partial string HostBroadcasterUsername { get; }
    /// <summary>The display name of the host channel</summary>
    [JsonPropertyName("host_broadcaster_user_name"), Intern]
    public partial string HostBroadcasterDisplayName { get; }
    /// <summary>The list of participants in the session</summary>
    public partial Participant[] Participants { get; }

}
