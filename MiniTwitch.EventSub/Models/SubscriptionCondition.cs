using System.Text.Json.Serialization;
using MiniTwitch.EventSub.Internal;

namespace MiniTwitch.EventSub.Models;

[EventProperty]
public partial struct SubscriptionCondition
{
    [JsonPropertyName("broadcaster_user_id")]
    public partial long? BroadcasterId { get; }
    public partial long? UserId { get; }
    [JsonPropertyName("moderator_user_id")]
    public partial long? ModeratorId { get; }
    [JsonPropertyName("from_broadcaster_user_id")]
    public partial long? FromBroadcasterId { get; }
    [JsonPropertyName("to_broadcaster_user_id")]
    public partial long? ToBroadcasterId { get; }
    public partial string? ClientId { get; }
    public partial string? ConduitId { get; }
    public partial string? OrganizationId { get; }
    public partial string? CategoryId { get; }
    public partial string? CampaignId { get; }
    public partial string? ExtensionClientId { get; }
    public partial string? RewardId { get; }
}
