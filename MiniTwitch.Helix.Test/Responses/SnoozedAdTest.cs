using System.Text.Json;
using MiniTwitch.Helix.Internal;
using MiniTwitch.Helix.Responses;

namespace MiniTwitch.Helix.Test.Responses;

public class SnoozedAdTest
{
    static readonly JsonSerializerOptions _options = HelixApiClient.SerializerOptions;

    [Fact]
    public void ReadsUnixTimestamps()
    {
        string json = """
            {
              "data": [
                {
                  "snooze_count": 1,
                  "snooze_refresh_at": 1690932198,
                  "next_ad_at": 1690931298
                }
              ]
            }
            """;

        var response = JsonSerializer.Deserialize<SnoozedAd>(json, _options);

        Assert.NotNull(response);
        var ad = Assert.Single(response.Data);
        Assert.Equal(1, ad.SnoozeCount);
        Assert.Equal(DateTimeOffset.FromUnixTimeSeconds(1690932198).UtcDateTime, ad.SnoozeRefreshAt);
        Assert.Equal(DateTimeOffset.FromUnixTimeSeconds(1690931298).UtcDateTime, ad.NextAdAt);
    }

    [Fact]
    public void TreatsZeroTimestampsAsNull()
    {
        string json = """{"data":[{"snooze_count":0,"snooze_refresh_at":0,"next_ad_at":0}]}""";

        var response = JsonSerializer.Deserialize<SnoozedAd>(json, _options);

        Assert.NotNull(response);
        var ad = Assert.Single(response.Data);
        Assert.Null(ad.SnoozeRefreshAt);
        Assert.Null(ad.NextAdAt);
    }
}
