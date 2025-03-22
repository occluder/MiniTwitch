using System.Text.Json.Serialization;
using MiniTwitch.Helix.Enums;
using MiniTwitch.Helix.Models;

namespace MiniTwitch.Helix.Responses;

public class CustomRewardRedemptions : PaginableResponse<CustomRewardRedemptions.Info>
{
    public record Info(
        [property: JsonPropertyName("broadcaster_name")] string BroadcasterDisplayName,
        [property: JsonPropertyName("broadcaster_login")] string BroadcasterName,
        long BroadcasterId,
        [property: JsonPropertyName("user_name")] string UserDisplayName,
        [property: JsonPropertyName("user_login")] string UserName,
        long UserId,
        string Id,
        string UserInput,
        DateTime RedeemedAt,
        RedemptionReward Reward,
        RewardRedemptionStatus Status
    );

    public record RedemptionReward(
        string Title,
        string Id,
        string Prompt,
        long Cost
    );
}

public class CustomRewardRedemption : PaginableResponse<CustomRewardRedemptions.Info>
{
}