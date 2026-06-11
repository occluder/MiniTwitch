using System.Text.Json.Serialization;
using MiniTwitch.EventSub.Internal;

namespace MiniTwitch.EventSub.Models;

/// <summary>
/// A user is notified if a message in the automod queue has its status changed.
/// Only public blocked terms trigger notifications, not private ones
/// </summary>
[EventSubEvent("automod.message.update", "2")]
public partial struct AutomodMessageUpdateV2
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
    /// <summary>The message sender's user ID</summary>
    public partial long UserId { get; }
    /// <summary>The message sender's login name</summary>
    [JsonPropertyName("user_login")]
    public partial string Username { get; }
    /// <summary>The message sender's display name</summary>
    [JsonPropertyName("user_name")]
    public partial string UserDisplayName { get; }
    /// <summary>The ID of the moderator</summary>
    [JsonPropertyName("moderator_user_id")]
    public partial long ModeratorId { get; }
    /// <summary>The login of the moderator</summary>
    [JsonPropertyName("moderator_user_login")]
    public partial string ModeratorUsername { get; }
    /// <summary>The moderator's user name</summary>
    [JsonPropertyName("moderator_user_name")]
    public partial string ModeratorDisplayName { get; }
    /// <summary>The ID of the message that was flagged by automod</summary>
    public partial string MessageId { get; }
    /// <summary>The body of the message</summary>
    public partial HeldMessage Message { get; }
    /// <summary>The message's status. Possible values: <c>approved</c>, <c>denied</c>, <c>expired</c></summary>
    [Intern]
    public partial string Status { get; }
    /// <summary>The timestamp of when automod saved the message</summary>
    public partial DateTimeOffset HeldAt { get; }
    /// <summary>
    /// The reason why the message was caught. Possible values:
    /// <c>automod</c>, <c>blocked_term</c>
    /// </summary>
    [Intern]
    public partial string Reason { get; }
    /// <summary>If the message was caught by automod, this will be populated</summary>
    public partial AutomodInfo? Automod { get; }
    /// <summary>If the message was caught due to a blocked term, this will be populated</summary>
    public partial BlockedTermInfo? BlockedTerm { get; }

    /// <summary>The body of the message</summary>
    [EventProperty]
    public partial struct HeldMessage
    {
        /// <summary>The contents of the message caught by automod</summary>
        public partial string Text { get; }
        /// <summary>Metadata surrounding the potential inappropriate fragments of the message</summary>
        public partial MessageFragment[] Fragments { get; }
    }

    /// <summary>If the message was caught by automod, this will be populated</summary>
    [EventProperty]
    public partial struct AutomodInfo
    {
        /// <summary>The category of the caught message</summary>
        public partial string Category { get; }
        /// <summary>The level of severity (1-4)</summary>
        public partial int Level { get; }
        /// <summary>The bounds of the text that caused the message to be caught</summary>
        public partial Boundary[] Boundaries { get; }
    }

    /// <summary>The bounds of the text that caused the message to be caught</summary>
    [EventProperty]
    public partial struct Boundary
    {
        /// <summary>Index in the message for the start of the problem (0 indexed, inclusive)</summary>
        public partial int StartPos { get; }
        /// <summary>Index in the message for the end of the problem (0 indexed, inclusive)</summary>
        public partial int EndPos { get; }
    }

    /// <summary>If the message was caught due to a blocked term, this will be populated</summary>
    [EventProperty]
    public partial struct BlockedTermInfo
    {
        /// <summary>The list of blocked terms found in the message</summary>
        public partial TermFound[] TermsFound { get; }
    }

    /// <summary>A blocked term found in the message</summary>
    [EventProperty]
    public partial struct TermFound
    {
        /// <summary>The id of the blocked term found</summary>
        public partial string TermId { get; }
        /// <summary>The user ID of the broadcaster that owns the blocked term</summary>
        [JsonPropertyName("owner_broadcaster_user_id")]
        public partial long OwnerBroadcasterId { get; }
        /// <summary>The login of the broadcaster that owns the blocked term</summary>
        [JsonPropertyName("owner_broadcaster_user_login")]
        public partial string OwnerBroadcasterUsername { get; }
        /// <summary>The user name of the broadcaster that owns the blocked term</summary>
        [JsonPropertyName("owner_broadcaster_user_name")]
        public partial string OwnerBroadcasterDisplayName { get; }
        /// <summary>The bounds of the text that caused the message to be caught</summary>
        public partial Boundary Boundary { get; }
    }
}
