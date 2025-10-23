using MiniTwitch.Helix.Models;

namespace MiniTwitch.Helix.Responses;

public class Clips : PaginableResponse<Clips.Clip>
{
    public record Clip(
        string Id,
        string Url,
        string EmbedUrl,
        long BroadcasterId,
        string BroadcasterName,
        long CreatorId,
        string CreatorName,
        string VideoId,
        string GameId,
        string Language,
        string Title,
        int ViewCount,
        DateTime CreatedAt,
        string ThumbnailUrl,
        double Duration,
        int? VodOffset,
        bool IsFeatured
    );
}