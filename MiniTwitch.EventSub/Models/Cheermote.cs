using MiniTwitch.EventSub.Internal;

namespace MiniTwitch.EventSub.Models;

[EventProperty]
public partial struct Cheermote
{
    public partial string Prefix { get; }
    public partial int Bits { get; }
    public partial int Tier { get; }
}
