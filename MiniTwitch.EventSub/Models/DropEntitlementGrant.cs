using System.Text.Json.Serialization;
using MiniTwitch.EventSub.Internal;

namespace MiniTwitch.EventSub.Models;

/// <summary>
/// A notification when an entitlement for a Drop is granted to a user.
/// This event type is batched — use <c>events</c> (plural array) instead of <c>event</c> in the outer message
/// </summary>
[EventSubEvent("drop.entitlement.grant", "1")]
public partial struct DropEntitlementGrant
{
    /// <summary>Individual event ID, as assigned by EventSub. Use this for de-duplicating messages</summary>
    public partial string Id { get; }
    /// <summary>The entitlement data</summary>
    public partial EntitlementData Data { get; }

    /// <summary>The entitlement data</summary>
    [EventProperty]
    public partial struct EntitlementData
    {
        /// <summary>The ID of the organization that owns the game that has Drops enabled</summary>
        public partial string OrganizationId { get; }
        /// <summary>Twitch category ID of the game that was being played when this benefit was entitled</summary>
        public partial string CategoryId { get; }
        /// <summary>The category name</summary>
        [Intern]
        public partial string CategoryName { get; }
        /// <summary>The campaign this entitlement is associated with</summary>
        public partial string CampaignId { get; }
        /// <summary>Twitch user ID of the user who was granted the entitlement</summary>
        public partial long UserId { get; }
        /// <summary>The user login of the user who was granted the entitlement</summary>
        [JsonPropertyName("user_login")]
        public partial string Username { get; }
        /// <summary>The user display name of the user who was granted the entitlement</summary>
        [JsonPropertyName("user_name")]
        public partial string UserDisplayName { get; }
        /// <summary>Unique identifier of the entitlement. Use this to de-duplicate entitlements</summary>
        public partial string EntitlementId { get; }
        /// <summary>Identifier of the Benefit</summary>
        public partial string BenefitId { get; }
        /// <summary>UTC timestamp when this entitlement was granted on Twitch</summary>
        public partial DateTimeOffset CreatedAt { get; }
    }
}
