using System.Text.Json.Serialization;
using MiniTwitch.EventSub.Internal;

namespace MiniTwitch.EventSub.Models;

/// <summary>
/// A user is notified if a message is caught by automod for review
/// </summary>
[EventSubEvent("automod.message.hold", "1")]
public partial struct AutomodMessageHold
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
    /// <summary>The ID of the message that was flagged by automod</summary>
    public partial Guid MessageId { get; }
    /// <summary>The body of the message</summary>
    public partial HeldMessage Message { get; }
    /// <summary>The category of the message</summary>
    public partial string Category { get; }
    /// <summary>The level of severity. Measured between 1 to 4</summary>
    public partial int Level { get; }
    /// <summary>The timestamp of when automod saved the message</summary>
    [JsonPropertyName("held_at")]
    public partial DateTimeOffset HeldAt { get; }

    /// <summary>The body of the message</summary>
    [EventProperty]
    public partial struct HeldMessage
    {
        /// <summary>The contents of the message caught by automod</summary>
        public partial string Text { get; }
        /// <summary>Metadata surrounding the potential inappropriate fragments of the message</summary>
        public partial MessageFragment[] Fragments { get; }
    }

    /// <summary>A message fragment</summary>
    [EventProperty]
    public partial struct MessageFragment
    {
        /// <summary>Message text in a fragment</summary>
        public partial string Text { get; }
        /// <summary>Optional. Metadata pertaining to the emote</summary>
        public partial EmoteV1? Emote { get; }
        /// <summary>Optional. Metadata pertaining to the cheermote</summary>
        public partial Cheermote? Cheermote { get; }
    }

}
