using System.Text;
using MiniTwitch.Irc.Interfaces;
using MiniTwitch.Irc.Internal.Enums;
using MiniTwitch.Irc.Internal.Models;
using MiniTwitch.Irc.Internal.Parsing;

namespace MiniTwitch.Irc.Models;

/// <summary>
/// Represents an IRC chatroom
/// </summary>
public readonly struct IrcChannel : IGazatuChannel, IPartedChannel, IBasicChannel,
    IEmoteOnlyModified, IFollowersOnlyModified, IR9KModified, ISlowModeModified, ISubOnlyModified,
    IEquatable<IrcChannel>, IEquatable<MessageAuthor>, IBanTarget, IDeletedMessageAuthor
{
    /// <inheritdoc/>
    public TimeSpan FollowerModeDuration { get; init; }
    /// <inheritdoc/>
    public TimeSpan SlowModeDuration { get; init; }
    /// <inheritdoc/>
    public string Name { get; init; }
    /// <inheritdoc/>
    public long Id { get; init; }
    /// <inheritdoc/>
    public bool EmoteOnlyEnabled { get; init; } = false;
    /// <inheritdoc/>
    public bool UniqueModeEnabled { get; init; } = false;
    /// <inheritdoc/>
    public bool SubOnlyEnabled { get; init; } = false;
    /// <inheritdoc/>
    public bool FollowerModeEnabled { get; init; } = false;
    /// <inheritdoc/>
    public bool SlowModeEnabled { get; init; } = false;

    internal RoomstateType Roomstate { get; init; } = RoomstateType.Unknown;

    private static readonly TimeSpan _followersOnlyOffTimeSpan = TimeSpan.FromMinutes(-1);

    internal IrcChannel(ref IrcMessage message)
    {
        int followerModeDuration = -1;
        int slowModeDuration = 0;
        // do this otherwise it will include '\r\n' in the name
        string name = message.GetChannel();
        long id = 0;

        bool emoteOnlyModified = false;
        bool uniqueModeModified = false;
        bool subModeModified = false;
        bool followerModeModified = false;
        bool slowModeModified = false;

        using IrcTags tags = message.ParseTags();
        foreach (IrcTag tag in tags)
        {
            ReadOnlySpan<byte> tagKey = tag.Key.Span;
            ReadOnlySpan<byte> tagValue = tag.Value.Span;
            switch (tagKey.Length)
            {
                //r9k
                case (int)Tags.R9K when tagKey.SequenceEqual("r9k"u8):
                    this.UniqueModeEnabled = TagHelper.GetBool(tagValue);
                    uniqueModeModified = true;
                    break;

                //slow
                case (int)Tags.Slow when tagKey.SequenceEqual("slow"u8):
                    slowModeDuration = TagHelper.GetInt(tagValue);
                    slowModeModified = true;
                    break;

                //room-id
                case (int)Tags.RoomId when tagKey.SequenceEqual("room-id"u8):
                    id = TagHelper.GetLong(tagValue);
                    break;

                //subs-only
                case (int)Tags.SubsOnly when tagKey.SequenceEqual("subs-only"u8):
                    this.SubOnlyEnabled = TagHelper.GetBool(tagValue);
                    subModeModified = true;
                    break;

                //emote-only
                case (int)Tags.EmoteOnly when tagKey.SequenceEqual("emote-only"u8):
                    this.EmoteOnlyEnabled = TagHelper.GetBool(tagValue);
                    emoteOnlyModified = true;
                    break;

                //followers-only
                case (int)Tags.FollowersOnly when tagKey.SequenceEqual("followers-only"u8):
                    followerModeDuration = TagHelper.GetInt(tagValue);
                    followerModeModified = true;
                    break;
            }
        }

        if (emoteOnlyModified
        && uniqueModeModified
        && subModeModified
        && followerModeModified
        && slowModeModified)
        {
            this.Roomstate = RoomstateType.All;
        }
        else if (emoteOnlyModified)
        {
            this.Roomstate = RoomstateType.EmoteOnly;
        }
        else if (uniqueModeModified)
        {
            this.Roomstate = RoomstateType.R9K;
        }
        else if (subModeModified)
        {
            this.Roomstate = RoomstateType.SubOnly;
        }
        else if (followerModeModified)
        {
            this.Roomstate = RoomstateType.FollowerOnly;
        }
        else if (slowModeModified)
        {
            this.Roomstate = RoomstateType.Slow;
        }

        this.FollowerModeEnabled = followerModeDuration != -1;
        this.FollowerModeDuration = followerModeDuration == -1 ? _followersOnlyOffTimeSpan : TimeSpan.FromMinutes(followerModeDuration);
        this.SlowModeEnabled = slowModeDuration != 0;
        this.SlowModeDuration = slowModeDuration == 0 ? TimeSpan.Zero : TimeSpan.FromSeconds(slowModeDuration);
        this.Name = name;
        this.Id = id;
    }

    /// <summary>
    /// Construct a channel from a string. Useful for testing
    /// </summary>
    /// <param name="rawData">The raw IRC message <para>Example input: @emote-only=0;followers-only=-1;r9k=0;room-id=783267696;slow=0;subs-only=0 :tmi.twitch.tv ROOMSTATE #occluder</para></param>
    /// <returns><see cref="IrcChannel"/> with the related data</returns>
    public static IrcChannel Construct(string rawData)
    {
        ReadOnlyMemory<byte> memory = new(Encoding.UTF8.GetBytes(rawData));
        var message = new IrcMessage(memory);
        return new(ref message);
    }

#pragma warning disable CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).
    /// <inheritdoc/>
    public bool Equals(IrcChannel other) => this.Name == other.Name;
    /// <inheritdoc/>
    public override bool Equals(object obj) => (obj is IrcChannel && Equals((IrcChannel)obj)) || (obj is MessageAuthor && Equals((MessageAuthor)obj));
    /// <inheritdoc/>
    public bool Equals(MessageAuthor other) => this.Name == other.Name || (this.Id != 0 && this.Id == other.Id);
#pragma warning restore CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).

    /// <inheritdoc/>
    public static bool operator ==(IrcChannel left, IrcChannel right) => left.Equals(right);
    /// <inheritdoc/>
    public static bool operator !=(IrcChannel left, IrcChannel right) => !(left == right);

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        var code = new HashCode();
        code.Add(this.Name);
        code.Add(this.Id);
        return code.ToHashCode();
    }

    /// <summary>
    /// Returns the channel's name
    /// </summary>
    public override string ToString() => this.Name;

    /// <inheritdoc/>
    public static implicit operator string(IrcChannel channel) => channel.Name;
    /// <inheritdoc/>
    public static implicit operator long(IrcChannel channel) => channel.Id;
}
