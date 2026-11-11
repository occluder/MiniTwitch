namespace MiniTwitch.Helix.Requests;

public class NewScheduleSegment
{
    public DateTime StartTime { get; }
    public string Timezone { get; }
    public int Duration { get; }
    public bool? IsRecurring { get; }
    public string? CategoryId { get; }
    public string? Title { get; }

    public NewScheduleSegment(
        DateTime startTime,
        string timeZone,
        int duration,
        bool? isRecurring = null,
        string? categoryId = null,
        string? title = null
    )
    {
        this.StartTime = startTime;
        this.Timezone = timeZone;
        this.Duration = duration;
        this.IsRecurring = isRecurring;
        this.CategoryId = categoryId;
        this.Title = title;
    }
}
