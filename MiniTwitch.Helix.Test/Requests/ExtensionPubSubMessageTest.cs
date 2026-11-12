using System.Text.Json;
using MiniTwitch.Helix.Internal;
using MiniTwitch.Helix.Requests;

namespace MiniTwitch.Helix.Test.Requests;

public class ExtensionPubSubMessageTest
{
    static readonly JsonSerializerOptions _options = HelixApiClient.SerializerOptions;

    [Fact]
    public void SerializesBroadcasterIdForChannelBroadcasts()
    {
        var body = new ExtensionPubSubMessage(["broadcast"], 123456, "hello", isGlobalBroadcast: false);

        string json = JsonSerializer.Serialize(body, _options);

        Assert.Contains("\"broadcaster_id\":\"123456\"", json);
        Assert.Contains("\"is_global_broadcast\":false", json);
        Assert.Contains("\"target\":[\"broadcast\"]", json);
    }

    [Fact]
    public void OmitsBroadcasterIdForGlobalBroadcasts()
    {
        var body = new ExtensionPubSubMessage(["global"], "hello");

        string json = JsonSerializer.Serialize(body, _options);

        Assert.DoesNotContain("broadcaster_id", json);
        Assert.Contains("\"is_global_broadcast\":true", json);
        Assert.Contains("\"target\":[\"global\"]", json);
    }
}
