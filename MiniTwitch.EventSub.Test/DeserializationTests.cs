using System.Reflection;
using System.Text;
using MiniTwitch.EventSub.Internal.Parsing;
using MiniTwitch.EventSub.Models;

namespace MiniTwitch.EventSub.Test;

public class DeserializationTests
{
    [Fact]
    public void ConstructPayloads()
    {
        foreach (var field in typeof(Payloads).GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            var attr = (EventPayloadAttribute?)Attribute.GetCustomAttribute(field, typeof(EventPayloadAttribute));
            if (attr is null)
            {
                continue;
            }

            var json = Encoding.UTF8.GetBytes((string)field.GetValue(null)!);
            try
            {
                var instance = Activator.CreateInstance(attr.OutType, new object[] { new ReadOnlyMemory<byte>(json) });
                Assert.NotNull(instance);
            }
            catch (Exception ex)
            {
                Assert.Fail($"[{field.Name}] Construction to {attr.OutType.Name} failure! {ex.Message}");
            }
        }
    }

    [Fact]
    public void ChannelChatMessage_Basic()
    {
        var json = Encoding.UTF8.GetBytes(Payloads.ChannelChatMessageBasicJson);
        var msg = new ChannelChatMessage(json.AsMemory());

        Assert.Equal(1337, msg.BroadcasterId);
        Assert.Equal("testbroadcaster", msg.BroadcasterUsername);
        Assert.Equal("TestBroadcaster", msg.BroadcasterDisplayName);
        Assert.Equal(Guid.Parse("550e8400-e29b-41d4-a716-446655440000"), msg.MessageId);

        var m = msg.Message;
        Assert.Equal("Hello world!", m.Text);
        Assert.Equal("text", m.MessageType);
        Assert.Empty(m.Badges);
        Assert.Single(m.Fragments);
        Assert.Null(m.ChannelPointsCustomRewardId);
    }

    [Fact]
    public void ChannelChatMessage_WithCheer()
    {
        var json = Encoding.UTF8.GetBytes(Payloads.ChannelChatMessageWithCheerJson);
        var msg = new ChannelChatMessage(json.AsMemory());

        Assert.Equal(1337, msg.BroadcasterId);
        Assert.Equal("Cheer100 Hello everyone!", msg.Message.Text);

        var fragments = msg.Message.Fragments;
        Assert.Equal(2, fragments.Length);

        var cheerFragment = fragments[0];
        Assert.Equal("cheermote", cheerFragment.Type);
        Assert.Equal("Cheer100", cheerFragment.Text);
        Assert.Equal("Cheer", cheerFragment.Cheermote.Prefix);
        Assert.Equal(100, cheerFragment.Cheermote.Bits);
        Assert.Equal(1, cheerFragment.Cheermote.Tier);

        var textFragment = fragments[1];
        Assert.Equal("text", textFragment.Type);
        Assert.Equal(" Hello everyone!", textFragment.Text);

        var badges = msg.Message.Badges;
        Assert.Single(badges);
        Assert.Equal("broadcaster", badges[0].SetId);
        Assert.Equal("1", badges[0].Id);

        Assert.NotNull(msg.Message.Cheer);
        Assert.Equal(100, msg.Message.Cheer.Value.Bits);

        Assert.Equal("#FF0000", msg.Message.Color);
    }

    [Fact]
    public void ChannelChatMessage_WithReply()
    {
        var json = Encoding.UTF8.GetBytes(Payloads.ChannelChatMessageWithReplyJson);
        var msg = new ChannelChatMessage(json.AsMemory());

        Assert.Equal(1337, msg.BroadcasterId);
        Assert.Equal("@user Great point!", msg.Message.Text);

        var fragments = msg.Message.Fragments;
        Assert.Equal(2, fragments.Length);

        var mentionFragment = fragments[0];
        Assert.Equal("mention", mentionFragment.Type);
        Assert.Equal("@user", mentionFragment.Text);
        Assert.Equal(202, mentionFragment.Mention.UserId);

        var reply = msg.Message.Reply;
        Assert.NotNull(reply);
        Assert.Equal(Guid.Parse("550e8400-e29b-41d4-a716-446655440000"), reply.Value.ParentMessageId);
        Assert.Equal("Hello world!", reply.Value.ParentMessageBody);
        Assert.Equal(456, reply.Value.ParentUserId);
        Assert.Equal("testchatter", reply.Value.ParentUsername);
        Assert.Equal("TestChatter", reply.Value.ParentUserDisplayName);

        Assert.Null(msg.Message.ChannelPointsCustomRewardId);
    }

    [Fact]
    public void EventSubscription_WebSocket()
    {
        var json = Encoding.UTF8.GetBytes(Payloads.SubscriptionWebSocketJson);
        var sub = new EventSubscription(json.AsMemory());

        Assert.Equal(Guid.Parse("0b7f3361-672b-4d39-b307-dd5b576c9b27"), sub.Id);
        Assert.Equal("enabled", sub.Status);
        Assert.Equal("channel.chat.message", sub.Type);
        Assert.Equal("1", sub.Version);
        Assert.Equal(DateTimeOffset.Parse("2023-11-06T18:11:47.492253549Z"), sub.CreatedAt);
        Assert.Equal(12, sub.Cost);

        var condition = sub.Condition;
        Assert.Equal(1971641, condition.BroadcasterId);
        Assert.Equal(2914196, condition.UserId);
    }

    [Fact]
    public void EventSubscription_Webhook()
    {
        var json = Encoding.UTF8.GetBytes(Payloads.SubscriptionWebhookJson);
        var sub = new EventSubscription(json.AsMemory());

        Assert.Equal(Guid.Parse("f1c2a387-161a-49f9-a165-0f21d7a4e1c4"), sub.Id);
        Assert.Equal("enabled", sub.Status);
        Assert.Equal("automod.message.hold", sub.Type);
        Assert.Equal("2", sub.Version);
        Assert.Equal(DateTimeOffset.Parse("2023-04-11T10:11:12.123Z"), sub.CreatedAt);
        Assert.Equal(0, sub.Cost);

        var condition = sub.Condition;
        Assert.Equal(1337, condition.BroadcasterId);
    }

    [Fact]
    public void FullEventSubMessage()
    {
        var fullJson = $$"""
        {
            "subscription": {{Payloads.SubscriptionWebSocketJson}},
            "event": {{Payloads.ChannelChatMessageBasicJson}}
        }
        """;
        ReadOnlyMemory<byte> mem = Encoding.UTF8.GetBytes(fullJson);

        var sub = new EventSubscription(mem.GetChild("subscription"u8));
        Assert.Equal("channel.chat.message", sub.Type);
        Assert.Equal(1971641, sub.Condition.BroadcasterId);

        var evt = new ChannelChatMessage(mem.GetChild("event"u8));
        Assert.Equal(1337, evt.BroadcasterId);
        Assert.Equal("Hello world!", evt.Message.Text);
    }

    [Fact]
    public void AutomodMessageHold_V1_Basic()
    {
        var json = Encoding.UTF8.GetBytes(Payloads.AutomodMessageHoldV1Json);
        var msg = new AutomodMessageHold(json.AsMemory());

        Assert.Equal(1337, msg.BroadcasterId);
        Assert.Equal("testbroadcaster", msg.BroadcasterUsername);
        Assert.Equal("TestBroadcaster", msg.BroadcasterDisplayName);
        Assert.Equal(456, msg.UserId);
        Assert.Equal("baduser", msg.Username);
        Assert.Equal("BadUser", msg.UserDisplayName);
        Assert.Equal(Guid.Parse("550e8400-e29b-41d4-a716-446655440000"), msg.MessageId);
        Assert.Equal("Bad message caught by automod", msg.Message.Text);
        Assert.Equal("aggressive", msg.Category);
        Assert.Equal(3, msg.Level);
        Assert.Equal(DateTimeOffset.Parse("2023-04-11T10:11:12.123Z"), msg.HeldAt);

        var fragments = msg.Message.Fragments;
        Assert.Single(fragments);
        Assert.Equal("Bad message caught by automod", fragments[0].Text);
    }

    [Fact]
    public void AutomodMessageHold_V1_WithCheermote()
    {
        var json = Encoding.UTF8.GetBytes(Payloads.AutomodMessageHoldV1WithCheermoteJson);
        var msg = new AutomodMessageHold(json.AsMemory());

        Assert.Equal(1337, msg.BroadcasterId);
        Assert.Equal(789, msg.UserId);
        Assert.Equal("cheeruser", msg.Username);
        Assert.Equal("CheerUser", msg.UserDisplayName);
        Assert.Equal("Bad message Cheer100", msg.Message.Text);

        var fragments = msg.Message.Fragments;
        Assert.Equal(2, fragments.Length);
        Assert.Equal("Bad message ", fragments[0].Text);

        var cheermoteFragment = fragments[1].Cheermote;
        Assert.NotNull(cheermoteFragment);
        Assert.Equal("Cheer", cheermoteFragment.Value.Prefix);
        Assert.Equal(100, cheermoteFragment.Value.Bits);
        Assert.Equal(1, cheermoteFragment.Value.Tier);

        Assert.Equal("bullying", msg.Category);
        Assert.Equal(2, msg.Level);
    }

    [Fact]
    public void AutomodMessageHoldV2_AutomodReason()
    {
        var json = Encoding.UTF8.GetBytes(Payloads.AutomodMessageHoldV2AutomodJson);
        var msg = new AutomodMessageHoldV2(json.AsMemory());

        Assert.Equal(1337, msg.BroadcasterId);
        Assert.Equal("testbroadcaster", msg.BroadcasterUsername);
        Assert.Equal("TestBroadcaster", msg.BroadcasterDisplayName);
        Assert.Equal(456, msg.UserId);
        Assert.Equal("baduser", msg.Username);
        Assert.Equal("BadUser", msg.UserDisplayName);
        Assert.Equal("bad-message-id-1", msg.MessageId);
        Assert.Equal("Bad message with pogchamp", msg.Message.Text);
        Assert.Equal("automod", msg.Reason);
        Assert.Equal(DateTimeOffset.Parse("2023-04-11T10:11:12.123Z"), msg.HeldAt);

        var automod = msg.Automod;
        Assert.NotNull(automod);
        Assert.Equal("aggressive", automod.Value.Category);
        Assert.Equal(1, automod.Value.Level);

        var boundaries = automod.Value.Boundaries;
        Assert.Equal(2, boundaries.Length);
        Assert.Equal(0, boundaries[0].StartPos);
        Assert.Equal(10, boundaries[0].EndPos);
        Assert.Equal(20, boundaries[1].StartPos);
        Assert.Equal(30, boundaries[1].EndPos);

        var fragments = msg.Message.Fragments;
        Assert.Equal(2, fragments.Length);
        Assert.Equal("text", fragments[0].Type);
        Assert.Equal("cheermote", fragments[1].Type);
        Assert.Equal("pogchamp", fragments[1].Text);
        Assert.Equal(1000, fragments[1].Cheermote.Bits);
    }

    [Fact]
    public void AutomodMessageHoldV2_BlockedTermReason()
    {
        var json = Encoding.UTF8.GetBytes(Payloads.AutomodMessageHoldV2BlockedTermJson);
        var msg = new AutomodMessageHoldV2(json.AsMemory());

        Assert.Equal(1337, msg.BroadcasterId);
        Assert.Equal("broadcaster", msg.BroadcasterUsername);
        Assert.Equal("Broadcaster", msg.BroadcasterDisplayName);
        Assert.Equal(789, msg.UserId);
        Assert.Equal("baduser2", msg.Username);
        Assert.Equal("BadUser2", msg.UserDisplayName);
        Assert.Equal("held-message-123", msg.MessageId);
        Assert.Equal("Message with blocked term", msg.Message.Text);
        Assert.Equal("blocked_term", msg.Reason);
        Assert.Equal(DateTimeOffset.Parse("2023-04-11T10:11:12.123Z"), msg.HeldAt);

        var blockedTerm = msg.BlockedTerm;
        Assert.NotNull(blockedTerm);
        Assert.Single(blockedTerm.Value.TermsFound);

        var term = blockedTerm.Value.TermsFound[0];
        Assert.Equal("term123", term.TermId);
        Assert.Equal(1337, term.OwnerBroadcasterId);
        Assert.Equal("broadcaster", term.OwnerBroadcasterUsername);
        Assert.Equal("Broadcaster", term.OwnerBroadcasterDisplayName);
        Assert.Equal(11, term.Boundary.StartPos);
        Assert.Equal(23, term.Boundary.EndPos);
    }

