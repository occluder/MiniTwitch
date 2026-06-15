using MiniTwitch.EventSub.Internal;

namespace MiniTwitch.EventSub.Models;

/// <summary>Channel Points voting settings</summary>
[EventProperty]
public partial struct ChannelPointsVotingInfo
{
    /// <summary>Indicates if Channel Points can be used for voting</summary>
    public partial bool IsEnabled { get; }
    /// <summary>Number of Channel Points required to vote once</summary>
    public partial int AmountPerVote { get; }
}
