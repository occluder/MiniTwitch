using System.Text.Json.Serialization;
using MiniTwitch.EventSub.Internal;

namespace MiniTwitch.EventSub.Models;

[EventProperty]
public partial struct Mention
{
    public partial long UserId { get; }
    [JsonPropertyName("user_login")]
    public partial string Username { get; }
    [JsonPropertyName("user_name")]
    public partial string UserDisplayName { get; }
}
