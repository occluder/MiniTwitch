using MiniTwitch.EventSub.Internal;

namespace MiniTwitch.EventSub.Models;

/// <summary>Metadata associated with the automod terms changes</summary>
[EventProperty]
public partial struct ActionAutomodTerms
{
    /// <summary>Either <c>add</c> or <c>remove</c></summary>
    public partial string Action { get; }
    /// <summary>Either <c>blocked</c> or <c>permitted</c></summary>
    public partial string List { get; }
    /// <summary>Terms being added or removed</summary>
    public partial string[] Terms { get; }
    /// <summary>Whether the terms were added due to an Automod action</summary>
    public partial bool FromAutomod { get; }
}
