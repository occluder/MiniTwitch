using System.Text.Json.Serialization;
using MiniTwitch.EventSub.Internal;

namespace MiniTwitch.EventSub.Models;

/// <summary>
/// A notification when a broadcaster's automod terms are updated.
/// Changes to private terms are not sent
/// </summary>
[EventSubEvent("automod.terms.update", "1")]
public partial struct AutomodTermsUpdate
{
    /// <summary>The ID of the broadcaster specified in the request</summary>
    [JsonPropertyName("broadcaster_user_id")]
    public partial long BroadcasterId { get; }
    /// <summary>The login of the broadcaster specified in the request</summary>
    [JsonPropertyName("broadcaster_user_login"), Intern]
    public partial string BroadcasterUsername { get; }
    /// <summary>The user name of the broadcaster specified in the request</summary>
    [JsonPropertyName("broadcaster_user_name"), Intern]
    public partial string BroadcasterDisplayName { get; }
    /// <summary>The ID of the moderator who changed the channel settings</summary>
    [JsonPropertyName("moderator_user_id")]
    public partial long ModeratorId { get; }
    /// <summary>The moderator's login</summary>
    [JsonPropertyName("moderator_user_login")]
    public partial string ModeratorUsername { get; }
    /// <summary>The moderator's user name</summary>
    [JsonPropertyName("moderator_user_name")]
    public partial string ModeratorDisplayName { get; }
    /// <summary>
    /// The status change applied to the terms. Possible values:
    /// <c>add_permitted</c>, <c>remove_permitted</c>, <c>add_blocked</c>, <c>remove_blocked</c>
    /// </summary>
    [Intern]
    public partial string Action { get; }
    /// <summary>Indicates whether this term was added due to an Automod message approve/deny action</summary>
    public partial bool FromAutomod { get; }
    /// <summary>The list of terms that had a status change</summary>
    public partial string[] Terms { get; }
}
