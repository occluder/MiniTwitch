using MiniTwitch.EventSub.Internal;

namespace MiniTwitch.EventSub.Models;

/// <summary>Metadata related to a chat badge</summary>
[EventProperty]
public partial struct Badge
{
    /// <summary>An ID that identifies this set of chat badges. For example, Bits or Subscriber</summary>
    public partial string SetId { get; }
    /// <summary>
    /// An ID that identifies this version of the badge.
    /// The ID can be any value. For example, for Bits, the ID is the Bits tier level,
    /// but for World of Warcraft, it could be Alliance or Horde
    /// </summary>
    public partial string Id { get; }
    /// <summary>
    /// Contains metadata related to the chat badges in the badges tag.
    /// Currently, this tag contains metadata only for subscriber badges,
    /// to indicate the number of months the user has been a subscriber
    /// </summary>
    public partial string Info { get; }
}
