using System.Text.Json;
using MiniTwitch.Helix.Internal;
using MiniTwitch.Helix.Requests;

namespace MiniTwitch.Helix.Test.Requests;

public class NewScheduleSegmentTest
{
    static readonly JsonSerializerOptions _options = HelixApiClient.SerializerOptions;

    [Fact]
    public void SerializesDurationAsIntegerMinutes()
    {
        var body = new NewScheduleSegment(
            new DateTime(2026, 11, 10, 18, 0, 0, DateTimeKind.Utc),
            "UTC",
            60
        );

        string json = JsonSerializer.Serialize(body, _options);

        Assert.Contains("\"duration\":60", json);
        Assert.DoesNotContain("duration_hours", json);
        Assert.Contains("\"start_time\":\"2026-11-10T18:00:00Z\"", json);
        Assert.Contains("\"timezone\":\"UTC\"", json);
    }

    [Fact]
    public void OmitsNullOptionalFields()
    {
        var body = new NewScheduleSegment(
            new DateTime(2026, 11, 10, 18, 0, 0, DateTimeKind.Utc),
            "UTC",
            60
        );

        string json = JsonSerializer.Serialize(body, _options);

        Assert.DoesNotContain("is_recurring", json);
        Assert.DoesNotContain("category_id", json);
        Assert.DoesNotContain("title", json);
    }

    [Fact]
    public void SerializesOptionalFields()
    {
        var body = new NewScheduleSegment(
            new DateTime(2026, 11, 10, 18, 0, 0, DateTimeKind.Utc),
            "UTC",
            60,
            isRecurring: true,
            categoryId: "509670",
            title: "TwitchDev Monthly Update"
        );

        string json = JsonSerializer.Serialize(body, _options);

        Assert.Contains("\"is_recurring\":true", json);
        Assert.Contains("\"category_id\":\"509670\"", json);
        Assert.Contains("\"title\":\"TwitchDev Monthly Update\"", json);
    }
}
