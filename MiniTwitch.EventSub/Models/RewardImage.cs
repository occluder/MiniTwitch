using System.Text.Json.Serialization;
using MiniTwitch.EventSub.Internal;

namespace MiniTwitch.EventSub.Models;

/// <summary>Reward image URLs</summary>
[EventProperty]
public partial struct RewardImage
{
    /// <summary>URL for 1x size image</summary>
    [JsonPropertyName("url_1x")]
    public partial string Url1x { get; }
    /// <summary>URL for 2x size image</summary>
    [JsonPropertyName("url_2x")]
    public partial string Url2x { get; }
    /// <summary>URL for 4x size image</summary>
    [JsonPropertyName("url_4x")]
    public partial string Url4x { get; }
}
