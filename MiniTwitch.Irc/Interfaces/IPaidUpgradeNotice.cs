using MiniTwitch.Irc.Models;

namespace MiniTwitch.Irc.Interfaces;

/// <summary>
/// Contains information about a user that continued their gifted subscription
/// </summary>
public interface IPaidUpgradeNotice : IUsernotice
{
    /// <summary>
    /// The message emitted in chat when the event occurs
    /// <para>Example: special_forces_of_russia is continuing the Gift Sub they got from potnayakatka64!</para>
    /// </summary>
    string SystemMessage { get; }
    /// <summary>
    /// Username of the previous subscription's gifter
    /// <para>Note: Value is <see cref="string.Empty"/> if the previous subscription's gifter was anonymous</para>
    /// </summary>
    string GifterUsername { get; }
    /// <summary>
    /// Display name of the previous subscription's gifter
    /// <para>Note: Value is <see cref="string.Empty"/> if the previous subscription's gifter was anonymous</para>
    /// </summary>
    string GifterDisplayName { get; }
    /// <inheritdoc cref="Usernotice.Source"/>
    NoticeSource Source { get; }
    /// <inheritdoc cref="Usernotice.ChannelGoal"/>
    ChannelGoal ChannelGoal { get; }
}