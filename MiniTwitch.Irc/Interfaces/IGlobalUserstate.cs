using System.Drawing;
using MiniTwitch.Irc.Enums;
using MiniTwitch.Irc.Models;

namespace MiniTwitch.Irc.Interfaces;

/// <summary>
/// Represents the global user state information for a user
/// </summary>
public interface IGlobalUserstate
{
    /// <inheritdoc cref="MessageAuthor.BadgeInfo"/>
    string BadgeInfo { get; }
    /// <inheritdoc cref="MessageAuthor.Badges"/>
    string Badges { get; }
    /// <inheritdoc cref="MessageAuthor.ChatColor"/>
    Color ChatColor { get; }
    /// <summary>
    /// Your display name
    /// </summary>
    string DisplayName { get; }
    /// <summary>
    /// Your user type
    /// </summary>
    UserType Type { get; }
}
