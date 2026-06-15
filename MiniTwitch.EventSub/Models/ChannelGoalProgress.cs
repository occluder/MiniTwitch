using System.Text.Json.Serialization;
using MiniTwitch.EventSub.Internal;

namespace MiniTwitch.EventSub.Models;

/// <summary>
/// A notification when progress is made towards the specified broadcaster's goal
/// </summary>
[EventSubEvent("channel.goal.progress", "1")]
public partial struct ChannelGoalProgress
{
    /// <summary>An ID that identifies this event</summary>
    public partial string Id { get; }
    /// <summary>An ID that uniquely identifies the broadcaster</summary>
    [JsonPropertyName("broadcaster_user_id")]
    public partial long BroadcasterId { get; }
    /// <summary>The broadcaster's user handle</summary>
    [JsonPropertyName("broadcaster_user_login"), Intern]
    public partial string BroadcasterUsername { get; }
    /// <summary>The broadcaster's display name</summary>
    [JsonPropertyName("broadcaster_user_name"), Intern]
    public partial string BroadcasterDisplayName { get; }
    /// <summary>
    /// The type of goal. Possible values:
    /// <c>follow</c>, <c>subscription</c>, <c>subscription_count</c>,
    /// <c>new_subscription</c>, <c>new_subscription_count</c>,
    /// <c>new_bit</c>, <c>new_cheerer</c>
    /// </summary>
    [Intern]
    public partial string Type { get; }
    /// <summary>A description of the goal, if specified</summary>
    public partial string Description { get; }
    /// <summary>The goal's current value</summary>
    public partial int CurrentAmount { get; }
    /// <summary>The goal's target value</summary>
    public partial int TargetAmount { get; }
    /// <summary>The UTC timestamp of when the broadcaster created the goal</summary>
    public partial DateTimeOffset StartedAt { get; }
}
