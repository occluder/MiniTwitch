using System.Text.Json.Serialization;
using MiniTwitch.Helix.Models;

namespace MiniTwitch.Helix.Responses;

public class GuestStarSession : BaseResponse<GuestStarSession.Info>
{
    public record Audio(
       bool IsAvailable,
       bool IsHostEnabled,
       bool IsGuestEnabled
   );

    public record Info(
        string Id,
        IReadOnlyList<Guest> Guests
    );

    public record Guest(
        string SlotId,
        long UserId,
        bool IsLive,
        int Volume,
        DateTime AssignedAt,
        [property: JsonPropertyName("audio")] Audio Audio,
        [property: JsonPropertyName("video")] Video Video
    );

    public record Video(
        bool IsAvailable,
        bool IsHostEnabled,
        bool IsGuestEnabled
    );
}
