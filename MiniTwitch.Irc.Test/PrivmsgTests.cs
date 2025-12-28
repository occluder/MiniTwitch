using MiniTwitch.Irc.Enums;
using MiniTwitch.Irc.Models;
using Xunit;

namespace MiniTwitch.Irc.Test;

public class PrivmsgTests
{
    [Fact]
    public void Construct()
    {
        var privmsg = Privmsg.Construct("@flags=;user-id=49435922;id=235ec71c-f4ed-485b-abd5-e0849d0d6972;emotes=;user-type=;returning-chatter=0;subscriber=1;display-name=synaxl;mod=0;tmi-sent-ts=1687855313886;turbo=0;room-id=11148817;color=#FF782A;badge-info=subscriber/30;first-msg=0;badges=subscriber/24,twitchconEU2019/1 :synaxl!synaxl@synaxl.tmi.twitch.tv PRIVMSG #pajlada :immer gern Evolution57 Okayge");

        // Author
        Assert.Equal(49435922, privmsg.Author.Id);
        Assert.Equal(UserType.None, privmsg.Author.Type);
        Assert.True(privmsg.Author.IsSubscriber);
        Assert.Equal("synaxl", privmsg.Author.DisplayName);
        Assert.Equal("synaxl", privmsg.Author.Name);
        Assert.False(privmsg.Author.IsMod);
        Assert.False(privmsg.Author.IsTurbo);
        Assert.Equal("FF782A".ToLower(), privmsg.Author.ChatColor.Name);
        Assert.Equal("subscriber/30", privmsg.Author.BadgeInfo);
        Assert.Equal("subscriber/24,twitchconEU2019/1", privmsg.Author.Badges);
        Assert.False(privmsg.Author.IsVip);

        // No reply
        Assert.False(privmsg.Reply.HasContent);
        Assert.Equal(string.Empty, privmsg.Reply.ParentMessage);
        Assert.Equal(string.Empty, privmsg.Reply.ParentDisplayName);
        Assert.Equal(string.Empty, privmsg.Reply.ParentUsername);
        Assert.Equal(string.Empty, privmsg.Reply.ParentMessageId);
        Assert.Equal(0, privmsg.Reply.ParentUserId);
        Assert.Equal(string.Empty, privmsg.Reply.ParentThreadMessageId);
        Assert.Equal(string.Empty, privmsg.Reply.ParentThreadUsername);

        // Channel
        Assert.Equal("pajlada", privmsg.Channel.Name);
        Assert.Equal(11148817, privmsg.Channel.Id);

        // No source
        Assert.Null(privmsg.SourceClient);
        Assert.True(privmsg.ReplyWith(default!, default!).IsCompleted);

        Assert.Equal(0, privmsg.Bits);
        Assert.Equal("immer gern Evolution57 Okayge", privmsg.Content);
        Assert.Equal(string.Empty, privmsg.Emotes);
        Assert.Equal(string.Empty, privmsg.Flags);
        Assert.Equal("235ec71c-f4ed-485b-abd5-e0849d0d6972", privmsg.Id);
        Assert.False(privmsg.IsAction);
        Assert.False(privmsg.IsFirstMessage);
        Assert.False(privmsg.IsReturningChatter);
        Assert.Equal(string.Empty, privmsg.Nonce);
        Assert.Equal(1687855313886, privmsg.TmiSentTs);
    }

