using System.Text.Json.Serialization;
using MiniTwitch.EventSub.Internal;

namespace MiniTwitch.EventSub.Models;

/// <summary>
/// A notification when a new transaction is created for a Twitch Extension
/// </summary>
[EventSubEvent("extension.bits_transaction.create", "1")]
public partial struct ExtensionBitsTransactionCreate
{
    /// <summary>Transaction ID</summary>
    public partial string Id { get; }
    /// <summary>Client ID of the extension</summary>
    public partial string ExtensionClientId { get; }
    /// <summary>The transaction's broadcaster ID</summary>
    [JsonPropertyName("broadcaster_user_id")]
    public partial long BroadcasterId { get; }
    /// <summary>The transaction's broadcaster login</summary>
    [JsonPropertyName("broadcaster_user_login"), Intern]
    public partial string BroadcasterUsername { get; }
    /// <summary>The transaction's broadcaster display name</summary>
    [JsonPropertyName("broadcaster_user_name"), Intern]
    public partial string BroadcasterDisplayName { get; }
    /// <summary>The transaction's user ID</summary>
    public partial long UserId { get; }
    /// <summary>The transaction's user login</summary>
    [JsonPropertyName("user_login")]
    public partial string Username { get; }
    /// <summary>The transaction's user display name</summary>
    [JsonPropertyName("user_name")]
    public partial string UserDisplayName { get; }
    /// <summary>Additional extension product information</summary>
    public partial ProductInfo Product { get; }

    /// <summary>Additional information about a product acquired via a Twitch Extension Bits transaction</summary>
    [EventProperty]
    public partial struct ProductInfo
    {
        /// <summary>Product name</summary>
        public partial string Name { get; }
        /// <summary>Bits involved in the transaction</summary>
        public partial int Bits { get; }
        /// <summary>Unique identifier for the product acquired</summary>
        public partial string Sku { get; }
        /// <summary>Flag indicating if the product is in development</summary>
        public partial bool InDevelopment { get; }
    }
}
