using System.Text.Json.Serialization;
using MiniTwitch.Helix.Internal.Json;
using MiniTwitch.Helix.Models;

namespace MiniTwitch.Helix.Responses;

public class SnoozedAd : BaseResponse<SnoozedAd.Info>
{
    public record Info(
        int SnoozeCount,
        [property: JsonConverter(typeof(UnixSecondsDateTimeConverter))] DateTime? SnoozeRefreshAt,
        [property: JsonConverter(typeof(UnixSecondsDateTimeConverter))] DateTime? NextAdAt
    );
}
