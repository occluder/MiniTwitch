using CaseConverter;
using MiniTwitch.Irc.Internal.Enums;
using Xunit;

namespace MiniTwitch.Irc.Test;

public class TagLengthTests
{
    // <string, expected length>
    static readonly Dictionary<string, int> _values = new()
    {
        { "id", (int)Tags.Id },
        { "mod", (int)Tags.Mod },
        { "vip", (int)Tags.Vip },
        { "r9k", (int)Tags.R9K },
        { "bits", (int)Tags.Bits },
        { "flags", (int)Tags.Flags },
        { "color", (int)Tags.Color },
        { "slow", (int)Tags.Slow },
        { "turbo", (int)Tags.Turbo },
        { "badges", (int)Tags.Badges },
        { "emotes", (int)Tags.Emotes },
        { "user-id", (int)Tags.UserId },
        { "first-msg", (int)Tags.FirstMsg },
        { "msg-id", (int)Tags.MsgId },
        { "login", (int)Tags.Login },
        { "room-id", (int)Tags.RoomId },
        { "subs-only", (int)Tags.SubsOnly },
        { "emote-only", (int)Tags.EmoteOnly },
        { "tmi-sent-ts", (int)Tags.TmiSentTs },
        { "ban-duration", (int)Tags.BanDuration },
        { "target-msg-id", (int)Tags.TargetMsgId },
        { "target-user-id", (int)Tags.TargetUserId },
        { "followers-only", (int)Tags.FollowersOnly },
        { "user-type", (int)Tags.UserType },
        { "badge-info", (int)Tags.BadgeInfo },
        { "subscriber", (int)Tags.Subscriber },
        { "client-nonce", (int)Tags.ClientNonce },
        { "display-name", (int)Tags.DisplayName },
        { "returning-chatter", (int)Tags.ReturningChatter },
        { "reply-parent-msg-id", (int)Tags.ReplyParentMsgId },
        { "reply-parent-user-id", (int)Tags.ReplyParentUserId },
        { "reply-parent-msg-body", (int)Tags.ReplyParentMsgBody },
        { "reply-parent-user-login", (int)Tags.ReplyParentUserLogin },
        { "reply-parent-display-name", (int)Tags.ReplyParentDisplayName },
        { "reply-thread-parent-msg-id", (int)Tags.ReplyThreadParentMsgId },
        { "reply-thread-parent-user-login", (int)Tags.ReplyThreadParentUserLogin },
        { "system-msg", (int)Tags.SystemMsg },
        { "msg-param-color", (int)Tags.MsgParamColor },
        { "msg-param-months", (int)Tags.MsgParamMonths },
        { "msg-param-sub-plan", (int)Tags.MsgParamSubPlan },
        { "msg-param-sender-name", (int)Tags.MsgParamSenderName },
        { "msg-param-gift-months", (int)Tags.MsgParamGiftMonths },
        { "msg-param-viewerCount", (int)Tags.MsgParamViewerCount },
        { "msg-param-recipient-id", (int)Tags.MsgParamRecipientId },
        { "msg-param-sender-login", (int)Tags.MsgParamSenderLogin },
        { "msg-param-sender-count", (int)Tags.MsgParamSenderCount },
        { "msg-param-sub-plan-name", (int)Tags.MsgParamSubPlanName },
        { "msg-param-streak-months", (int)Tags.MsgParamStreakMonths },
        { "msg-param-mass-gift-count", (int)Tags.MsgParamMassGiftCount },
        { "msg-param-community-gift-id", (int)Tags.MsgParamCommunityGiftId },
        { "msg-param-cumulative-months", (int)Tags.MsgParamCumulativeMonths },
        { "msg-param-recipient-user-name", (int)Tags.MsgParamRecipientUserName },
        { "msg-param-should-share-streak", (int)Tags.MsgParamShouldShareStreak },
        { "msg-param-recipient-display-name", (int)Tags.MsgParamRecipientDisplayName },
        { "msg-param-charity-name", (int)Tags.MsgParamCharityName },
        { "msg-param-donation-amount", (int)Tags.MsgParamDonationAmount },
        { "msg-param-exponent", (int)Tags.MsgParamExponent },
        { "msg-param-donation-currency", (int)Tags.MsgParamDonationCurrency },
        { "emote-sets", (int)Tags.EmoteSets },
        { "message-id", (int)Tags.MessageId },
        { "thread-id", (int)Tags.ThreadId },
        { "custom-reward-id", (int)Tags.CustomRewardId },
        { "animation-id", (int)Tags.AnimationId },
        { "source-badge-info", (int)Tags.SourceBadgeInfo },
        { "source-badges", (int)Tags.SourceBadges },
        { "source-id", (int)Tags.SourceId },
        { "source-room-id", (int)Tags.SourceRoomId },
        { "source-only", (int)Tags.SourceOnly },
        { "source-msg-id", (int)Tags.SourceMsgId },
        { "msg-param-goal-current-contributions", (int)Tags.MsgParamGoalCurrentContributions },
        { "msg-param-goal-target-contributions", (int)Tags.MsgParamGoalTargetContributions },
        { "msg-param-goal-description", (int)Tags.MsgParamGoalDescription },
        { "msg-param-goal-user-contributions", (int)Tags.MsgParamGoalUserContributions },
        { "msg-param-value", (int)Tags.MsgParamValue },
        { "msg-param-copoReward", (int)Tags.MsgParamCopoReward },
        { "msg-param-category", (int)Tags.MsgParamCategory },
    };

    [Fact]
    public void Test_Includes_All_Tags()
    {
        var tags = Enum.GetNames(typeof(Tags));
        if (tags.Length != _values.Count)
        {
            var notIncluded = tags.Where(x => x != "MsgParamViewerCount" && !_values.ContainsKey(x.ToKebabCase()));
            Assert.Fail($"Test does not account for all tags, expected {tags.Length}, got {_values.Count}." +
                $"\nMissing tags: \n{string.Join("\n", notIncluded)}");
        }
    }

    [Fact]
    public void All_Tag_Values_Match_Length()
    {
        foreach (var kvp in _values)
        {
            if (kvp.Value != kvp.Key.Length)
            {
                Assert.Fail($"Tag \"{kvp.Key}\" ({kvp.Key.Length}) does not match expected length {kvp.Value}");
            }
        }
    }
}
