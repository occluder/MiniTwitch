using System.Text.Json.Serialization;
using MiniTwitch.Helix.Models;

namespace MiniTwitch.Helix.Responses;

public class CustomPowerUp : BaseResponse<CustomPowerUp.PowerUp>
{
    public record PowerUp(
        long BroadcasterId,
        [property: JsonPropertyName("broadcaster_login")] string BroadcasterUsername,
        [property: JsonPropertyName("broadcaster_name")] string BroadcasterDisplayName,
        string Id,
        string Title,
        string Prompt,
        int Bits,
        PowerUpImage? Image,
        PowerUpImage DefaultImage,
        string BackgroundColor,
        bool IsEnabled,
        bool IsUserInputRequired,
        MaxSetting MaxPerStreamSetting,
        MaxPerUserSetting MaxPerUserPerStreamSetting,
        CooldownSetting GlobalCooldownSetting,
        bool IsPaused,
        bool IsInStock,
        int? RedemptionsRedeemedCurrentStream,
        string? CooldownExpiresAt
    );

    public record PowerUpImage(
        [property: JsonPropertyName("url_1x")] string Url1x,
        [property: JsonPropertyName("url_2x")] string Url2x,
        [property: JsonPropertyName("url_4x")] string Url4x
    );

    public record MaxSetting(
        bool IsEnabled,
        long MaxPerStream
    );

    public record MaxPerUserSetting(
        bool IsEnabled,
        long MaxPerUserPerStream
    );

    public record CooldownSetting(
        bool IsEnabled,
        long GlobalCooldownSeconds
    );
}
