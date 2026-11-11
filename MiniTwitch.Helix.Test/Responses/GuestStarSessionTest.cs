using System.Text.Json;
using MiniTwitch.Helix.Internal;
using MiniTwitch.Helix.Responses;

namespace MiniTwitch.Helix.Test.Responses;

public class GuestStarSessionTest
{
    static readonly JsonSerializerOptions _options = HelixApiClient.SerializerOptions;

    [Fact]
    public void ReadsAudioAndVideoObjects()
    {
        string json = """
            {
              "data": [
                {
                  "id": "session-id",
                  "guests": [
                    {
                      "slot_id": "0",
                      "user_id": "1234",
                      "is_live": true,
                      "volume": 100,
                      "assigned_at": "2024-01-01T00:00:00Z",
                      "audio": {
                        "is_host_enabled": true,
                        "is_guest_enabled": true,
                        "is_available": true
                      },
                      "video": {
                        "is_host_enabled": true,
                        "is_guest_enabled": false,
                        "is_available": true
                      }
                    }
                  ]
                }
              ]
            }
            """;

        var response = JsonSerializer.Deserialize<GuestStarSession>(json, _options);

        Assert.NotNull(response);
        var session = Assert.Single(response.Data);
        Assert.Equal("session-id", session.Id);
        var guest = Assert.Single(session.Guests);
        Assert.Equal("0", guest.SlotId);
        Assert.Equal(1234, guest.UserId);
        Assert.True(guest.IsLive);
        Assert.Equal(100, guest.Volume);
        Assert.Equal(new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), guest.AssignedAt);
        Assert.True(guest.Audio.IsHostEnabled);
        Assert.True(guest.Audio.IsGuestEnabled);
        Assert.True(guest.Audio.IsAvailable);
        Assert.True(guest.Video.IsHostEnabled);
        Assert.False(guest.Video.IsGuestEnabled);
        Assert.True(guest.Video.IsAvailable);
    }
}
