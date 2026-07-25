namespace MiniTwitch.Irc.Models;

/// <summary>
/// Information about a GIF in the message
/// </summary>
public readonly struct MessageGif
{
    /// <summary>
    /// Zero-based start position of the GIF in the message
    /// </summary>
    public int StartPosition { get; init; }
    /// <summary>
    /// Zero-based end position of the GIF in the message
    /// </summary>
    public int EndPosition { get; init; }
    /// <summary>
    /// An ID that uniquely identifies this GIF
    /// </summary>
    public string Id { get; init; }
    /// <summary>
    /// The URL of the GIF asset. Applications rendering the GIF must use the full URL provided; it must not be modified
    /// </summary>
    public string Url { get; init; }
}