    [Fact]
    public void AutomodMessageUpdate_V1()
    {
        var json = Encoding.UTF8.GetBytes(Payloads.AutomodMessageUpdateV1Json);
        var msg = new AutomodMessageUpdate(json.AsMemory());

        Assert.Equal(1337, msg.BroadcasterId);
        Assert.Equal("testbroadcaster", msg.BroadcasterUsername);
        Assert.Equal("TestBroadcaster", msg.BroadcasterDisplayName);
        Assert.Equal(456, msg.UserId);
        Assert.Equal("baduser", msg.Username);
        Assert.Equal("BadUser", msg.UserDisplayName);
        Assert.Equal(9001, msg.ModeratorId);
        Assert.Equal("the_mod", msg.ModeratorUsername);
        Assert.Equal("The_Mod", msg.ModeratorDisplayName);
        Assert.Equal(Guid.Parse("550e8400-e29b-41d4-a716-446655440000"), msg.MessageId);
        Assert.Equal("This is a bad message", msg.Message.Text);
        Assert.Equal(3, msg.Level);
        Assert.Equal("aggressive", msg.Category);
        Assert.Equal("approved", msg.Status);
        Assert.Equal(DateTimeOffset.Parse("2022-12-02T15:00:00.00Z"), msg.HeldAt);

        var fragments = msg.Message.Fragments;
        Assert.Single(fragments);
        Assert.Equal("This is a bad message", fragments[0].Text);
    }

    [Fact]
    public void AutomodMessageUpdate_V1_WithCheermote()
    {
        var json = Encoding.UTF8.GetBytes(Payloads.AutomodMessageUpdateV1WithEmoteAndCheermoteJson);
        var msg = new AutomodMessageUpdate(json.AsMemory());

        Assert.Equal(1337, msg.BroadcasterId);
        Assert.Equal(789, msg.UserId);
        Assert.Equal("cheeruser", msg.Username);
        Assert.Equal("CheerUser", msg.UserDisplayName);
        Assert.Equal("Bad message Cheer100", msg.Message.Text);

        var fragments = msg.Message.Fragments;
        Assert.Equal(2, fragments.Length);
        Assert.Equal("Bad message ", fragments[0].Text);

        var cheermote = fragments[1].Cheermote;
        Assert.NotNull(cheermote);
        Assert.Equal("Cheer", cheermote.Value.Prefix);
        Assert.Equal(100, cheermote.Value.Bits);
        Assert.Equal(1, cheermote.Value.Tier);

        Assert.Equal("bullying", msg.Category);
        Assert.Equal(2, msg.Level);
        Assert.Equal("denied", msg.Status);
    }

    [Fact]
    public void AutomodMessageUpdateV2_AutomodReason()
    {
        var json = Encoding.UTF8.GetBytes(Payloads.AutomodMessageUpdateV2AutomodJson);
        var msg = new AutomodMessageUpdateV2(json.AsMemory());

        Assert.Equal(1337, msg.BroadcasterId);
        Assert.Equal("testbroadcaster", msg.BroadcasterUsername);
        Assert.Equal("TestBroadcaster", msg.BroadcasterDisplayName);
        Assert.Equal(4242, msg.UserId);
        Assert.Equal("baduser", msg.Username);
        Assert.Equal("BadUserDisplay", msg.UserDisplayName);
        Assert.Equal(9001, msg.ModeratorId);
        Assert.Equal("the_mod", msg.ModeratorUsername);
        Assert.Equal("The_Mod", msg.ModeratorDisplayName);
        Assert.Equal("bad-message-id-1", msg.MessageId);
        Assert.Equal("This is a bad message pogchamp", msg.Message.Text);
        Assert.Equal("approved", msg.Status);
        Assert.Equal("automod", msg.Reason);
        Assert.Equal(DateTimeOffset.Parse("2022-12-02T15:00:00.00Z"), msg.HeldAt);

        var automod = msg.Automod;
        Assert.NotNull(automod);
        Assert.Equal("aggressive", automod.Value.Category);
        Assert.Equal(1, automod.Value.Level);

        var boundaries = automod.Value.Boundaries;
        Assert.Equal(2, boundaries.Length);
        Assert.Equal(0, boundaries[0].StartPos);
        Assert.Equal(10, boundaries[0].EndPos);
        Assert.Equal(20, boundaries[1].StartPos);
        Assert.Equal(30, boundaries[1].EndPos);

        var fragments = msg.Message.Fragments;
        Assert.Equal(2, fragments.Length);
        Assert.Equal("text", fragments[0].Type);
        Assert.Equal("cheermote", fragments[1].Type);
        Assert.Equal("pogchamp", fragments[1].Text);
        Assert.Equal(1000, fragments[1].Cheermote.Bits);
    }

    [Fact]
    public void AutomodSettingsUpdate_V1()
    {
        var json = Encoding.UTF8.GetBytes(Payloads.AutomodSettingsUpdateV1Json);
        var msg = new AutomodSettingsUpdate(json.AsMemory());

        Assert.Equal(1337, msg.BroadcasterId);
        Assert.Equal("cooluser", msg.BroadcasterUsername);
        Assert.Equal("CoolUser", msg.BroadcasterDisplayName);
        Assert.Equal(9001, msg.ModeratorId);
        Assert.Equal("coolmod", msg.ModeratorUsername);
        Assert.Equal("CoolMod", msg.ModeratorDisplayName);
        Assert.Null(msg.OverallLevel);
        Assert.Equal(3, msg.Disability);
        Assert.Equal(3, msg.Aggression);
        Assert.Equal(3, msg.SexualitySexOrGender);
        Assert.Equal(3, msg.Misogyny);
        Assert.Equal(3, msg.Bullying);
        Assert.Equal(0, msg.Swearing);
        Assert.Equal(3, msg.RaceEthnicityOrReligion);
        Assert.Equal(30, msg.SexBasedTerms);
    }

    [Fact]
    public void AutomodSettingsUpdate_V1_WithOverallLevel()
    {
        var json = Encoding.UTF8.GetBytes(Payloads.AutomodSettingsUpdateV1WithOverallLevelJson);
        var msg = new AutomodSettingsUpdate(json.AsMemory());

        Assert.Equal(42, msg.BroadcasterId);
        Assert.Equal("simple", msg.BroadcasterUsername);
        Assert.Equal("Simple", msg.BroadcasterDisplayName);
        Assert.Equal(99, msg.ModeratorId);
        Assert.Equal("mod", msg.ModeratorUsername);
        Assert.Equal("Mod", msg.ModeratorDisplayName);
        Assert.Equal(2, msg.OverallLevel);
        Assert.Equal(2, msg.Disability);
        Assert.Equal(2, msg.Aggression);
        Assert.Equal(2, msg.SexualitySexOrGender);
        Assert.Equal(2, msg.Misogyny);
        Assert.Equal(2, msg.Bullying);
        Assert.Equal(2, msg.Swearing);
        Assert.Equal(2, msg.RaceEthnicityOrReligion);
        Assert.Equal(2, msg.SexBasedTerms);
    }

    [Fact]
    public void AutomodMessageUpdateV2_BlockedTermReason()
    {
        var json = Encoding.UTF8.GetBytes(Payloads.AutomodMessageUpdateV2BlockedTermJson);
        var msg = new AutomodMessageUpdateV2(json.AsMemory());

        Assert.Equal(1337, msg.BroadcasterId);
        Assert.Equal("broadcaster", msg.BroadcasterUsername);
        Assert.Equal("Broadcaster", msg.BroadcasterDisplayName);
        Assert.Equal(789, msg.UserId);
        Assert.Equal("baduser2", msg.Username);
        Assert.Equal("BadUser2", msg.UserDisplayName);
        Assert.Equal(9001, msg.ModeratorId);
        Assert.Equal("the_mod", msg.ModeratorUsername);
        Assert.Equal("The_Mod", msg.ModeratorDisplayName);
        Assert.Equal("bad-message-id-2", msg.MessageId);
        Assert.Equal("Message with blocked term", msg.Message.Text);
        Assert.Equal("denied", msg.Status);
        Assert.Equal("blocked_term", msg.Reason);
        Assert.Equal(DateTimeOffset.Parse("2022-12-02T15:00:00.00Z"), msg.HeldAt);

        var blockedTerm = msg.BlockedTerm;
        Assert.NotNull(blockedTerm);
        Assert.Single(blockedTerm.Value.TermsFound);

        var term = blockedTerm.Value.TermsFound[0];
        Assert.Equal("term123", term.TermId);
        Assert.Equal(1337, term.OwnerBroadcasterId);
        Assert.Equal("broadcaster", term.OwnerBroadcasterUsername);
        Assert.Equal("Broadcaster", term.OwnerBroadcasterDisplayName);
        Assert.Equal(11, term.Boundary.StartPos);
        Assert.Equal(23, term.Boundary.EndPos);
    }

    [Fact]
    public void AutomodTermsUpdate_V1_AddBlocked()
    {
        var json = Encoding.UTF8.GetBytes(Payloads.AutomodTermsUpdateV1AddBlockedJson);
        var msg = new AutomodTermsUpdate(json.AsMemory());

        Assert.Equal(1337, msg.BroadcasterId);
        Assert.Equal("testbroadcaster", msg.BroadcasterUsername);
        Assert.Equal("TestBroadcaster", msg.BroadcasterDisplayName);
        Assert.Equal(9001, msg.ModeratorId);
        Assert.Equal("the_mod", msg.ModeratorUsername);
        Assert.Equal("The_Mod", msg.ModeratorDisplayName);
        Assert.Equal("add_blocked", msg.Action);
        Assert.False(msg.FromAutomod);
        Assert.Equal(2, msg.Terms.Length);
        Assert.Equal("badword1", msg.Terms[0]);
        Assert.Equal("badword2", msg.Terms[1]);
    }

    [Fact]
    public void AutomodTermsUpdate_V1_RemovePermitted()
    {
        var json = Encoding.UTF8.GetBytes(Payloads.AutomodTermsUpdateV1RemovePermittedJson);
        var msg = new AutomodTermsUpdate(json.AsMemory());

        Assert.Equal(42, msg.BroadcasterId);
        Assert.Equal("simple", msg.BroadcasterUsername);
        Assert.Equal("Simple", msg.BroadcasterDisplayName);
        Assert.Equal(99, msg.ModeratorId);
        Assert.Equal("mod", msg.ModeratorUsername);
        Assert.Equal("Mod", msg.ModeratorDisplayName);
        Assert.Equal("remove_permitted", msg.Action);
        Assert.True(msg.FromAutomod);
        Assert.Single(msg.Terms);
        Assert.Equal("allowedword", msg.Terms[0]);
    }

    [Fact]
    public void ChannelAdBreakBegin_V1_Manual()
    {
        var json = Encoding.UTF8.GetBytes(Payloads.ChannelAdBreakBeginV1ManualJson);
        var msg = new ChannelAdBreakBegin(json.AsMemory());

        Assert.Equal(60, msg.DurationSeconds);
        Assert.Equal(DateTimeOffset.Parse("2019-11-16T10:11:12.634234626Z"), msg.StartedAt);
        Assert.False(msg.IsAutomatic);
        Assert.Equal(1337, msg.BroadcasterId);
        Assert.Equal("cool_user", msg.BroadcasterUsername);
        Assert.Equal("Cool_User", msg.BroadcasterDisplayName);
        Assert.Equal(1337, msg.RequesterId);
        Assert.Equal("cool_user", msg.RequesterUsername);
        Assert.Equal("Cool_User", msg.RequesterDisplayName);
    }

