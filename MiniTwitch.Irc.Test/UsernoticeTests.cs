using MiniTwitch.Irc.Enums;
using MiniTwitch.Irc.Interfaces;
using MiniTwitch.Irc.Internal.Enums;
using MiniTwitch.Irc.Models;
using Xunit;

namespace MiniTwitch.Irc.Test;

public class UsernoticeTests
{
    [Fact]
    public void Announcement_USERNOTICE()
    {
        string raw = "@badge-info=subscriber/86;badges=moderator/1,subscriber/3072;color=#2E8B57;display-name=pajbot;emotes=;flags=;id=50f06104-1425-4e61-8d69-19b2e910cc92;login=pajbot;mod=1;msg-id=announcement;msg-param-color=PRIMARY;room-id=11148817;subscriber=1;system-msg=;tmi-sent-ts=1678652624650;user-id=82008718;user-type=mod :tmi.twitch.tv USERNOTICE #pajlada :guh";
        var usernotice = Usernotice.Construct(raw);
        Assert.Equal(UsernoticeType.Announcement, usernotice.MsgId);

        IAnnouncementNotice announcement = usernotice;
        Assert.Equal("50f06104-1425-4e61-8d69-19b2e910cc92", announcement.Id);
        Assert.Equal(string.Empty, announcement.Emotes);
        Assert.Equal(string.Empty, announcement.Flags);
        Assert.Equal(AnnouncementColor.Primary, announcement.Color);
        Assert.Equal(1678652624650, announcement.TmiSentTs);
        Assert.Equal("guh", announcement.Message);
    }

    [Fact]
    public void Announcement_Blue_USERNOTICE()
    {
        string raw = "@badge-info=;badges=broadcaster/1;color=#F2647B;display-name=occluder;emotes=;flags=;id=c10d18ca-c15b-4294-bf17-fe316acd21ff;login=occluder;mod=0;msg-id=announcement;msg-param-color=BLUE;room-id=783267696;subscriber=0;system-msg=;tmi-sent-ts=1678880148283;user-id=783267696;user-type= :tmi.twitch.tv USERNOTICE #occluder :kek";
        var usernotice = Usernotice.Construct(raw);
        Assert.Equal(UsernoticeType.Announcement, usernotice.MsgId);

        IAnnouncementNotice announcement = usernotice;
        Assert.Equal("c10d18ca-c15b-4294-bf17-fe316acd21ff", announcement.Id);
        Assert.Equal(string.Empty, announcement.Emotes);
        Assert.Equal(string.Empty, announcement.Flags);
        Assert.Equal(AnnouncementColor.Blue, announcement.Color);
        Assert.Equal(1678880148283, announcement.TmiSentTs);
        Assert.Equal("kek", announcement.Message);
    }

    [Fact]
    public void Sub_USERNOTICE()
    {
        string raw = "@badge-info=subscriber/1;badges=subscriber/0;color=#FF0000;display-name=SleepyHeadszZ;emotes=;flags=;id=a965a60c-1421-4878-b317-85f32b3637bc;login=sleepyheadszz;mod=0;msg-id=sub;msg-param-cumulative-months=1;msg-param-months=0;msg-param-multimonth-duration=1;msg-param-multimonth-tenure=0;msg-param-should-share-streak=0;msg-param-sub-plan-name=Channel\\sSubscription\\s(mandeow);msg-param-sub-plan=1000;msg-param-was-gifted=false;room-id=128856353;subscriber=1;system-msg=SleepyHeadszZ\\ssubscribed\\sat\\sTier\\s1.;tmi-sent-ts=1678860387524;user-id=183889438;user-type= :tmi.twitch.tv USERNOTICE #mande";
        var usernotice = Usernotice.Construct(raw);
        Assert.Equal(UsernoticeType.Sub, usernotice.MsgId);

        ISubNotice sub = usernotice;
        Assert.Equal("a965a60c-1421-4878-b317-85f32b3637bc", sub.Id);
        Assert.Equal(string.Empty, sub.Emotes);
        Assert.Equal(string.Empty, sub.Flags);
        Assert.Equal(SubPlan.Tier1, sub.SubPlan);
        Assert.Equal(1, sub.CumulativeMonths);
        Assert.Equal(0, sub.MonthStreak);
        Assert.Equal(1678860387524, sub.TmiSentTs);
        Assert.Equal(string.Empty, sub.Message);
        Assert.Equal($"Channel Subscription (mandeow)", sub.SubPlanName);
        Assert.Equal($"SleepyHeadszZ subscribed at Tier 1.", sub.SystemMessage);
        Assert.False(sub.ShouldShareStreak);
    }

