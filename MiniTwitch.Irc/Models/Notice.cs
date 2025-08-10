using System.Text;
using MiniTwitch.Irc.Enums;
using MiniTwitch.Irc.Interfaces;
using MiniTwitch.Irc.Internal.Enums;
using MiniTwitch.Irc.Internal.Models;
using MiniTwitch.Irc.Internal.Parsing;

namespace MiniTwitch.Irc.Models;

/// <summary>
/// Represents a notice received from Twitch
/// </summary>
public readonly struct Notice : IEquatable<Notice>
{
    /// <summary>
    /// The channel related to the notice
    /// </summary>
    public IGazatuChannel Channel { get; init; } = default!;
    /// <summary>
    /// The notice message
    /// </summary>
    public string SystemMessage { get; init; } = string.Empty;
    /// <summary>
    /// Type of the notice
    /// </summary>
    public NoticeType Type { get; init; } = NoticeType.Unknown;

    internal Notice(ref IrcMessage message)
    {
        this.SystemMessage = message.GetContent().Content;
        using IrcTags ircTags = message.ParseTags();
        foreach (IrcTag tag in ircTags)
        {
            ReadOnlySpan<byte> tagKey = tag.Key.Span;
            ReadOnlySpan<byte> tagValue = tag.Value.Span;

            // JUST in case they add more shit in the future
            switch (tagKey.Length)
            {
                //msg-id
                case (int)Tags.MsgId when tagKey.SequenceEqual("msg-id"u8):
                    this.Type = TagHelper.GetEnum<NoticeType>(tagValue);
                    break;
            }
        }

        this.Channel = new IrcChannel()
        {
            Name = message.IsGlobalChannel ? "*" : message.GetChannel()
        };
    }

    /// <summary>
    /// Construct a notice from a string. Useful for testing
    /// </summary>
    /// <param name="rawData">The raw IRC message <para>Example input: @msg-id=msg_channel_suspended :tmi.twitch.tv NOTICE #foretack :This channel does not exist or has been suspended.</para></param>
    /// <returns><see cref="Notice"/> with the related data</returns>
    public static Notice Construct(string rawData)
    {
        ReadOnlyMemory<byte> memory = new(Encoding.UTF8.GetBytes(rawData));
        var message = new IrcMessage(memory);
        return new(ref message);
    }

#pragma warning disable CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).
    /// <inheritdoc/>
    public bool Equals(Notice other) => this.Type == other.Type && this.Channel.Name == other.Channel.Name;
    /// <inheritdoc/>
    public override bool Equals(object obj) => obj is Notice && Equals((Notice)obj);
#pragma warning restore CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).
    /// <inheritdoc/>
    public static bool operator ==(Notice left, Notice right) => left.Equals(right);
    /// <inheritdoc/>
    public static bool operator !=(Notice left, Notice right) => !(left == right);
    /// <inheritdoc/>
    public override int GetHashCode()
    {
        var code = new HashCode();
        code.Add(this.Type);
        code.Add(this.Channel.Name);
        return code.ToHashCode();
    }

    /// <inheritdoc/>
    public static implicit operator string(Notice notice) => notice.SystemMessage;
    /// <inheritdoc/>
    public static implicit operator NoticeType(Notice notice) => notice.Type;
}