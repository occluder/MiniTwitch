using System.Text.Json.Serialization;
using MiniTwitch.EventSub.Internal;

namespace MiniTwitch.EventSub.Models;

/// <summary>Metadata pertaining to the mention</summary>
[EventProperty]
public partial struct Mention
{
    /// <summary>The user ID of the mentioned user</summary>
    public partial long UserId { get; }
    /// <summary>The user login of the mentioned user</summary>
    [JsonPropertyName("user_login")]
    public partial string Username { get; }
    /// <summary>The user name of the mentioned user</summary>
    [JsonPropertyName("user_name")]
    public partial string UserDisplayName { get; }
}
