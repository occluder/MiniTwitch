using System.Text.Json.Serialization;
using MiniTwitch.EventSub.Internal;

namespace MiniTwitch.EventSub.Models;

/// <summary>
/// A notification when a broadcaster's automod settings are updated
/// </summary>
[EventSubEvent("automod.settings.update", "1")]
public partial struct AutomodSettingsUpdate
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
    /// The default AutoMod level for the broadcaster.
    /// This field is null if the broadcaster has set one or more of the individual settings
    /// </summary>
    public partial int? OverallLevel { get; }
    /// <summary>The Automod level for hostility involving name calling or insults</summary>
    public partial int Bullying { get; }
    /// <summary>The Automod level for discrimination against disability</summary>
    public partial int Disability { get; }
    /// <summary>The Automod level for hostility involving aggression</summary>
    public partial int Aggression { get; }
    /// <summary>The AutoMod level for discrimination based on sexuality, sex, or gender</summary>
    public partial int SexualitySexOrGender { get; }
    /// <summary>The Automod level for discrimination against women</summary>
    public partial int Misogyny { get; }
    /// <summary>The Automod level for profanity</summary>
    public partial int Swearing { get; }
    /// <summary>The Automod level for racial discrimination</summary>
    public partial int RaceEthnicityOrReligion { get; }
    /// <summary>The Automod level for sexual content</summary>
    public partial int SexBasedTerms { get; }
}
