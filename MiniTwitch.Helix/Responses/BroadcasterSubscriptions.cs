using System.Text.Json.Serialization;
using MiniTwitch.Helix.Internal.Json;
using MiniTwitch.Helix.Models;

namespace MiniTwitch.Helix.Responses;

public class BroadcasterSubscriptions : PaginableResponse<BroadcasterSubscriptions.Subscription>
{

    public int? Total { get; init; }

    public int? Points { get; init; }

    public record Subscription(
         long BroadcasterId,
        [property: JsonPropertyName("broadcaster_login")] string BroadcasterName,
        [property: JsonPropertyName("broadcaster_name")] string BroadcasterDisplayName,
        [property: JsonPropertyName("gifter_login")] string GifterName,
        [property: JsonPropertyName("gifter_name")] string GifterDisplayName,
         bool IsGift,
         string Tier,
         string PlanName,
       long UserId,
        [property: JsonPropertyName("user_name")] string UserDisplayName,
        [property: JsonPropertyName("user_login")] string Username,
        [property: JsonConverter(typeof(OptionalLongConverter))] long? GifterId
    );
}