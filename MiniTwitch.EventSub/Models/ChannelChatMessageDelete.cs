using System.Text.Json.Serialization;
using MiniTwitch.EventSub.Internal;

namespace MiniTwitch.EventSub.Models;

/// <summary>
/// A notification when a moderator removes a specific message
/// </summary>
[EventSubEvent("channel.chat.message_delete", "1")]
public partial struct ChannelChatMessageDelete
{
    /// <summary>The broadcaster user ID</summary>
    [JsonPropertyName("broadcaster_user_id")]
    public partial long BroadcasterId { get; }
    /// <summary>The broadcaster login</summary>
    [JsonPropertyName("broadcaster_user_login"), Intern]
    public partial string BroadcasterUsername { get; }
    /// <summary>The broadcaster display name</summary>
    [JsonPropertyName("broadcaster_user_name"), Intern]
    public partial string BroadcasterDisplayName { get; }
    /// <summary>The ID of the user whose message was deleted</summary>
    [JsonPropertyName("target_user_id")]
    public partial long TargetId { get; }
    /// <summary>The user login of the user whose message was deleted</summary>
    [JsonPropertyName("target_user_login")]
    public partial string TargetUsername { get; }
    /// <summary>The user name of the user whose message was deleted</summary>
    [JsonPropertyName("target_user_name")]
    public partial string TargetDisplayName { get; }
    /// <summary>A UUID that identifies the message that was removed</summary>
    public partial Guid MessageId { get; }
}
