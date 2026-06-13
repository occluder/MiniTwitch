using System.Text.Json.Serialization;
using MiniTwitch.EventSub.Internal;

namespace MiniTwitch.EventSub.Models;

/// <summary>
/// A notification when a broadcaster's chat settings are updated
/// </summary>
[EventSubEvent("channel.chat_settings.update", "1")]
public partial struct ChannelChatSettingsUpdate
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
    /// <summary>Whether chat messages must contain only emotes</summary>
    public partial bool EmoteMode { get; }
    /// <summary>Whether the broadcaster restricts the chat room to followers only</summary>
    public partial bool FollowerMode { get; }
    /// <summary>
    /// The length of time, in minutes, that followers must have followed to participate.
    /// Null if <c>FollowerMode</c> is false
    /// </summary>
    public partial int? FollowerModeDurationMinutes { get; }
    /// <summary>Whether the broadcaster limits how often users can send messages</summary>
    public partial bool SlowMode { get; }
    /// <summary>
    /// The amount of time, in seconds, that users need to wait between sending messages.
    /// Null if <c>SlowMode</c> is false
    /// </summary>
    public partial int? SlowModeWaitTimeSeconds { get; }
    /// <summary>Whether only subscribers can talk in the chat room</summary>
    public partial bool SubscriberMode { get; }
    /// <summary>Whether users must post only unique messages</summary>
    public partial bool UniqueChatMode { get; }
}
