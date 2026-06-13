using System.Text.Json.Serialization;
using MiniTwitch.EventSub.Internal;

namespace MiniTwitch.EventSub.Models;

/// <summary>
/// A notification when a broadcaster updates the category, title,
/// content classification labels, or broadcast language for their channel
/// </summary>
[EventSubEvent("channel.update", "2")]
public partial struct ChannelUpdate
{
    /// <summary>The broadcaster's user ID</summary>
    [JsonPropertyName("broadcaster_user_id")]
    public partial long BroadcasterId { get; }
    /// <summary>The broadcaster's user login</summary>
    [JsonPropertyName("broadcaster_user_login"), Intern]
    public partial string BroadcasterUsername { get; }
    /// <summary>The broadcaster's user display name</summary>
    [JsonPropertyName("broadcaster_user_name"), Intern]
    public partial string BroadcasterDisplayName { get; }
    /// <summary>The channel's stream title</summary>
    public partial string Title { get; }
    /// <summary>The channel's broadcast language</summary>
    public partial string Language { get; }
    /// <summary>The channel's category ID</summary>
    public partial string CategoryId { get; }
    /// <summary>The category name</summary>
    public partial string CategoryName { get; }
    /// <summary>
    /// Array of content classification label IDs currently applied on the channel
    /// </summary>
    [Intern]
    public partial string[] ContentClassificationLabels { get; }
}