    [Fact]
    public void WithReply()
    {
        var privmsg = Privmsg.Construct("@display-name=pajbot;reply-parent-msg-id=c1b51680-dcab-4f31-b928-84c97cdc7654;flags=;user-type=mod;reply-thread-parent-msg-id=c1b51680-dcab-4f31-b928-84c97cdc7654;reply-thread-parent-user-login=occluder;tmi-sent-ts=1687867130401;user-id=82008718;returning-chatter=0;badge-info=subscriber/90;reply-parent-user-login=occluder;first-msg=0;reply-parent-user-id=783267696;id=922a75b1-6896-4f8f-a28a-40a8f341f655;subscriber=1;emotes=;turbo=0;color=#2E8B57;reply-parent-display-name=occluder;badges=moderator/1,subscriber/3072;reply-parent-msg-body=!;room-id=11148817;mod=1 :pajbot!pajbot@pajbot.tmi.twitch.tv PRIVMSG #pajlada :@occluder 󠀀!!!");

        // Author
        Assert.Equal(82008718, privmsg.Author.Id);
        Assert.Equal(UserType.Mod, privmsg.Author.Type);
        Assert.True(privmsg.Author.IsSubscriber);
        Assert.Equal("pajbot", privmsg.Author.DisplayName);
        Assert.Equal("pajbot", privmsg.Author.Name);
        Assert.True(privmsg.Author.IsMod);
        Assert.False(privmsg.Author.IsTurbo);
        Assert.Equal("2E8B57".ToLower(), privmsg.Author.ChatColor.Name);
        Assert.Equal("subscriber/90", privmsg.Author.BadgeInfo);
        Assert.Equal("moderator/1,subscriber/3072", privmsg.Author.Badges);
        Assert.False(privmsg.Author.IsVip);

        // Has a reply
        Assert.True(privmsg.Reply.HasContent);
        Assert.Equal("!", privmsg.Reply.ParentMessage);
        Assert.Equal("occluder", privmsg.Reply.ParentDisplayName);
        Assert.Equal("occluder", privmsg.Reply.ParentUsername);
        Assert.Equal("c1b51680-dcab-4f31-b928-84c97cdc7654", privmsg.Reply.ParentMessageId);
        Assert.Equal(783267696, privmsg.Reply.ParentUserId);
        // Not a nested reply
        Assert.Equal("c1b51680-dcab-4f31-b928-84c97cdc7654", privmsg.Reply.ParentThreadMessageId);
        Assert.Equal("occluder", privmsg.Reply.ParentThreadUsername);

        // Channel
        Assert.Equal("pajlada", privmsg.Channel.Name);
        Assert.Equal(11148817, privmsg.Channel.Id);

        // No source
        Assert.Null(privmsg.SourceClient);
        Assert.True(privmsg.ReplyWith(default!, default!).IsCompleted);

        Assert.Equal(0, privmsg.Bits);
        Assert.Equal("@occluder \U000e0000!!!", privmsg.Content);
        Assert.Equal(string.Empty, privmsg.Emotes);
        Assert.Equal(string.Empty, privmsg.Flags);
        Assert.Equal("922a75b1-6896-4f8f-a28a-40a8f341f655", privmsg.Id);
        Assert.False(privmsg.IsAction);
        Assert.False(privmsg.IsFirstMessage);
        Assert.False(privmsg.IsReturningChatter);
        Assert.Equal(string.Empty, privmsg.Nonce);
        Assert.Equal(1687867130401, privmsg.TmiSentTs);
    }

