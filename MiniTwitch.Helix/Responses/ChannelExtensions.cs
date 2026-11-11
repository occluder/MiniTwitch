using System.Text.Json.Serialization;
using MiniTwitch.Helix.Models;

namespace MiniTwitch.Helix.Responses;

public class ChannelExtensions : BaseResponse<ChannelExtensions.Extension>
{
    public record Component(
         string ViewerUrl,
         int AspectWidth,
         int AspectHeight,
         int AspectRatioX,
         int AspectRatioY,
         bool Autoscale,
         int ScalePixels,
         int TargetHeight,
         int Size,
         bool Zoom,
         int ZoomPixels,
         bool CanLinkExternalContent
    );

    public record Extension(
         string AuthorName,
         bool BitsEnabled,
         bool CanInstall,
         string ConfigurationLocation,
         string Description,
         string EulaTosUrl,
         bool HasChatSupport,
         string IconUrl,
         IconUrls IconUrls,
         string Id,
         string Name,
         string PrivacyPolicyUrl,
         bool RequestIdentityLink,
         IReadOnlyList<string> ScreenshotUrls,
         string State,
         string SubscriptionsSupportLevel,
         string Summary,
         string SupportEmail,
         string Version,
         string ViewerSummary,
        Views Views,
        IReadOnlyList<string> AllowlistedConfigUrls,
        IReadOnlyList<string> AllowlistedPanelUrls
    );

    public record IconUrls(
        [property: JsonPropertyName("100x100")] string Url100x100,
        [property: JsonPropertyName("24x24")] string Url24x24,
        [property: JsonPropertyName("300x200")] string Url300x200
    );

    public record Mobile(
        string ViewerUrl
    );

    public record Panel(
        string ViewerUrl,
        int Height,
        bool CanLinkExternalContent
    );

    public record VideoOverlay(
        string ViewerUrl,
        bool CanLinkExternalContent
    );

    public record LiveConfig(
        string ViewerUrl,
        bool CanLinkExternalContent
    );

    public record Views(
        Mobile Mobile,
        Panel Panel,
        VideoOverlay VideoOverlay,
        Component Component,
        LiveConfig? LiveConfig
    );
}