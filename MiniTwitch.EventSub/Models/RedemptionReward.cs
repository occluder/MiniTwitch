using MiniTwitch.EventSub.Internal;

namespace MiniTwitch.EventSub.Models;

/// <summary>Basic information about the reward that was redeemed</summary>
[EventProperty]
public partial struct RedemptionReward
{
    /// <summary>The reward identifier</summary>
    public partial string Id { get; }
    /// <summary>The reward title</summary>
    public partial string Title { get; }
    /// <summary>The reward cost</summary>
    public partial int Cost { get; }
    /// <summary>The reward description</summary>
    public partial string Prompt { get; }
}
