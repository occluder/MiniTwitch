using System.Text.Json.Serialization;
using MiniTwitch.EventSub.Internal;

namespace MiniTwitch.EventSub.Models;

[EventProperty]
public partial struct SubscriptionCondition
{
    [JsonPropertyName("broadcaster_user_id")]
    public partial long BroadcasterId { get; }
    public partial long UserId { get; }
}
