using System.Text.Json.Serialization;
using MiniTwitch.EventSub.Internal;

namespace MiniTwitch.EventSub.Models;

/// <summary>
/// A notification when an unban request has been resolved
/// </summary>
[EventSubEvent("channel.unban_request.resolve", "1")]
public partial struct ChannelUnbanRequestResolve
{
    /// <summary>The ID of the unban request</summary>
    public partial string Id { get; }
    /// <summary>The broadcaster's user ID for the channel the unban request was updated for</summary>
    [JsonPropertyName("broadcaster_user_id")]
    public partial long BroadcasterId { get; }
    /// <summary>The broadcaster's login name</summary>
    [JsonPropertyName("broadcaster_user_login"), Intern]
    public partial string BroadcasterUsername { get; }
    /// <summary>The broadcaster's display name</summary>
    [JsonPropertyName("broadcaster_user_name"), Intern]
    public partial string BroadcasterDisplayName { get; }
    /// <summary>User ID of moderator who approved/denied the request</summary>
    [JsonPropertyName("moderator_user_id")]
    public partial long ModeratorId { get; }
    /// <summary>The moderator's login name</summary>
    [JsonPropertyName("moderator_user_login")]
    public partial string ModeratorUsername { get; }
    /// <summary>The moderator's display name</summary>
    [JsonPropertyName("moderator_user_name")]
    public partial string ModeratorDisplayName { get; }
    /// <summary>User ID of user that requested to be unbanned</summary>
    public partial long UserId { get; }
    /// <summary>The user's login name</summary>
    [JsonPropertyName("user_login")]
    public partial string Username { get; }
    /// <summary>The user's display name</summary>
    [JsonPropertyName("user_name")]
    public partial string UserDisplayName { get; }
    /// <summary>Resolution text supplied by the mod upon approval/denial</summary>
    public partial string ResolutionText { get; }
    /// <summary>
    /// Whether the unban request was approved or denied.
    /// Possible values: <c>approved</c>, <c>canceled</c>, <c>denied</c>
    /// </summary>
    public partial string Status { get; }
}
