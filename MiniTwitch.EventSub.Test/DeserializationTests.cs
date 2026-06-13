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
}
