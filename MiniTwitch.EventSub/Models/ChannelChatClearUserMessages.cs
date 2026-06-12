using System.Text.Json.Serialization;
using MiniTwitch.EventSub.Internal;

namespace MiniTwitch.EventSub.Models;

/// <summary>
/// A notification when a moderator or bot clears all messages for a specific user
/// </summary>
[EventSubEvent("channel.chat.clear_user_messages", "1")]
public partial struct ChannelChatClearUserMessages
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
    /// <summary>The ID of the user whose messages were deleted</summary>
    [JsonPropertyName("target_user_id")]
    public partial long TargetId { get; }
    /// <summary>The login of the user whose messages were deleted</summary>
    [JsonPropertyName("target_user_login")]
    public partial string TargetUsername { get; }
    /// <summary>The display name of the user whose messages were deleted</summary>
    [JsonPropertyName("target_user_name")]
    public partial string TargetDisplayName { get; }
}