    [Fact]
    public void WithNestedReply()
    {
        var privmsg = Privmsg.Construct("@first-msg=0;reply-parent-user-login=treuks;badges=subscriber/12;display-name=occluder;color=#596FA0;emotes=;flags=;reply-thread-parent-user-login=occluder;returning-chatter=0;tmi-sent-ts=1687870580760;user-id=783267696;user-type=;reply-parent-msg-id=4fe6169e-0dab-4c23-b721-d00327e5eba0;reply-thread-parent-msg-id=32916ff4-780e-451e-b6cc-97d0e8b3e8dd;turbo=0;mod=0;reply-parent-display-name=TreuKS;reply-parent-msg-body=@occluder\\sOMEGALUL\\sgood\\sone;reply-parent-user-id=212793593;room-id=11148817;subscriber=1;id=3229c724-cc5e-43ab-82fe-f95da61c3df0;badge-info=subscriber/13 :occluder!occluder@occluder.tmi.twitch.tv PRIVMSG #pajlada :@TreuKS xd");

        // Author
        Assert.Equal(783267696, privmsg.Author.Id);
        Assert.Equal(UserType.None, privmsg.Author.Type);
        Assert.True(privmsg.Author.IsSubscriber);
        Assert.Equal("occluder", privmsg.Author.DisplayName);
        Assert.Equal("occluder", privmsg.Author.Name);
        Assert.False(privmsg.Author.IsMod);
        Assert.False(privmsg.Author.IsTurbo);
        Assert.Equal("596FA0".ToLower(), privmsg.Author.ChatColor.Name);
        Assert.Equal("subscriber/13", privmsg.Author.BadgeInfo);
        Assert.Equal("subscriber/12", privmsg.Author.Badges);
        Assert.False(privmsg.Author.IsVip);

        // Has a reply
        Assert.True(privmsg.Reply.HasContent);
        Assert.Equal("@occluder OMEGALUL good one", privmsg.Reply.ParentMessage);
        Assert.Equal("TreuKS", privmsg.Reply.ParentDisplayName);
        Assert.Equal("treuks", privmsg.Reply.ParentUsername);
        Assert.Equal("4fe6169e-0dab-4c23-b721-d00327e5eba0", privmsg.Reply.ParentMessageId);
        Assert.Equal(212793593, privmsg.Reply.ParentUserId);
        // Nested
        Assert.Equal("32916ff4-780e-451e-b6cc-97d0e8b3e8dd", privmsg.Reply.ParentThreadMessageId);
        Assert.Equal("occluder", privmsg.Reply.ParentThreadUsername);

        // Channel
        Assert.Equal("pajlada", privmsg.Channel.Name);
        Assert.Equal(11148817, privmsg.Channel.Id);

        // No source
        Assert.Null(privmsg.SourceClient);
        Assert.True(privmsg.ReplyWith(default!, default!).IsCompleted);

        Assert.Equal(0, privmsg.Bits);
        Assert.Equal("@TreuKS xd", privmsg.Content);
        Assert.Equal(string.Empty, privmsg.Emotes);
        Assert.Equal(string.Empty, privmsg.Flags);
        Assert.Equal("3229c724-cc5e-43ab-82fe-f95da61c3df0", privmsg.Id);
        Assert.False(privmsg.IsAction);
        Assert.False(privmsg.IsFirstMessage);
        Assert.False(privmsg.IsReturningChatter);
        Assert.Equal(string.Empty, privmsg.Nonce);
        Assert.Equal(1687870580760, privmsg.TmiSentTs);
    }

    [Fact]
    public void WithBits()
    {
        var privmsg = Privmsg.Construct("@badge-info=subscriber/10;color=#FFE100;badges=subscriber/6;flags=;turbo=0;display-name=demonbajs;tmi-sent-ts=1687597804051;first-msg=0;user-id=112944037;mod=0;emotes=;subscriber=1;id=3ba61bd7-7d13-46a9-8067-b141e6fa59ce;bits=10;user-type=;returning-chatter=0;room-id=11148817 :demonbajs!demonbajs@demonbajs.tmi.twitch.tv PRIVMSG #pajlada :Cheer1 Cheer1 Cheer1 Cheer1 Cheer1 Cheer1 Cheer1 Cheer1 Cheer1 Cheer1");

        // Author
        Assert.Equal(112944037, privmsg.Author.Id);
        Assert.Equal(UserType.None, privmsg.Author.Type);
        Assert.True(privmsg.Author.IsSubscriber);
        Assert.Equal("demonbajs", privmsg.Author.DisplayName);
        Assert.Equal("demonbajs", privmsg.Author.Name);
        Assert.False(privmsg.Author.IsMod);
        Assert.False(privmsg.Author.IsTurbo);
        Assert.Equal("FFE100".ToLower(), privmsg.Author.ChatColor.Name);
        Assert.Equal("subscriber/10", privmsg.Author.BadgeInfo);
        Assert.Equal("subscriber/6", privmsg.Author.Badges);
        Assert.False(privmsg.Author.IsVip);

        // No reply
        Assert.False(privmsg.Reply.HasContent);
        Assert.Equal(string.Empty, privmsg.Reply.ParentMessage);
        Assert.Equal(string.Empty, privmsg.Reply.ParentDisplayName);
        Assert.Equal(string.Empty, privmsg.Reply.ParentUsername);
        Assert.Equal(string.Empty, privmsg.Reply.ParentMessageId);
        Assert.Equal(0, privmsg.Reply.ParentUserId);
        Assert.Equal(string.Empty, privmsg.Reply.ParentThreadMessageId);
        Assert.Equal(string.Empty, privmsg.Reply.ParentThreadUsername);

        // Channel
        Assert.Equal("pajlada", privmsg.Channel.Name);
        Assert.Equal(11148817, privmsg.Channel.Id);

        // No source
        Assert.Null(privmsg.SourceClient);
        Assert.True(privmsg.ReplyWith(default!, default!).IsCompleted);

        Assert.Equal(10, privmsg.Bits);
        Assert.Equal("Cheer1 Cheer1 Cheer1 Cheer1 Cheer1 Cheer1 Cheer1 Cheer1 Cheer1 Cheer1", privmsg.Content);
        Assert.Equal(string.Empty, privmsg.Emotes);
        Assert.Equal(string.Empty, privmsg.Flags);
        Assert.Equal("3ba61bd7-7d13-46a9-8067-b141e6fa59ce", privmsg.Id);
        Assert.False(privmsg.IsAction);
        Assert.False(privmsg.IsFirstMessage);
        Assert.False(privmsg.IsReturningChatter);
        Assert.Equal(string.Empty, privmsg.Nonce);
        Assert.Equal(1687597804051, privmsg.TmiSentTs);
    }

