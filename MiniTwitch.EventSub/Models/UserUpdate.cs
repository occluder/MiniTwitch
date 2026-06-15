using System.Text.Json.Serialization;
using MiniTwitch.EventSub.Internal;

namespace MiniTwitch.EventSub.Models;

/// <summary>
/// A notification when a user updates their account
/// </summary>
[EventSubEvent("user.update", "1")]
public partial struct UserUpdate
{
    /// <summary>The user's user id</summary>
    public partial long UserId { get; }
    /// <summary>The user's user login</summary>
    [JsonPropertyName("user_login")]
    public partial string Username { get; }
    /// <summary>The user's user display name</summary>
    [JsonPropertyName("user_name")]
    public partial string UserDisplayName { get; }
    /// <summary>The user's email address. Empty string if the <c>user:read:email</c> scope was not included</summary>
    public partial string Email { get; }
    /// <summary>Whether Twitch has verified the user's email address. Ignore if <c>Email</c> is empty</summary>
    public partial bool EmailVerified { get; }
    /// <summary>The user's description</summary>
    public partial string Description { get; }
}
