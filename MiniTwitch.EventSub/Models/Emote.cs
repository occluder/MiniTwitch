using MiniTwitch.EventSub.Internal;

namespace MiniTwitch.EventSub.Models;

[EventProperty]
public partial struct Emote
{
    public partial string Id { get; }
    public partial string EmoteSetId { get; }
    public partial long OwnerId { get; }
    public partial string[] Format { get; }
}
