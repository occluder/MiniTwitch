using MiniTwitch.PubSub.Enums;

namespace MiniTwitch.PubSub.Interfaces;

/// <summary>
/// Represents viewer count information for an online stream
/// </summary>
public interface IViewerCountUpdate
{
    /// <summary>
    /// Server-side unix timestamp of when this data was sent
    /// </summary>
    double ServerTime { get; }
    /// <summary>
    /// The amount of viewers currently watching the stream
    /// </summary>
    int Viewers { get; }
    /// <summary>
    /// The amount of viewers currently watching the stream of the collaboration channel
    /// </summary>
    int CollaborationViewers { get; }
    /// <summary>
    /// The collaboration status of the channel
    /// </summary>
    CollaborationStatus CollaborationStatus { get; }
}