    [Fact]
    public void Resub_Empty_USERNOTICE()
    {
        string raw = "@badge-info=subscriber/5;badges=subscriber/3;color=#5F9EA0;display-name=Syn993;emotes=;flags=;id=401d17b8-363a-4f63-85c8-cd5996fbd4e0;login=syn993;mod=0;msg-id=resub;msg-param-cumulative-months=5;msg-param-months=0;msg-param-multimonth-duration=0;msg-param-multimonth-tenure=0;msg-param-should-share-streak=1;msg-param-streak-months=4;msg-param-sub-plan-name=Channel\\sSubscription\\s(mandeow);msg-param-sub-plan=1000;msg-param-was-gifted=false;room-id=128856353;subscriber=1;system-msg=Syn993\\ssubscribed\\sat\\sTier\\s1.\\sThey've\\ssubscribed\\sfor\\s5\\smonths,\\scurrently\\son\\sa\\s4\\smonth\\sstreak!;tmi-sent-ts=1678873100296;user-id=79085174;user-type= :tmi.twitch.tv USERNOTICE #mande";
        var usernotice = Usernotice.Construct(raw);
        Assert.Equal(UsernoticeType.Resub, usernotice.MsgId);

        ISubNotice sub = usernotice;
        Assert.Equal("401d17b8-363a-4f63-85c8-cd5996fbd4e0", sub.Id);
        Assert.Equal(string.Empty, sub.Emotes);
        Assert.Equal(string.Empty, sub.Flags);
        Assert.Equal(SubPlan.Tier1, sub.SubPlan);
        Assert.Equal(5, sub.CumulativeMonths);
        Assert.Equal(4, sub.MonthStreak);
        Assert.Equal(1678873100296, sub.TmiSentTs);
        Assert.Equal(string.Empty, sub.Message);
        Assert.Equal($"Channel Subscription (mandeow)", sub.SubPlanName);
        Assert.Equal($"Syn993 subscribed at Tier 1. They've subscribed for 5 months, currently on a 4 month streak!", sub.SystemMessage);
        Assert.True(sub.ShouldShareStreak);
    }

    [Fact]
    public void Resub_USERNOTICE()
    {
        string raw = "@badge-info=subscriber/14;badges=subscriber/12,sub-gifter/5;color=#FFB735;display-name=LordGrox;emotes=;flags=;id=b59b84a2-ae2a-437b-81c2-d049f71448d7;login=lordgrox;mod=0;msg-id=resub;msg-param-cumulative-months=14;msg-param-months=0;msg-param-multimonth-duration=0;msg-param-multimonth-tenure=0;msg-param-should-share-streak=0;msg-param-sub-plan-name=Channel\\sSubscription\\s(mandeow);msg-param-sub-plan=1000;msg-param-was-gifted=false;room-id=128856353;subscriber=1;system-msg=LordGrox\\ssubscribed\\sat\\sTier\\s1.\\sThey've\\ssubscribed\\sfor\\s14\\smonths!;tmi-sent-ts=1678873250647;user-id=38584526;user-type= :tmi.twitch.tv USERNOTICE #mande :obama: good day mande and chat, I hope everyone has a banger day. (silence) witch: Aint no way lil bro (bong)";
        var usernotice = Usernotice.Construct(raw);
        Assert.Equal(UsernoticeType.Resub, usernotice.MsgId);

        ISubNotice sub = usernotice;
        Assert.Equal("b59b84a2-ae2a-437b-81c2-d049f71448d7", sub.Id);
        Assert.Equal(string.Empty, sub.Emotes);
        Assert.Equal(string.Empty, sub.Flags);
        Assert.Equal(SubPlan.Tier1, sub.SubPlan);
        Assert.Equal(14, sub.CumulativeMonths);
        Assert.Equal(0, sub.MonthStreak);
        Assert.Equal(1678873250647, sub.TmiSentTs);
        Assert.Equal("obama: good day mande and chat, I hope everyone has a banger day. (silence) witch: Aint no way lil bro (bong)", sub.Message);
        Assert.Equal($"Channel Subscription (mandeow)", sub.SubPlanName);
        Assert.Equal($"LordGrox subscribed at Tier 1. They've subscribed for 14 months!", sub.SystemMessage);
        Assert.False(sub.ShouldShareStreak);
    }

