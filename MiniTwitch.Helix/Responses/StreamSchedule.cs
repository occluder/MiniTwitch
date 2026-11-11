using System.Text.Json.Serialization;
using MiniTwitch.Helix.Interfaces;
using MiniTwitch.Helix.Models;

namespace MiniTwitch.Helix.Responses;

public class StreamSchedule : IPaginable
{
    [property: JsonPropertyName("data")]
    public ScheduleData Data { get; init; }

    public Pagination Pagination { get; init; }

    public record Category(
        string Id,
        string Name
    );

    public record ScheduleData(
        IReadOnlyList<Segment> Segments,
        long BroadcasterId,
        [property: JsonPropertyName("broadcaster_name")] string BroadcasterDisplayName,
        [property: JsonPropertyName("broadcaster_login")] string BroadcasterName,
        Vacation Vacation
    );

    public record Segment(
        string Id,
        DateTime StartTime,
        DateTime EndTime,
        string Title,
        DateTime? CanceledUntil,
        Category Category,
        bool IsRecurring
    );

    public record Vacation(
        DateTime StartTime,
        DateTime EndTime
    );
}