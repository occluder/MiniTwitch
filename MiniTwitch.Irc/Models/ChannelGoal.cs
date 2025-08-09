namespace MiniTwitch.Irc.Models;


public sealed class ChannelGoal
{
    public bool HasGoal => TargetContributions > 0;
    public required string Description { get; init; }
    public required int TargetContributions { get; init; }
    public required int CurrentContributions { get; init; }
    public required int UserContribution { get; init; }

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
