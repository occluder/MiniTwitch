using MiniTwitch.Helix.Internal.Models;

namespace MiniTwitch.Helix.Test.Internal;

public class EndpointsTest
{
    [Fact]
    public void UpdateChannelGuestStarSettings_UsesPatch()
    {
        Assert.Equal(HttpMethod.Patch, Endpoints.UpdateChannelGuestStarSettings.Method);
    }
}
