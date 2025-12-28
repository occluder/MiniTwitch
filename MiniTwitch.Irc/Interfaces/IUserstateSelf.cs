using System.Drawing;
using MiniTwitch.Irc.Enums;
using MiniTwitch.Irc.Models;

namespace MiniTwitch.Irc.Interfaces;

/// <summary>
/// Represents information about you when you send a message or join a channel
/// </summary>
public interface IUserstateSelf
{
    /// <inheritdoc cref="MessageAuthor.BadgeInfo"/>
    string BadgeInfo { get; }
    /// <inheritdoc cref="MessageAuthor.Badges"/>
    string Badges { get; }
    /// <inheritdoc cref="MessageAuthor.ChatColor"/>
    Color ChatColor { get; }
    /// <summary>
    /// Your username
    /// </summary>
    string Name { get; }
    /// <summary>
    /// Your display name
    /// </summary>
    string DisplayName { get; }
    /// <summary>
    /// Your user type
    /// </summary>
    UserType Type { get; }
    /// <summary>
    /// Whether you are a moderator
    /// </summary>
    bool IsMod { get; }
    /// <summary>
    /// Whether you are a VIP
    /// </summary>
    bool IsVip { get; }
    /// <summary>
    /// Whether you are a subscriber
    /// </summary>
    bool IsSubscriber { get; }
    /// <summary>
    /// Whether you have site-wide commercial free mode enabled
    /// <para>Note: This value is always <see langword="false"/></para>
    /// </summary>
    bool IsTurbo { get; }
    /// <summary>
    /// Whether you are the lead moderator of the channel
    /// <para>Note: Might not be working as Twitch is not sending this info for USERSTATEs yet</para>
    /// </summary>
    bool IsLeadMod { get; }
}