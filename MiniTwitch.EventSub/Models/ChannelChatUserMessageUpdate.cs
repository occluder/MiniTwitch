using System.Text.Json.Serialization;
using MiniTwitch.EventSub.Internal;

namespace MiniTwitch.EventSub.Models;

/// <summary>
/// A notification when a user's message automod status is updated
/// </summary>
[EventSubEvent("channel.chat.user_message_update", "1")]
public partial struct ChannelChatUserMessageUpdate
{
    /// <summary>The ID of the broadcaster specified in the request</summary>
    [JsonPropertyName("broadcaster_user_id")]
    public partial long BroadcasterId { get; }
    /// <summary>The login of the broadcaster specified in the request</summary>
    [JsonPropertyName("broadcaster_user_login"), Intern]
    public partial string BroadcasterUsername { get; }
    /// <summary>The user name of the broadcaster specified in the request</summary>
    [JsonPropertyName("broadcaster_user_name"), Intern]
    public partial string BroadcasterDisplayName { get; }
    /// <summary>The User ID of the message sender</summary>
    public partial long UserId { get; }
    /// <summary>The message sender's login</summary>
    [JsonPropertyName("user_login")]
    public partial string Username { get; }
    /// <summary>The message sender's user name</summary>
    [JsonPropertyName("user_name")]
    public partial string UserDisplayName { get; }
    /// <summary>
    /// The message's status. Possible values:
    /// <c>approved</c>, <c>denied</c>, <c>invalid</c>
    /// </summary>
    public partial string Status { get; }
    /// <summary>The ID of the message that was flagged by automod</summary>
    public partial string MessageId { get; }
    /// <summary>The body of the message</summary>
    public partial HeldMessage Message { get; }

}