    [Fact]
    public void ChannelBan_V1_Timeout()
    {
        var json = Encoding.UTF8.GetBytes(Payloads.ChannelBanV1TimeoutJson);
        var msg = new ChannelBan(json.AsMemory());

        Assert.Equal(1234, msg.UserId);
        Assert.Equal("cool_user", msg.Username);
        Assert.Equal("Cool_User", msg.UserDisplayName);
        Assert.Equal(1337, msg.BroadcasterId);
        Assert.Equal("cooler_user", msg.BroadcasterUsername);
        Assert.Equal("Cooler_User", msg.BroadcasterDisplayName);
        Assert.Equal(1339, msg.ModeratorId);
        Assert.Equal("mod_user", msg.ModeratorUsername);
        Assert.Equal("Mod_User", msg.ModeratorDisplayName);
        Assert.Equal("Offensive language", msg.Reason);
        Assert.Equal(DateTimeOffset.Parse("2020-07-15T18:15:11.17106713Z"), msg.BannedAt);
        Assert.NotNull(msg.EndsAt);
        Assert.Equal(DateTimeOffset.Parse("2020-07-15T18:16:11.17106713Z"), msg.EndsAt.Value);
        Assert.False(msg.IsPermanent);
    }

    [Fact]
    public void ChannelBan_V1_Permanent()
    {
        var json = Encoding.UTF8.GetBytes(Payloads.ChannelBanV1PermanentJson);
        var msg = new ChannelBan(json.AsMemory());

        Assert.Equal(5678, msg.UserId);
        Assert.Equal("bad_actor", msg.Username);
        Assert.Equal("Bad_Actor", msg.UserDisplayName);
        Assert.Equal(42, msg.BroadcasterId);
        Assert.Equal("streamer", msg.BroadcasterUsername);
        Assert.Equal("Streamer", msg.BroadcasterDisplayName);
        Assert.Equal(99, msg.ModeratorId);
        Assert.Equal("mod", msg.ModeratorUsername);
        Assert.Equal("Mod", msg.ModeratorDisplayName);
        Assert.Equal("Repeated harassment", msg.Reason);
        Assert.Equal(DateTimeOffset.Parse("2020-08-01T12:00:00Z"), msg.BannedAt);
        Assert.Null(msg.EndsAt);
        Assert.True(msg.IsPermanent);
    }

    [Fact]
    public void ChannelAdBreakBegin_V1_Automatic()
    {
        var json = Encoding.UTF8.GetBytes(Payloads.ChannelAdBreakBeginV1AutomaticJson);
        var msg = new ChannelAdBreakBegin(json.AsMemory());

        Assert.Equal(30, msg.DurationSeconds);
        Assert.Equal(DateTimeOffset.Parse("2020-01-15T08:30:00Z"), msg.StartedAt);
        Assert.True(msg.IsAutomatic);
        Assert.Equal(42, msg.BroadcasterId);
        Assert.Equal("streamer", msg.BroadcasterUsername);
        Assert.Equal("Streamer", msg.BroadcasterDisplayName);
        Assert.Equal(42, msg.RequesterId);
        Assert.Equal("streamer", msg.RequesterUsername);
        Assert.Equal("Streamer", msg.RequesterDisplayName);
    }

    [Fact]
    public void ChannelChatClear_V1()
    {
        var json = Encoding.UTF8.GetBytes(Payloads.ChannelChatClearV1Json);
        var msg = new ChannelChatClear(json.AsMemory());

        Assert.Equal(1337, msg.BroadcasterId);
        Assert.Equal("cool_user", msg.BroadcasterUsername);
        Assert.Equal("Cool_User", msg.BroadcasterDisplayName);
    }

    [Fact]
    public void ChannelChatClearUserMessages_V1()
    {
        var json = Encoding.UTF8.GetBytes(Payloads.ChannelChatClearUserMessagesV1Json);
        var msg = new ChannelChatClearUserMessages(json.AsMemory());

        Assert.Equal(1337, msg.BroadcasterId);
        Assert.Equal("cool_user", msg.BroadcasterUsername);
        Assert.Equal("Cool_User", msg.BroadcasterDisplayName);
        Assert.Equal(7734, msg.TargetId);
        Assert.Equal("uncool_viewer", msg.TargetUsername);
        Assert.Equal("Uncool_viewer", msg.TargetDisplayName);
    }

    [Fact]
    public void ChannelChatMessageDelete_V1()
    {
        var json = Encoding.UTF8.GetBytes(Payloads.ChannelChatMessageDeleteV1Json);
        var msg = new ChannelChatMessageDelete(json.AsMemory());

        Assert.Equal(1337, msg.BroadcasterId);
        Assert.Equal("cool_user", msg.BroadcasterUsername);
        Assert.Equal("Cool_User", msg.BroadcasterDisplayName);
        Assert.Equal(7734, msg.TargetId);
        Assert.Equal("uncool_viewer", msg.TargetUsername);
        Assert.Equal("Uncool_viewer", msg.TargetDisplayName);
        Assert.Equal(Guid.Parse("ab24e0b0-2260-4bac-94e4-05eedd4ecd0e"), msg.MessageId);
    }

    [Fact]
    public void ChannelUpdate_V2()
    {
        var json = Encoding.UTF8.GetBytes(Payloads.ChannelUpdateV2Json);
        var msg = new ChannelUpdate(json.AsMemory());

        Assert.Equal(1337, msg.BroadcasterId);
        Assert.Equal("cool_user", msg.BroadcasterUsername);
        Assert.Equal("Cool_User", msg.BroadcasterDisplayName);
        Assert.Equal("Best Stream Ever", msg.Title);
        Assert.Equal("en", msg.Language);
        Assert.Equal("12453", msg.CategoryId);
        Assert.Equal("Grand Theft Auto", msg.CategoryName);
        Assert.Single(msg.ContentClassificationLabels);
        Assert.Equal("MatureGame", msg.ContentClassificationLabels[0]);
    }

    [Fact]
    public void ChannelUpdate_V2_NoLabels()
    {
        var json = Encoding.UTF8.GetBytes(Payloads.ChannelUpdateV2NoLabelsJson);
        var msg = new ChannelUpdate(json.AsMemory());

        Assert.Equal(42, msg.BroadcasterId);
        Assert.Equal("simple", msg.BroadcasterUsername);
        Assert.Equal("Simple", msg.BroadcasterDisplayName);
        Assert.Equal("Just Chatting", msg.Title);
        Assert.Equal("fr", msg.Language);
        Assert.Equal("509658", msg.CategoryId);
        Assert.Equal("Just Chatting", msg.CategoryName);
        Assert.Empty(msg.ContentClassificationLabels);
    }

    [Fact]
    public void ChannelFollow_V2()
    {
        var json = Encoding.UTF8.GetBytes(Payloads.ChannelFollowV2Json);
        var msg = new ChannelFollow(json.AsMemory());

        Assert.Equal(1234, msg.UserId);
        Assert.Equal("cool_user", msg.Username);
        Assert.Equal("Cool_User", msg.UserDisplayName);
        Assert.Equal(1337, msg.BroadcasterId);
        Assert.Equal("cooler_user", msg.BroadcasterUsername);
        Assert.Equal("Cooler_User", msg.BroadcasterDisplayName);
        Assert.Equal(DateTimeOffset.Parse("2020-07-15T18:16:11.17106713Z"), msg.FollowedAt);
    }

    [Fact]
    public void ChannelChatSettingsUpdate_V1()
    {
        var json = Encoding.UTF8.GetBytes(Payloads.ChannelChatSettingsUpdateV1Json);
        var msg = new ChannelChatSettingsUpdate(json.AsMemory());

        Assert.Equal(1337, msg.BroadcasterId);
        Assert.Equal("cool_user", msg.BroadcasterUsername);
        Assert.Equal("Cool_User", msg.BroadcasterDisplayName);
        Assert.True(msg.EmoteMode);
        Assert.False(msg.FollowerMode);
        Assert.Null(msg.FollowerModeDurationMinutes);
        Assert.True(msg.SlowMode);
        Assert.Equal(10, msg.SlowModeWaitTimeSeconds);
        Assert.False(msg.SubscriberMode);
        Assert.False(msg.UniqueChatMode);
    }

    [Fact]
    public void ChannelChatSettingsUpdate_V1_AllDisabled()
    {
        var json = Encoding.UTF8.GetBytes(Payloads.ChannelChatSettingsUpdateV1AllDisabledJson);
        var msg = new ChannelChatSettingsUpdate(json.AsMemory());

        Assert.Equal(42, msg.BroadcasterId);
        Assert.Equal("simple", msg.BroadcasterUsername);
        Assert.Equal("Simple", msg.BroadcasterDisplayName);
        Assert.False(msg.EmoteMode);
        Assert.True(msg.FollowerMode);
        Assert.Equal(30, msg.FollowerModeDurationMinutes);
        Assert.False(msg.SlowMode);
        Assert.Null(msg.SlowModeWaitTimeSeconds);
        Assert.True(msg.SubscriberMode);
        Assert.True(msg.UniqueChatMode);
    }

    [Fact]
    public void ChannelChatUserMessageHold_V1()
    {
        var json = Encoding.UTF8.GetBytes(Payloads.ChannelChatUserMessageHoldV1Json);
        var msg = new ChannelChatUserMessageHold(json.AsMemory());

        Assert.Equal(123, msg.BroadcasterId);
        Assert.Equal("bob", msg.BroadcasterUsername);
        Assert.Equal("Bob", msg.BroadcasterDisplayName);
        Assert.Equal(456, msg.UserId);
        Assert.Equal("tom", msg.Username);
        Assert.Equal("Tommy", msg.UserDisplayName);
        Assert.Equal("789", msg.MessageId);
        Assert.Equal("hey world", msg.Message.Text);

        var fragments = msg.Message.Fragments;
        Assert.Equal(3, fragments.Length);

        Assert.Equal("emote", fragments[0].Type);
        Assert.Equal("hey world", fragments[0].Text);
        Assert.Equal("foo", fragments[0].Emote.Id);

        Assert.Equal("cheermote", fragments[1].Type);
        Assert.Equal("bye world", fragments[1].Text);
        Assert.Equal("prefix", fragments[1].Cheermote.Prefix);
        Assert.Equal(100, fragments[1].Cheermote.Bits);
        Assert.Equal(1, fragments[1].Cheermote.Tier);

        Assert.Equal("text", fragments[2].Type);
        Assert.Equal("surprise", fragments[2].Text);
    }

    [Fact]
    public void ChannelChatUserMessageUpdate_V1()
    {
        var json = Encoding.UTF8.GetBytes(Payloads.ChannelChatUserMessageUpdateV1Json);
        var msg = new ChannelChatUserMessageUpdate(json.AsMemory());

        Assert.Equal(123, msg.BroadcasterId);
        Assert.Equal("bob", msg.BroadcasterUsername);
        Assert.Equal("Bob", msg.BroadcasterDisplayName);
        Assert.Equal(456, msg.UserId);
        Assert.Equal("tom", msg.Username);
        Assert.Equal("Tommy", msg.UserDisplayName);
        Assert.Equal("approved", msg.Status);
        Assert.Equal("789", msg.MessageId);
        Assert.Equal("hey world", msg.Message.Text);
        Assert.Single(msg.Message.Fragments);
        Assert.Equal("text", msg.Message.Fragments[0].Type);
    }

    [Fact]
    public void ChannelSharedChatBegin_V1()
    {
        var json = Encoding.UTF8.GetBytes(Payloads.ChannelSharedChatBeginV1Json);
        var msg = new ChannelSharedChatBegin(json.AsMemory());

        Assert.Equal("2b64a92a-dbb8-424e-b1c3-304423ba1b6f", msg.SessionId);
        Assert.Equal(1971641, msg.BroadcasterId);
        Assert.Equal("streamer", msg.BroadcasterUsername);
        Assert.Equal("streamer", msg.BroadcasterDisplayName);
        Assert.Equal(1971641, msg.HostBroadcasterId);
        Assert.Equal("streamer", msg.HostBroadcasterUsername);
        Assert.Equal("streamer", msg.HostBroadcasterDisplayName);

        var participants = msg.Participants;
        Assert.Equal(2, participants.Length);

        Assert.Equal(1971641, participants[0].BroadcasterId);
        Assert.Equal("streamer", participants[0].BroadcasterUsername);
        Assert.Equal("streamer", participants[0].BroadcasterDisplayName);

        Assert.Equal(112233, participants[1].BroadcasterId);
        Assert.Equal("streamer33", participants[1].BroadcasterUsername);
        Assert.Equal("streamer33", participants[1].BroadcasterDisplayName);
    }

