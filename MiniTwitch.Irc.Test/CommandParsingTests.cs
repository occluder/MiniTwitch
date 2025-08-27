using System.Text;
using MiniTwitch.Irc.Internal.Enums;
using MiniTwitch.Irc.Internal.Models;
using Xunit;

namespace MiniTwitch.Irc.Test;
public class CommandParsingTests
{
    [Fact]
    public void Parse_PRIVMSG()
    {
        string raw = "@badge-info=subscriber/42;badges=subscriber/42,glhf-pledge/1;color=#F97304;display-name=zneix;emotes=;first-msg=0;flags=;id=ffc033d5-862c-413c-8422-72a59aa1b0f1;mod=0;returning-chatter=0;room-id=11148817;subscriber=1;tmi-sent-ts=1678752306574;turbo=0;user-id=99631238;user-type= :zneix!zneix@zneix.tmi.twitch.tv PRIVMSG #pajlada :btw DFantasy, have you gotten an unused Dragon Hunter Crossbow just laying around by any chance? kkOna";
        Assert.Equal(IrcCommand.PRIVMSG, new IrcMessage(Encoding.UTF8.GetBytes(raw)).Command);
    }
    [Fact]
    public void Parse_USERNOTICE()
    {
        string raw = "@badge-info=subscriber/15;badges=subscriber/12;color=#BAFF2F;display-name=VOJTERIS;emotes=;flags=;id=267bffe6-534c-4ddc-a11c-d32b242d8734;login=vojteris;mod=0;msg-id=resub;msg-param-cumulative-months=15;msg-param-months=0;msg-param-multimonth-duration=0;msg-param-multimonth-tenure=0;msg-param-should-share-streak=0;msg-param-sub-plan-name=look\\sat\\sthose\\sshitty\\semotes,\\srip\\s$5\\sLUL;msg-param-sub-plan=1000;msg-param-was-gifted=false;room-id=11148817;subscriber=1;system-msg=VOJTERIS\\ssubscribed\\sat\\sTier\\s1.\\sThey've\\ssubscribed\\sfor\\s15\\smonths!;tmi-sent-ts=1678743959736;user-id=75436142;user-type= :tmi.twitch.tv USERNOTICE #pajlada :egFors TeaTime";
        Assert.Equal(IrcCommand.USERNOTICE, new IrcMessage(Encoding.UTF8.GetBytes(raw)).Command);
    }
    [Fact]
    public void Parse_CLEARCHAT()
    {
        string raw = "@ban-duration=30;room-id=11148817;target-user-id=783267696;tmi-sent-ts=1678712873805 :tmi.twitch.tv CLEARCHAT #pajlada :occluder";
        Assert.Equal(IrcCommand.CLEARCHAT, new IrcMessage(Encoding.UTF8.GetBytes(raw)).Command);
    }
    [Fact]
    public void Parse_WHISPER()
    {
        string raw = "@badges=;color=#2E8B57;display-name=pajbot;emotes=25:7-11;message-id=7;thread-id=82008718_783267696;turbo=0;user-id=82008718;user-type= :pajbot!pajbot@pajbot.tmi.twitch.tv WHISPER occluder :Riftey Kappa";
        Assert.Equal(IrcCommand.WHISPER, new IrcMessage(Encoding.UTF8.GetBytes(raw)).Command);
    }
    [Fact]
    public void Parse_ROOMSTATE()
    {
        string raw = "@emote-only=0;followers-only=-1;r9k=0;room-id=783267696;slow=0;subs-only=0 :tmi.twitch.tv ROOMSTATE #occluder";
        Assert.Equal(IrcCommand.ROOMSTATE, new IrcMessage(Encoding.UTF8.GetBytes(raw)).Command);
    }
    [Fact]
    public void Parse_NOTICE()
    {
        string raw = "@msg-id=msg_channel_suspended :tmi.twitch.tv NOTICE #foretack :This channel does not exist or has been suspended.";
        Assert.Equal(IrcCommand.NOTICE, new IrcMessage(Encoding.UTF8.GetBytes(raw)).Command);
    }
    [Fact]
    public void Parse_CLEARMSG()
    {
        string raw = "@login=occluder;room-id=;target-msg-id=55dc74c9-a6b2-4443-9b68-3446a5ddb7ed;tmi-sent-ts=1678798254260 :tmi.twitch.tv CLEARMSG #occluder :@emote-only=0;followers-only=-1;r9k=0;room-id=783267696;slow=0;subs-only=0 :tmi.twitch.tv ROOMSTATE #occluder ";
        Assert.Equal(IrcCommand.CLEARMSG, new IrcMessage(Encoding.UTF8.GetBytes(raw)).Command);
    }
    [Fact]
    public void Parse_USERSTATE()
    {
        string raw = "@badge-info=;badges=broadcaster/1;color=#F2647B;display-name=occluder;emote-sets=0,4236,15961,19194,376284588,385292559,477339272,5fc5e142-d394-481f-a561-5406c2ba3ef0,e21484e8-cf11-48b1-8b67-c180fa39f926;mod=0;subscriber=0;user-type= :tmi.twitch.tv USERSTATE #occluder";
        Assert.Equal(IrcCommand.USERSTATE, new IrcMessage(Encoding.UTF8.GetBytes(raw)).Command);
    }
    [Fact]
    public void Parse_RECONNECT()
    {
        string raw = ":tmi.twitch.tv RECONNECT";
        Assert.Equal(IrcCommand.RECONNECT, new IrcMessage(Encoding.UTF8.GetBytes(raw)).Command);
    }
    [Fact]
    public void Parse_PING()
    {
        string raw = "PING :tmi.twitch.tv";
        Assert.Equal(IrcCommand.PING, new IrcMessage(Encoding.UTF8.GetBytes(raw)).Command);
    }
    [Fact]
    public void Parse_PONG()
    {
        string raw = "PONG :tmi.twitch.tv";
        Assert.Equal(IrcCommand.PONG, new IrcMessage(Encoding.UTF8.GetBytes(raw)).Command);
    }
    [Fact]
    public void Parse_GLOBALUSERSTATE()
    {
        string raw = "@badge-info=;badges=gold-pixel-heart/1;color=#596FA0;display-name=occluder;emote-sets=0,300374282,477339272,ff12b988-d6da-4f86-a701-39ffa17c778c;user-id=783267696;user-type= :tmi.twitch.tv GLOBALUSERSTATE";
        Assert.Equal(IrcCommand.GLOBALUSERSTATE, new IrcMessage(Encoding.UTF8.GetBytes(raw)).Command);
    }
    [Fact]
    public void Parse_Multiple()
    {
        string raw = "PING :tmi.twitch.tv\r\n" +
            "@badge-info=;badges=broadcaster/1;color=#F2647B;display-name=occluder;emote-sets=0,4236,15961,19194,376284588,385292559,477339272,5fc5e142-d394-481f-a561-5406c2ba3ef0,e21484e8-cf11-48b1-8b67-c180fa39f926;mod=0;subscriber=0;user-type= :tmi.twitch.tv USERSTATE #occluder\r\n"
            + "@emote-only=0;followers-only=-1;r9k=0;room-id=783267696;slow=0;subs-only=0 :tmi.twitch.tv ROOMSTATE #occluder";
        ReadOnlyMemory<byte> memory = Encoding.UTF8.GetBytes(raw);
        IrcClient client = new(_ => { _.Anonymous = true; });
        bool ping = false;
        bool us = false;
        bool rs = false;

        client.OnPing += () =>
        {
            ping = true;
            return default;
        };
        client.OnUserstate += userstate =>
        {
            us = true;
            return default;
        };
        client.OnChannelJoin += channelJoin =>
        {
            rs = true;
            return default;
        };

        client.Parse(memory);

        Assert.True(ping);
        Assert.True(us);
        Assert.True(rs);
    }
    [Fact]
    public void Parse_Multiple2()
    {
        string raw = "@badge-info=;badges=premium/1;client-nonce=9644362412e9c403acd6c6f91d495efb;color=#4A87C9;display-name=tapCrush;emotes=;first-msg=0;flags=;id=5593b09e-e6d9-4c85-93cd-f0c122bac221;mod=0;returning-chatter=0;room-id=207813352;subscriber=0;tmi-sent-ts=1681072545387;turbo=0;user-id=58535630;user-type= :tapcrush!tapcrush@tapcrush.tmi.twitch.tv PRIVMSG #hasanabi :Thank you @HasanAbi this is better than watching mizkif watch emily get taped to wall LULW\r\n" +
            "@badge-info=subscriber/11;badges=subscriber/6;color=#F2647B;display-name=occluder;emotes=;first-msg=0;flags=;id=1a5a8c87-23b4-4061-bb39-b7b13b924153;mod=0;returning-chatter=0;room-id=22484632;subscriber=1;tmi-sent-ts=1681072545437;turbo=0;user-id=783267696;user-type= :occluder!occluder@occluder.tmi.twitch.tv PRIVMSG #forsen :@forsen DON'T RAGE 😡 󠀀  󠀀  󠀀";
        ReadOnlyMemory<byte> memory = Encoding.UTF8.GetBytes(raw);
        IrcClient client = new(_ => { _.Anonymous = true; });
        int msgs = 0;

        client.OnMessage += m =>
        {
            msgs++;
            if (msgs == 1)
                Assert.Equal("Thank you @HasanAbi this is better than watching mizkif watch emily get taped to wall LULW", m.Content);
            else
                Assert.Equal("@forsen DON'T RAGE 😡 \U000e0000  \U000e0000  \U000e0000", m.Content);

            return default;
        };

        client.Parse(memory);
        Assert.Equal(2, msgs);
    }
}
