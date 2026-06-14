using System.Text.Json.Serialization;
using MiniTwitch.EventSub.Internal;

namespace MiniTwitch.EventSub.Models;

/// <summary>
/// A notification when a user creates an unban request
/// </summary>
[EventSubEvent("channel.unban_request.create", "1")]
public partial struct ChannelUnbanRequestCreate
{
    /// <summary>The ID of the unban request</summary>
    public partial string Id { get; }
    /// <summary>The broadcaster's user ID for the channel the unban request was created for</summary>
    [JsonPropertyName("broadcaster_user_id")]
    public partial long BroadcasterId { get; }
    /// <summary>The broadcaster's login name</summary>
    [JsonPropertyName("broadcaster_user_login"), Intern]
    public partial string BroadcasterUsername { get; }
    /// <summary>The broadcaster's display name</summary>
    [JsonPropertyName("broadcaster_user_name"), Intern]
    public partial string BroadcasterDisplayName { get; }
    /// <summary>User ID of user that is requesting to be unbanned</summary>
    public partial long UserId { get; }
    /// <summary>The user's login name</summary>
    [JsonPropertyName("user_login")]
    public partial string Username { get; }
    /// <summary>The user's display name</summary>
    [JsonPropertyName("user_name")]
    public partial string UserDisplayName { get; }
    /// <summary>Message sent in the unban request</summary>
    public partial string Text { get; }
    /// <summary>The timestamp of when the unban request was created</summary>
    public partial DateTimeOffset CreatedAt { get; }
}
