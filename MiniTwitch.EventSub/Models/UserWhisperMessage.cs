using System.Text.Json.Serialization;
using MiniTwitch.EventSub.Internal;

namespace MiniTwitch.EventSub.Models;

/// <summary>
/// A notification when a user receives a whisper
/// </summary>
[EventSubEvent("user.whisper.message", "1")]
public partial struct UserWhisperMessage
{
    /// <summary>The ID of the user sending the message</summary>
    public partial long FromUserId { get; }
    /// <summary>The login of the user sending the message</summary>
    [JsonPropertyName("from_user_login")]
    public partial string FromUsername { get; }
    /// <summary>The name of the user sending the message</summary>
    [JsonPropertyName("from_user_name")]
    public partial string FromUserDisplayName { get; }
    /// <summary>The ID of the user receiving the message</summary>
    public partial long ToUserId { get; }
    /// <summary>The login of the user receiving the message</summary>
    [JsonPropertyName("to_user_login")]
    public partial string ToUsername { get; }
    /// <summary>The name of the user receiving the message</summary>
    [JsonPropertyName("to_user_name")]
    public partial string ToUserDisplayName { get; }
    /// <summary>The whisper ID</summary>
    public partial string WhisperId { get; }
    /// <summary>Object containing whisper information</summary>
    public partial WhisperContent Whisper { get; }

    /// <summary>Object containing whisper information</summary>
    [EventProperty]
    public partial struct WhisperContent
    {
        /// <summary>The body of the whisper message</summary>
        public partial string Text { get; }
    }
}
