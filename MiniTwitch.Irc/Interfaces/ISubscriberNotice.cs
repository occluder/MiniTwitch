using MiniTwitch.Irc.Enums;
using MiniTwitch.Irc.Models;

namespace MiniTwitch.Irc.Interfaces;

/// <summary>
/// Represents a user subscription
/// </summary>
public interface ISubNotice : IUsernotice
{
    /// <summary>
    /// Emote sets in the resubscription message
    /// <para><see cref="string.Empty"/> if there are none, or if the user is a first time sub</para>
    /// </summary>
    string Emotes { get; }
    /// <summary>
    /// Automod flags in the resubscription message
    /// <para><see cref="string.Empty"/> if there are none, or if the user is a first time sub</para>
    /// </summary>
    string Flags { get; }
    /// <summary>
    /// The message emitted in chat when the event occurs
    /// <para>Example 1: SleepyHeadszZ subscribed at Tier 1.</para>
    /// <para>Example 2: Syn993 subscribed at Tier 1. They've subscribed for 5 months, currently on a 4 month streak!</para>
    /// </summary>
    string SystemMessage { get; }
    /// <summary>
    /// Cumulative amount of months the user has been subscribed
    /// </summary>
    int CumulativeMonths { get; }
    /// <summary>
    /// Whether the user shared their month streak in the subscription message or not
    /// </summary>
    bool ShouldShareStreak { get; }
    /// <summary>
    /// How many months in a row the user has been subscribed
    /// <para>Note: Always 0 if <see cref="ShouldShareStreak"/> is <see langword="false"/></para>
    /// </summary>
    int MonthStreak { get; }
    /// <summary>
    /// The tier of the subscription
    /// </summary>
    SubPlan SubPlan { get; }
    /// <summary>
    /// Name of the subscription plan
    /// <para>Example 1: Channel Subscription (mandeow)</para>
    /// <para>Example 2: Channel Subscription (forsenlol)</para>
    /// </summary>
    string SubPlanName { get; }
    /// <summary>
    /// The user's resubscription message content
    /// <para>Note 1: Always <see cref="string.Empty"/> for first time subscribers</para>
    /// <para>Note 2: May be <see cref="string.Empty"/> even for resubscriptions</para>
    /// </summary>
    string Message { get; }
    /// <inheritdoc cref="Usernotice.Source"/>
    NoticeSource Source { get; }
}
