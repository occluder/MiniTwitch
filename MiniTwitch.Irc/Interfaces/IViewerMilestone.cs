namespace MiniTwitch.Irc.Interfaces;

/// <summary>
/// Represents a milestone in consecutive streams watched by a viewer
/// </summary>
public interface IViewerMilestone : IUnixTimestamped, IUsernotice
{
    /// <summary>
    /// The number of consecutive streams watched by the viewer
    /// </summary>
    int ConsecutiveStreamsWatched { get; }
    /// <summary>
    /// The message emitted in chat when the event occurs
    /// </summary>
    string SystemMessage { get; }
}