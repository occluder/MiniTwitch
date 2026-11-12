using MiniTwitch.Helix.Models;

namespace MiniTwitch.Helix.Responses;

public class ExtensionConfigurationSegment : BaseResponse<ExtensionConfigurationSegment.ConfigSegment>
{
    public record ConfigSegment(
        string Segment,
        string BroadcasterId,
        string Content,
        string Version
    );
}