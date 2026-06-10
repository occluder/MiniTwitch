using MiniTwitch.EventSub.Internal;

namespace MiniTwitch.EventSub.Models;

[EventProperty]
public partial struct Badge
{
    public partial string SetId { get; }
    public partial string Id { get; }
    public partial string Info { get; }
}
