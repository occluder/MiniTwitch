using System.Text.Json.Serialization;
using MiniTwitch.EventSub.Internal;

namespace MiniTwitch.EventSub.Models;

/// <summary>
/// A notification when a user's authorization has been granted to your client id
/// </summary>
[EventSubEvent("user.authorization.grant", "1")]
public partial struct UserAuthorizationGrant
{
    /// <summary>The client id of the application that was granted user access</summary>
    public partial string ClientId { get; }
    /// <summary>The user id for the user who has granted authorization for your client id</summary>
    public partial long UserId { get; }
    /// <summary>The user login for the user who has granted authorization for your client id</summary>
    [JsonPropertyName("user_login")]
    public partial string Username { get; }
    /// <summary>The user display name for the user who has granted authorization for your client id</summary>
    [JsonPropertyName("user_name")]
    public partial string UserDisplayName { get; }
}
