using System.Text.Json;
using MiniTwitch.Helix.Interfaces;
using MiniTwitch.Helix.Internal;
using MiniTwitch.Helix.Responses;

namespace MiniTwitch.Helix.Test.Responses;

public class StreamScheduleTest
{
    static readonly JsonSerializerOptions _options = HelixApiClient.SerializerOptions;

    [Fact]
    public void ReadsTopLevelPagination()
    {
        string json = """
            {
              "data": {
                "segments": [],
                "broadcaster_id": "1234",
                "broadcaster_name": "Broadcaster",
                "broadcaster_login": "broadcaster",
                "vacation": null
              },
              "pagination": {
                "cursor": "eyJiIjpudWxsLCJhIjp7Ik9mZnNldCI6NX19"
              }
            }
            """;

        var response = JsonSerializer.Deserialize<StreamSchedule>(json, _options);

        Assert.NotNull(response);
        Assert.Equal("eyJiIjpudWxsLCJhIjp7Ik9mZnNldCI6NX19", response.Pagination.Cursor);
        Assert.IsAssignableFrom<IPaginable>(response);
    }

    [Fact]
    public void ReadsSegmentsAndMissingPagination()
    {
        string json = """
            {
              "data": {
                "segments": [
                  {
                    "id": "segment-id",
                    "start_time": "2021-07-01T18:00:00Z",
                    "end_time": "2021-07-01T19:00:00Z",
                    "title": "TwitchDev Monthly Update",
                    "canceled_until": null,
                    "category": {
                      "id": "509670",
                      "name": "Science & Technology"
                    },
                    "is_recurring": false
                  }
                ],
                "broadcaster_id": "141981764",
                "broadcaster_name": "TwitchDev",
                "broadcaster_login": "twitchdev",
                "vacation": null
              }
            }
            """;

        var response = JsonSerializer.Deserialize<StreamSchedule>(json, _options);

        Assert.NotNull(response);
        var segment = Assert.Single(response.Data.Segments);
        Assert.Equal("segment-id", segment.Id);
        Assert.Equal("TwitchDev Monthly Update", segment.Title);
        Assert.Null(segment.CanceledUntil);
        Assert.Equal("509670", segment.Category.Id);
        Assert.False(segment.IsRecurring);
        Assert.Null(response.Pagination.Cursor);
    }
}
