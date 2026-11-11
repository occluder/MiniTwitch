using MiniTwitch.Helix.Models;

namespace MiniTwitch.Helix.Responses;

public class ChannelGuestStarSettings : BaseResponse<ChannelGuestStarSettings.Settings>
{
    public record Settings(
        bool IsModeratorSendLiveEnabled,
        int SlotCount,
        bool IsBrowserSourceAudioEnabled,
        string GroupLayout,
        string? BrowserSourceToken
    );
}
