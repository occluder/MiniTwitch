using MiniTwitch.EventSub.Internal;

namespace MiniTwitch.EventSub.Models;

/// <summary>Metadata pertaining to the emote</summary>
[EventProperty]
public partial struct EmoteV1
{
    /// <summary>An ID that uniquely identifies this emote</summary>
    public partial string Id { get; }
    /// <summary>An ID that identifies the emote set that the emote belongs to</summary>
    public partial string EmoteSetId { get; }
}
