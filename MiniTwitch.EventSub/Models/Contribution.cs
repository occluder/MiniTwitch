using System.Text.Json.Serialization;
using MiniTwitch.EventSub.Internal;

namespace MiniTwitch.EventSub.Models;

/// <summary>A contributor with points contributed to the Hype Train</summary>
[EventProperty]
public partial struct Contribution
{
    /// <summary>The ID of the user that made the contribution</summary>
    public partial long UserId { get; }
    /// <summary>The user's login name</summary>
    [JsonPropertyName("user_login")]
    public partial string Username { get; }
    /// <summary>The user's display name</summary>
    [JsonPropertyName("user_name")]
    public partial string UserDisplayName { get; }
    /// <summary>
    /// The contribution method used. Possible values: <c>bits</c>, <c>subscription</c>, <c>other</c>
    /// </summary>
    [Intern]
    public partial string Type { get; }
    /// <summary>
    /// The total amount contributed.
    /// If type is bits, total represents the amount of Bits used.
    /// If type is subscription, total is 500, 1000, or 2500 to represent tier 1, 2, or 3 subscriptions
    /// </summary>
    public partial int Total { get; }
}
