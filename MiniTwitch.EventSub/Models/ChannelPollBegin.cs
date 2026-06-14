using System.Text.Json.Serialization;
using MiniTwitch.EventSub.Internal;

namespace MiniTwitch.EventSub.Models;

/// <summary>
/// A notification when a poll begins on the specified channel
/// </summary>
[EventSubEvent("channel.poll.begin", "1")]
public partial struct ChannelPollBegin
{
    /// <summary>ID of the poll</summary>
    public partial string Id { get; }
    /// <summary>The requested broadcaster ID</summary>
    [JsonPropertyName("broadcaster_user_id")]
    public partial long BroadcasterId { get; }
    /// <summary>The requested broadcaster login</summary>
    [JsonPropertyName("broadcaster_user_login"), Intern]
    public partial string BroadcasterUsername { get; }
    /// <summary>The requested broadcaster display name</summary>
    [JsonPropertyName("broadcaster_user_name"), Intern]
    public partial string BroadcasterDisplayName { get; }
    /// <summary>Question displayed for the poll</summary>
    public partial string Title { get; }
    /// <summary>An array of choices for the poll</summary>
    public partial PollChoice[] Choices { get; }
    /// <summary>The Bits voting settings for the poll (not supported)</summary>
    public partial BitsVotingInfo BitsVoting { get; }
    /// <summary>The Channel Points voting settings for the poll</summary>
    public partial ChannelPointsVotingInfo ChannelPointsVoting { get; }
    /// <summary>The time the poll started</summary>
    public partial DateTimeOffset StartedAt { get; }
    /// <summary>The time the poll will end</summary>
    public partial DateTimeOffset EndsAt { get; }

    /// <summary>A choice for the poll</summary>
    [EventProperty]
    public partial struct PollChoice
    {
        /// <summary>ID for the choice</summary>
        public partial string Id { get; }
        /// <summary>Text displayed for the choice</summary>
        public partial string Title { get; }
    }

    /// <summary>Bits voting settings</summary>
    [EventProperty]
    public partial struct BitsVotingInfo
    {
        /// <summary>Indicates if Bits can be used for voting</summary>
        public partial bool IsEnabled { get; }
        /// <summary>Number of Bits required to vote once</summary>
        public partial int AmountPerVote { get; }
    }

    /// <summary>Channel Points voting settings</summary>
    [EventProperty]
    public partial struct ChannelPointsVotingInfo
    {
        /// <summary>Indicates if Channel Points can be used for voting</summary>
        public partial bool IsEnabled { get; }
        /// <summary>Number of Channel Points required to vote once</summary>
        public partial int AmountPerVote { get; }
    }
}
