using System.Text.Json.Serialization;
using MiniTwitch.EventSub.Internal;

namespace MiniTwitch.EventSub.Models;

/// <summary>
/// A notification when a user's message is caught by automod
/// </summary>
[EventSubEvent("channel.chat.user_message_hold", "1")]
public partial struct ChannelChatUserMessageHold
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
    /// <summary>The message sender's display name</summary>
    [JsonPropertyName("user_name")]
    public partial string UserDisplayName { get; }
    /// <summary>The ID of the message that was flagged by automod</summary>
    public partial string MessageId { get; }
    /// <summary>The body of the message</summary>
    public partial HeldMessage Message { get; }

    /// <summary>The body of the message</summary>
    [EventProperty]
    public partial struct HeldMessage
    {
        /// <summary>The contents of the message caught by automod</summary>
        public partial string Text { get; }
        /// <summary>Ordered list of chat message fragments</summary>
        public partial MessageFragment[] Fragments { get; }
    }
}
