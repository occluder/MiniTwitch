using MiniTwitch.EventSub.Internal;

namespace MiniTwitch.EventSub.Models;

/// <summary>Metadata associated with the followers command</summary>
[EventProperty]
public partial struct ActionFollowers
{
    /// <summary>
    /// The length of time, in minutes, that the followers must have
    /// followed the broadcaster to participate in the chat room
    /// </summary>
    public partial int FollowDurationMinutes { get; }
}
