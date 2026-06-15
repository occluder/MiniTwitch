using System.Text.Json.Serialization;
using MiniTwitch.EventSub.Internal;

namespace MiniTwitch.EventSub.Models;

/// <summary>
/// A user is notified if a message was caught by automod for review.
/// Only public blocked terms trigger notifications, not private ones
/// </summary>
[EventSubEvent("automod.message.hold", "2")]
public partial struct AutomodMessageHoldV2
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
    [JsonPropertyName("user_id")]
    public partial long UserId { get; }
    /// <summary>The message sender's login name</summary>
    [JsonPropertyName("user_login")]
    public partial string Username { get; }
    /// <summary>The message sender's display name</summary>
    [JsonPropertyName("user_name")]
    public partial string UserDisplayName { get; }
    /// <summary>The ID of the held message</summary>
    [JsonPropertyName("message_id")]
    public partial string MessageId { get; }
    /// <summary>The body of the message</summary>
    public partial HeldMessage Message { get; }
    /// <summary>
    /// The reason the message was held. Possible values:
    /// <c>automod</c>, <c>blocked_term</c>
    /// </summary>
    public partial string Reason { get; }
    /// <summary>If the message was caught by automod, this will be populated</summary>
    public partial AutomodInfo? Automod { get; }
    /// <summary>If the message was caught due to a blocked term, this will be populated</summary>
    [JsonPropertyName("blocked_term")]
    public partial BlockedTermInfo? BlockedTerm { get; }
    /// <summary>The timestamp of when automod saved the message</summary>
    [JsonPropertyName("held_at")]
    public partial DateTimeOffset HeldAt { get; }

    /// <summary>If the message was caught due to a blocked term, this will be populated</summary>
    [EventProperty]
    public partial struct BlockedTermInfo
    {
        /// <summary>The list of blocked terms found in the message</summary>
        [JsonPropertyName("terms_found")]
        public partial TermFound[] TermsFound { get; }
    }

    /// <summary>A blocked term found in the message</summary>
    [EventProperty]
    public partial struct TermFound
    {
        /// <summary>The id of the blocked term found</summary>
        [JsonPropertyName("term_id")]
        public partial string TermId { get; }
        /// <summary>The user ID of the broadcaster that owns the blocked term</summary>
        [JsonPropertyName("owner_broadcaster_user_id")]
        public partial long? OwnerBroadcasterId { get; }
        /// <summary>The login of the broadcaster that owns the blocked term</summary>
        [JsonPropertyName("owner_broadcaster_user_login")]
        public partial string? OwnerBroadcasterUsername { get; }
        /// <summary>The user name of the broadcaster that owns the blocked term</summary>
        [JsonPropertyName("owner_broadcaster_user_name")]
        public partial string? OwnerBroadcasterDisplayName { get; }
        /// <summary>The bounds of the text that caused the message to be caught</summary>
        public partial Boundary Boundary { get; }
    }
}
