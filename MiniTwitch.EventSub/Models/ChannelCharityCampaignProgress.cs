using System.Text.Json.Serialization;
using MiniTwitch.EventSub.Internal;

namespace MiniTwitch.EventSub.Models;

/// <summary>
/// A notification when progress is made towards the campaign's goal or when the broadcaster changes the fundraising goal
/// </summary>
[EventSubEvent("channel.charity_campaign.progress", "1")]
public partial struct ChannelCharityCampaignProgress
{
    /// <summary>An ID that identifies the charity campaign</summary>
    public partial string Id { get; }
    /// <summary>An ID that identifies the broadcaster running the campaign</summary>
    public partial long BroadcasterId { get; }
    /// <summary>The broadcaster's login name</summary>
    [JsonPropertyName("broadcaster_login"), Intern]
    public partial string BroadcasterUsername { get; }
    /// <summary>The broadcaster's display name</summary>
    [JsonPropertyName("broadcaster_name"), Intern]
    public partial string BroadcasterDisplayName { get; }
    /// <summary>The charity's name</summary>
    [Intern]
    public partial string CharityName { get; }
    /// <summary>A description of the charity</summary>
    public partial string CharityDescription { get; }
    /// <summary>A URL to an image of the charity's logo</summary>
    public partial string CharityLogo { get; }
    /// <summary>A URL to the charity's website</summary>
    public partial string CharityWebsite { get; }
    /// <summary>An object that contains the current amount of donations that the campaign has received</summary>
    public partial CampaignAmount CurrentAmount { get; }
    /// <summary>An object that contains the campaign's target fundraising goal</summary>
    public partial CampaignAmount TargetAmount { get; }

    /// <summary>An object that contains a monetary amount</summary>
    [EventProperty]
    public partial struct CampaignAmount
    {
        /// <summary>
        /// The monetary amount in the currency's minor unit.
        /// For example, USD uses cents, so $5.50 is <c>550</c>
        /// </summary>
        public partial int Value { get; }
        /// <summary>
        /// The number of decimal places used by the currency.
        /// For example, USD uses two decimal places
        /// </summary>
        public partial int DecimalPlaces { get; }
        /// <summary>The ISO-4217 three-letter currency code for the amount</summary>
        [Intern]
        public partial string Currency { get; }
    }
}
