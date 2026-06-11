using MiniTwitch.EventSub.Internal;

namespace MiniTwitch.EventSub.Models;

/// <summary>Metadata if the message is a cheer</summary>
[EventProperty]
public partial struct Cheer
{
    /// <summary>The amount of Bits the user cheered</summary>
    public partial int Bits { get; }
}
