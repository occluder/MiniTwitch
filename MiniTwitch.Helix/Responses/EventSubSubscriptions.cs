using System.Text.Json.Serialization;
using MiniTwitch.Helix.Enums;
using MiniTwitch.Helix.Models;

namespace MiniTwitch.Helix.Responses;

public class EventSubSubscriptions : PaginableResponse<EventSubSubscriptions.Subscription>
{
    [JsonPropertyName("total_cost")]
    public int TotalCost { get; init; }
    [JsonPropertyName("max_total_cost")]
    public int MaxTotalCost { get; init; }
    [JsonPropertyName("total")]
    public int Total { get; init; }

    public record Condition(
        [property: JsonPropertyName("broadcaster_user_id")] long BroadcasterId,
      long UserId
    );

    public record Subscription(
        string Id,
        [property: JsonConverter(typeof(JsonStringEnumConverter))]
        EventSubStatus Status,
        string Type,
        string Version,
        Condition Condition,
        string CreatedAt,
        Transport Transport,
        int Cost
    );

    public record Transport(
        string Method,
        string Callback
    );
}