    [Fact]
    public void GiftSub_USERNOTICE()
    {
        string raw = "@badge-info=subscriber/13;badges=subscriber/12,premium/1;color=#0000FF;display-name=Goop_456789;emotes=;flags=;id=f27c6766-80b3-4eb5-875e-ec892ca4cb3a;login=goop_456789;mod=0;msg-id=subgift;msg-param-gift-months=1;msg-param-months=12;msg-param-origin-id=a3\\s66\\s1f\\s0f\\sc7\\sb9\\sbb\\sc7\\s82\\sb4\\s8f\\sad\\s7b\\sb0\\s10\\s2c\\sfe\\s0b\\sb2\\sbf;msg-param-recipient-display-name=Zackpanjang;msg-param-recipient-id=412581855;msg-param-recipient-user-name=zackpanjang;msg-param-sender-count=11;msg-param-sub-plan-name=Channel\\sSubscription\\s(mandeow);msg-param-sub-plan=1000;room-id=128856353;subscriber=1;system-msg=Goop_456789\\sgifted\\sa\\sTier\\s1\\ssub\\sto\\sZackpanjang!\\sThey\\shave\\sgiven\\s11\\sGift\\sSubs\\sin\\sthe\\schannel!;tmi-sent-ts=1678876353907;user-id=93668177;user-type= :tmi.twitch.tv USERNOTICE #mande";
        var usernotice = Usernotice.Construct(raw);
        Assert.Equal(UsernoticeType.Subgift, usernotice.MsgId);

        IGiftSubNotice gift = usernotice;
        Assert.Equal("f27c6766-80b3-4eb5-875e-ec892ca4cb3a", gift.Id);
        Assert.Equal(1678876353907, gift.TmiSentTs);
        Assert.Equal(12, gift.Months);
        Assert.Equal(1, gift.GiftedMonths);
        Assert.Equal(SubPlan.Tier1, gift.SubPlan);
        Assert.Equal("Zackpanjang", gift.Recipient.DisplayName);
        Assert.Equal("zackpanjang", gift.Recipient.Name);
        Assert.Equal(412581855, gift.Recipient.Id);
        Assert.Equal(11, gift.TotalGiftCount);
        Assert.Equal($"Channel Subscription (mandeow)", gift.SubPlanName);
        Assert.Equal($"Goop_456789 gifted a Tier 1 sub to Zackpanjang! They have given 11 Gift Subs in the channel!", gift.SystemMessage);
    }

    [Fact]
    public void GiftSubIntro_USERNOTICE()
    {
        string raw = "@badge-info=subscriber/2;badges=subscriber/2,sub-gifter/5;color=#8A2BE2;display-name=xHypnoticPowerx;emotes=;flags=;id=90dc688f-979f-4a22-a495-868704739290;login=xhypnoticpowerx;mod=0;msg-id=submysterygift;msg-param-mass-gift-count=1;msg-param-origin-id=fb\\sa5\\s95\\s76\\s75\\s95\\sd5\\sa1\\s64\\s7d\\s3c\\s20\\s81\\s3f\\sb2\\s96\\sdb\\sfb\\sf5\\s28;msg-param-sender-count=6;msg-param-sub-plan=1000;room-id=128856353;subscriber=1;system-msg=xHypnoticPowerx\\sis\\sgifting\\s1\\sTier\\s1\\sSubs\\sto\\sMande's\\scommunity!\\sThey've\\sgifted\\sa\\stotal\\sof\\s6\\sin\\sthe\\schannel!;tmi-sent-ts=1678806701670;user-id=656885789;user-type= :tmi.twitch.tv USERNOTICE #mande";
        var usernotice = Usernotice.Construct(raw);
        Assert.Equal(UsernoticeType.SubMysteryGift, usernotice.MsgId);

        IGiftSubNoticeIntro intro = usernotice;
        Assert.Equal("90dc688f-979f-4a22-a495-868704739290", intro.Id);
        Assert.Equal(1678806701670, intro.TmiSentTs);
        Assert.Equal(6, intro.TotalGiftCount);
        Assert.Equal(1, intro.GiftCount);
        Assert.Equal(SubPlan.Tier1, intro.SubPlan);
        Assert.Equal($"xHypnoticPowerx is gifting 1 Tier 1 Subs to Mande's community! They've gifted a total of 6 in the channel!", intro.SystemMessage);
    }

