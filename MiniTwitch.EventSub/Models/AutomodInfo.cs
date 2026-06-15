using MiniTwitch.EventSub.Internal;

namespace MiniTwitch.EventSub.Models;

/// <summary>If the message was caught by automod, this will be populated</summary>
[EventProperty]
public partial struct AutomodInfo
{
    /// <summary>The category of the caught message</summary>
    public partial string Category { get; }
    /// <summary>The level of severity (1-4)</summary>
    public partial int Level { get; }
    /// <summary>The bounds of the text that caused the message to be caught</summary>
    public partial Boundary[] Boundaries { get; }
}
