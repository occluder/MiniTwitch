using System.Text.Json.Serialization;
using MiniTwitch.Helix.Models;

namespace MiniTwitch.Helix.Responses;

public class CreatedSubscription : BaseResponse<CreatedSubscription.Info>
{
    [JsonPropertyName("total")]
    public int Total { get; init; }
    [JsonPropertyName("total_cost")]
    public int TotalCost { get; init; }
    [JsonPropertyName("max_total_cost")]
    public int MaxTotalCost { get; init; }

    public record Condition(
      string BroadcasterUserId,
      string UserId
    );

    public record Info(
        string Id,
        string Status,
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