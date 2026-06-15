using System.Text.Json.Serialization;
using MiniTwitch.EventSub.Internal;

namespace MiniTwitch.EventSub.Models;

/// <summary>Metadata associated with an unban request</summary>
[EventProperty]
public partial struct ActionUnbanRequest
{
    /// <summary>Whether or not the unban request was approved</summary>
    public partial bool IsApproved { get; }
    /// <summary>The ID of the banned user</summary>
    public partial long UserId { get; }
    /// <summary>The login of the user</summary>
    [JsonPropertyName("user_login")]
    public partial string Username { get; }
    /// <summary>The user name of the user</summary>
    [JsonPropertyName("user_name")]
    public partial string UserDisplayName { get; }
    /// <summary>The message included by the moderator explaining their decision</summary>
    public partial string ModeratorMessage { get; }
}