    [Fact]
    public void WithCustomRewardId()
    {
        var privmsg = Privmsg.Construct("@flags=;user-id=49435922;id=235ec71c-f4ed-485b-abd5-e0849d0d6972;emotes=;user-type=;returning-chatter=0;subscriber=1;custom-reward-id=1a0b4553-e01e-4d1f-a250-c9ecadcdc129;display-name=synaxl;mod=0;tmi-sent-ts=1687855313886;turbo=0;room-id=11148817;color=#FF782A;badge-info=subscriber/30;first-msg=0;badges=subscriber/24,twitchconEU2019/1 :synaxl!synaxl@synaxl.tmi.twitch.tv PRIVMSG #pajlada :immer gern Evolution57 Okayge");

        // Author
        Assert.Equal(49435922, privmsg.Author.Id);
        Assert.Equal(UserType.None, privmsg.Author.Type);
        Assert.True(privmsg.Author.IsSubscriber);
        Assert.Equal("synaxl", privmsg.Author.DisplayName);
        Assert.Equal("synaxl", privmsg.Author.Name);
        Assert.False(privmsg.Author.IsMod);
        Assert.False(privmsg.Author.IsTurbo);
        Assert.Equal("FF782A".ToLower(), privmsg.Author.ChatColor.Name);
        Assert.Equal("subscriber/30", privmsg.Author.BadgeInfo);
        Assert.Equal("subscriber/24,twitchconEU2019/1", privmsg.Author.Badges);
        Assert.False(privmsg.Author.IsVip);

        // No reply
        Assert.False(privmsg.Reply.HasContent);
        Assert.Equal(string.Empty, privmsg.Reply.ParentMessage);
        Assert.Equal(string.Empty, privmsg.Reply.ParentDisplayName);
        Assert.Equal(string.Empty, privmsg.Reply.ParentUsername);
        Assert.Equal(string.Empty, privmsg.Reply.ParentMessageId);
        Assert.Equal(0, privmsg.Reply.ParentUserId);
        Assert.Equal(string.Empty, privmsg.Reply.ParentThreadMessageId);
        Assert.Equal(string.Empty, privmsg.Reply.ParentThreadUsername);

        // Channel
        Assert.Equal("pajlada", privmsg.Channel.Name);
        Assert.Equal(11148817, privmsg.Channel.Id);

        // No source
        Assert.Null(privmsg.SourceClient);
        Assert.True(privmsg.ReplyWith(default!, default!).IsCompleted);

        Assert.Equal(0, privmsg.Bits);
        Assert.Equal("immer gern Evolution57 Okayge", privmsg.Content);
        Assert.Equal(string.Empty, privmsg.Emotes);
        Assert.Equal(string.Empty, privmsg.Flags);
        Assert.Equal("235ec71c-f4ed-485b-abd5-e0849d0d6972", privmsg.Id);
        Assert.False(privmsg.IsAction);
        Assert.False(privmsg.IsFirstMessage);
        Assert.False(privmsg.IsReturningChatter);
        Assert.Equal(string.Empty, privmsg.Nonce);
        Assert.Equal(1687855313886, privmsg.TmiSentTs);
        Assert.Equal("1a0b4553-e01e-4d1f-a250-c9ecadcdc129", privmsg.CustomRewardId);
    }

