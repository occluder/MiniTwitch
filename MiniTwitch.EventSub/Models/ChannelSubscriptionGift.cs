using System.Text.Json.Serialization;
using MiniTwitch.EventSub.Internal;

namespace MiniTwitch.EventSub.Models;

/// <summary>
/// A notification when a user gives one or more gifted subscriptions in a channel
/// </summary>
[EventSubEvent("channel.subscription.gift", "1")]
public partial struct ChannelSubscriptionGift
{
    /// <summary>
    /// The user ID of the user who sent the subscription gift.
    /// Set to <c>null</c> if it was an anonymous subscription gift
    /// </summary>
    public partial long? UserId { get; }
    /// <summary>
    /// The user login of the user who sent the gift.
    /// Set to <c>null</c> if it was an anonymous subscription gift
    /// </summary>
    [JsonPropertyName("user_login")]
    public partial string? Username { get; }
    /// <summary>
    /// The user display name of the user who sent the gift.
    /// Set to <c>null</c> if it was an anonymous subscription gift
    /// </summary>
    [JsonPropertyName("user_name")]
    public partial string? UserDisplayName { get; }
    /// <summary>The broadcaster user ID</summary>
    [JsonPropertyName("broadcaster_user_id")]
    public partial long BroadcasterId { get; }
    /// <summary>The broadcaster login</summary>
    [JsonPropertyName("broadcaster_user_login"), Intern]
    public partial string BroadcasterUsername { get; }
    /// <summary>The broadcaster display name</summary>
    [JsonPropertyName("broadcaster_user_name"), Intern]
    public partial string BroadcasterDisplayName { get; }
    /// <summary>The number of subscriptions in the subscription gift</summary>
    public partial int Total { get; }
    /// <summary>The tier of subscriptions in the subscription gift</summary>
    public partial string Tier { get; }
    /// <summary>
    /// The number of subscriptions gifted by this user in the channel.
    /// This value is <c>null</c> for anonymous gifts or if the gifter has opted out
    /// </summary>
    public partial int? CumulativeTotal { get; }
    /// <summary>Whether the subscription gift was anonymous</summary>
    public partial bool IsAnonymous { get; }
}
