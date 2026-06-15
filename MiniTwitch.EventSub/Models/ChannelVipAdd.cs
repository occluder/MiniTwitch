using System.Text.Json.Serialization;
using MiniTwitch.EventSub.Internal;

namespace MiniTwitch.EventSub.Models;

/// <summary>
/// A notification when a VIP is added to the channel
/// </summary>
[EventSubEvent("channel.vip.add", "1")]
public partial struct ChannelVipAdd
{
    /// <summary>The ID of the user who was added as a VIP</summary>
    public partial long UserId { get; }
    /// <summary>The login of the user who was added as a VIP</summary>
    [JsonPropertyName("user_login")]
    public partial string Username { get; }
    /// <summary>The display name of the user who was added as a VIP</summary>
    [JsonPropertyName("user_name")]
    public partial string UserDisplayName { get; }
    /// <summary>The ID of the broadcaster</summary>
    [JsonPropertyName("broadcaster_user_id")]
    public partial long BroadcasterId { get; }
    /// <summary>The login of the broadcaster</summary>
    [JsonPropertyName("broadcaster_user_login"), Intern]
    public partial string BroadcasterUsername { get; }
    /// <summary>The display name of the broadcaster</summary>
    [JsonPropertyName("broadcaster_user_name"), Intern]
    public partial string BroadcasterDisplayName { get; }
}
