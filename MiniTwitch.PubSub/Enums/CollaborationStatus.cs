namespace MiniTwitch.PubSub.Enums;

/// <summary>
/// Represents the state of stream-together collaboration
/// </summary>
public enum CollaborationStatus
{
    /// <summary>
    /// Unknown collaboration status
    /// </summary>
    UNKNOWN,
    /// <summary>
    /// Channel is not in a Stream Together session.
    /// </summary>
    None,
    /// <summary>
    /// Channel is engaged in a Stream Together session.
    /// </summary>
    InCollaboration
}
