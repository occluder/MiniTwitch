using System.Text.Json.Serialization;
using MiniTwitch.EventSub.Internal;

namespace MiniTwitch.EventSub.Models;

/// <summary>
/// A notification when a user sends a resubscription chat message in a specific channel
/// </summary>
[EventSubEvent("channel.subscription.message", "1")]
public partial struct ChannelSubscriptionMessage
{
    /// <summary>The user ID of the user who sent a resubscription chat message</summary>
    public partial long UserId { get; }
    /// <summary>The user login of the user who sent a resubscription chat message</summary>
    [JsonPropertyName("user_login")]
    public partial string Username { get; }
    /// <summary>The user display name of the user who sent a resubscription chat message</summary>
    [JsonPropertyName("user_name")]
    public partial string UserDisplayName { get; }
    /// <summary>The broadcaster user ID</summary>
    [JsonPropertyName("broadcaster_user_id")]
    public partial long BroadcasterId { get; }
    /// <summary>The broadcaster login</summary>
    [JsonPropertyName("broadcaster_user_login"), Intern]
    public partial string BroadcasterUsername { get; }
    /// <summary>The broadcaster display name</summary>
    [JsonPropertyName("broadcaster_user_name"), Intern]
    public partial string BroadcasterDisplayName { get; }
    /// <summary>The tier of the user's subscription</summary>
    public partial string Tier { get; }
    /// <summary>An object that contains the resubscription message and emote information</summary>
    public partial SubMessage Message { get; }
    /// <summary>The total number of months the user has been subscribed to the channel</summary>
    public partial int CumulativeMonths { get; }
    /// <summary>
    /// The number of consecutive months the user's current subscription has been active.
    /// This value is <c>null</c> if the user has opted out of sharing this information
    /// </summary>
    public partial int? StreakMonths { get; }
    /// <summary>The month duration of the subscription</summary>
    public partial int DurationMonths { get; }

    /// <summary>The resubscription message and emote information</summary>
    [EventProperty]
    public partial struct SubMessage
    {
        /// <summary>The text of the resubscription message</summary>
        public partial string Text { get; }
        /// <summary>Emote information needed to recreate the message</summary>
        public partial EmoteInfo[] Emotes { get; }
    }

    /// <summary>An emote within the resubscription message</summary>
    [EventProperty]
    public partial struct EmoteInfo
    {
        /// <summary>The index of where the emote starts in the text</summary>
        public partial int Begin { get; }
        /// <summary>The index of where the emote ends in the text</summary>
        public partial int End { get; }
        /// <summary>The emote ID</summary>
        public partial string Id { get; }
    }
}
