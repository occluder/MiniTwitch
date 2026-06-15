using MiniTwitch.EventSub.Internal;

namespace MiniTwitch.EventSub.Models;

/// <summary>Metadata associated with the slow command</summary>
[EventProperty]
public partial struct ActionSlow
{
    /// <summary>
    /// The amount of time, in seconds, that users need to wait
    /// between sending messages
    /// </summary>
    public partial int WaitTimeSeconds { get; }
}
