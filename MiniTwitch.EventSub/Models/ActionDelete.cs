using System.Text.Json.Serialization;
using MiniTwitch.EventSub.Internal;

namespace MiniTwitch.EventSub.Models;

/// <summary>Metadata associated with a delete action</summary>
[EventProperty]
public partial struct ActionDelete
{
    /// <summary>The ID of the user whose message is being deleted</summary>
    public partial long UserId { get; }
    /// <summary>The login of the user</summary>
    [JsonPropertyName("user_login")]
    public partial string Username { get; }
    /// <summary>The user name of the user</summary>
    [JsonPropertyName("user_name")]
    public partial string UserDisplayName { get; }
    /// <summary>The ID of the message being deleted</summary>
    public partial string MessageId { get; }
    /// <summary>The message body of the message being deleted</summary>
    public partial string MessageBody { get; }
}
