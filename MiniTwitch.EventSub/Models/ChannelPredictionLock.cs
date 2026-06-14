using System.Text.Json.Serialization;
using MiniTwitch.EventSub.Internal;

namespace MiniTwitch.EventSub.Models;

/// <summary>
/// A notification when a Prediction is locked on the specified channel
/// </summary>
[EventSubEvent("channel.prediction.lock", "1")]
public partial struct ChannelPredictionLock
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
    /// <summary>An array of outcomes for the Channel Points Prediction</summary>
    public partial Outcome[] Outcomes { get; }
    /// <summary>The time the Channel Points Prediction started</summary>
    public partial DateTimeOffset StartedAt { get; }
    /// <summary>The time the Channel Points Prediction will automatically lock</summary>
    public partial DateTimeOffset LocksAt { get; }

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

    /// <summary>A top predictor user</summary>
    [EventProperty]
    public partial struct TopPredictor
    {
        /// <summary>The user ID of the predictor</summary>
        public partial long UserId { get; }
        /// <summary>The login of the predictor</summary>
        [JsonPropertyName("user_login")]
        public partial string Username { get; }
        /// <summary>The display name of the predictor</summary>
        [JsonPropertyName("user_name")]
        public partial string UserDisplayName { get; }
        /// <summary>The number of Channel Points won (null during progress)</summary>
        public partial int? ChannelPointsWon { get; }
        /// <summary>The number of Channel Points used by the predictor</summary>
        public partial int ChannelPointsUsed { get; }
    }
}
