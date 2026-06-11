using MiniTwitch.EventSub.Internal;

namespace MiniTwitch.EventSub.Models;

/// <summary>Metadata pertaining to the cheermote</summary>
[EventProperty]
public partial struct Cheermote
{
    /// <summary>
    /// The name portion of the Cheermote string that you use in chat to cheer Bits.
    /// The full Cheermote string is the concatenation of <c>{prefix} + {number of Bits}</c>.
    /// For example, if the prefix is "Cheer" and you want to cheer 100 Bits,
    /// the full Cheermote string is Cheer100. When the Cheermote string is entered in chat,
    /// Twitch converts it to the image associated with the Bits tier that was cheered
    /// </summary>
    public partial string Prefix { get; }
    /// <summary>The amount of Bits cheered</summary>
    public partial int Bits { get; }
    /// <summary>The tier level of the cheermote</summary>
    public partial int Tier { get; }
}
