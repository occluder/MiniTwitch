using MiniTwitch.Irc.Enums;
using MiniTwitch.Irc.Models;
using Xunit;

namespace MiniTwitch.Irc.Test;

public class OtherTests
{
    [Fact]
    public void GlobalUserstate1()
    {
        string raw = "@badge-info=;badges=gold-pixel-heart/1;color=#596FA0;display-name=occluder;emote-sets=0,300374282,477339272,ff12b988-d6da-4f86-a701-39ffa17c778c;user-id=783267696;user-type= :tmi.twitch.tv GLOBALUSERSTATE";
        var globalUserstate = GlobalUserstate.Construct(raw);
        Assert.Equal(string.Empty, globalUserstate.User.BadgeInfo);
        Assert.Equal("gold-pixel-heart/1", globalUserstate.User.Badges);
        Assert.Equal("596fa0", globalUserstate.User.ChatColor.Name);
        Assert.Equal("occluder", globalUserstate.User.DisplayName);
        Assert.Equal("0,300374282,477339272,ff12b988-d6da-4f86-a701-39ffa17c778c", globalUserstate.EmoteSets);
        Assert.Equal(783267696, globalUserstate.User.Id);
        Assert.Equal(UserType.None, globalUserstate.User.Type);
    }
}
