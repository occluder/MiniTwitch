using MiniTwitch.EventSub.Internal;

namespace MiniTwitch.EventSub.Models;

/// <summary>The body of the message</summary>
[EventProperty]
public partial struct HeldMessage
{
    /// <summary>The contents of the message caught by automod</summary>
    public partial string Text { get; }
    /// <summary>Metadata surrounding the potential inappropriate fragments of the message</summary>
    public partial MessageFragment[] Fragments { get; }
}
