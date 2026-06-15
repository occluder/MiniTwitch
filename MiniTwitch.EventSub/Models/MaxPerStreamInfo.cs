using MiniTwitch.EventSub.Internal;

namespace MiniTwitch.EventSub.Models;

/// <summary>Max per stream settings</summary>
[EventProperty]
public partial struct MaxPerStreamInfo
{
    /// <summary>Whether a maximum per stream is enabled</summary>
    public partial bool IsEnabled { get; }
    /// <summary>The maximum number of redemptions per stream</summary>
    public partial int Value { get; }
}
