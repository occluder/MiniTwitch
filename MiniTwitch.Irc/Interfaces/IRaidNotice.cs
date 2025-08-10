using MiniTwitch.Irc.Models;

namespace MiniTwitch.Irc.Interfaces;

/// <summary>
/// Contains information about a raid
/// </summary>
public interface IRaidNotice : IUsernotice
{
    /// <summary>
    /// The user raiding the channel
    /// </summary>
    new MessageAuthor Author { get; }
    /// <summary>
    /// The message emitted in chat when the event occurs
    /// <para>Example: 1 raiders from occluder have joined!</para>
    /// </summary>
    string SystemMessage { get; }
    /// <summary>
    /// The amount of viewers joining from the raid
    /// </summary>
    int ViewerCount { get; }
    /// <inheritdoc cref="Usernotice.Source"/>
    NoticeSource Source { get; }
}
