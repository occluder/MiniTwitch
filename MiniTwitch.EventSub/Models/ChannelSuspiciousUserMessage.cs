using System.Text.Json.Serialization;
using MiniTwitch.EventSub.Internal;

namespace MiniTwitch.EventSub.Models;

/// <summary>
/// A notification when a chat message has been sent from a suspicious user
/// </summary>
[EventSubEvent("channel.suspicious_user.message", "1")]
public partial struct ChannelSuspiciousUserMessage
{
    /// <summary>The ID of the channel where the treatment for a suspicious user was updated</summary>
    [JsonPropertyName("broadcaster_user_id")]
    public partial long BroadcasterId { get; }
    /// <summary>The login of the channel where the treatment for a suspicious user was updated</summary>
    [JsonPropertyName("broadcaster_user_login"), Intern]
    public partial string BroadcasterUsername { get; }
    /// <summary>The display name of the channel where the treatment for a suspicious user was updated</summary>
    [JsonPropertyName("broadcaster_user_name"), Intern]
    public partial string BroadcasterDisplayName { get; }
    /// <summary>The user ID of the user that sent the message</summary>
    public partial long UserId { get; }
    /// <summary>The user login of the user that sent the message</summary>
    [JsonPropertyName("user_login")]
    public partial string Username { get; }
    /// <summary>The user name of the user that sent the message</summary>
    [JsonPropertyName("user_name")]
    public partial string UserDisplayName { get; }
    /// <summary>
    /// The status set for the suspicious user.
    /// Possible values: <c>none</c>, <c>active_monitoring</c>, <c>restricted</c>
    /// </summary>
    [Intern]
    public partial string LowTrustStatus { get; }
    /// <summary>A list of channel IDs where the suspicious user is also banned</summary>
    public partial long[] SharedBanChannelIds { get; }
    /// <summary>
    /// User types (if any) that apply to the suspicious user.
    /// Possible values: <c>manually_added</c>, <c>ban_evader</c>, <c>banned_in_shared_channel</c>
    /// </summary>
    [Intern]
    public partial string[] Types { get; }
    /// <summary>
    /// A ban evasion likelihood value (if any) that has been applied to the user automatically by Twitch.
    /// Possible values: <c>unknown</c>, <c>possible</c>, <c>likely</c>
    /// </summary>
    [Intern]
    public partial string? BanEvasionEvaluation { get; }
    /// <summary>The structured chat message</summary>
    public partial SuspiciousMessage Message { get; }

    /// <summary>The structured chat message</summary>
    [EventProperty]
    public partial struct SuspiciousMessage
    {
        /// <summary>The UUID that identifies the message</summary>
        public partial string MessageId { get; }
        /// <summary>The chat message in plain text</summary>
        public partial string Text { get; }
        /// <summary>Ordered list of chat message fragments</summary>
        public partial MessageFragment[] Fragments { get; }
    }
}
