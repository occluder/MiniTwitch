using MiniTwitch.EventSub.Internal;

namespace MiniTwitch.EventSub.Models;

/// <summary>Max per user per stream settings</summary>
[EventProperty]
public partial struct MaxPerUserPerStreamInfo
{
    /// <summary>Whether a maximum per user per stream is enabled</summary>
    public partial bool IsEnabled { get; }
    /// <summary>The maximum number of redemptions per user per stream</summary>
    public partial int Value { get; }
}