    [Fact]
    public void PrimeToPaid_USERNOTICE()
    {
        string raw = "@badge-info=subscriber/17;badges=subscriber/12;color=#FF0000;display-name=DrDisRespexs;emotes=;flags=;id=8e88f9b5-840d-4ca4-9dc3-88c4fed21f42;login=drdisrespexs;mod=0;msg-id=primepaidupgrade;msg-param-sub-plan=1000;room-id=22484632;subscriber=1;system-msg=DrDisRespexs\\sconverted\\sfrom\\sa\\sPrime\\ssub\\sto\\sa\\sTier\\s1\\ssub!;tmi-sent-ts=1677696547747;user-id=99687275;user-type= :tmi.twitch.tv USERNOTICE #forsen";
        var usernotice = Usernotice.Construct(raw);
        Assert.Equal(UsernoticeType.PrimePaidUpgrade, usernotice.MsgId);

        IPrimeUpgradeNotice upgrade = usernotice;
        Assert.Equal("8e88f9b5-840d-4ca4-9dc3-88c4fed21f42", upgrade.Id);
        Assert.Equal(SubPlan.Tier1, upgrade.SubPlan);
        Assert.Equal("DrDisRespexs converted from a Prime sub to a Tier 1 sub!", upgrade.SystemMessage);
        Assert.Equal(1677696547747, upgrade.TmiSentTs);
    }

    [Fact]
    public void Anon_ContinueGiftedSub_USERNOTICE()
    {
        string raw = "@badge-info=subscriber/33;badges=subscriber/24,game-developer/1;color=#3F968C;display-name=NaMTheWeebs;emotes=;flags=;id=f9284d35-b50e-48b8-886b-65bb448b5da7;login=namtheweebs;mod=0;msg-id=anongiftpaidupgrade;room-id=22484632;subscriber=1;system-msg=NaMTheWeebs\\sis\\scontinuing\\sthe\\sGift\\sSub\\sthey\\sgot\\sfrom\\san\\sanonymous\\suser!;tmi-sent-ts=1675748995095;user-id=232490245;user-type= :tmi.twitch.tv USERNOTICE #forsen";
        var usernotice = Usernotice.Construct(raw);
        Assert.Equal(UsernoticeType.AnonGiftPaidUpgrade, usernotice.MsgId);

        IPaidUpgradeNotice upgrade = usernotice;
        Assert.Equal("f9284d35-b50e-48b8-886b-65bb448b5da7", upgrade.Id);
        Assert.Equal($"NaMTheWeebs is continuing the Gift Sub they got from an anonymous user!", upgrade.SystemMessage);
        Assert.Equal(string.Empty, upgrade.GifterUsername);
        Assert.Equal(string.Empty, upgrade.GifterDisplayName);
        Assert.Equal(1675748995095, upgrade.TmiSentTs);
    }

    [Fact]
    public void ContinueGiftedSub_USERNOTICE()
    {
        string raw = "@badge-info=subscriber/13;badges=subscriber/12,game-developer/1;color=#FFFAFA;display-name=special_forces_of_russia;emotes=;flags=;id=cde7a201-41e1-4cd3-afda-17520ed7289d;login=special_forces_of_russia;mod=0;msg-id=giftpaidupgrade;msg-param-sender-login=potnayakatka64;msg-param-sender-name=potnayakatka64;room-id=22484632;subscriber=1;system-msg=special_forces_of_russia\\sis\\scontinuing\\sthe\\sGift\\sSub\\sthey\\sgot\\sfrom\\spotnayakatka64!;tmi-sent-ts=1677361938937;user-id=80718208;user-type= :tmi.twitch.tv USERNOTICE #forsen";
        var usernotice = Usernotice.Construct(raw);
        Assert.Equal(UsernoticeType.GiftPaidUpgrade, usernotice.MsgId);

        IPaidUpgradeNotice upgrade = usernotice;
        Assert.Equal("cde7a201-41e1-4cd3-afda-17520ed7289d", upgrade.Id);
        Assert.Equal($"special_forces_of_russia is continuing the Gift Sub they got from potnayakatka64!", upgrade.SystemMessage);
        Assert.Equal("potnayakatka64", upgrade.GifterUsername);
        Assert.Equal("potnayakatka64", upgrade.GifterDisplayName);
        Assert.Equal(1677361938937, upgrade.TmiSentTs);
    }

