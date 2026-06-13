using System.Text.Json.Serialization;
using MiniTwitch.EventSub.Internal;

namespace MiniTwitch.EventSub.Models;

/// <summary>
/// A notification when a specified channel receives a subscriber.
/// This does not include resubscribes
/// </summary>
[EventSubEvent("channel.subscribe", "1")]
public partial struct ChannelSubscribe
{
    /// <summary>The user ID for the user who subscribed to the specified channel</summary>
    public partial long UserId { get; }
    /// <summary>The user login for the user who subscribed to the specified channel</summary>
    [JsonPropertyName("user_login")]
    public partial string Username { get; }
    /// <summary>The user display name for the user who subscribed to the specified channel</summary>
    [JsonPropertyName("user_name")]
    public partial string UserDisplayName { get; }
    /// <summary>The requested broadcaster ID</summary>
    [JsonPropertyName("broadcaster_user_id")]
    public partial long BroadcasterId { get; }
    /// <summary>The requested broadcaster login</summary>
    [JsonPropertyName("broadcaster_user_login"), Intern]
    public partial string BroadcasterUsername { get; }
    /// <summary>The requested broadcaster display name</summary>
    [JsonPropertyName("broadcaster_user_name"), Intern]
    public partial string BroadcasterDisplayName { get; }
    /// <summary>
    /// The tier of the subscription.
    /// Valid values are <c>1000</c>, <c>2000</c>, and <c>3000</c>
    /// </summary>
    public partial string Tier { get; }
    /// <summary>Whether the subscription is a gift</summary>
    public partial bool IsGift { get; }
}