    [Fact]
    public void AnimatedMessage()
    {
        var privmsg = Privmsg.Construct("@tmi-sent-ts=1718428504630;id=763b536a-1591-4503-abd2-f57245e2509f;room-id=641972806;user-id=268704573;display-name=xOJGxRIZZO;badges=bits/100;badge-info=;color=#FF4500;flags=;user-type=;emotes=;msg-id=animated-message;animation-id=rainbow-eclipse :xojgxrizzo!xojgxrizzo@xojgxrizzo.tmi.twitch.tv PRIVMSG #kaicenat :JAYSHAWN NALGAS IS GOATED");
        // Author
        Assert.Equal(268704573, privmsg.Author.Id);
        Assert.Equal(UserType.None, privmsg.Author.Type);
        Assert.False(privmsg.Author.IsSubscriber);
        Assert.Equal("xOJGxRIZZO", privmsg.Author.DisplayName);
        Assert.Equal("xojgxrizzo", privmsg.Author.Name);
        Assert.False(privmsg.Author.IsMod);
        Assert.False(privmsg.Author.IsTurbo);
        Assert.Equal("FF4500".ToLower(), privmsg.Author.ChatColor.Name);
        Assert.Empty(privmsg.Author.BadgeInfo);
        Assert.Equal("bits/100", privmsg.Author.Badges);
        Assert.False(privmsg.Author.IsVip);

        // No reply
        Assert.False(privmsg.Reply.HasContent);
        Assert.Equal(string.Empty, privmsg.Reply.ParentMessage);
        Assert.Equal(string.Empty, privmsg.Reply.ParentDisplayName);
        Assert.Equal(string.Empty, privmsg.Reply.ParentUsername);
        Assert.Equal(string.Empty, privmsg.Reply.ParentMessageId);
        Assert.Equal(0, privmsg.Reply.ParentUserId);
        Assert.Equal(string.Empty, privmsg.Reply.ParentThreadMessageId);
        Assert.Equal(string.Empty, privmsg.Reply.ParentThreadUsername);

        // Channel
        Assert.Equal("kaicenat", privmsg.Channel.Name);
        Assert.Equal(641972806, privmsg.Channel.Id);

        Assert.Equal(0, privmsg.Bits);
        Assert.Equal("JAYSHAWN NALGAS IS GOATED", privmsg.Content);
        Assert.Empty(privmsg.Emotes);
        Assert.Empty(privmsg.Flags);
        Assert.Equal("763b536a-1591-4503-abd2-f57245e2509f", privmsg.Id);
        Assert.False(privmsg.IsAction);
        Assert.False(privmsg.IsFirstMessage);
        Assert.False(privmsg.IsReturningChatter);
        Assert.Empty(privmsg.Nonce);
        Assert.Equal(1718428504630, privmsg.TmiSentTs);
        Assert.Empty(privmsg.CustomRewardId);

        // Animated
        Assert.True(privmsg.Animation.IsAnimated);
        Assert.Equal("rainbow-eclipse", privmsg.Animation.AnimationId);

        // Not gigantified
        Assert.False(privmsg.IsGigantifiedEmoteMessage);
    }

