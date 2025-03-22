using System.Text.Json.Serialization;

namespace MiniTwitch.Helix.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum RewardRedemptionStatus
{
    CANCELED,
    FULFILLED,
    UNFULFILLED
}