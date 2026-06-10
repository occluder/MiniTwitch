using MiniTwitch.EventSub.Internal;

namespace MiniTwitch.EventSub.Models;

[EventProperty]
public partial struct Cheer
{
    public partial int Bits { get; }
}