    [Fact]
    public void ChannelSharedChatUpdate_V1()
    {
        var json = Encoding.UTF8.GetBytes(Payloads.ChannelSharedChatUpdateV1Json);
        var msg = new ChannelSharedChatUpdate(json.AsMemory());

        Assert.Equal("3c75a92a-dbb8-424e-b1c3-304423ba1b6f", msg.SessionId);
        Assert.Equal(1971641, msg.BroadcasterId);
        Assert.Equal("streamer", msg.BroadcasterUsername);
        Assert.Equal("streamer", msg.BroadcasterDisplayName);
        Assert.Equal(1971641, msg.HostBroadcasterId);
        Assert.Equal("streamer", msg.HostBroadcasterUsername);
        Assert.Equal("streamer", msg.HostBroadcasterDisplayName);

        var participants = msg.Participants;
        Assert.Equal(2, participants.Length);
        Assert.Equal(1971641, participants[0].BroadcasterId);
        Assert.Equal("streamer", participants[0].BroadcasterUsername);
        Assert.Equal("streamer", participants[0].BroadcasterDisplayName);
        Assert.Equal(332211, participants[1].BroadcasterId);
        Assert.Equal("streamer11", participants[1].BroadcasterUsername);
        Assert.Equal("streamer11", participants[1].BroadcasterDisplayName);
    }

    [Fact]
    public void ChannelSharedChatEnd_V1()
    {
        var json = Encoding.UTF8.GetBytes(Payloads.ChannelSharedChatEndV1Json);
        var msg = new ChannelSharedChatEnd(json.AsMemory());

        Assert.Equal("2b64a92a-dbb8-424e-b1c3-304423ba1b6f", msg.SessionId);
        Assert.Equal(1971641, msg.BroadcasterId);
        Assert.Equal("streamer", msg.BroadcasterUsername);
        Assert.Equal("streamer", msg.BroadcasterDisplayName);
        Assert.Equal(1971641, msg.HostBroadcasterId);
        Assert.Equal("streamer", msg.HostBroadcasterUsername);
        Assert.Equal("streamer", msg.HostBroadcasterDisplayName);
    }

    [Fact]
    public void ChannelSubscribe_V1()
    {
        var json = Encoding.UTF8.GetBytes(Payloads.ChannelSubscribeV1Json);
        var msg = new ChannelSubscribe(json.AsMemory());

        Assert.Equal(1234, msg.UserId);
        Assert.Equal("cool_user", msg.Username);
        Assert.Equal("Cool_User", msg.UserDisplayName);
        Assert.Equal(1337, msg.BroadcasterId);
        Assert.Equal("cooler_user", msg.BroadcasterUsername);
        Assert.Equal("Cooler_User", msg.BroadcasterDisplayName);
        Assert.Equal("1000", msg.Tier);
        Assert.False(msg.IsGift);
    }

    [Fact]
    public void ChannelSubscribe_V1_Gift()
    {
        var json = Encoding.UTF8.GetBytes(Payloads.ChannelSubscribeV1GiftJson);
        var msg = new ChannelSubscribe(json.AsMemory());

        Assert.Equal(5678, msg.UserId);
        Assert.Equal("gifter", msg.Username);
        Assert.Equal("Gifter", msg.UserDisplayName);
        Assert.Equal(42, msg.BroadcasterId);
        Assert.Equal("streamer", msg.BroadcasterUsername);
        Assert.Equal("Streamer", msg.BroadcasterDisplayName);
        Assert.Equal("3000", msg.Tier);
        Assert.True(msg.IsGift);
    }

    [Fact]
    public void ChannelSubscriptionEnd_V1()
    {
        var json = Encoding.UTF8.GetBytes(Payloads.ChannelSubscriptionEndV1Json);
        var msg = new ChannelSubscriptionEnd(json.AsMemory());

        Assert.Equal(1234, msg.UserId);
        Assert.Equal("cool_user", msg.Username);
        Assert.Equal("Cool_User", msg.UserDisplayName);
        Assert.Equal(1337, msg.BroadcasterId);
        Assert.Equal("cooler_user", msg.BroadcasterUsername);
        Assert.Equal("Cooler_User", msg.BroadcasterDisplayName);
        Assert.Equal("1000", msg.Tier);
        Assert.False(msg.IsGift);
    }

    [Fact]
    public void ChannelSubscriptionEnd_V1_Gift()
    {
        var json = Encoding.UTF8.GetBytes(Payloads.ChannelSubscriptionEndV1GiftJson);
        var msg = new ChannelSubscriptionEnd(json.AsMemory());

        Assert.Equal(5678, msg.UserId);
        Assert.Equal("gifter", msg.Username);
        Assert.Equal("Gifter", msg.UserDisplayName);
        Assert.Equal(42, msg.BroadcasterId);
        Assert.Equal("streamer", msg.BroadcasterUsername);
        Assert.Equal("Streamer", msg.BroadcasterDisplayName);
        Assert.Equal("2000", msg.Tier);
        Assert.True(msg.IsGift);
    }

    [Fact]
    public void ChannelSubscriptionGift_V1()
    {
        var json = Encoding.UTF8.GetBytes(Payloads.ChannelSubscriptionGiftV1Json);
        var msg = new ChannelSubscriptionGift(json.AsMemory());

        Assert.Equal(1234, msg.UserId);
        Assert.Equal("cool_user", msg.Username);
        Assert.Equal("Cool_User", msg.UserDisplayName);
        Assert.Equal(1337, msg.BroadcasterId);
        Assert.Equal("cooler_user", msg.BroadcasterUsername);
        Assert.Equal("Cooler_User", msg.BroadcasterDisplayName);
        Assert.Equal(2, msg.Total);
        Assert.Equal("1000", msg.Tier);
        Assert.Equal(284, msg.CumulativeTotal);
        Assert.False(msg.IsAnonymous);
    }

    [Fact]
    public void ChannelSubscriptionGift_V1_Anonymous()
    {
        var json = Encoding.UTF8.GetBytes(Payloads.ChannelSubscriptionGiftV1AnonymousJson);
        var msg = new ChannelSubscriptionGift(json.AsMemory());

        Assert.Null(msg.UserId);
        Assert.Null(msg.Username);
        Assert.Null(msg.UserDisplayName);
        Assert.Equal(42, msg.BroadcasterId);
        Assert.Equal("streamer", msg.BroadcasterUsername);
        Assert.Equal("Streamer", msg.BroadcasterDisplayName);
        Assert.Equal(5, msg.Total);
        Assert.Equal("3000", msg.Tier);
        Assert.Null(msg.CumulativeTotal);
        Assert.True(msg.IsAnonymous);
    }

    [Fact]
    public void ChannelSubscriptionMessage_V1()
    {
        var json = Encoding.UTF8.GetBytes(Payloads.ChannelSubscriptionMessageV1Json);
        var msg = new ChannelSubscriptionMessage(json.AsMemory());

        Assert.Equal(1234, msg.UserId);
        Assert.Equal("cool_user", msg.Username);
        Assert.Equal("Cool_User", msg.UserDisplayName);
        Assert.Equal(1337, msg.BroadcasterId);
        Assert.Equal("cooler_user", msg.BroadcasterUsername);
        Assert.Equal("Cooler_User", msg.BroadcasterDisplayName);
        Assert.Equal("1000", msg.Tier);
        Assert.Equal("Love the stream!", msg.Message.Text);
        Assert.Single(msg.Message.Emotes);
        Assert.Equal(23, msg.Message.Emotes[0].Begin);
        Assert.Equal(30, msg.Message.Emotes[0].End);
        Assert.Equal("302976485", msg.Message.Emotes[0].Id);
        Assert.Equal(15, msg.CumulativeMonths);
        Assert.Equal(1, msg.StreakMonths);
        Assert.Equal(6, msg.DurationMonths);
    }

    [Fact]
    public void ChannelSubscriptionMessage_V1_NoStreak()
    {
        var json = Encoding.UTF8.GetBytes(Payloads.ChannelSubscriptionMessageV1NoStreakJson);
        var msg = new ChannelSubscriptionMessage(json.AsMemory());

        Assert.Equal(5678, msg.UserId);
        Assert.Equal("subscriber", msg.Username);
        Assert.Equal("Subscriber", msg.UserDisplayName);
        Assert.Equal(42, msg.BroadcasterId);
        Assert.Equal("streamer", msg.BroadcasterUsername);
        Assert.Equal("Streamer", msg.BroadcasterDisplayName);
        Assert.Equal("2000", msg.Tier);
        Assert.Equal("Great streams!", msg.Message.Text);
        Assert.Empty(msg.Message.Emotes);
        Assert.Equal(3, msg.CumulativeMonths);
        Assert.Null(msg.StreakMonths);
        Assert.Equal(1, msg.DurationMonths);
    }

    [Fact]
    public void ChannelCheer_V1()
    {
        var json = Encoding.UTF8.GetBytes(Payloads.ChannelCheerV1Json);
        var msg = new ChannelCheer(json.AsMemory());

        Assert.False(msg.IsAnonymous);
        Assert.Equal(1234, msg.UserId);
        Assert.Equal("cool_user", msg.Username);
        Assert.Equal("Cool_User", msg.UserDisplayName);
        Assert.Equal(1337, msg.BroadcasterId);
        Assert.Equal("cooler_user", msg.BroadcasterUsername);
        Assert.Equal("Cooler_User", msg.BroadcasterDisplayName);
        Assert.Equal("pogchamp", msg.Message);
        Assert.Equal(1000, msg.Bits);
    }

    [Fact]
    public void ChannelCheer_V1_Anonymous()
    {
        var json = Encoding.UTF8.GetBytes(Payloads.ChannelCheerV1AnonymousJson);
        var msg = new ChannelCheer(json.AsMemory());

        Assert.True(msg.IsAnonymous);
        Assert.Null(msg.UserId);
        Assert.Null(msg.Username);
        Assert.Null(msg.UserDisplayName);
        Assert.Equal(42, msg.BroadcasterId);
        Assert.Equal("streamer", msg.BroadcasterUsername);
        Assert.Equal("Streamer", msg.BroadcasterDisplayName);
        Assert.Equal("AnonCheer", msg.Message);
        Assert.Equal(500, msg.Bits);
    }

    [Fact]
    public void ChannelRaid_V1()
    {
        var json = Encoding.UTF8.GetBytes(Payloads.ChannelRaidV1Json);
        var msg = new ChannelRaid(json.AsMemory());

        Assert.Equal(1234, msg.FromBroadcasterId);
        Assert.Equal("cool_user", msg.FromBroadcasterUsername);
        Assert.Equal("Cool_User", msg.FromBroadcasterDisplayName);
        Assert.Equal(1337, msg.ToBroadcasterId);
        Assert.Equal("cooler_user", msg.ToBroadcasterUsername);
        Assert.Equal("Cooler_User", msg.ToBroadcasterDisplayName);
        Assert.Equal(9001, msg.Viewers);
    }

    [Fact]
    public void ChannelUnban_V1()
    {
        var json = Encoding.UTF8.GetBytes(Payloads.ChannelUnbanV1Json);
        var msg = new ChannelUnban(json.AsMemory());

        Assert.Equal(1234, msg.UserId);
        Assert.Equal("cool_user", msg.Username);
        Assert.Equal("Cool_User", msg.UserDisplayName);
        Assert.Equal(1337, msg.BroadcasterId);
        Assert.Equal("cooler_user", msg.BroadcasterUsername);
        Assert.Equal("Cooler_User", msg.BroadcasterDisplayName);
        Assert.Equal(1339, msg.ModeratorId);
        Assert.Equal("mod_user", msg.ModeratorUsername);
        Assert.Equal("Mod_User", msg.ModeratorDisplayName);
    }

    [Fact]
    public void ChannelUnbanRequestCreate_V1()
    {
        var json = Encoding.UTF8.GetBytes(Payloads.ChannelUnbanRequestCreateV1Json);
        var msg = new ChannelUnbanRequestCreate(json.AsMemory());

        Assert.Equal("60", msg.Id);
        Assert.Equal(1337, msg.BroadcasterId);
        Assert.Equal("cool_user", msg.BroadcasterUsername);
        Assert.Equal("Cool_User", msg.BroadcasterDisplayName);
        Assert.Equal(1339, msg.UserId);
        Assert.Equal("not_cool_user", msg.Username);
        Assert.Equal("Not_Cool_User", msg.UserDisplayName);
        Assert.Equal("unban me", msg.Text);
        Assert.Equal(DateTimeOffset.Parse("2023-11-16T10:11:12.634234626Z"), msg.CreatedAt);
    }

