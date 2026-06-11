using MiniTwitch.EventSub.Internal;

namespace MiniTwitch.EventSub.Models;

/// <summary>A chat message fragment</summary>
[EventProperty]
public partial struct MessageFragment
{
    /// <summary>
    /// The type of message fragment. Possible values:
    /// <c>text</c>, <c>cheermote</c>, <c>emote</c>, <c>mention</c>
    /// </summary>
    [Intern]
    public partial string Type { get; }
    /// <summary>Message text in fragment</summary>
    public partial string Text { get; }
    /// <summary>Metadata pertaining to the cheermote</summary>
    public partial Cheermote Cheermote { get; }
    /// <summary>Metadata pertaining to the emote</summary>
    public partial Emote Emote { get; }
    /// <summary>Metadata pertaining to the mention</summary>
    public partial Mention Mention { get; }
}
