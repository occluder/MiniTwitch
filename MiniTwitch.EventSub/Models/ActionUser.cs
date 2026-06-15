using System.Text.Json.Serialization;
using MiniTwitch.EventSub.Internal;

namespace MiniTwitch.EventSub.Models;

/// <summary>Metadata associated with a moderation action targeting a user</summary>
[EventProperty]
public partial struct ActionUser
{
    /// <summary>The ID of the user</summary>
    public partial long UserId { get; }
    /// <summary>The login of the user</summary>
    [JsonPropertyName("user_login")]
    public partial string Username { get; }
    /// <summary>The user name of the user</summary>
    [JsonPropertyName("user_name")]
    public partial string UserDisplayName { get; }
}
