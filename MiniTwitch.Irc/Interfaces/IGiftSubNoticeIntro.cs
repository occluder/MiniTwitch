using MiniTwitch.Irc.Enums;
using MiniTwitch.Irc.Models;

namespace MiniTwitch.Irc.Interfaces;

/// <summary>
/// contains information about a user who is about to gift one or more subscriptions to the community
/// </summary>
public interface IGiftSubNoticeIntro : IUsernotice
{
    /// <summary>
    /// The message emitted in chat when the event occurs
    /// <para>Example: xHypnoticPowerx is gifting 25 Tier 1 Subs to Mande's community! They've gifted a total of 62 in the channel!</para>
    /// </summary>
    string SystemMessage { get; }
    /// <summary>
    /// The amount of subscriptions the author is gifting
    /// </summary>
    int GiftCount { get; }
    /// <summary>
    /// Total amount of the gifts the author has given
    /// </summary>
    int TotalGiftCount { get; }
    /// <summary>
    /// The tier of the gift subscriptions
    /// </summary>
    SubPlan SubPlan { get; }
    /// <summary>
    /// Id of the gift message to link it with <see cref="IGiftSubNotice"/> messages
    /// </summary>
    ulong CommunityGiftId { get; }
    /// <inheritdoc cref="Usernotice.Source"/>
    NoticeSource Source { get; }
}
