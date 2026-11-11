using System.Text.Json;
using MiniTwitch.Helix.Internal;
using MiniTwitch.Helix.Responses;

namespace MiniTwitch.Helix.Test.Responses;

public class ExtensionConfigurationSegmentTest
{
    static readonly JsonSerializerOptions _options = HelixApiClient.SerializerOptions;

    [Fact]
    public void ReadsBroadcasterId()
    {
        string json = """
            {
              "data": [
                {
                  "segment": "broadcaster",
                  "broadcaster_id": "1234",
                  "content": "{}",
                  "version": "1"
                }
              ]
            }
            """;

        var response = JsonSerializer.Deserialize<ExtensionConfigurationSegment>(json, _options);

        Assert.NotNull(response);
        var segment = Assert.Single(response.Data);
        Assert.Equal("broadcaster", segment.Segment);
        Assert.Equal("1234", segment.BroadcasterId);
        Assert.Equal("{}", segment.Content);
        Assert.Equal("1", segment.Version);
    }

    [Fact]
    public void ReadsEmptyBroadcasterIdForGlobalSegment()
    {
        string json = """
            {
              "data": [
                {
                  "segment": "global",
                  "broadcaster_id": "",
                  "content": "hello config!",
                  "version": "0.0.1"
                }
              ]
            }
            """;

        var response = JsonSerializer.Deserialize<ExtensionConfigurationSegment>(json, _options);

        Assert.NotNull(response);
        var segment = Assert.Single(response.Data);
        Assert.Equal("global", segment.Segment);
        Assert.Empty(segment.BroadcasterId);
    }
}
