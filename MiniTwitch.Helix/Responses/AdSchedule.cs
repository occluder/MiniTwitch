using System.Text.Json.Serialization;
using MiniTwitch.Helix.Internal.Json;
using MiniTwitch.Helix.Models;

namespace MiniTwitch.Helix.Responses;

public class AdSchedule : BaseResponse<AdSchedule.Ad>
{
    public record Ad(
         [property: JsonConverter(typeof(UnixSecondsDateTimeConverter))] DateTime? NextAdAt,
         [property: JsonConverter(typeof(UnixSecondsDateTimeConverter))] DateTime? LastAdAt,
         int Duration,
         int PrerollFreeTime,
         int SnoozeCount,
         [property: JsonConverter(typeof(UnixSecondsDateTimeConverter))] DateTime? SnoozeRefreshAt
    );
}
