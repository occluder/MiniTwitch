using System.Text.Json.Serialization;
using MiniTwitch.EventSub.Internal;

namespace MiniTwitch.EventSub.Models;

/// <summary>A top predictor user</summary>
[EventProperty]
public partial struct TopPredictor
{
    /// <summary>The user ID of the predictor</summary>
    public partial long UserId { get; }
    /// <summary>The login of the predictor</summary>
    [JsonPropertyName("user_login")]
    public partial string Username { get; }
    /// <summary>The display name of the predictor</summary>
    [JsonPropertyName("user_name")]
    public partial string UserDisplayName { get; }
    /// <summary>The number of Channel Points won (null during progress)</summary>
    public partial int? ChannelPointsWon { get; }
    /// <summary>The number of Channel Points used by the predictor</summary>
    public partial int ChannelPointsUsed { get; }
}
