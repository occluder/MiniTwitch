using System.Drawing;
using System.Text;
using MiniTwitch.Irc.Enums;
using MiniTwitch.Irc.Interfaces;
using MiniTwitch.Irc.Internal.Enums;
using MiniTwitch.Irc.Internal.Models;
using MiniTwitch.Irc.Internal.Parsing;

namespace MiniTwitch.Irc.Models;

/// <summary>
/// Represents a whispered message
/// </summary>
public readonly struct Whisper
{
    /// <summary>
    /// The author of the whisper
    /// </summary>
    public IWhisperAuthor Author { get; init; }
    /// <inheritdoc cref="Privmsg.Emotes"/>
    public string Emotes { get; init; }
    /// <inheritdoc cref="Privmsg.Id"/>
    public int Id { get; init; }
    /// <summary>
    /// The index of the message
    /// </summary>
    public string ThreadId { get; init; }
    /// <summary>
    /// The content of the message
    /// </summary>
    public string Content { get; init; }
    /// <summary>
    /// <inheritdoc cref="Privmsg.IsAction"/>
    /// </summary>
    public bool IsAction { get; init; }

    internal Whisper(ref IrcMessage message)
    {
        string badges = string.Empty;
        Color color = default;
        string displayName = string.Empty;
        string username = message.GetUsername();
        long uid = 0;
        UserType type = UserType.None;
        bool turbo = false;

        string emotes = string.Empty;
        int id = 0;
        string threadId = string.Empty;
        (string content, bool action) = message.GetContent(maybeAction: true);
        using IrcTags tags = message.ParseTags();
        foreach (IrcTag tag in tags)
        {
            ReadOnlySpan<byte> tagKey = tag.Key.Span;
            ReadOnlySpan<byte> tagValue = tag.Value.Span;
            switch (tagKey.Length)
            {
                //color
                case (int)Tags.Color when tagKey.SequenceEqual("color"u8):
                    color = TagHelper.GetColor(tagValue);
                    break;

                //turbo
                case (int)Tags.Turbo when tagKey.SequenceEqual("turbo"u8):
                    turbo = TagHelper.GetBool(tagValue);
                    break;

                //badges
                case (int)Tags.Badges when tagKey.SequenceEqual("badges"u8):
                    badges = TagHelper.GetString(tagValue, true);
                    break;

                //emotes
                case (int)Tags.Emotes when tagKey.SequenceEqual("emotes"u8):
                    emotes = TagHelper.GetString(tagValue);
                    break;

                //user-id
                case (int)Tags.UserId when tagKey.SequenceEqual("user-id"u8):
                    uid = TagHelper.GetLong(tagValue);
                    break;

                //thread-id
                case (int)Tags.ThreadId when tagKey.SequenceEqual("thread-id"u8):
                    threadId = TagHelper.GetString(tagValue, true);
                    break;

                //user-type
                case (int)Tags.UserType when tagKey.SequenceEqual("user-type"u8) && tagValue.Length > 0:
                    type = tagValue.Length switch
                    {
                        3 when tagValue.SequenceEqual("mod"u8) => UserType.Mod,
                        5 when tagValue.SequenceEqual("admin"u8) => UserType.Admin,
                        5 when tagValue.SequenceEqual("staff"u8) => UserType.Staff,
                        10 when tagValue.SequenceEqual("global_mod"u8) => UserType.GlobalModerator,
                        _ => UserType.None
                    };
                    break;

                //message-id
                case (int)Tags.MessageId when tagKey.SequenceEqual("message-id"u8):
                    id = TagHelper.GetInt(tagValue);
                    break;

                //display-name
                case (int)Tags.DisplayName when tagKey.SequenceEqual("display-name"u8):
                    displayName = TagHelper.GetString(tagValue);
                    break;
            }
        }

        this.Author = new MessageAuthor()
        {
            Badges = badges,
            ChatColor = color,
            DisplayName = displayName,
            Name = username,
            Id = uid,
            Type = type,
            IsTurbo = turbo
        };
        this.Emotes = emotes;
        this.Id = id;
        this.ThreadId = threadId;
        this.Content = content;
        this.IsAction = action;
    }

    /// <summary>
    /// Construct a <see cref="Whisper"/> from a string. Useful for testing
    /// </summary>
    /// <param name="rawData">The raw IRC message</param>
    /// <returns><see cref="Whisper"/> with the related data</returns>
    public static Whisper Construct(string rawData)
    {
        ReadOnlyMemory<byte> memory = new(Encoding.UTF8.GetBytes(rawData));
        var message = new IrcMessage(memory);
        return new(ref message);
    }

    /// <inheritdoc/>
    public static implicit operator string(Whisper whisper) => whisper.Content;
}