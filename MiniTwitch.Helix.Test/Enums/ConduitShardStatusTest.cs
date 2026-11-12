using MiniTwitch.Helix.Enums;
using MiniTwitch.Helix.Internal.Json;

namespace MiniTwitch.Helix.Test.Enums;

public class ConduitShardStatusTest
{
    [Fact]
    public void NumericValues_KeepTheirOriginalOrder()
    {
        Assert.Equal(0, (int)ConduitShardStatus.Enabled);
        Assert.Equal(10, (int)ConduitShardStatus.WebsocketFailedToReconnect);
    }

    [Fact]
    public void WebsocketConnectionUnused_IsAppended()
    {
        Assert.Equal(11, (int)ConduitShardStatus.WebsocketConnectionUnused);
    }

    [Theory]
    [InlineData("websocket_connection_unused", ConduitShardStatus.WebsocketConnectionUnused)]
    [InlineData("websocket_failed_to_reconnect", ConduitShardStatus.WebsocketFailedToReconnect)]
    public void Read_NewStatusValues(string value, ConduitShardStatus expected)
    {
        Assert.Equal(expected, EnumConverter<ConduitShardStatus>.ReadEnum(value));
    }
}
