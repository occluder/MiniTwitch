using MiniTwitch.EventSub.Internal;

namespace MiniTwitch.EventSub.Models;

/// <summary>Metadata pertaining to the GIF</summary>
[EventProperty]
public partial struct Gif
{
    /// <summary>An ID that uniquely identifies this GIF</summary>
    public partial string Id { get; }
    /// <summary>The URL of the GIF asset. Applications rendering the GIF must use the full URL provided; it must not be modified</summary>
    public partial string Url { get; }
}
