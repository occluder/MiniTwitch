namespace MiniTwitch.Irc.Models;


/// <summary>
/// Contains information about a channel goal.
/// </summary>
public sealed class ChannelGoal
{
    /// <summary>
    /// Whether the channel has a goal set.
    /// </summary>
    public bool HasGoal => TargetContributions > 0;
    /// <summary>
    /// Description of the goal.
    /// </summary>
    public required string Description { get; init; }
    /// <summary>
    /// Target number of contributions for the goal.
    /// </summary>
    public required int TargetContributions { get; init; }
    /// <summary>
    /// Current number of contributions towards the goal.
    /// </summary>
    public required int CurrentContributions { get; init; }
    /// <summary>
    /// How many contributions the user has made towards the goal through this event.
    /// </summary>
    public required int UserContribution { get; init; }

    /// <summary>
    /// The percentage of the goal that has been reached.
    /// </summary>
    public double ProgressPercentage
    {
        get
        {
            if (TargetContributions == 0)
            {
                return 100.0;
            }
            return (double)CurrentContributions / TargetContributions * 100.0;
        }
    }

    /// <summary>
    /// The percentage of the goal that the user has contributed towards.
    /// </summary>
    public double UserContributedPercentage
    {
        get
        {
            if (TargetContributions == 0)
            {
                return 0.0;
            }

            return (double)UserContribution / TargetContributions * 100.0;
        }
    }
}
