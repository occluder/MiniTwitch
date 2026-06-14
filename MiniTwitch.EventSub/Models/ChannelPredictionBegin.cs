using System.Text.Json.Serialization;
using MiniTwitch.EventSub.Internal;

namespace MiniTwitch.EventSub.Models;

/// <summary>
/// A notification when a Prediction begins on the specified channel
/// </summary>
[EventSubEvent("channel.prediction.begin", "1")]
public partial struct ChannelPredictionBegin
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
        /// <summary>ID for the outcome</summary>
        public partial string Id { get; }
        /// <summary>Text displayed for the outcome</summary>
        public partial string Title { get; }
        /// <summary>The color of the outcome. Possible values: <c>blue</c> or <c>pink</c></summary>
        public partial string Color { get; }
    }
}