    [Fact]
    public void ChannelUnbanRequestResolve_V1()
    {
        var json = Encoding.UTF8.GetBytes(Payloads.ChannelUnbanRequestResolveV1Json);
        var msg = new ChannelUnbanRequestResolve(json.AsMemory());

        Assert.Equal("60", msg.Id);
        Assert.Equal(1337, msg.BroadcasterId);
        Assert.Equal("cool_user", msg.BroadcasterUsername);
        Assert.Equal("Cool_User", msg.BroadcasterDisplayName);
        Assert.Equal(1337, msg.ModeratorId);
        Assert.Equal("cool_user", msg.ModeratorUsername);
        Assert.Equal("Cool_User", msg.ModeratorDisplayName);
        Assert.Equal(1339, msg.UserId);
        Assert.Equal("not_cool_user", msg.Username);
        Assert.Equal("Not_Cool_User", msg.UserDisplayName);
        Assert.Equal("no", msg.ResolutionText);
        Assert.Equal("denied", msg.Status);
    }

    [Fact]
    public void ChannelModerate_V1_Mod()
    {
        var json = Encoding.UTF8.GetBytes(Payloads.ChannelModerateV1ModJson);
        var msg = new ChannelModerate(json.AsMemory());

        Assert.Equal(1337, msg.BroadcasterId);
        Assert.Equal("glowillig", msg.BroadcasterUsername);
        Assert.Equal("glowillig", msg.BroadcasterDisplayName);
        Assert.Equal(424596340, msg.ModeratorId);
        Assert.Equal("quotrok", msg.ModeratorUsername);
        Assert.Equal("quotrok", msg.ModeratorDisplayName);
        Assert.Equal("mod", msg.Action);

        var mod = msg.Mod;
        Assert.NotNull(mod);
        Assert.Equal(141981764, mod.Value.UserId);
        Assert.Equal("twitchdev", mod.Value.Username);
        Assert.Equal("TwitchDev", mod.Value.UserDisplayName);
    }

    [Fact]
    public void ChannelModerate_V1_SharedChatTimeout()
    {
        var json = Encoding.UTF8.GetBytes(Payloads.ChannelModerateV1SharedChatTimeoutJson);
        var msg = new ChannelModerate(json.AsMemory());

        Assert.Equal(423374343, msg.BroadcasterId);
        Assert.Equal(41292030, msg.SourceBroadcasterId);
        Assert.Equal("adflynn404", msg.SourceBroadcasterUsername);
        Assert.Equal("adflynn404", msg.SourceBroadcasterDisplayName);
        Assert.Equal(424596340, msg.ModeratorId);
        Assert.Equal("shared_chat_timeout", msg.Action);

        var timeout = msg.SharedChatTimeout;
        Assert.NotNull(timeout);
        Assert.Equal(141981764, timeout.Value.UserId);
        Assert.Equal("twitchdev", timeout.Value.Username);
        Assert.Equal("TwitchDev", timeout.Value.UserDisplayName);
        Assert.Equal("Does not like pineapple on pizza.", timeout.Value.Reason);
        Assert.Equal(DateTimeOffset.Parse("2022-03-15T02:00:28Z"), timeout.Value.ExpiresAt);
    }

    [Fact]
    public void ChannelModerate_V1_EmoteOnly()
    {
        var json = Encoding.UTF8.GetBytes(Payloads.ChannelModerateV1EmoteOnlyJson);
        var msg = new ChannelModerate(json.AsMemory());

        Assert.Equal(1337, msg.BroadcasterId);
        Assert.Equal("glowillig", msg.BroadcasterUsername);
        Assert.Equal("glowillig", msg.BroadcasterDisplayName);
        Assert.Equal(424596340, msg.ModeratorId);
        Assert.Equal("emoteonly", msg.Action);
    }

    [Fact]
    public void ChannelModerateV2_Warn()
    {
        var json = Encoding.UTF8.GetBytes(Payloads.ChannelModerateV2WarnJson);
        var msg = new ChannelModerateV2(json.AsMemory());

        Assert.Equal(423374343, msg.BroadcasterId);
        Assert.Equal(41292030, msg.SourceBroadcasterId);
        Assert.Equal("warn", msg.Action);

        var warn = msg.Warn;
        Assert.NotNull(warn);
        Assert.Equal(141981764, warn.Value.UserId);
        Assert.Equal("twitchdev", warn.Value.Username);
        Assert.Equal("TwitchDev", warn.Value.UserDisplayName);
        Assert.Equal("cut it out", warn.Value.Reason);
    }

    [Fact]
    public void ChannelModerateV2_Mod()
    {
        var json = Encoding.UTF8.GetBytes(Payloads.ChannelModerateV2ModJson);
        var msg = new ChannelModerateV2(json.AsMemory());

        Assert.Equal(423374343, msg.BroadcasterId);
        Assert.Equal("mod", msg.Action);

        var mod = msg.Mod;
        Assert.NotNull(mod);
        Assert.Equal(141981764, mod.Value.UserId);
        Assert.Equal("twitchdev", mod.Value.Username);
        Assert.Equal("TwitchDev", mod.Value.UserDisplayName);
    }

    [Fact]
    public void ChannelModeratorAdd_V1()
    {
        var json = Encoding.UTF8.GetBytes(Payloads.ChannelModeratorAddV1Json);
        var msg = new ChannelModeratorAdd(json.AsMemory());

        Assert.Equal(1337, msg.BroadcasterId);
        Assert.Equal("cooler_user", msg.BroadcasterUsername);
        Assert.Equal("Cooler_User", msg.BroadcasterDisplayName);
        Assert.Equal(1234, msg.UserId);
        Assert.Equal("mod_user", msg.Username);
        Assert.Equal("Mod_User", msg.UserDisplayName);
    }

    [Fact]
    public void ChannelModeratorRemove_V1()
    {
        var json = Encoding.UTF8.GetBytes(Payloads.ChannelModeratorRemoveV1Json);
        var msg = new ChannelModeratorRemove(json.AsMemory());

        Assert.Equal(1337, msg.BroadcasterId);
        Assert.Equal("cooler_user", msg.BroadcasterUsername);
        Assert.Equal("Cooler_User", msg.BroadcasterDisplayName);
        Assert.Equal(5678, msg.UserId);
        Assert.Equal("old_mod", msg.Username);
        Assert.Equal("Old_Mod", msg.UserDisplayName);
    }

    [Fact]
    public void ChannelPointsAutomaticRewardRedemptionAdd_V1()
    {
        var json = Encoding.UTF8.GetBytes(Payloads.ChannelPointsAutomaticRewardRedemptionAddV1Json);
        var msg = new ChannelPointsAutomaticRewardRedemptionAdd(json.AsMemory());

        Assert.Equal(12826, msg.BroadcasterId);
        Assert.Equal("twitch", msg.BroadcasterUsername);
        Assert.Equal("Twitch", msg.BroadcasterDisplayName);
        Assert.Equal(141981764, msg.UserId);
        Assert.Equal("twitchdev", msg.Username);
        Assert.Equal("TwitchDev", msg.UserDisplayName);
        Assert.Equal("f024099a-e0fe-4339-9a0a-a706fb59f353", msg.Id);

        var reward = msg.Reward;
        Assert.Equal("send_highlighted_message", reward.Type);
        Assert.Equal(100, reward.Cost);

        var message = msg.Message;
        Assert.Equal("Hello world!", message.Text);
        Assert.Single(message.Emotes);
        Assert.Equal("81274", message.Emotes[0].Id);
        Assert.Equal(13, message.Emotes[0].Begin);
        Assert.Equal(18, message.Emotes[0].End);

        Assert.Equal("Hello world!", msg.UserInput);
        Assert.Equal(DateTimeOffset.Parse("2024-02-23T21:14:34.260398045Z"), msg.RedeemedAt);
    }

    [Fact]
    public void ChannelPointsAutomaticRewardRedemptionAddV2_Basic()
    {
        var json = Encoding.UTF8.GetBytes(Payloads.ChannelPointsAutomaticRewardRedemptionAddV2Json);
        var msg = new ChannelPointsAutomaticRewardRedemptionAddV2(json.AsMemory());

        Assert.Equal(12826, msg.BroadcasterId);
        Assert.Equal("twitch", msg.BroadcasterUsername);
        Assert.Equal("Twitch", msg.BroadcasterDisplayName);
        Assert.Equal(141981764, msg.UserId);
        Assert.Equal("twitchdev", msg.Username);
        Assert.Equal("TwitchDev", msg.UserDisplayName);
        Assert.Equal("f024099a-e0fe-4339-9a0a-a706fb59f353", msg.Id);

        var reward = msg.Reward;
        Assert.Equal("send_highlighted_message", reward.Type);
        Assert.Equal(100, reward.ChannelPoints);

        var message = msg.Message;
        Assert.Equal("Hello world! VoHiYo", message.Text);
        Assert.Equal(2, message.Fragments.Length);
        Assert.Equal("text", message.Fragments[0].Type);
        Assert.Equal("Hello world! ", message.Fragments[0].Text);
        Assert.Equal("emote", message.Fragments[1].Type);
        Assert.Equal("VoHiYo", message.Fragments[1].Text);
        Assert.Equal("81274", message.Fragments[1].Emote.Value.Id);

        Assert.Equal(DateTimeOffset.Parse("2024-08-12T21:14:34.260398045Z"), msg.RedeemedAt);
    }

    [Fact]
    public void ChannelPointsCustomRewardAdd_V1()
    {
        var json = Encoding.UTF8.GetBytes(Payloads.ChannelPointsCustomRewardAddV1Json);
        var msg = new ChannelPointsCustomRewardAdd(json.AsMemory());

        Assert.Equal("9001", msg.Id);
        Assert.Equal(1337, msg.BroadcasterId);
        Assert.Equal("cool_user", msg.BroadcasterUsername);
        Assert.Equal("Cool_User", msg.BroadcasterDisplayName);
        Assert.True(msg.IsEnabled);
        Assert.False(msg.IsPaused);
        Assert.True(msg.IsInStock);
        Assert.Equal("Cool Reward", msg.Title);
        Assert.Equal(100, msg.Cost);
        Assert.Equal("reward prompt", msg.Prompt);
        Assert.True(msg.IsUserInputRequired);
        Assert.False(msg.ShouldRedemptionsSkipRequestQueue);
        Assert.Equal("#FA1ED2", msg.BackgroundColor);

        var maxPerStream = msg.MaxPerStream;
        Assert.NotNull(maxPerStream);
        Assert.True(maxPerStream.Value.IsEnabled);
        Assert.Equal(1000, maxPerStream.Value.Value);

        var maxPerUser = msg.MaxPerUserPerStream;
        Assert.NotNull(maxPerUser);
        Assert.True(maxPerUser.Value.IsEnabled);
        Assert.Equal(1000, maxPerUser.Value.Value);

        var image = msg.Image;
        Assert.NotNull(image);
        Assert.Equal("https://static-cdn.jtvnw.net/image-1.png", image.Value.Url1x);
        Assert.Equal("https://static-cdn.jtvnw.net/image-2.png", image.Value.Url2x);
        Assert.Equal("https://static-cdn.jtvnw.net/image-4.png", image.Value.Url4x);

        var defaultImage = msg.DefaultImage;
        Assert.Equal("https://static-cdn.jtvnw.net/default-1.png", defaultImage.Url1x);
        Assert.Equal("https://static-cdn.jtvnw.net/default-2.png", defaultImage.Url2x);
        Assert.Equal("https://static-cdn.jtvnw.net/default-4.png", defaultImage.Url4x);

        var cooldown = msg.GlobalCooldown;
        Assert.NotNull(cooldown);
        Assert.True(cooldown.Value.IsEnabled);
        Assert.Equal(1000, cooldown.Value.Seconds);
    }

