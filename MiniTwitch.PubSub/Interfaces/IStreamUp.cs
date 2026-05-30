namespace MiniTwitch.PubSub.Interfaces;

/// <summary>
/// Represents information about a stream that just went online
/// </summary>
public interface IStreamUp
{
    /// <summary>
    /// Server-side unix timestamp of when the stream went online
    /// </summary>
    double ServerTime { get; }
    /// <summary>
    /// This is likely the stream delay
    /// </summary>
    int PlayDelay { get; }
}
