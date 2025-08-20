using MiniTwitch.Irc.Enums;
using MiniTwitch.Irc.Models;

namespace MiniTwitch.Irc.Interfaces;
/// <summary>
/// Contains information about an announcement message
/// </summary>
public interface IAnnouncementNotice : IUsernotice
{
    /// <summary>
    /// Color of the announcement
    /// <para>Default is <see cref="AnnouncementColor.Primary"/></para>
    /// </summary>
    AnnouncementColor Color { get; }
    /// <summary>
    /// The message content of the announcement
    /// </summary>
    string Message { get; }
    /// <summary>
    /// Emote sets in the announcement message
    /// <para><see cref="string.Empty"/> if there are none</para>
    /// </summary>
    string Emotes { get; }
    /// <summary>
    /// Automod flags in the announcement message
    /// <para><see cref="string.Empty"/> if there are none</para>
    /// </summary>
    string Flags { get; }
    /// <inheritdoc cref="Usernotice.Source"/>
    NoticeSource Source { get; }
}
