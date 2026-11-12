using System.Text.Json;
using MiniTwitch.Helix.Internal;
using MiniTwitch.Helix.Requests;

namespace MiniTwitch.Helix.Test.Requests;

public class NewSubscriptionTest
{
    static readonly JsonSerializerOptions _options = HelixApiClient.SerializerOptions;

    static NewSubscription CreateSubscription(bool? isBatchingEnabled = null) => new(
        "drop.entitlement.grant",
        "1",
        new NewSubscription.EventsubTransport("webhook", callback: "https://example.com"),
        new { organization_id = "1234" },
        isBatchingEnabled
    );

    [Fact]
    public void SerializesBatchingFlag()
    {
        string json = JsonSerializer.Serialize(CreateSubscription(true), _options);

        Assert.Contains("\"is_batching_enabled\":true", json);
    }

    [Fact]
    public void OmitsBatchingFlagWhenNotSet()
    {
        string json = JsonSerializer.Serialize(CreateSubscription(), _options);

        Assert.DoesNotContain("is_batching_enabled", json);
    }

    [Fact]
    public void SerializesConditionAsObject()
    {
        string json = JsonSerializer.Serialize(CreateSubscription(), _options);

        Assert.Contains("\"condition\":{\"organization_id\":\"1234\"}", json);
    }
}