    [Fact]
    public void ChannelPointsCustomRewardUpdate_V1()
    {
        var json = Encoding.UTF8.GetBytes(Payloads.ChannelPointsCustomRewardUpdateV1Json);
        var msg = new ChannelPointsCustomRewardUpdate(json.AsMemory());

        Assert.Equal("9002", msg.Id);
        Assert.Equal(1337, msg.BroadcasterId);
        Assert.Equal("cool_user", msg.BroadcasterUsername);
        Assert.Equal("Cool_User", msg.BroadcasterDisplayName);
        Assert.False(msg.IsEnabled);
        Assert.True(msg.IsPaused);
        Assert.True(msg.IsInStock);
        Assert.Equal("Updated Reward", msg.Title);
        Assert.Equal(200, msg.Cost);
        Assert.Equal("updated prompt", msg.Prompt);
        Assert.False(msg.IsUserInputRequired);
        Assert.True(msg.ShouldRedemptionsSkipRequestQueue);
        Assert.Equal("#000000", msg.BackgroundColor);

        var defaultImage = msg.DefaultImage;
        Assert.Equal("https://default-1.png", defaultImage.Url1x);
        Assert.Equal("https://default-2.png", defaultImage.Url2x);
        Assert.Equal("https://default-4.png", defaultImage.Url4x);
    }

    [Fact]
    public void ChannelPointsCustomRewardRemove_V1()
    {
        var json = Encoding.UTF8.GetBytes(Payloads.ChannelPointsCustomRewardRemoveV1Json);
        var msg = new ChannelPointsCustomRewardRemove(json.AsMemory());

        Assert.Equal("9003", msg.Id);
        Assert.Equal(42, msg.BroadcasterId);
        Assert.Equal("simple", msg.BroadcasterUsername);
        Assert.Equal("Simple", msg.BroadcasterDisplayName);
        Assert.Equal("Removed Reward", msg.Title);
        Assert.Equal(50, msg.Cost);
    }

    [Fact]
    public void ChannelPointsCustomRewardRedemptionAdd_V1()
    {
        var json = Encoding.UTF8.GetBytes(Payloads.ChannelPointsCustomRewardRedemptionAddV1Json);
        var msg = new ChannelPointsCustomRewardRedemptionAdd(json.AsMemory());

        Assert.Equal("17fa2df1-ad76-4804-bfa5-a40ef63efe63", msg.Id);
        Assert.Equal(1337, msg.BroadcasterId);
        Assert.Equal("cool_user", msg.BroadcasterUsername);
        Assert.Equal("Cool_User", msg.BroadcasterDisplayName);
        Assert.Equal(9001, msg.UserId);
        Assert.Equal("cooler_user", msg.Username);
        Assert.Equal("Cooler_User", msg.UserDisplayName);
        Assert.Equal("pogchamp", msg.UserInput);
        Assert.Equal("unfulfilled", msg.Status);

        var reward = msg.Reward;
        Assert.Equal("92af127c-7326-4483-a52b-b0da0be61c01", reward.Id);
        Assert.Equal("title", reward.Title);
        Assert.Equal(100, reward.Cost);
        Assert.Equal("reward prompt", reward.Prompt);

        Assert.Equal(DateTimeOffset.Parse("2020-07-15T17:16:03.17106713Z"), msg.RedeemedAt);
    }

    [Fact]
    public void ChannelPointsCustomRewardRedemptionUpdate_V1()
    {
        var json = Encoding.UTF8.GetBytes(Payloads.ChannelPointsCustomRewardRedemptionUpdateV1Json);
        var msg = new ChannelPointsCustomRewardRedemptionUpdate(json.AsMemory());

        Assert.Equal("17fa2df1-ad76-4804-bfa5-a40ef63efe63", msg.Id);
        Assert.Equal(1337, msg.BroadcasterId);
        Assert.Equal("cool_user", msg.BroadcasterUsername);
        Assert.Equal("Cool_User", msg.BroadcasterDisplayName);
        Assert.Equal(9001, msg.UserId);
        Assert.Equal("cooler_user", msg.Username);
        Assert.Equal("Cooler_User", msg.UserDisplayName);
        Assert.Equal("", msg.UserInput);
        Assert.Equal("fulfilled", msg.Status);
    }

    [Fact]
    public void ChannelCustomPowerUpRedemptionAdd_V1()
    {
        var json = Encoding.UTF8.GetBytes(Payloads.ChannelCustomPowerUpRedemptionAddV1Json);
        var msg = new ChannelCustomPowerUpRedemptionAdd(json.AsMemory());

        Assert.Equal("17fa2df1-ad76-4804-bfa5-a40ef63efe63", msg.Id);
        Assert.Equal(1337, msg.BroadcasterId);
        Assert.Equal("cool_user", msg.BroadcasterUsername);
        Assert.Equal("Cool_User", msg.BroadcasterDisplayName);
        Assert.Equal(9001, msg.UserId);
        Assert.Equal("cooler_user", msg.Username);
        Assert.Equal("Cooler_User", msg.UserDisplayName);
        Assert.Equal("pogchamp", msg.UserInput);
        Assert.Equal("unfulfilled", msg.Status);

        var pu = msg.CustomPowerUp;
        Assert.Equal("92af127c-7326-4483-a52b-b0da0be61c01", pu.Id);
        Assert.Equal("title", pu.Title);
        Assert.Equal(100, pu.Bits);
        Assert.Equal("Power-up prompt", pu.Prompt);

        Assert.Equal(DateTimeOffset.Parse("2026-05-01T17:16:03.17106713Z"), msg.RedeemedAt);
    }

    [Fact]
    public void ChannelPollBegin_V1()
    {
        var json = Encoding.UTF8.GetBytes(Payloads.ChannelPollBeginV1Json);
        var msg = new ChannelPollBegin(json.AsMemory());

        Assert.Equal("1243456", msg.Id);
        Assert.Equal(1337, msg.BroadcasterId);
        Assert.Equal("cool_user", msg.BroadcasterUsername);
        Assert.Equal("Cool_User", msg.BroadcasterDisplayName);
        Assert.Equal("Aren't shoes just really hard socks?", msg.Title);

        Assert.Equal(3, msg.Choices.Length);
        Assert.Equal("123", msg.Choices[0].Id);
        Assert.Equal("Yeah!", msg.Choices[0].Title);
        Assert.Equal("124", msg.Choices[1].Id);
        Assert.Equal("No!", msg.Choices[1].Title);
        Assert.Equal("125", msg.Choices[2].Id);
        Assert.Equal("Maybe!", msg.Choices[2].Title);

        Assert.True(msg.BitsVoting.IsEnabled);
        Assert.Equal(10, msg.BitsVoting.AmountPerVote);

        Assert.True(msg.ChannelPointsVoting.IsEnabled);
        Assert.Equal(10, msg.ChannelPointsVoting.AmountPerVote);

        Assert.Equal(DateTimeOffset.Parse("2020-07-15T17:16:03.17106713Z"), msg.StartedAt);
        Assert.Equal(DateTimeOffset.Parse("2020-07-15T17:16:08.17106713Z"), msg.EndsAt);
    }

    [Fact]
    public void ChannelPollProgress_V1()
    {
        var json = Encoding.UTF8.GetBytes(Payloads.ChannelPollProgressV1Json);
        var msg = new ChannelPollProgress(json.AsMemory());

        Assert.Equal("1243456", msg.Id);
        Assert.Equal(1337, msg.BroadcasterId);
        Assert.Equal("Aren't shoes just really hard socks?", msg.Title);

        Assert.Equal(3, msg.Choices.Length);
        Assert.Equal("123", msg.Choices[0].Id);
        Assert.Equal("Yeah!", msg.Choices[0].Title);
        Assert.Equal(5, msg.Choices[0].BitsVotes);
        Assert.Equal(7, msg.Choices[0].ChannelPointsVotes);
        Assert.Equal(12, msg.Choices[0].Votes);

        Assert.Equal(10, msg.Choices[1].BitsVotes);
        Assert.Equal(4, msg.Choices[1].ChannelPointsVotes);
        Assert.Equal(14, msg.Choices[1].Votes);

        Assert.Equal(0, msg.Choices[2].BitsVotes);
        Assert.Equal(7, msg.Choices[2].ChannelPointsVotes);
        Assert.Equal(7, msg.Choices[2].Votes);
    }

    [Fact]
    public void ChannelPollEnd_V1()
    {
        var json = Encoding.UTF8.GetBytes(Payloads.ChannelPollEndV1Json);
        var msg = new ChannelPollEnd(json.AsMemory());

        Assert.Equal("1243456", msg.Id);
        Assert.Equal(1337, msg.BroadcasterId);
        Assert.Equal("Cool_User", msg.BroadcasterDisplayName);
        Assert.Equal("Aren't shoes just really hard socks?", msg.Title);
        Assert.Equal("completed", msg.Status);

        Assert.Equal(2, msg.Choices.Length);
        Assert.Equal(12, msg.Choices[0].Votes);
        Assert.Equal(14, msg.Choices[1].Votes);

        Assert.Equal(DateTimeOffset.Parse("2020-07-15T17:16:03.17106713Z"), msg.StartedAt);
        Assert.Equal(DateTimeOffset.Parse("2020-07-15T17:16:08.17106713Z"), msg.EndedAt);
    }

    [Fact]
    public void ChannelPredictionBegin_V1()
    {
        var json = Encoding.UTF8.GetBytes(Payloads.ChannelPredictionBeginV1Json);
        var msg = new ChannelPredictionBegin(json.AsMemory());

        Assert.Equal("1243456", msg.Id);
        Assert.Equal(1337, msg.BroadcasterId);
        Assert.Equal("cool_user", msg.BroadcasterUsername);
        Assert.Equal("Cool_User", msg.BroadcasterDisplayName);
        Assert.Equal("Aren't shoes just really hard socks?", msg.Title);

        Assert.Equal(2, msg.Outcomes.Length);
        Assert.Equal("1243456", msg.Outcomes[0].Id);
        Assert.Equal("Yeah!", msg.Outcomes[0].Title);
        Assert.Equal("blue", msg.Outcomes[0].Color);
        Assert.Equal("2243456", msg.Outcomes[1].Id);
        Assert.Equal("No!", msg.Outcomes[1].Title);
        Assert.Equal("pink", msg.Outcomes[1].Color);

        Assert.Equal(DateTimeOffset.Parse("2020-07-15T17:16:03.17106713Z"), msg.StartedAt);
        Assert.Equal(DateTimeOffset.Parse("2020-07-15T17:21:03.17106713Z"), msg.LocksAt);
    }

    [Fact]
    public void ChannelPredictionProgress_V1()
    {
        var json = Encoding.UTF8.GetBytes(Payloads.ChannelPredictionProgressV1Json);
        var msg = new ChannelPredictionProgress(json.AsMemory());

        Assert.Equal("1243456", msg.Id);
        Assert.Equal(1337, msg.BroadcasterId);
        Assert.Equal("Aren't shoes just really hard socks?", msg.Title);

        var outcomes = msg.Outcomes;
        Assert.Equal(2, outcomes.Length);

        Assert.Equal("1243456", outcomes[0].Id);
        Assert.Equal("Yeah!", outcomes[0].Title);
        Assert.Equal("blue", outcomes[0].Color);
        Assert.Equal(10, outcomes[0].Users);
        Assert.Equal(15000, outcomes[0].ChannelPoints);

        var predictors = outcomes[0].TopPredictors;
        Assert.Equal(2, predictors.Length);
        Assert.Equal(1234, predictors[0].UserId);
        Assert.Equal("cool_user", predictors[0].Username);
        Assert.Equal("Cool_User", predictors[0].UserDisplayName);
        Assert.Equal(500, predictors[0].ChannelPointsUsed);

        Assert.Equal(1236, predictors[1].UserId);
        Assert.Equal(200, predictors[1].ChannelPointsUsed);

        Assert.Equal(5, outcomes[1].Users);
        Assert.Equal(5000, outcomes[1].ChannelPoints);
        Assert.Single(outcomes[1].TopPredictors);
        Assert.Equal(12345, outcomes[1].TopPredictors[0].UserId);
        Assert.Equal(5000, outcomes[1].TopPredictors[0].ChannelPointsUsed);
    }

    [Fact]
    public void ChannelPredictionLock_V1()
    {
        var json = Encoding.UTF8.GetBytes(Payloads.ChannelPredictionLockV1Json);
        var msg = new ChannelPredictionLock(json.AsMemory());

        Assert.Equal("1243456", msg.Id);
        Assert.Equal(1337, msg.BroadcasterId);
        Assert.Equal("Aren't shoes just really hard socks?", msg.Title);

        Assert.Single(msg.Outcomes);
        Assert.Equal(10, msg.Outcomes[0].Users);
        Assert.Equal(15000, msg.Outcomes[0].ChannelPoints);
        Assert.Single(msg.Outcomes[0].TopPredictors);
        Assert.Equal(1234, msg.Outcomes[0].TopPredictors[0].UserId);
    }

