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
}
