namespace MiniTwitch.Irc.Models;

/// <summary>
/// Source information about messages in shared chats
/// </summary>
public sealed class MessageSource
{
    /// <summary>
    /// Whether source information is available.
    /// <para>When <see langword="false"/>, indicates that the channel does not have shared chat enabled</para>
    /// </summary>
    public bool HasSource => this.ChannelId != 0;
    /// <inheritdoc cref="MessageAuthor.BadgeInfo"/>
    public string BadgeInfo { get; init; } = string.Empty;
    /// <inheritdoc cref="MessageAuthor.Badges"/>
    public string Badges { get; init; } = string.Empty;
    /// <inheritdoc cref="Privmsg.Id"/>
    public string MessageId { get; init; } = string.Empty;
    /// <summary>
    /// ID of the channel where this message originates from
    /// </summary>
    public long ChannelId { get; init; } = 0;
    /// <summary>
    /// A Boolean that indicates if a message sent during a shared chat session is only sent to the source channel.
    /// <para>Has no effect if the message is not sent during a shared chat session.</para>
    /// </summary>
    public bool SourceOnly { get; init; } = false;
}