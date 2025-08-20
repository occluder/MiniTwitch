using MiniTwitch.Irc.Enums;
using MiniTwitch.Irc.Models;

namespace MiniTwitch.Irc.Interfaces;

/// <summary>
/// Contains information about a single gifted subscription
/// </summary>
public interface IGiftSubNotice : IUsernotice
{
    /// <summary>
    /// The message emitted in chat when the user gifted the subscription
    /// <para>Example: Goop_456789 gifted a Tier 1 sub to Zackpanjang! They have given 11 Gift Subs in the channel!</para>
    /// </summary>
    string SystemMessage { get; }
    /// <summary>
    /// The recipient of the gift subscription
    /// </summary>
    IGiftSubRecipient Recipient { get; }
    /// <summary>
    /// Name of the subscription plan
    /// <para>Example 1: Channel Subscription (mandeow)</para>
    /// <para>Example 2: Channel Subscription (forsenlol)</para>
    /// </summary>
    string SubPlanName { get; }
    /// <summary>
    /// The cumulative amount of months the recipient has been subscribed
    /// </summary>
    int Months { get; }
    /// <summary>
    /// The amount of months the recipient received in the gift subscription
    /// </summary>
    int GiftedMonths { get; }
    /// <summary>
    /// Total amount of the gifts the author has given
    /// </summary>
    int TotalGiftCount { get; }
    /// <summary>
    /// The tier of the gift subscription
    /// </summary>
    SubPlan SubPlan { get; }
    /// <summary>
    /// Id of the parent <see cref="IGiftSubNoticeIntro"/> gift message
    /// </summary>
    ulong CommunityGiftId { get; }
    /// <inheritdoc cref="Usernotice.Source"/>
    NoticeSource Source { get; }
    /// <inheritdoc cref="Usernotice.ChannelGoal"/>
    ChannelGoal ChannelGoal { get; }
}