    [Fact]
    public void Raid_USERNOTICE()
    {
        string raw = "@badge-info=subscriber/9;badges=subscriber/6;color=#F2647B;display-name=occluder;emotes=;flags=;id=8b1984d1-9e2a-4a72-86be-8f4c11adc3dd;login=occluder;mod=0;msg-id=raid;msg-param-displayName=occluder;msg-param-login=occluder;msg-param-profileImageURL=https://static-cdn.jtvnw.net/jtv_user_pictures/cce5d685-8fcb-4aca-ba7a-5e331068e423-profile_image-70x70.png;msg-param-viewerCount=1;room-id=11148817;subscriber=1;system-msg=1\\sraiders\\sfrom\\soccluder\\shave\\sjoined!;tmi-sent-ts=1676557512027;user-id=783267696;user-type= :tmi.twitch.tv USERNOTICE #pajlada";
        var usernotice = Usernotice.Construct(raw);
        Assert.Equal(UsernoticeType.Raid, usernotice.MsgId);

        IRaidNotice raid = usernotice;
        Assert.Equal("8b1984d1-9e2a-4a72-86be-8f4c11adc3dd", raid.Id);
        Assert.Equal(1, raid.ViewerCount);
        Assert.Equal("1 raiders from occluder have joined!", raid.SystemMessage);
        Assert.Equal(1676557512027, raid.TmiSentTs);
    }

    [Fact]
    public void Shared_Announcement_USERNOTICE()
    {
        string raw = "@badge-info=;badges=staff/1,raging-wolf-helm/1;color=#DAA520;display-name=lahoooo;emotes=;flags=;id=01cd601f-bc3f-49d5-ab4b-136fa9d6ec22;login=lahoooo;mod=0;msg-id=sharedchatnotice;msg-param-color=PRIMARY;room-id=11148817;source-badge-info=;source-badges=staff/1,moderator/1,bits-leader/1;source-id=4083dadc-9f20-40f9-ba92-949ebf6bc294;source-msg-id=announcement;source-room-id=1025594235;subscriber=0;system-msg=;tmi-sent-ts=1726118378465;user-id=612865661;user-type=staff;vip=0 :tmi.twitch.tv USERNOTICE #pajlada :hi this is an announcement from 1";
        var usernotice = Usernotice.Construct(raw);
        Assert.Equal(UsernoticeType.SharedChatNotice, usernotice.MsgId);

        IAnnouncementNotice announcement = usernotice;
        Assert.Equal("01cd601f-bc3f-49d5-ab4b-136fa9d6ec22", announcement.Id);
        Assert.Equal(string.Empty, announcement.Emotes);
        Assert.Equal(string.Empty, announcement.Flags);
        Assert.Equal(AnnouncementColor.Primary, announcement.Color);
        Assert.Equal(1726118378465, announcement.TmiSentTs);
        Assert.Equal("hi this is an announcement from 1", announcement.Message);
        Assert.Equal("pajlada", announcement.Channel.Name);
        Assert.Equal(11148817, announcement.Channel.Id);
        Assert.Equal("lahoooo", announcement.Author.DisplayName);
        Assert.Equal("lahoooo", announcement.Author.Name);
        Assert.Equal(string.Empty, announcement.Author.BadgeInfo);
        Assert.Equal("staff/1,raging-wolf-helm/1", announcement.Author.Badges);

        Assert.True(announcement.Source.HasSource);
        Assert.Equal(UsernoticeType.Announcement, usernotice.Source.MsgId);
        Assert.Equal("4083dadc-9f20-40f9-ba92-949ebf6bc294", announcement.Source.Id);
        Assert.Equal(1025594235, announcement.Source.ChannelId);
        Assert.Equal(string.Empty, announcement.Source.BadgeInfo);
        Assert.Equal("staff/1,moderator/1,bits-leader/1", announcement.Source.Badges);

    }
}
