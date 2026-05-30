namespace MiniTwitch.Irc.Interfaces;

/// <summary>
/// Represents a moderator anniversary notice
/// </summary>
public interface IModiversaryNotice : IUnixTimestamped, IUsernotice
{
    /// <summary>
    /// The number of months the user has been a moderator
    /// </summary>
    int ModerationMonths { get; }
    /// <summary>
    /// The message emitted in chat when the event occurs
    /// </summary>
    string SystemMessage { get; }
    /// <summary>
    /// The user's message content for the modiversary
    /// </summary>
    string Message { get; }
}
