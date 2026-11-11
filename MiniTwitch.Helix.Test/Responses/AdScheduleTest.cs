using System.Text.Json;
using MiniTwitch.Helix.Internal;
using MiniTwitch.Helix.Responses;

namespace MiniTwitch.Helix.Test.Responses;

public class AdScheduleTest
{
    static readonly JsonSerializerOptions _options = HelixApiClient.SerializerOptions;

    [Fact]
    public void ReadsUnixTimestamps()
    {
        string json = """
            {
              "data": [
                {
                  "next_ad_at": 1690931298,
                  "last_ad_at": 1690930398,
                  "duration": 60,
                  "preroll_free_time": 90,
                  "snooze_count": 1,
                  "snooze_refresh_at": 1690932198
                }
              ]
            }
            """;

        var response = JsonSerializer.Deserialize<AdSchedule>(json, _options);

        Assert.NotNull(response);
        var ad = Assert.Single(response.Data);
        Assert.Equal(DateTimeOffset.FromUnixTimeSeconds(1690931298).UtcDateTime, ad.NextAdAt);
        Assert.Equal(DateTimeOffset.FromUnixTimeSeconds(1690930398).UtcDateTime, ad.LastAdAt);
        Assert.Equal(DateTimeOffset.FromUnixTimeSeconds(1690932198).UtcDateTime, ad.SnoozeRefreshAt);
        Assert.Equal(60, ad.Duration);
        Assert.Equal(90, ad.PrerollFreeTime);
        Assert.Equal(1, ad.SnoozeCount);
    }

    [Fact]
    public void TreatsZeroTimestampsAsNull()
    {
        string json = """
            {
              "data": [
                {
                  "next_ad_at": 0,
                  "last_ad_at": 0,
                  "duration": 0,
                  "preroll_free_time": 0,
                  "snooze_count": 0,
                  "snooze_refresh_at": 0
                }
              ]
            }
            """;

        var response = JsonSerializer.Deserialize<AdSchedule>(json, _options);

        Assert.NotNull(response);
        var ad = Assert.Single(response.Data);
        Assert.Null(ad.NextAdAt);
        Assert.Null(ad.LastAdAt);
        Assert.Null(ad.SnoozeRefreshAt);
    }
}
