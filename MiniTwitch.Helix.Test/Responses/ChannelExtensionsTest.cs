using System.Text.Json;
using MiniTwitch.Helix.Internal;
using MiniTwitch.Helix.Responses;

namespace MiniTwitch.Helix.Test.Responses;

public class ChannelExtensionsTest
{
    static readonly JsonSerializerOptions _options = HelixApiClient.SerializerOptions;

    [Fact]
    public void ReadsLiveConfigView()
    {
        string json = """
            {
              "data": [
                {
                  "views": {
                    "live_config": {
                      "viewer_url": "https://example.com/live_config.html",
                      "can_link_external_content": true
                    }
                  }
                }
              ]
            }
            """;

        var response = JsonSerializer.Deserialize<ChannelExtensions>(json, _options);

        Assert.NotNull(response);
        var extension = Assert.Single(response.Data);
        var liveConfig = Assert.IsType<ChannelExtensions.LiveConfig>(extension.Views.LiveConfig);
        Assert.Equal("https://example.com/live_config.html", liveConfig.ViewerUrl);
        Assert.True(liveConfig.CanLinkExternalContent);
        Assert.Null(extension.Views.Panel);
        Assert.Null(extension.Views.Component);
    }

    [Fact]
    public void ReadsConfigAndPanelViews()
    {
        string json = """
            {
              "data": [
                {
                  "views": {
                    "config": {
                      "viewer_url": "https://example.com/config.html",
                      "can_link_external_content": false
                    },
                    "panel": {
                      "viewer_url": "https://example.com/panel.html",
                      "height": 300,
                      "can_link_external_content": true
                    }
                  }
                }
              ]
            }
            """;

        var response = JsonSerializer.Deserialize<ChannelExtensions>(json, _options);

        Assert.NotNull(response);
        var extension = Assert.Single(response.Data);
        var config = Assert.IsType<ChannelExtensions.Config>(extension.Views.Config);
        Assert.Equal("https://example.com/config.html", config.ViewerUrl);
        Assert.False(config.CanLinkExternalContent);
        var panel = Assert.IsType<ChannelExtensions.Panel>(extension.Views.Panel);
        Assert.Equal(300, panel.Height);
        Assert.Null(extension.Views.LiveConfig);
    }
}
