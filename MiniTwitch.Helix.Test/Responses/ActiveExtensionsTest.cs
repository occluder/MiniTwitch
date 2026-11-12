using System.Text.Json;
using MiniTwitch.Helix.Internal;
using MiniTwitch.Helix.Responses;

namespace MiniTwitch.Helix.Test.Responses;

public class ActiveExtensionsTest
{
    static readonly JsonSerializerOptions _options = HelixApiClient.SerializerOptions;

    [Fact]
    public void AllowsSlotsWithOnlyTheActiveField()
    {
        string json = """
            {
              "data": {
                "panel": {
                  "1": { "active": false }
                },
                "overlay": {
                  "1": { "active": false }
                },
                "component": {
                  "1": { "active": false },
                  "2": {
                    "active": true,
                    "id": "component-id",
                    "version": "1.0.0",
                    "name": "Component",
                    "x": 0,
                    "y": 5
                  }
                }
              }
            }
            """;

        var response = JsonSerializer.Deserialize<ActiveExtensions>(json, _options);

        Assert.NotNull(response);
        var panel = response.Data.Panel[1];
        Assert.False(panel.Active);
        Assert.Null(panel.Id);
        Assert.Null(panel.Version);
        Assert.Null(panel.Name);

        Assert.False(response.Data.Overlay[1].Active);

        var component = response.Data.Component[2];
        Assert.True(component.Active);
        Assert.Equal("component-id", component.Id);
        Assert.Equal("1.0.0", component.Version);
        Assert.Equal("Component", component.Name);
        Assert.Equal(0, component.X);
        Assert.Equal(5, component.Y);
    }
}
