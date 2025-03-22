using System.Text.Json.Serialization;
using MiniTwitch.Helix.Models;

namespace MiniTwitch.Helix.Responses;

public class CustomReward : BaseResponse<CustomReward.Reward>
{
    public record Reward(
        [property: JsonPropertyName("broadcaster_name")] string BroadcasterDisplayName,
        [property: JsonPropertyName("broadcaster_login")] string BroadcasterName,
        long BroadcasterId,
        string Id,
        Image? Image,
        string BackgroundColor,
        bool IsEnabled,
        long Cost,
        string Title,
        string Prompt,
        bool IsUserInputRequired,
        MaxPerStreamSetting MaxPerStreamSetting,
        MaxPerUserPerStreamSetting MaxPerUserPerStreamSetting,
        GlobalCooldownSetting GlobalCooldownSetting,
        bool IsPaused,
        bool IsInStock,
        Image DefaultImage,
        bool ShouldRedemptionsSkipRequestQueue,
        [property: JsonPropertyName("redemptions_redeemed_current_stream")] int? RedemptionsRedeemedInCurrentStream,
        DateTime? CooldownExpiresAt
    );

    public record Image(
       [property: JsonPropertyName("url_1x")] string Url1x,
       [property: JsonPropertyName("url_2x")] string Url2x,
       [property: JsonPropertyName("url_4x")] string Url4x
    );

    public record GlobalCooldownSetting(
        bool IsEnabled,
        long GlobalCooldownSeconds
    );

    public record MaxPerStreamSetting(
        bool IsEnabled,
        long MaxPerStream
    );

    public record MaxPerUserPerStreamSetting(
        bool IsEnabled,
        long MaxPerUserPerStream
    );
}