    [Fact]
    public void ChannelPredictionEnd_V1()
    {
        var json = Encoding.UTF8.GetBytes(Payloads.ChannelPredictionEndV1Json);
        var msg = new ChannelPredictionEnd(json.AsMemory());

        Assert.Equal("1243456", msg.Id);
        Assert.Equal(1337, msg.BroadcasterId);
        Assert.Equal("1243456", msg.WinningOutcomeId);

        Assert.Equal(2, msg.Outcomes.Length);
        Assert.Equal(15000, msg.Outcomes[0].ChannelPoints);
        Assert.Equal(500, msg.Outcomes[0].TopPredictors[0].ChannelPointsWon);
        Assert.Empty(msg.Outcomes[1].TopPredictors);

        Assert.Equal(DateTimeOffset.Parse("2020-07-15T17:26:03.17106713Z"), msg.EndedAt);
    }

    [Fact]
    public void ChannelBitsUse_V1_Cheer()
    {
        var json = Encoding.UTF8.GetBytes(Payloads.ChannelBitsUseV1CheerJson);
        var msg = new ChannelBitsUse(json.AsMemory());

        Assert.Equal(1337, msg.BroadcasterId);
        Assert.Equal("cooler_user", msg.BroadcasterUsername);
        Assert.Equal("Cooler_User", msg.BroadcasterDisplayName);
        Assert.Equal(1234, msg.UserId);
        Assert.Equal("cool_user", msg.Username);
        Assert.Equal("Cool_User", msg.UserDisplayName);
        Assert.Equal(100, msg.Bits);
        Assert.Equal("cheer", msg.Type);

        var m = msg.Message;
        Assert.NotNull(m);
        Assert.Equal("Cheer100 Hello!", m.Value.Text);
        Assert.Equal(2, m.Value.Fragments.Length);
        Assert.Equal("cheermote", m.Value.Fragments[0].Type);
        Assert.Equal("Cheer100", m.Value.Fragments[0].Text);
        Assert.Equal("Cheer", m.Value.Fragments[0].Cheermote.Prefix);
        Assert.Equal(100, m.Value.Fragments[0].Cheermote.Bits);
        Assert.Equal(1, m.Value.Fragments[0].Cheermote.Tier);
        Assert.Equal("text", m.Value.Fragments[1].Type);
        Assert.Equal(" Hello!", m.Value.Fragments[1].Text);
    }

    [Fact]
    public void ChannelBitsUse_V1_PowerUp()
    {
        var json = Encoding.UTF8.GetBytes(Payloads.ChannelBitsUseV1PowerUpJson);
        var msg = new ChannelBitsUse(json.AsMemory());

        Assert.Equal(42, msg.BroadcasterId);
        Assert.Equal(99, msg.UserId);
        Assert.Equal("poweruser", msg.Username);
        Assert.Equal("PowerUser", msg.UserDisplayName);
        Assert.Equal(0, msg.Bits);
        Assert.Equal("power_up", msg.Type);

        var pu = msg.PowerUp;
        Assert.NotNull(pu);
        Assert.Equal("message_effect", pu.Value.Type);
        Assert.Equal("effect_123", pu.Value.MessageEffectId);
    }

    [Fact]
    public void ChannelBitsUse_V1_CustomPowerUp()
    {
        var json = Encoding.UTF8.GetBytes(Payloads.ChannelBitsUseV1CustomPowerUpJson);
        var msg = new ChannelBitsUse(json.AsMemory());

        Assert.Equal(42, msg.BroadcasterId);
        Assert.Equal(55, msg.UserId);
        Assert.Equal("customuser", msg.Username);
        Assert.Equal("CustomUser", msg.UserDisplayName);
        Assert.Equal(500, msg.Bits);
        Assert.Equal("custom_power_up", msg.Type);

        var cpu = msg.CustomPowerUp;
        Assert.NotNull(cpu);
        Assert.Equal("Super Reward", cpu.Value.Title);
        Assert.Equal("reward-001", cpu.Value.RewardId);
    }

    [Fact]
    public void ChannelSuspiciousUserMessage_V1()
    {
        var json = Encoding.UTF8.GetBytes(Payloads.ChannelSuspiciousUserMessageV1Json);
        var msg = new ChannelSuspiciousUserMessage(json.AsMemory());

        Assert.Equal(1050263432, msg.BroadcasterId);
        Assert.Equal("dcf9dd9336034d23b65", msg.BroadcasterUsername);
        Assert.Equal("dcf9dd9336034d23b65", msg.BroadcasterDisplayName);
        Assert.Equal(1050263434, msg.UserId);
        Assert.Equal("4a46e2cf2e2f4d6a9e6", msg.Username);
        Assert.Equal("4a46e2cf2e2f4d6a9e6", msg.UserDisplayName);
        Assert.Equal("active_monitoring", msg.LowTrustStatus);
        Assert.Equal([100L, 200L], msg.SharedBanChannelIds);
        Assert.Equal(["ban_evader"], msg.Types);
        Assert.Equal("likely", msg.BanEvasionEvaluation);

        var m = msg.Message;
        Assert.Equal("101010", m.MessageId);
        Assert.Equal("bad stuff pogchamp", m.Text);
        Assert.Equal(2, m.Fragments.Length);

        Assert.Equal("text", m.Fragments[0].Type);
        Assert.Equal("bad stuff", m.Fragments[0].Text);

        Assert.Equal("cheermote", m.Fragments[1].Type);
        Assert.Equal("pogchamp", m.Fragments[1].Text);
        Assert.Equal("pogchamp", m.Fragments[1].Cheermote.Prefix);
        Assert.Equal(100, m.Fragments[1].Cheermote.Bits);
        Assert.Equal(1, m.Fragments[1].Cheermote.Tier);
    }

    [Fact]
    public void ChannelSuspiciousUserMessage_V1_Restricted()
    {
        var json = Encoding.UTF8.GetBytes(Payloads.ChannelSuspiciousUserMessageV1RestrictedJson);
        var msg = new ChannelSuspiciousUserMessage(json.AsMemory());

        Assert.Equal(1050263432, msg.BroadcasterId);
        Assert.Equal("streamer", msg.BroadcasterUsername);
        Assert.Equal("Streamer", msg.BroadcasterDisplayName);
        Assert.Equal(1050263434, msg.UserId);
        Assert.Equal("suspicioususer", msg.Username);
        Assert.Equal("SuspiciousUser", msg.UserDisplayName);
        Assert.Equal("restricted", msg.LowTrustStatus);
        Assert.Empty(msg.SharedBanChannelIds);
        Assert.Equal(["manually_added", "banned_in_shared_channel"], msg.Types);
        Assert.Null(msg.BanEvasionEvaluation);

        var m = msg.Message;
        Assert.Equal("42", m.MessageId);
        Assert.Equal("hello", m.Text);
        Assert.Single(m.Fragments);
    }

    [Fact]
    public void ChannelSuspiciousUserUpdate_V1()
    {
        var json = Encoding.UTF8.GetBytes(Payloads.ChannelSuspiciousUserUpdateV1Json);
        var msg = new ChannelSuspiciousUserUpdate(json.AsMemory());

        Assert.Equal(1050263435, msg.BroadcasterId);
        Assert.Equal("77f111cbb75341449f5", msg.BroadcasterUsername);
        Assert.Equal("77f111cbb75341449f5", msg.BroadcasterDisplayName);
        Assert.Equal(1050263436, msg.ModeratorId);
        Assert.Equal("29087e59dfc441968f6", msg.ModeratorUsername);
        Assert.Equal("29087e59dfc441968f6", msg.ModeratorDisplayName);
        Assert.Equal(1050263437, msg.UserId);
        Assert.Equal("06fbcc75952245c5a87", msg.Username);
        Assert.Equal("06fbcc75952245c5a87", msg.UserDisplayName);
        Assert.Equal("restricted", msg.LowTrustStatus);
    }

    [Fact]
    public void ChannelSuspiciousUserUpdate_V1_None()
    {
        var json = Encoding.UTF8.GetBytes(Payloads.ChannelSuspiciousUserUpdateV1NoneJson);
        var msg = new ChannelSuspiciousUserUpdate(json.AsMemory());

        Assert.Equal(42, msg.BroadcasterId);
        Assert.Equal("broadcaster", msg.BroadcasterUsername);
        Assert.Equal("Broadcaster", msg.BroadcasterDisplayName);
        Assert.Equal(43, msg.ModeratorId);
        Assert.Equal("mod", msg.ModeratorUsername);
        Assert.Equal("Mod", msg.ModeratorDisplayName);
        Assert.Equal(44, msg.UserId);
        Assert.Equal("suspicioususer", msg.Username);
        Assert.Equal("SuspiciousUser", msg.UserDisplayName);
        Assert.Equal("none", msg.LowTrustStatus);
    }

    [Fact]
    public void ChannelVipAdd_V1()
    {
        var json = Encoding.UTF8.GetBytes(Payloads.ChannelVipAddV1Json);
        var msg = new ChannelVipAdd(json.AsMemory());

        Assert.Equal(1234, msg.UserId);
        Assert.Equal("mod_user", msg.Username);
        Assert.Equal("Mod_User", msg.UserDisplayName);
        Assert.Equal(1337, msg.BroadcasterId);
        Assert.Equal("cooler_user", msg.BroadcasterUsername);
        Assert.Equal("Cooler_User", msg.BroadcasterDisplayName);
    }

    [Fact]
    public void ChannelVipRemove_V1()
    {
        var json = Encoding.UTF8.GetBytes(Payloads.ChannelVipRemoveV1Json);
        var msg = new ChannelVipRemove(json.AsMemory());

        Assert.Equal(5678, msg.UserId);
        Assert.Equal("removed_user", msg.Username);
        Assert.Equal("Removed_User", msg.UserDisplayName);
        Assert.Equal(1337, msg.BroadcasterId);
        Assert.Equal("cooler_user", msg.BroadcasterUsername);
        Assert.Equal("Cooler_User", msg.BroadcasterDisplayName);
    }

    [Fact]
    public void ChannelWarningAcknowledge_V1()
    {
        var json = Encoding.UTF8.GetBytes(Payloads.ChannelWarningAcknowledgeV1Json);
        var msg = new ChannelWarningAcknowledge(json.AsMemory());

        Assert.Equal(423374343, msg.BroadcasterId);
        Assert.Equal("glowillig", msg.BroadcasterUsername);
        Assert.Equal("glowillig", msg.BroadcasterDisplayName);
        Assert.Equal(141981764, msg.UserId);
        Assert.Equal("twitchdev", msg.Username);
        Assert.Equal("TwitchDev", msg.UserDisplayName);
    }

    [Fact]
    public void ChannelWarningSend_V1()
    {
        var json = Encoding.UTF8.GetBytes(Payloads.ChannelWarningSendV1Json);
        var msg = new ChannelWarningSend(json.AsMemory());

        Assert.Equal(423374343, msg.BroadcasterId);
        Assert.Equal("glowillig", msg.BroadcasterUsername);
        Assert.Equal("glowillig", msg.BroadcasterDisplayName);
        Assert.Equal(424596340, msg.ModeratorId);
        Assert.Equal("quotrok", msg.ModeratorUsername);
        Assert.Equal("quotrok", msg.ModeratorDisplayName);
        Assert.Equal(141981764, msg.UserId);
        Assert.Equal("twitchdev", msg.Username);
        Assert.Equal("TwitchDev", msg.UserDisplayName);
        Assert.Equal("cut it out", msg.Reason);
        Assert.Null(msg.ChatRulesCited);
    }

    [Fact]
    public void ChannelWarningSend_V1_NoReason()
    {
        var json = Encoding.UTF8.GetBytes(Payloads.ChannelWarningSendV1NoReasonJson);
        var msg = new ChannelWarningSend(json.AsMemory());

        Assert.Equal(1337, msg.BroadcasterId);
        Assert.Equal("streamer", msg.BroadcasterUsername);
        Assert.Equal("Streamer", msg.BroadcasterDisplayName);
        Assert.Equal(43, msg.ModeratorId);
        Assert.Equal("mod", msg.ModeratorUsername);
        Assert.Equal("Mod", msg.ModeratorDisplayName);
        Assert.Equal(44, msg.UserId);
        Assert.Equal("user", msg.Username);
        Assert.Equal("User", msg.UserDisplayName);
        Assert.Null(msg.Reason);
        Assert.NotNull(msg.ChatRulesCited);
        Assert.Equal(["1", "3", "5"], msg.ChatRulesCited);
    }

