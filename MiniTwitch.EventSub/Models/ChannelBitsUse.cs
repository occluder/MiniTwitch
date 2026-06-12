using System.Text.Json.Serialization;
using MiniTwitch.EventSub.Internal;

namespace MiniTwitch.EventSub.Models;

/// <summary>
/// A notification whenever Bits are used on a channel
/// </summary>
[EventSubEvent("channel.bits.use", "1")]
public partial struct ChannelBitsUse
{
    /// <summary>The User ID of the channel where the Bits were redeemed</summary>
    [JsonPropertyName("broadcaster_user_id")]
    public partial long BroadcasterId { get; }
    /// <summary>The login of the channel where the Bits were used</summary>
    [JsonPropertyName("broadcaster_user_login"), Intern]
    public partial string BroadcasterUsername { get; }
    /// <summary>The display name of the channel where the Bits were used</summary>
    [JsonPropertyName("broadcaster_user_name"), Intern]
    public partial string BroadcasterDisplayName { get; }
    /// <summary>The User ID of the redeeming user</summary>
    public partial long UserId { get; }
    /// <summary>The login name of the redeeming user</summary>
    [JsonPropertyName("user_login")]
    public partial string Username { get; }
    /// <summary>The display name of the redeeming user</summary>
    [JsonPropertyName("user_name")]
    public partial string UserDisplayName { get; }
    /// <summary>The number of Bits used</summary>
    public partial int Bits { get; }
    /// <summary>The type of Bits usage. Possible values: <c>cheer</c>, <c>power_up</c>, <c>custom_power_up</c></summary>
    [Intern]
    public partial string Type { get; }
    /// <summary>Optional. An object that contains the user message and emote information</summary>
    public partial BitsMessage? Message { get; }
    /// <summary>Optional. Data about a default (i.e. built-in) Power-up</summary>
    public partial PowerUpInfo? PowerUp { get; }
    /// <summary>Optional. Data about a custom Power-up</summary>
    public partial CustomPowerUpInfo? CustomPowerUp { get; }

    /// <summary>An object that contains the user message and emote information needed to recreate the message</summary>
    [EventProperty]
    public partial struct BitsMessage
    {
        /// <summary>The chat message in plain text</summary>
        public partial string Text { get; }
        /// <summary>The ordered list of chat message fragments</summary>
        public partial MessageFragment[] Fragments { get; }
    }

    /// <summary>Data about a default (i.e. built-in) Power-up</summary>
    [EventProperty]
    public partial struct PowerUpInfo
    {
        /// <summary>The type of Power-up. Possible values: <c>message_effect</c>, <c>celebration</c>, <c>gigantify_an_emote</c></summary>
        public partial string Type { get; }
        /// <summary>Optional. Emote associated with the reward</summary>
        public partial PowerUpEmote? Emote { get; }
        /// <summary>Optional. The ID of the message effect</summary>
        public partial string? MessageEffectId { get; }
    }

    /// <summary>Emote associated with the Power-up reward</summary>
    [EventProperty]
    public partial struct PowerUpEmote
    {
        /// <summary>The ID that uniquely identifies this emote</summary>
        public partial string Id { get; }
        /// <summary>The human readable emote token</summary>
        public partial string Name { get; }
    }

    /// <summary>Data about a custom Power-up</summary>
    [EventProperty]
    public partial struct CustomPowerUpInfo
    {
        /// <summary>The title of the custom Power-up</summary>
        public partial string Title { get; }
        /// <summary>The ID of the custom Power-up</summary>
        [JsonPropertyName("reward_id")]
        public partial string RewardId { get; }
    }
}
