using System.Text.Json.Serialization;
using MiniTwitch.EventSub.Internal;

namespace MiniTwitch.EventSub.Models;

/// <summary>
/// A notification when a Hype Train makes progress on the specified channel
/// </summary>
[EventSubEvent("channel.hype_train.progress", "2")]
public partial struct ChannelHypeTrainProgress
{
    /// <summary>The Hype Train ID</summary>
    public partial string Id { get; }
    /// <summary>The requested broadcaster ID</summary>
    [JsonPropertyName("broadcaster_user_id")]
    public partial long BroadcasterId { get; }
    /// <summary>The requested broadcaster login</summary>
    [JsonPropertyName("broadcaster_user_login"), Intern]
    public partial string BroadcasterUsername { get; }
    /// <summary>The requested broadcaster display name</summary>
    [JsonPropertyName("broadcaster_user_name"), Intern]
    public partial string BroadcasterDisplayName { get; }
    /// <summary>Total points contributed to the Hype Train</summary>
    public partial int Total { get; }
    /// <summary>The number of points contributed to the Hype Train at the current level</summary>
    public partial int Progress { get; }
    /// <summary>The number of points required to reach the next level</summary>
    public partial int Goal { get; }
    /// <summary>The contributors with the most points contributed</summary>
    public partial Contribution[] TopContributions { get; }
    /// <summary>The current level of the Hype Train</summary>
    public partial int Level { get; }
    /// <summary>The all-time high level this type of Hype Train has reached for this broadcaster</summary>
    public partial int AllTimeHighLevel { get; }
    /// <summary>The all-time high total this type of Hype Train has reached for this broadcaster</summary>
    public partial int AllTimeHighTotal { get; }
    /// <summary>Optional. Non-null for a shared Hype Train. Contains the list of broadcasters in the shared Hype Train</summary>
    public partial SharedTrainParticipant[]? SharedTrainParticipants { get; }
    /// <summary>The time when the Hype Train started</summary>
    public partial DateTimeOffset StartedAt { get; }
    /// <summary>The time when the Hype Train expires</summary>
    public partial DateTimeOffset ExpiresAt { get; }
    /// <summary>
    /// The type of the Hype Train. Possible values: <c>treasure</c>, <c>golden_kappa</c>, <c>regular</c>
    /// </summary>
    [Intern]
    public partial string Type { get; }
    /// <summary>Indicates if the Hype Train is shared</summary>
    public partial bool IsSharedTrain { get; }

}
