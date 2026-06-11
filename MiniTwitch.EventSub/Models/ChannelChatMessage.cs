using System.Text.Json.Serialization;
using MiniTwitch.EventSub.Internal;

namespace MiniTwitch.EventSub.Models;

/// <summary>
/// The Channel Chat Message event is sent when a user sends a message to the specified channel's chat room
/// </summary>
[EventSubEvent("channel.chat.message", "1")]
public partial struct ChannelChatMessage
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
    /// <summary>The user ID of the user that sent the message</summary>
    [JsonPropertyName("chatter_user_id")]
    public partial long ChatterId { get; }
    /// <summary>The user login of the user that sent the message</summary>
    [JsonPropertyName("chatter_user_login")]
    public partial string ChatterUsername { get; }
    /// <summary>The user name of the user that sent the message</summary>
    [JsonPropertyName("chatter_user_name")]
    public partial string ChatterDisplayName { get; }
    /// <summary>A UUID that identifies the message</summary>
    public partial Guid MessageId { get; }
    /// <summary>The structured chat message</summary>
    public partial ChatMessage Message { get; }

    /// <summary>The structured chat message</summary>
    [EventProperty]
    public partial struct ChatMessage
    {
        /// <summary>The chat message in plain text</summary>
        public partial string Text { get; }
        /// <summary>Ordered list of chat message fragments</summary>
        public partial MessageFragment[] Fragments { get; }
        /// <summary>
        /// The type of message. Possible values:
        /// <c>text</c>, <c>channel_points_highlighted</c>, <c>channel_points_sub_only</c>,
        /// <c>user_intro</c>, <c>power_ups_message_effect</c>, <c>power_ups_gigantified_emote</c>
        /// </summary>
        public partial string MessageType { get; }
        /// <summary>List of chat badges</summary>
        public partial Badge[] Badges { get; }
        /// <summary>Metadata if this message is a cheer</summary>
        public partial Cheer? Cheer { get; }
        /// <summary>
        /// The color of the user's name in the chat room.
        /// This is a hexadecimal RGB color code in the form, <c>#&lt;RGB&gt;</c>.
        /// This may be empty if it is never set
        /// </summary>
        public partial string Color { get; }
        /// <summary>Metadata if this message is a reply</summary>
        public partial ChatReply? Reply { get; }
        /// <summary>
        /// The ID of a channel points custom reward that was redeemed
        /// </summary>
        public partial string? ChannelPointsCustomRewardId { get; }
        /// <summary>
        /// The broadcaster user ID of the channel the message was sent from.
        /// Is null when the message happens in the same channel as the broadcaster.
        /// Is not null when in a shared chat session, and the action happens in the channel of a participant other than the broadcaster
        /// </summary>
        [JsonPropertyName("source_broadcaster_user_id")]
        public partial long? SourceBroadcasterId { get; }
        /// <summary>
        /// The login of the broadcaster of the channel the message was sent from.
        /// Is null when the message happens in the same channel as the broadcaster.
        /// Is not null when in a shared chat session, and the action happens in the channel of a participant other than the broadcaster
        /// </summary>
        [JsonPropertyName("source_broadcaster_user_login")]
        public partial string? SourceBroadcasterUsername { get; }
        /// <summary>
        /// The user name of the broadcaster of the channel the message was sent from.
        /// Is null when the message happens in the same channel as the broadcaster.
        /// Is not null when in a shared chat session, and the action happens in the channel of a participant other than the broadcaster
        /// </summary>
        [JsonPropertyName("source_broadcaster_user_name")]
        public partial string? SourceBroadcasterDisplayName { get; }
        /// <summary>
        /// The UUID that identifies the source message from the channel the message was sent from.
        /// Is null when the message happens in the same channel as the broadcaster.
        /// Is not null when in a shared chat session, and the action happens in the channel of a participant other than the broadcaster
        /// </summary>
        public partial Guid? SourceMessageId { get; }
        /// <summary>
        /// The list of chat badges for the chatter in the channel the message was sent from.
        /// Is null when the message happens in the same channel as the broadcaster.
        /// Is not null when in a shared chat session, and the action happens in the channel of a participant other than the broadcaster
        /// </summary>
        public partial Badge[]? SourceBadges { get; }
        /// <summary>
        /// Determines if a message delivered during a shared chat session is only sent to the source channel.
        /// Has no effect if the message is not sent during a shared chat session
        /// </summary>
        public partial bool? IsSourceOnly { get; }
    }

    /// <summary>Metadata if the message is a reply</summary>
    [EventProperty]
    public partial struct ChatReply
    {
        /// <summary>An ID that uniquely identifies the parent message that this message is replying to</summary>
        public partial Guid ParentMessageId { get; }
        /// <summary>The message body of the parent message</summary>
        public partial string ParentMessageBody { get; }
        /// <summary>User ID of the sender of the parent message</summary>
        public partial long ParentUserId { get; }
        /// <summary>User login of the sender of the parent message</summary>
        [JsonPropertyName("parent_user_login")]
        public partial string ParentUsername { get; }
        /// <summary>User name of the sender of the parent message</summary>
        [JsonPropertyName("parent_user_name")]
        public partial string ParentUserDisplayName { get; }
        /// <summary>An ID that identifies the parent message of the reply thread</summary>
        public partial Guid ThreadMessageId { get; }
        /// <summary>User ID of the sender of the thread's parent message</summary>
        public partial long ThreadUserId { get; }
        /// <summary>User login of the sender of the thread's parent message</summary>
        [JsonPropertyName("thread_user_login")]
        public partial string ThreadUsername { get; }
        /// <summary>User name of the sender of the thread's parent message</summary>
        [JsonPropertyName("thread_user_name")]
        public partial string ThreadUserDisplayName { get; }
    }
}