    [Fact]
    public void WithGiganticEmote()
    {
        var privmsg = Privmsg.Construct("@tmi-sent-ts=1718432119583;turbo=1;first-msg=1;emote-only=1;id=69249344-c303-415f-b8da-cb36381b1bb3;room-id=641972806;user-id=241634307;display-name=Timelus_;badges=turbo/1;badge-info=;color=#8A2BE2;flags=;user-type=;emotes=41:0-7;msg-id=gigantified-emote-message :timelus_!timelus_@timelus_.tmi.twitch.tv PRIVMSG #kaicenat :Kreygasm");

        // Author
        Assert.Equal(241634307, privmsg.Author.Id);
        Assert.Equal(UserType.None, privmsg.Author.Type);
        Assert.False(privmsg.Author.IsSubscriber);
        Assert.Equal("Timelus_", privmsg.Author.DisplayName);
        Assert.Equal("timelus_", privmsg.Author.Name);
        Assert.False(privmsg.Author.IsMod);
        Assert.True(privmsg.Author.IsTurbo);
        Assert.Equal("8A2BE2".ToLower(), privmsg.Author.ChatColor.Name);
        Assert.Empty(privmsg.Author.BadgeInfo);
        Assert.Equal("turbo/1", privmsg.Author.Badges);
        Assert.False(privmsg.Author.IsVip);

        // No reply
        Assert.False(privmsg.Reply.HasContent);
        Assert.Equal(string.Empty, privmsg.Reply.ParentMessage);
        Assert.Equal(string.Empty, privmsg.Reply.ParentDisplayName);
        Assert.Equal(string.Empty, privmsg.Reply.ParentUsername);
        Assert.Equal(string.Empty, privmsg.Reply.ParentMessageId);
        Assert.Equal(0, privmsg.Reply.ParentUserId);
        Assert.Equal(string.Empty, privmsg.Reply.ParentThreadMessageId);
        Assert.Equal(string.Empty, privmsg.Reply.ParentThreadUsername);

        // Channel
        Assert.Equal("kaicenat", privmsg.Channel.Name);
        Assert.Equal(641972806, privmsg.Channel.Id);

        Assert.Equal(0, privmsg.Bits);
        Assert.Equal("Kreygasm", privmsg.Content);
        Assert.Equal("41:0-7", privmsg.Emotes);
        Assert.Empty(privmsg.Flags);
        Assert.Equal("69249344-c303-415f-b8da-cb36381b1bb3", privmsg.Id);
        Assert.False(privmsg.IsAction);
        Assert.True(privmsg.IsFirstMessage);
        Assert.False(privmsg.IsReturningChatter);
        Assert.Empty(privmsg.Nonce);
        Assert.Equal(1718432119583, privmsg.TmiSentTs);
        Assert.Empty(privmsg.CustomRewardId);

        // Not Animated
        Assert.False(privmsg.Animation.IsAnimated);
        Assert.Equal(string.Empty, privmsg.Animation.AnimationId);

        // Gigantified
        Assert.True(privmsg.IsGigantifiedEmoteMessage);
    }

    [Fact]
    public void LeadMod()
    {
        var privmsg = Privmsg.Construct("@badge-info=;badges=lead_moderator/1;client-nonce=eed215dbfe1f62033bd287c5f832ff64;color=#1E90FF;display-name=AlcaBot;emotes=;first-msg=0;flags=;id=e85c9083-b998-44fc-a392-a9a8ea64d3aa;mod=1;returning-chatter=0;room-id=7676884;subscriber=0;tmi-sent-ts=1766900598952;turbo=0;user-id=70310077;user-type=mod :alcabot!alcabot@alcabot.tmi.twitch.tv PRIVMSG #alca :Test");
        // Author
        Assert.Equal(70310077, privmsg.Author.Id);
        Assert.Equal(UserType.Mod, privmsg.Author.Type);
        Assert.False(privmsg.Author.IsSubscriber);
        Assert.Equal("AlcaBot", privmsg.Author.DisplayName);
        Assert.Equal("alcabot", privmsg.Author.Name);
        Assert.True(privmsg.Author.IsMod);
        Assert.False(privmsg.Author.IsTurbo);
        Assert.Equal("1E90FF".ToLower(), privmsg.Author.ChatColor.Name);
        Assert.Empty(privmsg.Author.BadgeInfo);
        Assert.Equal("lead_moderator/1", privmsg.Author.Badges);
        Assert.False(privmsg.Author.IsVip);
        Assert.True(privmsg.Author.IsLeadMod);
    }
}
