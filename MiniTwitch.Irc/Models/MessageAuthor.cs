using System.Drawing;
using MiniTwitch.Irc.Enums;
using MiniTwitch.Irc.Interfaces;

namespace MiniTwitch.Irc.Models;

/// <summary>
/// Represents an author of a message or a certain event
/// </summary>
public readonly struct MessageAuthor : IBanTarget, IDeletedMessageAuthor, IWhisperAuthor,
    IGiftSubRecipient, IUserstateSelf, IMembershipUser, IGazatuChannel, IPartedChannel,
    IBasicChannel, IGlobalUserstate, IEquatable<MessageAuthor>, IEquatable<IrcChannel>
{
    /// <summary>
    /// Contains metadata related to the chat badges in the badges tag
    /// <para>Currently, this tag contains metadata only for subscriber badges, to indicate the number of months the user has been a subscriber</para>
    /// </summary>
    public string BadgeInfo { get; init; }
    /// <summary>
    /// Comma-separated list of chat badges in the form, &lt;badge&gt;/&lt;version&gt;. For example, admin/1. There are many possible badge values, but here are few:
    /// <list type="bullet">
    /// <item>admin</item>
    /// <item>bits</item>
    /// <item>broadcaster</item>
    /// <item>moderator</item>
    /// <item>subscriber</item>
    /// <item>turbo</item>
    /// </list>
    /// <para>Most badges have only 1 version, but some badges like subscriber badges offer different versions of the badge depending on how long the user has subscribed</para>
    /// <para>To get the badge, use the <see href="https://dev.twitch.tv/docs/api/reference#get-global-chat-badges">Get Global Chat Badges</see> and <see href="https://dev.twitch.tv/docs/api/reference#get-channel-chat-badges">Get Channel Chat Badges</see> APIs. Match the badge to the <c>set-id</c> field’s value in the response. Then, match the version to the <c>id</c> field in the list of versions</para>
    /// </summary>
    public string Badges { get; init; }
    /// <summary>
    /// The color of the user’s name in the chat room
    /// </summary>
    public Color ChatColor { get; init; }
    /// <summary>
    /// The user’s display name, escaped as described in the <see href="https://ircv3.net/specs/core/message-tags-3.2.html">IRCv3</see> spec
    /// <para>Note: Can contain characters outside [a-zA-Z0-9_]</para>
    /// </summary>
    public string DisplayName { get; init; }
    /// <summary>
    /// The user's name
    /// </summary>
    public string Name { get; init; }
    /// <summary>
    /// The user's ID
    /// </summary>
    public long Id { get; init; }
    /// <summary>
    /// The type of the user 
    /// </summary>
    public UserType Type { get; init; }
    /// <summary>
    /// whether the user is a subscriber
    /// </summary>
    public bool IsSubscriber { get; init; }
    /// <summary>
    /// Whether the user is a moderator
    /// </summary>
    public bool IsMod { get; init; }
    /// <summary>
    /// Whether the user is a VIP
    /// </summary>
    public bool IsVip { get; init; }
    /// <summary>
    /// Whether the user has site-wide commercial free mode enabled
    /// <para>Note: This value is seemingly always <see langword="false"/></para>
    /// </summary>
    public bool IsTurbo { get; init; }

#pragma warning disable CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).
    /// <inheritdoc/>
    public bool Equals(IrcChannel other) => this.Name == other.Name;
    /// <inheritdoc/>
    public override bool Equals(object obj) => (obj is MessageAuthor && Equals((MessageAuthor)obj)) || (obj is IrcChannel && Equals((IrcChannel)obj));
    /// <inheritdoc/>
    public bool Equals(MessageAuthor other) => this.Name == other.Name || (this.Id != 0 && this.Id == other.Id);
#pragma warning restore CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).

    /// <inheritdoc/>
    public static bool operator ==(MessageAuthor left, MessageAuthor right) => left.Equals(right);
    /// <inheritdoc/>
    public static bool operator !=(MessageAuthor left, MessageAuthor right) => !(left == right);

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        var code = new HashCode();
        code.Add(this.Name);
        code.Add(this.Id);
        return code.ToHashCode();
    }

    /// <summary>
    /// Returns the author's name
    /// </summary>
    public override string ToString() => this.Name;

    /// <inheritdoc/>
    public static implicit operator string(MessageAuthor messageAuthor) => messageAuthor.Name;
    /// <inheritdoc/>
    public static implicit operator long(MessageAuthor messageAuthor) => messageAuthor.Id;
}
