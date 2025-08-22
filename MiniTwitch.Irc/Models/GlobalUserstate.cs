using System.Drawing;
using System.Text;
using MiniTwitch.Irc.Enums;
using MiniTwitch.Irc.Interfaces;
using MiniTwitch.Irc.Internal.Enums;
using MiniTwitch.Irc.Internal.Models;
using MiniTwitch.Irc.Internal.Parsing;

namespace MiniTwitch.Irc.Models;

/// <summary>
/// Represents the global state information for a user
/// </summary>
public readonly struct GlobalUserstate
{
    /// <summary>
    /// Gets the current user's state information.
    /// </summary>
    public IGlobalUserstate User { get; init; }
    /// <summary>
    /// Gets the current user's emote sets.
    /// </summary>
    public string EmoteSets { get; init; }

    internal GlobalUserstate(ref IrcMessage message)
    {
        string badgeInfo = string.Empty;
        string badges = string.Empty;
        Color color = default;
        string displayName = string.Empty;
        long uid = 0;
        UserType type = UserType.None;
        string emoteSets = string.Empty;

        using IrcTags tags = message.ParseTags();
        foreach (IrcTag tag in tags)
        {
            if (tag.Key.Length == 0)
            {
                continue;
            }

            ReadOnlySpan<byte> tagKey = tag.Key.Span;
            ReadOnlySpan<byte> tagValue = tag.Value.Span;
            switch (tagKey.Length)
            {
                //color
                case (int)Tags.Color when tagKey.SequenceEqual("color"u8):
                    color = TagHelper.GetColor(tagValue);
                    break;

                //badges
                case (int)Tags.Badges when tagKey.SequenceEqual("badges"u8):
                    badges = TagHelper.GetString(tagValue, true);
                    break;

                //user-id
                case (int)Tags.UserId when tagKey.SequenceEqual("user-id"u8):
                    uid = TagHelper.GetLong(tagValue);
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

                //badge-info
                case (int)Tags.BadgeInfo when tagKey.SequenceEqual("badge-info"u8):
                    badgeInfo = TagHelper.GetString(tagValue, true, true);
                    break;

                //emote-sets
                case (int)Tags.EmoteSets when tagKey.SequenceEqual("emote-sets"u8):
                    emoteSets = TagHelper.GetString(tagValue, true);
                    break;

                //display-name
                case (int)Tags.DisplayName when tagKey.SequenceEqual("display-name"u8):
                    displayName = TagHelper.GetString(tagValue);
                    break;
            }
        }

        this.User = new MessageAuthor()
        {
            BadgeInfo = badgeInfo,
            ChatColor = color,
            Badges = badges,
            DisplayName = displayName,
            Id = uid,
            Type = type
        };
        this.EmoteSets = emoteSets;
    }

    /// <summary>
    /// Construct a <see cref="GlobalUserstate"/> from a string. Useful for testing
    /// </summary>
    /// <param name="rawData">The raw IRC message</param>
    /// <returns><see cref="GlobalUserstate"/> with the related data</returns>
    public static GlobalUserstate Construct(string rawData)
    {
        ReadOnlyMemory<byte> memory = new(Encoding.UTF8.GetBytes(rawData));
        var message = new IrcMessage(memory);
        return new(ref message);
    }
}
