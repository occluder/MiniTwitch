using MiniTwitch.Irc.Enums;
using MiniTwitch.Irc.Models;

namespace MiniTwitch.Irc.Interfaces;

/// <summary>
/// Contains information about a user that changed their prime subscription into a paid one
/// </summary>
public interface IPrimeUpgradeNotice : IUsernotice
{
    /// <summary>
    /// The message emitted in chat when the event occurs
    /// <para>Example: DrDisRespexs converted from a Prime sub to a Tier 1 sub!</para>
    /// </summary>
    string SystemMessage { get; }
    /// <summary>
    /// The tier of the new subscription
    /// </summary>
    SubPlan SubPlan { get; }
    /// <inheritdoc cref="Usernotice.Source"/>
    NoticeSource Source { get; }
    /// <inheritdoc cref="Usernotice.ChannelGoal"/>
    ChannelGoal ChannelGoal { get; }
}
