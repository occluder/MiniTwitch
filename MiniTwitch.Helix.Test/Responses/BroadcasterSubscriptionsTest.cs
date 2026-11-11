using System.Text.Json;
using MiniTwitch.Helix.Internal;
using MiniTwitch.Helix.Responses;

namespace MiniTwitch.Helix.Test.Responses;

public class BroadcasterSubscriptionsTest
{
    static readonly JsonSerializerOptions _options = HelixApiClient.SerializerOptions;

    [Fact]
    public void OmittedPointsAndTotalAreNull()
    {
        string json = """
            {
              "data": [
                {
                  "broadcaster_id": "1234",
                  "broadcaster_login": "broadcaster",
                  "broadcaster_name": "Broadcaster",
                  "gifter_id": "",
                  "gifter_login": "",
                  "gifter_name": "",
                  "is_gift": false,
                  "tier": "1000",
                  "plan_name": "Channel Subscription (broadcaster)",
                  "user_id": "5678",
                  "user_login": "subscriber",
                  "user_name": "Subscriber"
                }
              ],
              "pagination": {}
            }
            """;

        var response = JsonSerializer.Deserialize<BroadcasterSubscriptions>(json, _options);

        Assert.NotNull(response);
        Assert.Null(response.Points);
        Assert.Null(response.Total);

        var subscription = Assert.Single(response.Data);
        Assert.Null(subscription.GifterId);
        Assert.Empty(subscription.GifterName);
        Assert.False(subscription.IsGift);
    }

    [Fact]
    public void ReadsPointsTotalAndGifterId()
    {
        string json = """
            {
              "data": [
                {
                  "broadcaster_id": "141981764",
                  "broadcaster_login": "twitchdev",
                  "broadcaster_name": "TwitchDev",
                  "gifter_id": "12826",
                  "gifter_login": "twitch",
                  "gifter_name": "Twitch",
                  "is_gift": true,
                  "tier": "1000",
                  "plan_name": "Channel Subscription (twitchdev)",
                  "user_id": "527115020",
                  "user_login": "twitchgaming",
                  "user_name": "twitchgaming"
                }
              ],
              "pagination": {
                "cursor": "xxxx"
              },
              "total": 13,
              "points": 13
            }
            """;

        var response = JsonSerializer.Deserialize<BroadcasterSubscriptions>(json, _options);

        Assert.NotNull(response);
        Assert.Equal(13, response.Points);
        Assert.Equal(13, response.Total);

        var subscription = Assert.Single(response.Data);
        Assert.Equal(12826, subscription.GifterId);
        Assert.Equal("Twitch", subscription.GifterDisplayName);
    }
}
