using MiniTwitch.EventSub.Internal;

namespace MiniTwitch.EventSub.Models;

/// <summary>Bits voting settings</summary>
[EventProperty]
public partial struct BitsVotingInfo
{
    /// <summary>Indicates if Bits can be used for voting</summary>
    public partial bool IsEnabled { get; }
    /// <summary>Number of Bits required to vote once</summary>
    public partial int AmountPerVote { get; }
}
