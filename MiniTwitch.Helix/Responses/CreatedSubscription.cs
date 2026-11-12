using System.Text.Json.Serialization;
using MiniTwitch.Helix.Enums;
using MiniTwitch.Helix.Internal.Json;
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

    public record Info(
        string Id,
        [property: JsonConverter(typeof(EnumConverter<EventSubStatus>))]
        EventSubStatus Status,
        string Type,
        string Version,
        IReadOnlyDictionary<string, string> Condition,
        string CreatedAt,
        Transport Transport,
        int Cost,
        bool IsBatchingEnabled
    );

    public record Transport(
        string Method,
        string? Callback,
        string? SessionId,
        string? ConnectedAt,
        string? ConduitId
    );
}
