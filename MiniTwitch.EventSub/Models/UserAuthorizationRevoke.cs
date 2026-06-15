using System.Text.Json.Serialization;
using MiniTwitch.EventSub.Internal;

namespace MiniTwitch.EventSub.Models;

/// <summary>
/// A notification when a user's authorization has been revoked for your client id
/// </summary>
[EventSubEvent("user.authorization.revoke", "1")]
public partial struct UserAuthorizationRevoke
{
    /// <summary>The client id of the application with revoked user access</summary>
    public partial string ClientId { get; }
    /// <summary>The user id for the user who has revoked authorization for your client id</summary>
    public partial long UserId { get; }
    /// <summary>The user login for the user who has revoked authorization for your client id. Null if the user no longer exists</summary>
    [JsonPropertyName("user_login")]
    public partial string? Username { get; }
    /// <summary>The user display name for the user who has revoked authorization for your client id. Null if the user no longer exists</summary>
    [JsonPropertyName("user_name")]
    public partial string? UserDisplayName { get; }
}