    [Fact]
    public void ChannelCharityCampaignDonate_V1()
    {
        var json = Encoding.UTF8.GetBytes(Payloads.ChannelCharityCampaignDonateV1Json);
        var msg = new ChannelCharityCampaignDonate(json.AsMemory());

        Assert.Equal("a1b2c3-aabb-4455-d1e2f3", msg.Id);
        Assert.Equal("123-abc-456-def", msg.CampaignId);
        Assert.Equal(123456, msg.BroadcasterId);
        Assert.Equal("sunnysideup", msg.BroadcasterUsername);
        Assert.Equal("SunnySideUp", msg.BroadcasterDisplayName);
        Assert.Equal(654321, msg.UserId);
        Assert.Equal("generoususer1", msg.Username);
        Assert.Equal("GenerousUser1", msg.UserDisplayName);
        Assert.Equal("Example name", msg.CharityName);
        Assert.Equal("Example description", msg.CharityDescription);
        Assert.Equal("https://abc.cloudfront.net/ppgf/1000/100.png", msg.CharityLogo);
        Assert.Equal("https://www.example.com", msg.CharityWebsite);

        var a = msg.Amount;
        Assert.Equal(10000, a.Value);
        Assert.Equal(2, a.DecimalPlaces);
        Assert.Equal("USD", a.Currency);
    }

    [Fact]
    public void ChannelCharityCampaignStart_V1()
    {
        var json = Encoding.UTF8.GetBytes(Payloads.ChannelCharityCampaignStartV1Json);
        var msg = new ChannelCharityCampaignStart(json.AsMemory());

        Assert.Equal("123-abc-456-def", msg.Id);
        Assert.Equal(123456, msg.BroadcasterId);
        Assert.Equal("sunnysideup", msg.BroadcasterUsername);
        Assert.Equal("SunnySideUp", msg.BroadcasterDisplayName);
        Assert.Equal("Example name", msg.CharityName);
        Assert.Equal("Example description", msg.CharityDescription);
        Assert.Equal("https://abc.cloudfront.net/ppgf/1000/100.png", msg.CharityLogo);
        Assert.Equal("https://www.example.com", msg.CharityWebsite);

        var cur = msg.CurrentAmount;
        Assert.Equal(0, cur.Value);
        Assert.Equal(2, cur.DecimalPlaces);
        Assert.Equal("USD", cur.Currency);

        var tgt = msg.TargetAmount;
        Assert.Equal(1500000, tgt.Value);
        Assert.Equal(2, tgt.DecimalPlaces);
        Assert.Equal("USD", tgt.Currency);

        Assert.Equal(DateTimeOffset.Parse("2022-07-26T17:00:03.17106713Z"), msg.StartedAt);
    }

    [Fact]
    public void ChannelCharityCampaignProgress_V1()
    {
        var json = Encoding.UTF8.GetBytes(Payloads.ChannelCharityCampaignProgressV1Json);
        var msg = new ChannelCharityCampaignProgress(json.AsMemory());

        Assert.Equal("123-abc-456-def", msg.Id);
        Assert.Equal(123456, msg.BroadcasterId);
        Assert.Equal("sunnysideup", msg.BroadcasterUsername);
        Assert.Equal("SunnySideUp", msg.BroadcasterDisplayName);
        Assert.Equal("Example name", msg.CharityName);
        Assert.Equal("Example description", msg.CharityDescription);
        Assert.Equal("https://abc.cloudfront.net/ppgf/1000/100.png", msg.CharityLogo);
        Assert.Equal("https://www.example.com", msg.CharityWebsite);

        var cur = msg.CurrentAmount;
        Assert.Equal(260000, cur.Value);
        Assert.Equal(2, cur.DecimalPlaces);
        Assert.Equal("USD", cur.Currency);

        var tgt = msg.TargetAmount;
        Assert.Equal(1500000, tgt.Value);
        Assert.Equal(2, tgt.DecimalPlaces);
        Assert.Equal("USD", tgt.Currency);
    }

    [Fact]
    public void ChannelCharityCampaignStop_V1()
    {
        var json = Encoding.UTF8.GetBytes(Payloads.ChannelCharityCampaignStopV1Json);
        var msg = new ChannelCharityCampaignStop(json.AsMemory());

        Assert.Equal("123-abc-456-def", msg.Id);
        Assert.Equal(123456, msg.BroadcasterId);
        Assert.Equal("sunnysideup", msg.BroadcasterUsername);
        Assert.Equal("SunnySideUp", msg.BroadcasterDisplayName);
        Assert.Equal("Example name", msg.CharityName);
        Assert.Equal("Example description", msg.CharityDescription);
        Assert.Equal("https://abc.cloudfront.net/ppgf/1000/100.png", msg.CharityLogo);
        Assert.Equal("https://www.example.com", msg.CharityWebsite);

        var cur = msg.CurrentAmount;
        Assert.Equal(1450000, cur.Value);
        Assert.Equal(2, cur.DecimalPlaces);
        Assert.Equal("USD", cur.Currency);

        var tgt = msg.TargetAmount;
        Assert.Equal(1500000, tgt.Value);
        Assert.Equal(2, tgt.DecimalPlaces);
        Assert.Equal("USD", tgt.Currency);

        Assert.Equal(DateTimeOffset.Parse("2022-07-26T22:00:03.17106713Z"), msg.StoppedAt);
    }

    [Fact]
    public void ConduitShardDisabled_V1_WebSocket()
    {
        var json = Encoding.UTF8.GetBytes(Payloads.ConduitShardDisabledV1WebSocketJson);
        var msg = new ConduitShardDisabled(json.AsMemory());

        Assert.Equal("bfcfc993-26b1-b876-44d9-afe75a379dac", msg.ConduitId);
        Assert.Equal("4", msg.ShardId);
        Assert.Equal("websocket_disconnected", msg.Status);

        var t = msg.Transport;
        Assert.Equal("websocket", t.Method);
        Assert.Null(t.Callback);
        Assert.Equal("ad1c9fc3-0d99-4eb7-8a04-8608e8ff9ec9", t.SessionId);
        Assert.Equal(DateTimeOffset.Parse("2020-11-10T14:32:18.730260295Z"), t.ConnectedAt);
        Assert.Equal(DateTimeOffset.Parse("2020-11-11T14:32:18.730260295Z"), t.DisconnectedAt);
    }

    [Fact]
    public void ConduitShardDisabled_V1_Webhook()
    {
        var json = Encoding.UTF8.GetBytes(Payloads.ConduitShardDisabledV1WebhookJson);
        var msg = new ConduitShardDisabled(json.AsMemory());

        Assert.Equal("bfcfc993-26b1-b876-44d9-afe75a379dac", msg.ConduitId);
        Assert.Equal("7", msg.ShardId);
        Assert.Equal("webhook_callback_none", msg.Status);

        var t = msg.Transport;
        Assert.Equal("webhook", t.Method);
        Assert.Equal("https://example.com/webhooks/callback", t.Callback);
        Assert.Null(t.SessionId);
        Assert.Null(t.ConnectedAt);
        Assert.Null(t.DisconnectedAt);
    }

    [Fact]
    public void DropEntitlementGrant_V1()
    {
        var json = Encoding.UTF8.GetBytes(Payloads.DropEntitlementGrantV1Json);
        var msg = new DropEntitlementGrant(json.AsMemory());

        Assert.Equal("bf7c8577-e3e3-4881-a78a-e9446641d45d", msg.Id);

        var d = msg.Data;
        Assert.Equal("9001", d.OrganizationId);
        Assert.Equal("9002", d.CategoryId);
        Assert.Equal("Fortnite", d.CategoryName);
        Assert.Equal("9003", d.CampaignId);
        Assert.Equal(1234, d.UserId);
        Assert.Equal("cool_user", d.Username);
        Assert.Equal("Cool_User", d.UserDisplayName);
        Assert.Equal("fb78259e-fb81-4d1b-8333-34a06ffc24c0", d.EntitlementId);
        Assert.Equal("74c52265-e214-48a6-91b9-23b6014e8041", d.BenefitId);
        Assert.Equal(DateTimeOffset.Parse("2019-01-28T04:17:53.325Z"), d.CreatedAt);
    }

    [Fact]
    public void ExtensionBitsTransactionCreate_V1()
    {
        var json = Encoding.UTF8.GetBytes(Payloads.ExtensionBitsTransactionCreateV1Json);
        var msg = new ExtensionBitsTransactionCreate(json.AsMemory());

        Assert.Equal("bits-tx-id", msg.Id);
        Assert.Equal("deadbeef", msg.ExtensionClientId);
        Assert.Equal(1337, msg.BroadcasterId);
        Assert.Equal("cool_user", msg.BroadcasterUsername);
        Assert.Equal("Cool_User", msg.BroadcasterDisplayName);
        Assert.Equal(1236, msg.UserId);
        Assert.Equal("coolest_user", msg.Username);
        Assert.Equal("Coolest_User", msg.UserDisplayName);

        var p = msg.Product;
        Assert.Equal("great_product", p.Name);
        Assert.Equal("skuskusku", p.Sku);
        Assert.Equal(1234, p.Bits);
        Assert.False(p.InDevelopment);
    }

    [Fact]
    public void ChannelGoalBegin_V1()
    {
        var json = Encoding.UTF8.GetBytes(Payloads.ChannelGoalBeginV1Json);
        var msg = new ChannelGoalBegin(json.AsMemory());

        Assert.Equal("12345-cool-event", msg.Id);
        Assert.Equal(141981764, msg.BroadcasterId);
        Assert.Equal("twitchdev", msg.BroadcasterUsername);
        Assert.Equal("TwitchDev", msg.BroadcasterDisplayName);
        Assert.Equal("subscription", msg.Type);
        Assert.Equal("Help me get partner!", msg.Description);
        Assert.Equal(100, msg.CurrentAmount);
        Assert.Equal(220, msg.TargetAmount);
        Assert.Equal(DateTimeOffset.Parse("2021-07-15T17:16:03.17106713Z"), msg.StartedAt);
    }

    [Fact]
    public void ChannelGoalProgress_V1()
    {
        var json = Encoding.UTF8.GetBytes(Payloads.ChannelGoalProgressV1Json);
        var msg = new ChannelGoalProgress(json.AsMemory());

        Assert.Equal("12345-cool-event", msg.Id);
        Assert.Equal(141981764, msg.BroadcasterId);
        Assert.Equal("twitchdev", msg.BroadcasterUsername);
        Assert.Equal("TwitchDev", msg.BroadcasterDisplayName);
        Assert.Equal("subscription", msg.Type);
        Assert.Equal("Help me get partner!", msg.Description);
        Assert.Equal(120, msg.CurrentAmount);
        Assert.Equal(220, msg.TargetAmount);
        Assert.Equal(DateTimeOffset.Parse("2021-07-15T17:16:03.17106713Z"), msg.StartedAt);
    }

    [Fact]
    public void ChannelGoalEnd_V1()
    {
        var json = Encoding.UTF8.GetBytes(Payloads.ChannelGoalEndV1Json);
        var msg = new ChannelGoalEnd(json.AsMemory());

        Assert.Equal("12345-abc-678-defgh", msg.Id);
        Assert.Equal(141981764, msg.BroadcasterId);
        Assert.Equal("twitchdev", msg.BroadcasterUsername);
        Assert.Equal("TwitchDev", msg.BroadcasterDisplayName);
        Assert.Equal("subscription", msg.Type);
        Assert.Equal("Help me get partner!", msg.Description);
        Assert.False(msg.IsAchieved);
        Assert.Equal(180, msg.CurrentAmount);
        Assert.Equal(220, msg.TargetAmount);
        Assert.Equal(DateTimeOffset.Parse("2021-07-15T17:16:03.17106713Z"), msg.StartedAt);
        Assert.Equal(DateTimeOffset.Parse("2020-07-16T17:16:03.17106713Z"), msg.EndedAt);
    }
}
