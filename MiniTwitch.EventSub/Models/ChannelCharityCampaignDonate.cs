using System.Text.Json.Serialization;
using MiniTwitch.EventSub.Internal;

namespace MiniTwitch.EventSub.Models;

/// <summary>
/// A notification when a user donates to the broadcaster's charity campaign
/// </summary>
[EventSubEvent("channel.charity_campaign.donate", "1")]
public partial struct ChannelCharityCampaignDonate
{
    /// <summary>An ID that identifies the donation</summary>
    public partial string Id { get; }
    /// <summary>An ID that identifies the charity campaign</summary>
    public partial string CampaignId { get; }
    /// <summary>An ID that identifies the broadcaster running the campaign</summary>
    [JsonPropertyName("broadcaster_user_id")]
    public partial long BroadcasterId { get; }
    /// <summary>The broadcaster's login name</summary>
    [JsonPropertyName("broadcaster_user_login"), Intern]
    public partial string BroadcasterUsername { get; }
    /// <summary>The broadcaster's display name</summary>
    [JsonPropertyName("broadcaster_user_name"), Intern]
    public partial string BroadcasterDisplayName { get; }
    /// <summary>An ID that identifies the user that donated to the campaign</summary>
    public partial long UserId { get; }
    /// <summary>The user's login name</summary>
    [JsonPropertyName("user_login")]
    public partial string Username { get; }
    /// <summary>The user's display name</summary>
    [JsonPropertyName("user_name")]
    public partial string UserDisplayName { get; }
    /// <summary>The charity's name</summary>
    [Intern]
    public partial string CharityName { get; }
    /// <summary>A description of the charity</summary>
    public partial string CharityDescription { get; }
    /// <summary>A URL to an image of the charity's logo</summary>
    public partial string CharityLogo { get; }
    /// <summary>A URL to the charity's website</summary>
    public partial string CharityWebsite { get; }
    /// <summary>An object that contains the amount of money that the user donated</summary>
    public partial DonationAmount Amount { get; }

    /// <summary>An object that contains the amount of money that the user donated</summary>
    [EventProperty]
    public partial struct DonationAmount
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
