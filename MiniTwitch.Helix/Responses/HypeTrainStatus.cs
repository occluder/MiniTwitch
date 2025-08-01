using System.Text.Json.Serialization;
using MiniTwitch.Helix.Models;

namespace MiniTwitch.Helix.Responses;

public class HypeTrainStatus : BaseResponse<HypeTrainStatus.Info>
{
    public record Info(
        HypeTrain Current,
        HypeTrainRecord AllTimeHigh,
        HypeTrainRecord SharedAllTimeHigh
    );

    public record HypeTrain(
        string Id,
        [property: JsonPropertyName("broadcaster_user_login")] string BroadcasterName,
        [property: JsonPropertyName("broadcaster_user_name")] string BroadcasterDisplayName,
        [property: JsonPropertyName("broadcaster_user_id")] long BroadcasterId,
        int Level,
        int Total,
        int Progress,
        int Goal,
        Contribution[] TopContributions,
        SharedParticipant[] SharedTrainParticipants,
        DateTime StartedAt,
        DateTime ExpiresAt,
        string Type
    );

    public record HypeTrainRecord(
        int Level,
        int Total,
        DateTime AchievedAt
    );

    public record SharedParticipant(
        [property: JsonPropertyName("broadcaster_user_login")] string BroadcasterName,
        [property: JsonPropertyName("broadcaster_user_name")] string BroadcasterDisplayName,
        [property: JsonPropertyName("broadcaster_user_id")] long BroadcasterId
    );

    public record Contribution(
        int Total,
        string Type,
        [property: JsonPropertyName("user_id")] long UserId,
        [property: JsonPropertyName("user_login")] string Username,
        [property: JsonPropertyName("user_name")] string UserDisplayName
    );
}
