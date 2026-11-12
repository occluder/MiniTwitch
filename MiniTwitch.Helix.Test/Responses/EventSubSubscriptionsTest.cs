using System.Text.Json;
using MiniTwitch.Helix.Enums;
using MiniTwitch.Helix.Internal;
using MiniTwitch.Helix.Responses;

namespace MiniTwitch.Helix.Test.Responses;

public class EventSubSubscriptionsTest
{
    static readonly JsonSerializerOptions _options = HelixApiClient.SerializerOptions;

    [Fact]
    public void ReadsConditionAsObjectAndConduitTransport()
    {
        string json = """
            {
              "total": 1,
              "data": [
                {
                  "id": "26b1c993-bfcf-44d9-b876-379dacafe75a",
                  "status": "conduit_deleted",
                  "type": "stream.online",
                  "version": "1",
                  "condition": {
                    "broadcaster_user_id": "1234",
                    "reward_id": "5678"
                  },
                  "created_at": "2020-11-10T20:08:33.12345678Z",
                  "transport": {
                    "method": "conduit",
                    "conduit_id": "bfcfc993-26b1-b876-44d9-afe75a379dac"
                  },
                  "cost": 1,
                  "is_batching_enabled": true
                }
              ],
              "total_cost": 1,
              "max_total_cost": 10000,
              "pagination": {}
            }
            """;

        var response = JsonSerializer.Deserialize<EventSubSubscriptions>(json, _options);

        Assert.NotNull(response);
        var sub = Assert.Single(response.Data);
        Assert.Equal(EventSubStatus.ConduitDeleted, sub.Status);
        Assert.Equal("1234", sub.Condition["broadcaster_user_id"]);
        Assert.Equal("5678", sub.Condition["reward_id"]);
        Assert.Equal("conduit", sub.Transport.Method);
        Assert.Equal("bfcfc993-26b1-b876-44d9-afe75a379dac", sub.Transport.ConduitId);
        Assert.Null(sub.Transport.Callback);
        Assert.True(sub.IsBatchingEnabled);
        Assert.Equal(1, response.Total);
        Assert.Equal(1, response.TotalCost);
        Assert.Equal(10000, response.MaxTotalCost);
    }

    [Fact]
    public void OmittedBatchingFlag_DefaultsToFalse()
    {
        string json = """
            {
              "data": [
                {
                  "id": "35016908-41ff-33ce-7879-61b8dfc2ee16",
                  "status": "enabled",
                  "type": "stream.online",
                  "version": "1",
                  "condition": {
                    "broadcaster_user_id": "1234"
                  },
                  "created_at": "2020-11-10T14:32:18.730260295Z",
                  "transport": {
                    "method": "webhook",
                    "callback": "https://this-is-a-callback.com"
                  },
                  "cost": 1
                }
              ]
            }
            """;

        var response = JsonSerializer.Deserialize<EventSubSubscriptions>(json, _options);

        Assert.NotNull(response);
        var sub = Assert.Single(response.Data);
        Assert.False(sub.IsBatchingEnabled);
        Assert.Equal(EventSubStatus.Enabled, sub.Status);
        Assert.Null(sub.Transport.ConduitId);
    }
}
