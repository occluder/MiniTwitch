using System.Text;
using MiniTwitch.Irc.Interfaces;
using MiniTwitch.Irc.Internal.Enums;
using MiniTwitch.Irc.Internal.Models;
using MiniTwitch.Irc.Internal.Parsing;

namespace MiniTwitch.Irc.Models;

/// <summary>
/// Represents a CLEARMSG message
/// </summary>
public readonly struct Clearmsg : IUnixTimestamped
{
    /// <inheritdoc cref="IDeletedMessageAuthor"/>
    public IDeletedMessageAuthor Target { get; init; }
    /// <summary>
    /// The channel where the event occurred
    /// </summary>
    public IGazatuChannel Channel { get; init; }
    /// <summary>
    /// Unique ID identifying the deleted message
    /// </summary>
    public string MessageId { get; init; }
    /// <summary>
    /// The content of the deleted message
    /// </summary>
    public string MessageContent { get; init; }

    /// <inheritdoc/>
    public long TmiSentTs { get; init; }
    /// <inheritdoc/>
    public DateTimeOffset SentTimestamp => DateTimeOffset.FromUnixTimeMilliseconds(this.TmiSentTs);

    internal Clearmsg(ref IrcMessage message)
    {
        string targetUsername = string.Empty;
        string channelName = message.GetChannel();
        string messageId = string.Empty;
        long tmiSentTs = 0;

        using IrcTags tags = message.ParseTags();
        foreach (IrcTag tag in tags)
        {
            ReadOnlySpan<byte> tagKey = tag.Key.Span;
            ReadOnlySpan<byte> tagValue = tag.Value.Span;

            switch (tagKey.Length)
            {
                //login
                case (int)Tags.Login when tagKey.SequenceEqual("login"u8):
                    targetUsername = TagHelper.GetString(tagValue);
                    break;

                //tmi-sent-ts
                case (int)Tags.TmiSentTs when tagKey.SequenceEqual("tmi-sent-ts"u8):
                    tmiSentTs = TagHelper.GetLong(tagValue);
                    break;

                //target-msg-id
                case (int)Tags.TargetMsgId when tagKey.SequenceEqual("target-msg-id"u8):
                    messageId = TagHelper.GetString(tagValue);
                    break;
            }
        }

        this.Target = new MessageAuthor()
        {
            Name = targetUsername
        };
        this.Channel = new IrcChannel()
        {
            Name = channelName
        };
        this.MessageId = messageId;
        this.MessageContent = message.GetContent(maybeAction: true).Content;
        this.TmiSentTs = tmiSentTs;
    }

    /// <summary>
    /// Construct a "deleted message" message from a string. Useful for testing
    /// </summary>
    /// <param name="rawData">The raw IRC message <para>Example input: @login=occluder;room-id=;target-msg-id=55dc74c9-a6b2-4443-9b68-3446a5ddb7ed;tmi-sent-ts=1678798254260 :tmi.twitch.tv CLEARMSG #occluder :frozen lol! </para></param>
    /// <returns><see cref="Clearmsg"/> with the related data</returns>
    public static Clearmsg Construct(string rawData)
    {
        ReadOnlyMemory<byte> memory = new(Encoding.UTF8.GetBytes(rawData));
        var message = new IrcMessage(memory);
        return new(ref message);
    }

    /// <inheritdoc/>
    public static implicit operator string(Clearmsg clearmsg) => clearmsg.MessageContent;
}