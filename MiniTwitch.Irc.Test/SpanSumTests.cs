using System.Text;
using MiniTwitch.Common.Extensions;
using MiniTwitch.Irc.Internal.Enums;
using Xunit;
using Xunit.Sdk;

namespace MiniTwitch.Irc.Test;
public class SpanSumTests
{
    // <expected, string>
    static readonly Dictionary<int, string> _values = new()
    {
        { (int)Tags.Id, "id" },
        { (int)Tags.Mod, "mod" },
        { (int)Tags.Vip, "vip" },
        { (int)Tags.R9K, "r9k" },
        { (int)Tags.Bits, "bits" },
        { (int)Tags.Flags, "flags" },
        { (int)Tags.Color, "color" },
        { (int)Tags.Slow, "slow" },
        { (int)Tags.Turbo, "turbo" },
        { (int)Tags.Badges, "badges" },
        { (int)Tags.Emotes, "emotes" },
        { (int)Tags.UserId, "user-id" },
        { (int)Tags.FirstMsg, "first-msg" },
        { (int)Tags.MsgId, "msg-id" },
        { (int)Tags.Login, "login" },
        { (int)Tags.RoomId, "room-id" },
        { (int)Tags.SubsOnly, "subs-only" },
        { (int)Tags.EmoteOnly, "emote-only" },
        { (int)Tags.TmiSentTs, "tmi-sent-ts" },
        { (int)Tags.BanDuration, "ban-duration" },
        { (int)Tags.TargetMsgId, "target-msg-id" },
        { (int)Tags.TargetUserId, "target-user-id" },
        { (int)Tags.FollowersOnly, "followers-only" },
        { (int)Tags.UserType, "user-type" },
        { (int)Tags.BadgeInfo, "badge-info" },
        { (int)Tags.Subscriber, "subscriber" },
        { (int)Tags.ClientNonce, "client-nonce" },
        { (int)Tags.DisplayName, "display-name" },
        { (int)Tags.ReturningChatter, "returning-chatter" },
        { (int)Tags.ReplyParentMsgId, "reply-parent-msg-id" },
        { (int)Tags.ReplyParentUserId, "reply-parent-user-id" },
        { (int)Tags.ReplyParentMsgBody, "reply-parent-msg-body" },
        { (int)Tags.ReplyParentUserLogin, "reply-parent-user-login" },
        { (int)Tags.ReplyParentDisplayName, "reply-parent-display-name" },
        { (int)Tags.ReplyThreadParentMsgId, "reply-thread-parent-msg-id" },
        { (int)Tags.ReplyThreadParentUserLogin, "reply-thread-parent-user-login" },
        { (int)Tags.SystemMsg, "system-msg" },
        { (int)Tags.MsgParamColor, "msg-param-color" },
        { (int)Tags.MsgParamMonths, "msg-param-months" },
        { (int)Tags.MsgParamSubPlan, "msg-param-sub-plan" },
        { (int)Tags.MsgParamSenderName, "msg-param-sender-name" },
        { (int)Tags.MsgParamGiftMonths, "msg-param-gift-months" },
        { (int)Tags.MsgParamViewerCount, "msg-param-viewerCount" },
        { (int)Tags.MsgParamRecipientId, "msg-param-recipient-id" },
        { (int)Tags.MsgParamSenderLogin, "msg-param-sender-login" },
        { (int)Tags.MsgParamSenderCount, "msg-param-sender-count" },
        { (int)Tags.MsgParamSubPlanName, "msg-param-sub-plan-name" },
        { (int)Tags.MsgParamStreakMonths, "msg-param-streak-months" },
        { (int)Tags.MsgParamMassGiftCount, "msg-param-mass-gift-count" },
        { (int)Tags.MsgParamCommunityGiftId, "msg-param-community-gift-id" },
        { (int)Tags.MsgParamCumulativeMonths, "msg-param-cumulative-months" },
        { (int)Tags.MsgParamRecipientUserName, "msg-param-recipient-user-name" },
        { (int)Tags.MsgParamShouldShareStreak, "msg-param-should-share-streak" },
        { (int)Tags.MsgParamRecipientDisplayName, "msg-param-recipient-display-name" },
        { (int)Tags.MsgParamCharityName, "msg-param-charity-name" },
        { (int)Tags.MsgParamDonationAmount, "msg-param-donation-amount" },
        { (int)Tags.MsgParamExponent, "msg-param-exponent" },
        { (int)Tags.MsgParamDonationCurrency, "msg-param-donation-currency" },
        { (int)Tags.EmoteSets, "emote-sets" },
        { (int)Tags.MessageId, "message-id" },
        { (int)Tags.ThreadId, "thread-id" },
        { (int)Tags.CustomRewardId, "custom-reward-id" },
        { (int)Tags.AnimationId, "animation-id" },
        { (int)Tags.SourceBadgeInfo, "source-badge-info" },
        { (int)Tags.SourceBadges, "source-badges" },
        { (int)Tags.SourceId, "source-id" },
        { (int)Tags.SourceRoomId, "source-room-id" },
        { (int)Tags.SourceOnly, "source-only" },
    };

    [Fact]
    public void Test_Includes_All_Tags()
    {
        var tags = Enum.GetNames(typeof(Tags));
        if (tags.Length != _values.Count)
        {
            var notIncluded = tags.Where(x => !_values.ContainsKey((int)Enum.Parse<Tags>(x)));
            Assert.Fail($"Test does not account for all tags, expected {tags.Length}, got {_values.Count}." +
                $"\nMissing tags: \n{string.Join("\n", notIncluded)}");
        }

        foreach (var tag in tags)
        {
            var enumValue = Enum.Parse<Tags>(tag);
            try
            {
                Assert.True(_values.ContainsKey((int)enumValue));
            }
            catch (TrueException)
            {
                Assert.Fail($"Enum value {enumValue} not present.");
            }
        }
    }

    [Fact]
    public void All_Tag_Values_Match_Sum()
    {
        foreach (var kvp in _values)
        {
            if (kvp.Key != MSum(kvp.Value))
            {
                Assert.Fail($"Tag \"{kvp.Value}\" does not match expected sum {kvp.Key}. Got {MSum(kvp.Value)} instead.");
            }
        }
    }


    private static int MSum(string s)
    {
        var source = Encoding.UTF8.GetBytes(s);
        return SpanExtensions.MSum(source);
    }
}
