using System.Text.Json.Serialization;
using MiniTwitch.EventSub.Internal;

namespace MiniTwitch.EventSub.Models;

/// <summary>Metadata associated with a raid action</summary>
[EventProperty]
public partial struct ActionRaid
{
    /// <summary>The ID of the user being raided</summary>
    public partial long UserId { get; }
    /// <summary>The login of the user being raided</summary>
    [JsonPropertyName("user_login")]
    public partial string Username { get; }
    /// <summary>The user name of the user raided</summary>
    [JsonPropertyName("user_name")]
    public partial string UserDisplayName { get; }
    /// <summary>The viewer count</summary>
    public partial int ViewerCount { get; }
}
