using System.Text.Json.Serialization;
using MiniTwitch.EventSub.Internal;

namespace MiniTwitch.EventSub.Models;

[EventSubEvent("channel.chat.message", "1")]
public partial struct ChannelChatMessage
{
    [JsonPropertyName("broadcaster_user_id")]
    public partial long BroadcasterId { get; }
    [JsonPropertyName("broadcaster_user_login"), Intern]
    public partial string BroadcasterUsername { get; }
    [JsonPropertyName("broadcaster_user_name"), Intern]
    public partial string BroadcasterDisplayName { get; }
    [JsonPropertyName("chatter_user_id")]
    public partial long ChatterId { get; }
    [JsonPropertyName("chatter_user_login")]
    public partial string ChatterUsername { get; }
    [JsonPropertyName("chatter_user_name")]
    public partial string ChatterDisplayName { get; }
    public partial Guid MessageId { get; }
    public partial ChatMessage Message { get; }

    [EventProperty]
    public partial struct ChatMessage
    {
        public partial string Text { get; }
        public partial Fragment[] Fragments { get; }
        public partial string MessageType { get; }
        public partial Badge[] Badges { get; }
        public partial Cheer? Cheer { get; }
        public partial string Color { get; }
        public partial ChatReply? Reply { get; }
        public partial string? ChannelPointsCustomRewardId { get; }
        [JsonPropertyName("source_broadcaster_user_id")]
        public partial long? SourceBroadcasterId { get; }
        [JsonPropertyName("source_broadcaster_user_login")]
        public partial string? SourceBroadcasterUsername { get; }
        [JsonPropertyName("source_broadcaster_user_name")]
        public partial string? SourceBroadcasterDisplayname { get; }
        public partial Guid? SourceMessageId { get; }
        public partial Badge[]? SourceBadges { get; }
        public partial bool? IsSourceOnly { get; }

        [EventProperty]
        public partial struct Fragment
        {
            public partial string Type { get; }
            public partial string Text { get; }
            public partial Cheermote Cheermote { get; }
            public partial Emote Emote { get; }
            public partial Mention Mention { get; }
        }
    }

    [EventProperty]
    public partial struct ChatReply
    {
        public partial Guid ParentMessageId { get; }
        public partial string ParentMessageBody { get; }
        public partial long ParentUserId { get; }
        [JsonPropertyName("parent_user_login")]
        public partial string ParentUsername { get; }
        [JsonPropertyName("parent_user_name")]
        public partial string ParentUserDisplayName { get; }
        public partial Guid ThreadMessageId { get; }
        public partial long ThreadUserId { get; }
        [JsonPropertyName("thread_user_login")]
        public partial string ThreadUsername { get; }
        [JsonPropertyName("thread_user_name")]
        public partial string ThreadUserDisplayName { get; }

    }
}