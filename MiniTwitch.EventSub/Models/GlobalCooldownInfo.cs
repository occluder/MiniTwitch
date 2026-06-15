using MiniTwitch.EventSub.Internal;

namespace MiniTwitch.EventSub.Models;

/// <summary>Global cooldown settings</summary>
[EventProperty]
public partial struct GlobalCooldownInfo
{
    /// <summary>Whether a cooldown is enabled</summary>
    public partial bool IsEnabled { get; }
    /// <summary>The cooldown in seconds</summary>
    public partial int Seconds { get; }
}
