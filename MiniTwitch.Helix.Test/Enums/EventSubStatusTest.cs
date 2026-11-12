using MiniTwitch.Helix.Enums;
using MiniTwitch.Helix.Internal.Json;

namespace MiniTwitch.Helix.Test.Enums;

public class EventSubStatusTest
{
    [Fact]
    public void NumericValues_KeepTheirOriginalOrder()
    {
        Assert.Equal(0, (int)EventSubStatus.Enabled);
        Assert.Equal(14, (int)EventSubStatus.WebsocketNetworkError);
    }

    [Fact]
    public void NewStatuses_AreAppended()
    {
        Assert.Equal(15, (int)EventSubStatus.GuestStarModeratorRemoved);
        Assert.Equal(16, (int)EventSubStatus.ChatUserBanned);
        Assert.Equal(17, (int)EventSubStatus.BetaMaintenance);
        Assert.Equal(18, (int)EventSubStatus.WebsocketFailedToReconnect);
        Assert.Equal(19, (int)EventSubStatus.ConduitDeleted);
        Assert.Equal(20, (int)EventSubStatus.Unknown);
    }

    [Theory]
    [InlineData("guest_star_moderator_removed", EventSubStatus.GuestStarModeratorRemoved)]
    [InlineData("chat_user_banned", EventSubStatus.ChatUserBanned)]
    [InlineData("beta_maintenance", EventSubStatus.BetaMaintenance)]
    [InlineData("websocket_failed_to_reconnect", EventSubStatus.WebsocketFailedToReconnect)]
    [InlineData("conduit_deleted", EventSubStatus.ConduitDeleted)]
    public void Read_NewStatusValues(string value, EventSubStatus expected)
    {
        Assert.Equal(expected, EnumConverter<EventSubStatus>.ReadEnum(value));
    }

    [Fact]
    public void Read_UnknownStatus_FallsBackToUnknown()
    {
        Assert.Equal(EventSubStatus.Unknown, EnumConverter<EventSubStatus>.ReadEnum("not_a_status"));
        Assert.Equal(EventSubStatus.Unknown, EnumConverter<EventSubStatus>.ReadEnum("9999"));
    }
}
