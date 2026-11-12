using System.Text.Json;
using MiniTwitch.Helix.Internal;
using MiniTwitch.Helix.Responses;

namespace MiniTwitch.Helix.Test.Responses;

public class ChannelGuestStarSettingsTest
{
    static readonly JsonSerializerOptions _options = HelixApiClient.SerializerOptions;

    [Fact]
    public void ReadsGroupLayoutAndBrowserSourceToken()
    {
        string json = """
            {
              "data": [
                {
                  "is_moderator_send_live_enabled": true,
                  "slot_count": 4,
                  "is_browser_source_audio_enabled": true,
                  "group_layout": "TILED",
                  "browser_source_token": "eihq8rew7q3hgierufhi3q"
                }
              ]
            }
            """;

        var response = JsonSerializer.Deserialize<ChannelGuestStarSettings>(json, _options);

        Assert.NotNull(response);
        var settings = Assert.Single(response.Data);
        Assert.True(settings.IsModeratorSendLiveEnabled);
        Assert.Equal(4, settings.SlotCount);
        Assert.True(settings.IsBrowserSourceAudioEnabled);
        Assert.Equal("TILED", settings.GroupLayout);
        Assert.Equal("eihq8rew7q3hgierufhi3q", settings.BrowserSourceToken);
    }

    [Fact]
    public void ReadsNullBrowserSourceToken()
    {
        string json = """
            {
              "data": [
                {
                  "is_moderator_send_live_enabled": false,
                  "slot_count": 2,
                  "is_browser_source_audio_enabled": false,
                  "group_layout": "SCREENSHARE",
                  "browser_source_token": null
                }
              ]
            }
            """;

        var response = JsonSerializer.Deserialize<ChannelGuestStarSettings>(json, _options);

        Assert.NotNull(response);
        var settings = Assert.Single(response.Data);
        Assert.Equal("SCREENSHARE", settings.GroupLayout);
        Assert.Null(settings.BrowserSourceToken);
    }
}
