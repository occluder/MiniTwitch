using System.Text.Json;
using MiniTwitch.Helix.Internal;
using MiniTwitch.Helix.Requests;

namespace MiniTwitch.Helix.Test.Requests;

public class UpdatedScheduleSegmentTest
{
    static readonly JsonSerializerOptions _options = HelixApiClient.SerializerOptions;

    [Fact]
    public void SerializesDurationAsIntegerMinutes()
    {
        var body = new UpdatedScheduleSegment(
            new DateTime(2026, 11, 10, 18, 0, 0, DateTimeKind.Utc),
            "UTC",
            120
        );

        string json = JsonSerializer.Serialize(body, _options);

        Assert.Contains("\"duration\":120", json);
        Assert.DoesNotContain("duration_hours", json);
    }
}
