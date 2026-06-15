using System.Text.Json.Serialization;
using MiniTwitch.EventSub.Internal;

namespace MiniTwitch.EventSub.Models;

/// <summary>
/// A notification when a Hype Train ends on the specified channel
/// </summary>
[EventSubEvent("channel.hype_train.end", "2")]
public partial struct ChannelHypeTrainEnd
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
    /// <summary>The contributors with the most points contributed</summary>
    public partial Contribution[] TopContributions { get; }
    /// <summary>The current level of the Hype Train</summary>
    public partial int Level { get; }
    /// <summary>Optional. Non-null for a shared Hype Train. Contains the list of broadcasters in the shared Hype Train</summary>
    public partial SharedTrainParticipant[]? SharedTrainParticipants { get; }
    /// <summary>The time when the Hype Train started</summary>
    public partial DateTimeOffset StartedAt { get; }
    /// <summary>The time when the Hype Train cooldown ends so that the next Hype Train can start</summary>
    public partial DateTimeOffset CooldownEndsAt { get; }
    /// <summary>The time when the Hype Train ended</summary>
    public partial DateTimeOffset EndedAt { get; }
    /// <summary>
    /// The type of the Hype Train. Possible values: <c>treasure</c>, <c>golden_kappa</c>, <c>regular</c>
    /// </summary>
    [Intern]
    public partial string Type { get; }
    /// <summary>Indicates if the Hype Train is shared</summary>
    public partial bool IsSharedTrain { get; }

    /// <summary>A contributor with points contributed to the Hype Train</summary>
    [EventProperty]
    public partial struct Contribution
    {
        /// <summary>The ID of the user that made the contribution</summary>
        public partial long UserId { get; }
        /// <summary>The user's login name</summary>
        [JsonPropertyName("user_login")]
        public partial string Username { get; }
        /// <summary>The user's display name</summary>
        [JsonPropertyName("user_name")]
        public partial string UserDisplayName { get; }
        /// <summary>
        /// The contribution method used. Possible values: <c>bits</c>, <c>subscription</c>, <c>other</c>
        /// </summary>
        [Intern]
        public partial string Type { get; }
        /// <summary>
        /// The total amount contributed.
        /// If type is bits, total represents the amount of Bits used.
        /// If type is subscription, total is 500, 1000, or 2500 to represent tier 1, 2, or 3 subscriptions
        /// </summary>
        public partial int Total { get; }
    }

    /// <summary>A broadcaster participating in a shared Hype Train</summary>
    [EventProperty]
    public partial struct SharedTrainParticipant
    {
        /// <summary>The ID of the broadcaster participating in the shared Hype Train</summary>
        [JsonPropertyName("broadcaster_user_id")]
        public partial long BroadcasterId { get; }
        /// <summary>The login of the broadcaster participating in the shared Hype Train</summary>
        [JsonPropertyName("broadcaster_user_login"), Intern]
        public partial string BroadcasterUsername { get; }
        /// <summary>The display name of the broadcaster participating in the shared Hype Train</summary>
        [JsonPropertyName("broadcaster_user_name"), Intern]
        public partial string BroadcasterDisplayName { get; }
    }
}
