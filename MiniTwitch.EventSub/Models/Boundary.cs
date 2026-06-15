using System.Text.Json.Serialization;
using MiniTwitch.EventSub.Internal;

namespace MiniTwitch.EventSub.Models;

/// <summary>The bounds of the text that caused the message to be caught</summary>
[EventProperty]
public partial struct Boundary
{
    /// <summary>Index in the message for the start of the problem (0 indexed, inclusive)</summary>
    public partial int StartPos { get; }
    /// <summary>Index in the message for the end of the problem (0 indexed, inclusive)</summary>
    public partial int EndPos { get; }
}
