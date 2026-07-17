using MiniTwitch.EventSub.Internal;

namespace MiniTwitch.EventSub.Models;

/// <summary>Metadata pertaining to the emote</summary>
[EventProperty]
public partial struct Emote
{
    /// <summary>An ID that uniquely identifies this emote</summary>
    public partial string Id { get; }
    /// <summary>An ID that identifies the emote set that the emote belongs to</summary>
    public partial string EmoteSetId { get; }
    /// <summary>The ID of the broadcaster who owns the emote</summary>
    public partial long OwnerId { get; }
    /// <summary>
    /// The formats that the emote is available in.
    /// Possible values: <c>animated</c> (An animated GIF is available for this emote),
    /// <c>static</c> (A static PNG file is available for this emote)
    /// </summary>
    [Intern]
    public partial string[] Format { get; }
}
