using MiniTwitch.EventSub.Internal;

namespace MiniTwitch.EventSub.Models;

[EventProperty]
public partial struct MessageFragment
{
    public partial string Type { get; }
    public partial string Text { get; }
    public partial Cheermote Cheermote { get; }
    public partial Emote Emote { get; }
    public partial Mention Mention { get; }
}
