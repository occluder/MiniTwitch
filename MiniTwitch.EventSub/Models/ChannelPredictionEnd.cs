using System.Text.Json.Serialization;
using MiniTwitch.EventSub.Internal;

namespace MiniTwitch.EventSub.Models;

/// <summary>
/// A notification when a Prediction ends on the specified channel
/// </summary>
[EventSubEvent("channel.prediction.end", "1")]
public partial struct ChannelPredictionEnd
{
    /// <summary>Channel Points Prediction ID</summary>
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
    /// <summary>Title for the Channel Points Prediction</summary>
    public partial string Title { get; }
    /// <summary>ID of the winning outcome</summary>
    public partial string WinningOutcomeId { get; }
    /// <summary>An array of outcomes for the Channel Points Prediction</summary>
    public partial Outcome[] Outcomes { get; }
    /// <summary>The time the Channel Points Prediction started</summary>
    public partial DateTimeOffset StartedAt { get; }
    /// <summary>The time the Channel Points Prediction ended</summary>
    public partial DateTimeOffset EndedAt { get; }

    /// <summary>A prediction outcome</summary>
    [EventProperty]
    public partial struct Outcome
    {
        /// <summary>The outcome ID</summary>
        public partial string Id { get; }
        /// <summary>The outcome title</summary>
        public partial string Title { get; }
        /// <summary>The color for the outcome. Valid values: <c>pink</c> and <c>blue</c></summary>
        public partial string Color { get; }
        /// <summary>The number of users who used Channel Points on this outcome</summary>
        public partial int Users { get; }
        /// <summary>The total number of Channel Points used on this outcome</summary>
        public partial int ChannelPoints { get; }
        /// <summary>An array of users who used the most Channel Points on this outcome</summary>
        public partial TopPredictor[] TopPredictors { get; }
    }

}
