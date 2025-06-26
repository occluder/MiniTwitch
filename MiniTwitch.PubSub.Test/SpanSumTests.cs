using System.Text;
using MiniTwitch.Common.Extensions;
using MiniTwitch.PubSub.Internal.Enums;
using Xunit.Sdk;

namespace MiniTwitch.PubSub.Test;
public class SpanSumTests
{
    private static readonly Dictionary<int, string> _topics = new()
    {
        { (int)MessageTopic.None, "None" },
        { (int)MessageTopic.BitsEventsV1, "channel-bits-events-v1" },
        { (int)MessageTopic.BitsEventsV2, "channel-bits-events-v2" },
        { (int)MessageTopic.BitsBadgeUnlock, "channel-bits-badge-unlocks" },
        { (int)MessageTopic.ChannelPoints, "channel-points-channel-v1" },
        { (int)MessageTopic.SubscribeEvents, "channel-subscribe-events-v1" },
        { (int)MessageTopic.AutomodQueue, "automod-queue" },
        { (int)MessageTopic.LowTrustUsers, "low-trust-users" },
        { (int)MessageTopic.ModerationNotifications, "user-moderation-notifications" },
        { (int)MessageTopic.ChatroomsUser, "chatrooms-user-v1" },
        { (int)MessageTopic.ChannelPredictions, "predictions-channel-v1" },
        { (int)MessageTopic.PinnedChatUpdates, "pinned-chat-updates-v1" },
        { (int)MessageTopic.VideoPlayback, "video-playback-by-id" },
        { (int)MessageTopic.BroadcastSettingsUpdate, "broadcast-settings-update" },
        { (int)MessageTopic.ModeratorActions, "chat_moderator_actions" },
        { (int)MessageTopic.Polls, "polls" },
        { (int)MessageTopic.CommunityChannelPoints, "community-points-channel-v1" },
        { (int)MessageTopic.Following, "following" },
        { (int)MessageTopic.CommunityMoments, "community-moments-channel-v1" },
        { (int)MessageTopic.LeaderboardEvents, "leaderboard-events-v1" }
    };

    private static readonly Dictionary<int, string> _types = new()
    {
        { (int)MessageType.None, "None" },
        { (int)MessageType.PONG, "PONG" },
        { (int)MessageType.MESSAGE, "MESSAGE" },
        { (int)MessageType.RESPONSE, "RESPONSE" },
        { (int)MessageType.RECONNECT, "RECONNECT" },
    };

    [Fact]
    public void Test_Includes_All_Topics()
    {
        var topics = Enum.GetNames(typeof(MessageTopic));
        if (topics.Length != _topics.Count)
        {
            var notIncluded = topics.Where(x => !_topics.ContainsKey((int)Enum.Parse<MessageTopic>(x)));
            Assert.Fail($"Test does not account for all topics, expected {topics.Length}, got {_topics.Count}." +
                $"\nMissing topics: \n{string.Join("\n", notIncluded)}");
        }

        foreach (var topic in topics)
        {
            var enumValue = Enum.Parse<MessageTopic>(topic);
            try
            {
                Assert.True(_topics.ContainsKey((int)enumValue));
            }
            catch (TrueException)
            {
                Assert.Fail($"Enum value {enumValue} not present.");
            }
        }
    }

    [Fact]
    public void Test_Includes_All_Types()
    {
        var types = Enum.GetNames(typeof(MessageType));
        if (types.Length != _types.Count)
        {
            var notIncluded = types.Where(x => !_topics.ContainsKey((int)Enum.Parse<MessageType>(x)));
            Assert.Fail($"Test does not account for all types, expected {types.Length}, got {_types.Count}." +
                $"\nMissing types: \n{string.Join("\n", notIncluded)}");
        }

        foreach (var type in types)
        {
            var enumValue = Enum.Parse<MessageType>(type);
            try
            {
                Assert.True(_types.ContainsKey((int)enumValue));
            }
            catch (TrueException)
            {
                Assert.Fail($"Enum value {enumValue} not present.");
            }
        }
    }

    [Fact]
    public void All_Topic_Values_Match_Sum()
    {
        foreach (var kvp in _topics)
        {
            if (kvp.Key != MSum(kvp.Value))
            {
                Assert.Fail($"Topic \"{kvp.Value}\" does not match expected sum {kvp.Key}. Got {MSum(kvp.Value)} instead.");
            }
        }
    }

    [Fact]
    public void All_Type_Values_Match_Sum()
    {
        foreach (var kvp in _types)
        {
            if (kvp.Key != MSum(kvp.Value))
            {
                Assert.Fail($"Type \"{kvp.Value}\" does not match expected sum {kvp.Key}. Got {MSum(kvp.Value)} instead.");
            }
        }
    }

    private static int MSum(string s)
    {
        var source = Encoding.UTF8.GetBytes(s);
        return SpanExtensions.MSum(source);
    }
}
