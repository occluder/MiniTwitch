using System.Text.Json.Serialization;
using MiniTwitch.EventSub.Internal;

namespace MiniTwitch.EventSub.Models;

/// <summary>
/// A notification when a moderator performs a moderation action in a channel.
/// This version adds warnings support
/// </summary>
[EventSubEvent("channel.moderate", "2")]
public partial struct ChannelModerateV2
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
    /// <summary>Metadata associated with the warn command</summary>
    public partial ActionWarn? Warn { get; }
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

    /// <summary>Metadata associated with a moderation action targeting a user</summary>
    [EventProperty]
    public partial struct ActionUser
    {
        /// <summary>The ID of the user</summary>
        public partial long UserId { get; }
        /// <summary>The login of the user</summary>
        [JsonPropertyName("user_login")]
        public partial string Username { get; }
        /// <summary>The user name of the user</summary>
        [JsonPropertyName("user_name")]
        public partial string UserDisplayName { get; }
    }

    /// <summary>Metadata associated with a warn action</summary>
    [EventProperty]
    public partial struct ActionWarn
    {
        /// <summary>The ID of the user being warned</summary>
        public partial long UserId { get; }
        /// <summary>The login of the user being warned</summary>
        [JsonPropertyName("user_login")]
        public partial string Username { get; }
        /// <summary>The user name of the user being warned</summary>
        [JsonPropertyName("user_name")]
        public partial string UserDisplayName { get; }
        /// <summary>Reason given for the warning</summary>
        public partial string? Reason { get; }
        /// <summary>Chat rules cited for the warning</summary>
        public partial string[]? ChatRulesCited { get; }
    }

    /// <summary>Metadata associated with a ban action</summary>
    [EventProperty]
    public partial struct ActionBan
    {
        /// <summary>The ID of the user being banned</summary>
        public partial long UserId { get; }
        /// <summary>The login of the user being banned</summary>
        [JsonPropertyName("user_login")]
        public partial string Username { get; }
        /// <summary>The user name of the user being banned</summary>
        [JsonPropertyName("user_name")]
        public partial string UserDisplayName { get; }
        /// <summary>Reason given for the ban</summary>
        public partial string? Reason { get; }
    }

    /// <summary>Metadata associated with a timeout action</summary>
    [EventProperty]
    public partial struct ActionTimeout
    {
        /// <summary>The ID of the user being timed out</summary>
        public partial long UserId { get; }
        /// <summary>The login of the user being timed out</summary>
        [JsonPropertyName("user_login")]
        public partial string Username { get; }
        /// <summary>The user name of the user being timed out</summary>
        [JsonPropertyName("user_name")]
        public partial string UserDisplayName { get; }
        /// <summary>The reason given for the timeout</summary>
        public partial string? Reason { get; }
        /// <summary>The time at which the timeout ends</summary>
        public partial DateTimeOffset ExpiresAt { get; }
    }

    /// <summary>Metadata associated with a delete action</summary>
    [EventProperty]
    public partial struct ActionDelete
    {
        /// <summary>The ID of the user whose message is being deleted</summary>
        public partial long UserId { get; }
        /// <summary>The login of the user</summary>
        [JsonPropertyName("user_login")]
        public partial string Username { get; }
        /// <summary>The user name of the user</summary>
        [JsonPropertyName("user_name")]
        public partial string UserDisplayName { get; }
        /// <summary>The ID of the message being deleted</summary>
        public partial string MessageId { get; }
        /// <summary>The message body of the message being deleted</summary>
        public partial string MessageBody { get; }
    }

    /// <summary>Metadata associated with a raid action</summary>
    [EventProperty]
    public partial struct ActionRaid
    {
        /// <summary>The ID of the user being raided</summary>
        public partial long UserId { get; }
        /// <summary>The login of the user being raided</summary>
        [JsonPropertyName("user_login")]
        public partial string Username { get; }
        /// <summary>The user name of the user raided</summary>
        [JsonPropertyName("user_name")]
        public partial string UserDisplayName { get; }
        /// <summary>The viewer count</summary>
        public partial int ViewerCount { get; }
    }

    /// <summary>Metadata associated with the followers command</summary>
    [EventProperty]
    public partial struct ActionFollowers
    {
        /// <summary>
        /// The length of time, in minutes, that the followers must have
        /// followed the broadcaster to participate in the chat room
        /// </summary>
        public partial int FollowDurationMinutes { get; }
    }

    /// <summary>Metadata associated with the slow command</summary>
    [EventProperty]
    public partial struct ActionSlow
    {
        /// <summary>
        /// The amount of time, in seconds, that users need to wait
        /// between sending messages
        /// </summary>
        public partial int WaitTimeSeconds { get; }
    }

    /// <summary>Metadata associated with the automod terms changes</summary>
    [EventProperty]
    public partial struct ActionAutomodTerms
    {
        /// <summary>Either <c>add</c> or <c>remove</c></summary>
        public partial string Action { get; }
        /// <summary>Either <c>blocked</c> or <c>permitted</c></summary>
        public partial string List { get; }
        /// <summary>Terms being added or removed</summary>
        public partial string[] Terms { get; }
        /// <summary>Whether the terms were added due to an Automod action</summary>
        public partial bool FromAutomod { get; }
    }

    /// <summary>Metadata associated with an unban request</summary>
    [EventProperty]
    public partial struct ActionUnbanRequest
    {
        /// <summary>Whether or not the unban request was approved</summary>
        public partial bool IsApproved { get; }
        /// <summary>The ID of the banned user</summary>
        public partial long UserId { get; }
        /// <summary>The login of the user</summary>
        [JsonPropertyName("user_login")]
        public partial string Username { get; }
        /// <summary>The user name of the user</summary>
        [JsonPropertyName("user_name")]
        public partial string UserDisplayName { get; }
        /// <summary>The message included by the moderator explaining their decision</summary>
        public partial string ModeratorMessage { get; }
    }
}
