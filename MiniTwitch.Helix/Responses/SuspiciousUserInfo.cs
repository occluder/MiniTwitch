using MiniTwitch.Helix.Models;

namespace MiniTwitch.Helix.Responses;

public class SuspiciousUserInfo : BaseResponse<SuspiciousUserInfo.User>
{
    public record User(
        long UserId,
        long BroadcasterId,
        long ModeratorId,
        DateTime UpdatedAt,
        string Status,
        string[] Types
    );
}
