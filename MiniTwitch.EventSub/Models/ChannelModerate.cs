using System.Text.Json.Serialization;
using MiniTwitch.EventSub.Internal;

namespace MiniTwitch.EventSub.Models;

/// <summary>
/// A notification when a moderator performs a moderation action in a channel
/// </summary>
[EventSubEvent("channel.moderate", "1")]
public partial struct ChannelModerate
{
    /// <summary>The ID of the broadcaster</summary>
    [JsonPropertyName("broadcaster_user_id")]
    public partial long BroadcasterId { get; }
    /// <summary>The login of the broadcaster</summary>
    [JsonPropertyName("broadcaster_user_login"), Intern]
    public partial string BroadcasterUsername { get; }
    /// <summary>The user name of the broadcaster</summary>
    [JsonPropertyName("broadcaster_user_name"), Intern]
    public partial string BroadcasterDisplayName { get; }
    /// <summary>
    /// The channel in which the action originally occurred.
    /// Is the same as <c>BroadcasterId</c> if not in shared chat
    /// </summary>
    [JsonPropertyName("source_broadcaster_user_id")]
    public partial long? SourceBroadcasterId { get; }
    /// <summary>
    /// The channel in which the action originally occurred.
    /// Is the same as <c>BroadcasterUsername</c> if not in shared chat
    /// </summary>
    [JsonPropertyName("source_broadcaster_user_login")]
    public partial string? SourceBroadcasterUsername { get; }
    /// <summary>
    /// The channel in which the action originally occurred.
    /// Is null when the moderator action happens in the same channel as the broadcaster
    /// </summary>
    [JsonPropertyName("source_broadcaster_user_name")]
    public partial string? SourceBroadcasterDisplayName { get; }
    /// <summary>The ID of the moderator who performed the action</summary>
    [JsonPropertyName("moderator_user_id")]
    public partial long ModeratorId { get; }
    /// <summary>The login of the moderator</summary>
    [JsonPropertyName("moderator_user_login")]
    public partial string ModeratorUsername { get; }
    /// <summary>The user name of the moderator</summary>
    [JsonPropertyName("moderator_user_name")]
    public partial string ModeratorDisplayName { get; }
    /// <summary>The type of action</summary>
    public partial string Action { get; }
    /// <summary>Metadata associated with the followers command</summary>
    public partial ActionFollowers? Followers { get; }
    /// <summary>Metadata associated with the slow command</summary>
    public partial ActionSlow? Slow { get; }
    /// <summary>Metadata associated with the vip command</summary>
    public partial ActionUser? Vip { get; }
    /// <summary>Metadata associated with the unvip command</summary>
    public partial ActionUser? Unvip { get; }
    /// <summary>Metadata associated with the mod command</summary>
    public partial ActionUser? Mod { get; }
    /// <summary>Metadata associated with the unmod command</summary>
    public partial ActionUser? Unmod { get; }
    /// <summary>Metadata associated with the ban command</summary>
    public partial ActionBan? Ban { get; }
    /// <summary>Metadata associated with the unban command</summary>
    public partial ActionUser? Unban { get; }
    /// <summary>Metadata associated with the timeout command</summary>
    public partial ActionTimeout? Timeout { get; }
    /// <summary>Metadata associated with the untimeout command</summary>
    public partial ActionUser? Untimeout { get; }
    /// <summary>Metadata associated with the raid command</summary>
    public partial ActionRaid? Raid { get; }
    /// <summary>Metadata associated with the unraid command</summary>
    public partial ActionUser? Unraid { get; }
    /// <summary>Metadata associated with the delete command</summary>
    public partial ActionDelete? Delete { get; }
    /// <summary>Metadata associated with the automod terms changes</summary>
    public partial ActionAutomodTerms? AutomodTerms { get; }
    /// <summary>Metadata associated with an unban request</summary>
    public partial ActionUnbanRequest? UnbanRequest { get; }
    /// <summary>Information about the shared_chat_ban event</summary>
    public partial ActionBan? SharedChatBan { get; }
    /// <summary>Information about the shared_chat_unban event</summary>
    public partial ActionUser? SharedChatUnban { get; }
    /// <summary>Information about the shared_chat_timeout event</summary>
    public partial ActionTimeout? SharedChatTimeout { get; }
    /// <summary>Information about the shared_chat_untimeout event</summary>
    public partial ActionUser? SharedChatUntimeout { get; }
    /// <summary>Information about the shared_chat_delete event</summary>
    public partial ActionDelete? SharedChatDelete { get; }

}
