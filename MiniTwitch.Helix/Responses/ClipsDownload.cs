using MiniTwitch.Helix.Models;

namespace MiniTwitch.Helix.Responses;

public class ClipsDownload : BaseResponse<ClipsDownload.ClipDownload>
{
    public record ClipDownload(
        string ClipId,
        string? LandscapeDownloadUrl,
        string? PortraitDownloadUrl
    );
}
