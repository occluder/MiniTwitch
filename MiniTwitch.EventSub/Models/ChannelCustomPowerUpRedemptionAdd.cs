using System.Text.Json.Serialization;
using MiniTwitch.EventSub.Internal;

namespace MiniTwitch.EventSub.Models;

/// <summary>
/// A notification when a viewer has redeemed a custom Power-up
/// </summary>
[EventSubEvent("channel.custom_power_up_redemption.add", "1")]
public partial struct ChannelCustomPowerUpRedemptionAdd
{
    /// <summary>The redemption identifier</summary>
    public partial string Id { get; }
    /// <summary>The requested broadcaster ID</summary>
    [JsonPropertyName("broadcaster_user_id")]
    public partial long BroadcasterId { get; }
    /// <summary>The requested broadcaster login</summary>
    [JsonPropertyName("broadcaster_user_login"), Intern]
    public partial string BroadcasterUsername { get; }
    /// <summary>The requested broadcaster display name</summary>
    [JsonPropertyName("broadcaster_user_name"), Intern]
    public partial string BroadcasterDisplayName { get; }
    /// <summary>User ID of the user that redeemed the custom Power-up</summary>
    public partial long UserId { get; }
    /// <summary>Login of the user that redeemed the custom Power-up</summary>
    [JsonPropertyName("user_login")]
    public partial string Username { get; }
    /// <summary>Display name of the user that redeemed the custom Power-up</summary>
    [JsonPropertyName("user_name")]
    public partial string UserDisplayName { get; }
    /// <summary>The user input provided. Empty string if not provided</summary>
    public partial string UserInput { get; }
    /// <summary>
    /// Defaults to <c>unfulfilled</c>.
    /// Possible values: <c>unknown</c>, <c>unfulfilled</c>, <c>fulfilled</c>, <c>canceled</c>
    /// </summary>
    public partial string Status { get; }
    /// <summary>Basic information about the custom Power-up that was redeemed</summary>
    public partial CustomPowerUpInfo CustomPowerUp { get; }
    /// <summary>RFC3339 timestamp of when the custom Power-up was redeemed</summary>
    public partial DateTimeOffset RedeemedAt { get; }

    /// <summary>Basic information about the custom Power-up that was redeemed</summary>
    [EventProperty]
    public partial struct CustomPowerUpInfo
    {
        /// <summary>The ID of the custom Power-up</summary>
        public partial string Id { get; }
        /// <summary>The title of the custom Power-up</summary>
        public partial string Title { get; }
        /// <summary>The cost of the custom Power-up to redeem</summary>
        public partial int Bits { get; }
        /// <summary>The creator-provided description for this Power-up</summary>
        public partial string Prompt { get; }
    }
}
