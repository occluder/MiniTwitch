using System.Text.Json.Serialization;
using MiniTwitch.EventSub.Internal;

namespace MiniTwitch.EventSub.Models;

/// <summary>
/// A notification when a subscription to the specified channel expires
/// </summary>
[EventSubEvent("channel.subscription.end", "1")]
public partial struct ChannelSubscriptionEnd
{
    /// <summary>The user ID for the user whose subscription ended</summary>
    public partial long UserId { get; }
    /// <summary>The user login for the user whose subscription ended</summary>
    [JsonPropertyName("user_login")]
    public partial string Username { get; }
    /// <summary>The user display name for the user whose subscription ended</summary>
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
    /// <summary>
    /// The tier of the subscription that ended.
    /// Valid values are <c>1000</c>, <c>2000</c>, and <c>3000</c>
    /// </summary>
    public partial string Tier { get; }
    /// <summary>Whether the subscription was a gift</summary>
    public partial bool IsGift { get; }
}
