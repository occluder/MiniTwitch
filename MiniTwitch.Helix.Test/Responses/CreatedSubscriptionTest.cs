using System.Text.Json;
using MiniTwitch.Helix.Enums;
using MiniTwitch.Helix.Internal;
using MiniTwitch.Helix.Responses;

namespace MiniTwitch.Helix.Test.Responses;

public class CreatedSubscriptionTest
{
    static readonly JsonSerializerOptions _options = HelixApiClient.SerializerOptions;

    [Fact]
    public void ReadsStatusAsEnumAndConditionAsObject()
    {
        string json = """
            {
              "data": [
                {
                  "id": "26b1c993-bfcf-44d9-b876-379dacafe75a",
                  "status": "webhook_callback_verification_pending",
                  "type": "user.update",
                  "version": "1",
                  "condition": {
                    "user_id": "1234"
                  },
                  "created_at": "2020-11-10T14:32:18.730260295Z",
                  "transport": {
                    "method": "webhook",
                    "callback": "https://this-is-a-callback.com"
                  },
                  "cost": 1
                }
              ],
              "total": 1,
              "total_cost": 1,
              "max_total_cost": 10000
            }
            """;

        var response = JsonSerializer.Deserialize<CreatedSubscription>(json, _options);

        Assert.NotNull(response);
        var sub = Assert.Single(response.Data);
        Assert.Equal(EventSubStatus.WebhookCallbackVerificationPending, sub.Status);
        Assert.Equal("1234", sub.Condition["user_id"]);
        Assert.Equal("webhook", sub.Transport.Method);
        Assert.Equal("https://this-is-a-callback.com", sub.Transport.Callback);
        Assert.False(sub.IsBatchingEnabled);
    }

    [Fact]
    public void ReadsConduitTransport()
    {
        string json = """
            {
              "data": [
                {
                  "id": "26b1c993-bfcf-44d9-b876-379dacafe75a",
                  "status": "enabled",
                  "type": "stream.online",
                  "version": "1",
                  "condition": {
                    "broadcaster_user_id": "1234"
                  },
                  "created_at": "2020-11-10T14:32:18.730260295Z",
                  "transport": {
                    "method": "conduit",
                    "conduit_id": "bfcfc993-26b1-b876-44d9-afe75a379dac"
                  },
                  "cost": 0,
                  "is_batching_enabled": true
                }
              ]
            }
            """;

        var response = JsonSerializer.Deserialize<CreatedSubscription>(json, _options);

        Assert.NotNull(response);
        var sub = Assert.Single(response.Data);
        Assert.Equal(EventSubStatus.Enabled, sub.Status);
        Assert.Equal("conduit", sub.Transport.Method);
        Assert.Equal("bfcfc993-26b1-b876-44d9-afe75a379dac", sub.Transport.ConduitId);
        Assert.Null(sub.Transport.Callback);
        Assert.True(sub.IsBatchingEnabled);
    }
}
