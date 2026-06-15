using System.Text.Json.Serialization;
using MiniTwitch.EventSub.Internal;

namespace MiniTwitch.EventSub.Models;

/// <summary>Metadata associated with a ban action</summary>
[EventProperty]
public partial struct ActionBan
{
    /// <summary>The ID of the user being banned</summary>
    public partial long UserId { get; }
    /// <summary>The login of the user being banned</summary>
    [JsonPropertyName("user_login")]
    public partial string Username { get; }
    /// <summary>The user name of the user being banned</summary>
    [JsonPropertyName("user_name")]
    public partial string UserDisplayName { get; }
    /// <summary>Reason given for the ban</summary>
    public partial string? Reason { get; }
}
