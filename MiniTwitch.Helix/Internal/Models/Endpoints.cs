namespace MiniTwitch.Helix.Internal.Models;

// Values scraped from https://dev.twitch.tv/docs/api/reference/
// If you see anything wrong, please open an issue!
internal static class Endpoints
{
    public static readonly HelixEndpoint StartCommercial = new()
    {
        Method = HttpMethod.Post,
        Route = "/channels/commercial",
    };

    public static readonly HelixEndpoint GetExtensionAnalytics = new()
    {
        Method = HttpMethod.Get,
        Route = "/analytics/extensions",
    };

    public static readonly HelixEndpoint GetGameAnalytics = new()
    {
        Method = HttpMethod.Get,
        Route = "/analytics/games",
    };

    public static readonly HelixEndpoint GetBitsLeaderboard = new()
    {
        Method = HttpMethod.Get,
        Route = "/bits/leaderboard",
    };

    public static readonly HelixEndpoint GetCheermotes = new()
    {
        Method = HttpMethod.Get,
        Route = "/bits/cheermotes",
    };

    public static readonly HelixEndpoint GetExtensionTransactions = new()
    {
        Method = HttpMethod.Get,
        Route = "/extensions/transactions",
    };

    public static readonly HelixEndpoint GetChannelInformation = new()
    {
        Method = HttpMethod.Get,
        Route = "/channels",
    };

    public static readonly HelixEndpoint ModifyChannelInformation = new()
    {
        Method = HttpMethod.Patch,
        Route = "/channels",
    };

    public static readonly HelixEndpoint GetChannelEditors = new()
    {
        Method = HttpMethod.Get,
        Route = "/channels/editors",
    };

    public static readonly HelixEndpoint GetFollowedChannels = new()
    {
        Method = HttpMethod.Get,
        Route = "/channels/followed",
    };

    public static readonly HelixEndpoint GetChannelFollowers = new()
    {
        Method = HttpMethod.Get,
        Route = "/channels/followers",
    };

    public static readonly HelixEndpoint CreateCustomRewards = new()
    {
        Method = HttpMethod.Post,
        Route = "/channel_points/custom_rewards",
    };

    public static readonly HelixEndpoint DeleteCustomReward = new()
    {
        Method = HttpMethod.Delete,
        Route = "/channel_points/custom_rewards",
    };

    public static readonly HelixEndpoint GetCustomReward = new()
    {
        Method = HttpMethod.Get,
        Route = "/channel_points/custom_rewards",
    };

    public static readonly HelixEndpoint GetCustomRewardRedemption = new()
    {
        Method = HttpMethod.Get,
        Route = "/channel_points/custom_rewards/redemptions",
    };

    public static readonly HelixEndpoint UpdateCustomReward = new()
    {
        Method = HttpMethod.Patch,
        Route = "/channel_points/custom_rewards",
    };

    public static readonly HelixEndpoint UpdateRedemptionStatus = new()
    {
        Method = HttpMethod.Patch,
        Route = "/channel_points/custom_rewards/redemptions",
    };

    public static readonly HelixEndpoint GetCharityCampaign = new()
    {
        Method = HttpMethod.Get,
        Route = "/charity/campaigns",
    };

    public static readonly HelixEndpoint GetCharityCampaignDonations = new()
    {
        Method = HttpMethod.Get,
        Route = "/charity/donations",
    };

    public static readonly HelixEndpoint GetChatters = new()
    {
        Method = HttpMethod.Get,
        Route = "/chat/chatters",
    };

    public static readonly HelixEndpoint GetChannelEmotes = new()
    {
        Method = HttpMethod.Get,
        Route = "/chat/emotes",
    };

    public static readonly HelixEndpoint GetGlobalEmotes = new()
    {
        Method = HttpMethod.Get,
        Route = "/chat/emotes/global",
    };

    public static readonly HelixEndpoint GetEmoteSets = new()
    {
        Method = HttpMethod.Get,
        Route = "/chat/emotes/set",
    };

    public static readonly HelixEndpoint GetChannelChatBadges = new()
    {
        Method = HttpMethod.Get,
        Route = "/chat/badges",
    };

    public static readonly HelixEndpoint GetGlobalChatBadges = new()
    {
        Method = HttpMethod.Get,
        Route = "/chat/badges/global",
    };

    public static readonly HelixEndpoint GetChatSettings = new()
    {
        Method = HttpMethod.Get,
        Route = "/chat/settings",
    };

    public static readonly HelixEndpoint UpdateChatSettings = new()
    {
        Method = HttpMethod.Patch,
        Route = "/chat/settings",
    };

    public static readonly HelixEndpoint SendChatAnnouncement = new()
    {
        Method = HttpMethod.Post,
        Route = "/chat/announcements",
    };

    public static readonly HelixEndpoint SendAShoutout = new()
    {
        Method = HttpMethod.Post,
        Route = "/chat/shoutouts",
    };

    public static readonly HelixEndpoint GetUserChatColor = new()
    {
        Method = HttpMethod.Get,
        Route = "/chat/color",
    };

    public static readonly HelixEndpoint UpdateUserChatColor = new()
    {
        Method = HttpMethod.Put,
        Route = "/chat/color",
    };

    public static readonly HelixEndpoint CreateClip = new()
    {
        Method = HttpMethod.Post,
        Route = "/clips",
    };

    public static readonly HelixEndpoint GetClips = new()
    {
        Method = HttpMethod.Get,
        Route = "/clips",
    };

    public static readonly HelixEndpoint GetConduits = new()
    {
        Method = HttpMethod.Get,
        Route = "/eventsub/conduits",
    };

    public static readonly HelixEndpoint CreateConduits = new()
    {
        Method = HttpMethod.Post,
        Route = "/eventsub/conduits",
    };

    public static readonly HelixEndpoint UpdateConduits = new()
    {
        Method = HttpMethod.Patch,
        Route = "/eventsub/conduits",
    };

    public static readonly HelixEndpoint DeleteConduits = new()
    {
        Method = HttpMethod.Delete,
        Route = "/eventsub/conduits",
    };

    public static readonly HelixEndpoint GetConduitShards = new()
    {
        Method = HttpMethod.Get,
        Route = "/eventsub/conduits/shards",
    };

    public static readonly HelixEndpoint UpdateConduitShards = new()
    {
        Method = HttpMethod.Patch,
        Route = "/eventsub/conduits/shards",
    };

    public static readonly HelixEndpoint GetContentClassificationLabels = new()
    {
        Method = HttpMethod.Get,
        Route = "/content_classification_labels",
    };

    public static readonly HelixEndpoint GetDropsEntitlements = new()
    {
        Method = HttpMethod.Get,
        Route = "/entitlements/drops",
    };

    public static readonly HelixEndpoint UpdateDropsEntitlements = new()
    {
        Method = HttpMethod.Patch,
        Route = "/entitlements/drops",
    };

    public static readonly HelixEndpoint GetExtensionConfigurationSegment = new()
    {
        Method = HttpMethod.Get,
        Route = "/extensions/configurations",
    };

    public static readonly HelixEndpoint SetExtensionConfigurationSegment = new()
    {
        Method = HttpMethod.Put,
        Route = "/extensions/configurations",
    };

    public static readonly HelixEndpoint SetExtensionRequiredConfiguration = new()
    {
        Method = HttpMethod.Put,
        Route = "/extensions/required_configuration",
    };

    public static readonly HelixEndpoint SendExtensionPubSubMessage = new()
    {
        Method = HttpMethod.Post,
        Route = "/extensions/pubsub",
    };

    public static readonly HelixEndpoint GetExtensionLiveChannels = new()
    {
        Method = HttpMethod.Get,
        Route = "/extensions/live",
    };

    public static readonly HelixEndpoint GetExtensionSecrets = new()
    {
        Method = HttpMethod.Get,
        Route = "/extensions/jwt/secrets",
    };

    public static readonly HelixEndpoint CreateExtensionSecret = new()
    {
        Method = HttpMethod.Post,
        Route = "/extensions/jwt/secrets",
    };

    public static readonly HelixEndpoint SendExtensionChatMessage = new()
    {
        Method = HttpMethod.Post,
        Route = "/extensions/chat",
    };

    public static readonly HelixEndpoint GetExtensions = new()
    {
        Method = HttpMethod.Get,
        Route = "/extensions",
    };

    public static readonly HelixEndpoint GetReleasedExtensions = new()
    {
        Method = HttpMethod.Get,
        Route = "/extensions/released",
    };

    public static readonly HelixEndpoint GetExtensionBitsProducts = new()
    {
        Method = HttpMethod.Get,
        Route = "/bits/extensions",
    };

    public static readonly HelixEndpoint UpdateExtensionBitsProduct = new()
    {
        Method = HttpMethod.Put,
        Route = "/bits/extensions",
    };

    public static readonly HelixEndpoint CreateEventSubSubscription = new()
    {
        Method = HttpMethod.Post,
        Route = "/eventsub/subscriptions",
    };

    public static readonly HelixEndpoint DeleteEventSubSubscription = new()
    {
        Method = HttpMethod.Delete,
        Route = "/eventsub/subscriptions",
    };

    public static readonly HelixEndpoint GetEventSubSubscriptions = new()
    {
        Method = HttpMethod.Get,
        Route = "/eventsub/subscriptions",
    };

    public static readonly HelixEndpoint GetTopGames = new()
    {
        Method = HttpMethod.Get,
        Route = "/games/top",
    };

    public static readonly HelixEndpoint GetGames = new()
    {
        Method = HttpMethod.Get,
        Route = "/games",
    };

    public static readonly HelixEndpoint GetCreatorGoals = new()
    {
        Method = HttpMethod.Get,
        Route = "/goals",
    };

    public static readonly HelixEndpoint GetChannelGuestStarSettings = new()
    {
        Method = HttpMethod.Get,
        Route = "/guest_star/channel_settings",
    };

    public static readonly HelixEndpoint UpdateChannelGuestStarSettings = new()
    {
        Method = HttpMethod.Put,
        Route = "/guest_star/channel_settings",
    };

    public static readonly HelixEndpoint GetGuestStarSession = new()
    {
        Method = HttpMethod.Get,
        Route = "/guest_star/session",
    };

    public static readonly HelixEndpoint CreateGuestStarSession = new()
    {
        Method = HttpMethod.Post,
        Route = "/guest_star/session",
    };

    public static readonly HelixEndpoint EndGuestStarSession = new()
    {
        Method = HttpMethod.Delete,
        Route = "/guest_star/session",
    };

    public static readonly HelixEndpoint GetGuestStarInvites = new()
    {
        Method = HttpMethod.Get,
        Route = "/guest_star/invites",
    };

    public static readonly HelixEndpoint SendGuestStarInvite = new()
    {
        Method = HttpMethod.Post,
        Route = "/guest_star/invites",
    };

    public static readonly HelixEndpoint DeleteGuestStarInvite = new()
    {
        Method = HttpMethod.Delete,
        Route = "/guest_star/invites",
    };

    public static readonly HelixEndpoint AssignGuestStarSlot = new()
    {
        Method = HttpMethod.Post,
        Route = "/guest_star/slot",
    };

    public static readonly HelixEndpoint UpdateGuestStarSlot = new()
    {
        Method = HttpMethod.Patch,
        Route = "/guest_star/slot",
    };

    public static readonly HelixEndpoint DeleteGuestStarSlot = new()
    {
        Method = HttpMethod.Delete,
        Route = "/guest_star/slot",
    };

    public static readonly HelixEndpoint UpdateGuestStarSlotSettings = new()
    {
        Method = HttpMethod.Patch,
        Route = "/guest_star/slot_settings",
    };

    public static readonly HelixEndpoint GetHypeTrainEvents = new()
    {
        Method = HttpMethod.Get,
        Route = "/hypetrain/events",
    };

    public static readonly HelixEndpoint CheckAutoModStatus = new()
    {
        Method = HttpMethod.Post,
        Route = "/moderation/enforcements/status",
    };

    public static readonly HelixEndpoint ManageHeldAutoModMessages = new()
    {
        Method = HttpMethod.Post,
        Route = "/moderation/automod/message",
    };

    public static readonly HelixEndpoint GetAutoModSettings = new()
    {
        Method = HttpMethod.Get,
        Route = "/moderation/automod/settings",
    };

    public static readonly HelixEndpoint UpdateAutoModSettings = new()
    {
        Method = HttpMethod.Put,
        Route = "/moderation/automod/settings",
    };

    public static readonly HelixEndpoint GetBannedUsers = new()
    {
        Method = HttpMethod.Get,
        Route = "/moderation/banned",
    };

    public static readonly HelixEndpoint BanUser = new()
    {
        Method = HttpMethod.Post,
        Route = "/moderation/bans",
    };

    public static readonly HelixEndpoint UnbanUser = new()
    {
        Method = HttpMethod.Delete,
        Route = "/moderation/bans",
    };

    public static readonly HelixEndpoint GetBlockedTerms = new()
    {
        Method = HttpMethod.Get,
        Route = "/moderation/blocked_terms",
    };

    public static readonly HelixEndpoint AddBlockedTerm = new()
    {
        Method = HttpMethod.Post,
        Route = "/moderation/blocked_terms",
    };

    public static readonly HelixEndpoint RemoveBlockedTerm = new()
    {
        Method = HttpMethod.Delete,
        Route = "/moderation/blocked_terms",
    };

    public static readonly HelixEndpoint DeleteChatMessages = new()
    {
        Method = HttpMethod.Delete,
        Route = "/moderation/chat",
    };

    public static readonly HelixEndpoint GetModerators = new()
    {
        Method = HttpMethod.Get,
        Route = "/moderation/moderators",
    };

    public static readonly HelixEndpoint AddChannelModerator = new()
    {
        Method = HttpMethod.Post,
        Route = "/moderation/moderators",
    };

    public static readonly HelixEndpoint RemoveChannelModerator = new()
    {
        Method = HttpMethod.Delete,
        Route = "/moderation/moderators",
    };

    public static readonly HelixEndpoint GetVIPs = new()
    {
        Method = HttpMethod.Get,
        Route = "/channels/vips",
    };

    public static readonly HelixEndpoint AddChannelVIP = new()
    {
        Method = HttpMethod.Post,
        Route = "/channels/vips",
    };

    public static readonly HelixEndpoint RemoveChannelVIP = new()
    {
        Method = HttpMethod.Delete,
        Route = "/channels/vips",
    };

    public static readonly HelixEndpoint UpdateShieldModeStatus = new()
    {
        Method = HttpMethod.Put,
        Route = "/moderation/shield_mode",
    };

    public static readonly HelixEndpoint GetShieldModeStatus = new()
    {
        Method = HttpMethod.Get,
        Route = "/moderation/shield_mode",
    };

    public static readonly HelixEndpoint GetPolls = new()
    {
        Method = HttpMethod.Get,
        Route = "/polls",
    };

    public static readonly HelixEndpoint CreatePoll = new()
    {
        Method = HttpMethod.Post,
        Route = "/polls",
    };

    public static readonly HelixEndpoint EndPoll = new()
    {
        Method = HttpMethod.Patch,
        Route = "/polls",
    };

    public static readonly HelixEndpoint GetPredictions = new()
    {
        Method = HttpMethod.Get,
        Route = "/predictions",
    };

    public static readonly HelixEndpoint CreatePrediction = new()
    {
        Method = HttpMethod.Post,
        Route = "/predictions",
    };

    public static readonly HelixEndpoint EndPrediction = new()
    {
        Method = HttpMethod.Patch,
        Route = "/predictions",
    };

    public static readonly HelixEndpoint StartARaid = new()
    {
        Method = HttpMethod.Post,
        Route = "/raids",
    };

    public static readonly HelixEndpoint CancelARaid = new()
    {
        Method = HttpMethod.Delete,
        Route = "/raids",
    };

    public static readonly HelixEndpoint GetChannelStreamSchedule = new()
    {
        Method = HttpMethod.Get,
        Route = "/schedule",
    };

    public static readonly HelixEndpoint GetChannelICalendar = new()
    {
        Method = HttpMethod.Get,
        Route = "/schedule/icalendar",
    };

    public static readonly HelixEndpoint UpdateChannelStreamSchedule = new()
    {
        Method = HttpMethod.Patch,
        Route = "/schedule/settings",
    };

    public static readonly HelixEndpoint CreateChannelStreamScheduleSegment = new()
    {
        Method = HttpMethod.Post,
        Route = "/schedule/segment",
    };

    public static readonly HelixEndpoint UpdateChannelStreamScheduleSegment = new()
    {
        Method = HttpMethod.Patch,
        Route = "/schedule/segment",
    };

    public static readonly HelixEndpoint DeleteChannelStreamScheduleSegment = new()
    {
        Method = HttpMethod.Delete,
        Route = "/schedule/segment",
    };

    public static readonly HelixEndpoint SearchCategories = new()
    {
        Method = HttpMethod.Get,
        Route = "/search/categories",
    };

    public static readonly HelixEndpoint SearchChannels = new()
    {
        Method = HttpMethod.Get,
        Route = "/search/channels",
    };

    public static readonly HelixEndpoint GetSoundtrackCurrentTrack = new()
    {
        Method = HttpMethod.Get,
        Route = "/soundtrack/current_track",
    };

    public static readonly HelixEndpoint GetSoundtrackPlaylist = new()
    {
        Method = HttpMethod.Get,
        Route = "/soundtrack/playlist",
    };

    public static readonly HelixEndpoint GetSoundtrackPlaylists = new()
    {
        Method = HttpMethod.Get,
        Route = "/soundtrack/playlists",
    };

    public static readonly HelixEndpoint GetStreamKey = new()
    {
        Method = HttpMethod.Get,
        Route = "/streams/key",
    };

    public static readonly HelixEndpoint GetStreams = new()
    {
        Method = HttpMethod.Get,
        Route = "/streams",
    };

    public static readonly HelixEndpoint GetFollowedStreams = new()
    {
        Method = HttpMethod.Get,
        Route = "/streams/followed",
    };

    public static readonly HelixEndpoint CreateStreamMarker = new()
    {
        Method = HttpMethod.Post,
        Route = "/streams/markers",
    };

    public static readonly HelixEndpoint GetStreamMarkers = new()
    {
        Method = HttpMethod.Get,
        Route = "/streams/markers",
    };

    public static readonly HelixEndpoint GetBroadcasterSubscriptions = new()
    {
        Method = HttpMethod.Get,
        Route = "/subscriptions",
    };

    public static readonly HelixEndpoint CheckUserSubscription = new()
    {
        Method = HttpMethod.Get,
        Route = "/subscriptions/user",
    };

    public static readonly HelixEndpoint GetAllStreamTags = new()
    {
        Method = HttpMethod.Get,
        Route = "/tags/streams",
    };

    public static readonly HelixEndpoint GetStreamTags = new()
    {
        Method = HttpMethod.Get,
        Route = "/streams/tags",
    };

    public static readonly HelixEndpoint GetChannelTeams = new()
    {
        Method = HttpMethod.Get,
        Route = "/teams/channel",
    };

    public static readonly HelixEndpoint GetTeams = new()
    {
        Method = HttpMethod.Get,
        Route = "/teams",
    };

    public static readonly HelixEndpoint GetUsers = new()
    {
        Method = HttpMethod.Get,
        Route = "/users",
    };

    public static readonly HelixEndpoint UpdateUser = new()
    {
        Method = HttpMethod.Put,
        Route = "/users",
    };

    public static readonly HelixEndpoint GetUsersFollows = new()
    {
        Method = HttpMethod.Get,
        Route = "/users/follows",
    };

    public static readonly HelixEndpoint GetUserBlockList = new()
    {
        Method = HttpMethod.Get,
        Route = "/users/blocks",
    };

    public static readonly HelixEndpoint BlockUser = new()
    {
        Method = HttpMethod.Put,
        Route = "/users/blocks",
    };

    public static readonly HelixEndpoint UnblockUser = new()
    {
        Method = HttpMethod.Delete,
        Route = "/users/blocks",
    };

    public static readonly HelixEndpoint GetUserExtensions = new()
    {
        Method = HttpMethod.Get,
        Route = "/users/extensions/list",
    };

    public static readonly HelixEndpoint GetUserActiveExtensions = new()
    {
        Method = HttpMethod.Get,
        Route = "/users/extensions",
    };

    public static readonly HelixEndpoint UpdateUserExtensions = new()
    {
        Method = HttpMethod.Put,
        Route = "/users/extensions",
    };

    public static readonly HelixEndpoint GetVideos = new()
    {
        Method = HttpMethod.Get,
        Route = "/videos",
    };

    public static readonly HelixEndpoint DeleteVideos = new()
    {
        Method = HttpMethod.Delete,
        Route = "/videos",
    };

    public static readonly HelixEndpoint SendWhisper = new()
    {
        Method = HttpMethod.Post,
        Route = "/whispers",
    };

    public static readonly HelixEndpoint SnoozeNextAd = new()
    {
        Method = HttpMethod.Post,
        Route = "/channels/ads/schedule/snooze",
    };

    public static readonly HelixEndpoint GetAdSchedule = new()
    {
        Method = HttpMethod.Get,
        Route = "/channels/ads",
    };

    public static readonly HelixEndpoint GetModeratedChannels = new()
    {
        Method = HttpMethod.Get,
        Route = "/moderation/channels",
    };

    public static readonly HelixEndpoint SendChatMessage = new()
    {
        Method = HttpMethod.Post,
        Route = "/chat/messages",
    };

    public static readonly HelixEndpoint GetUserEmotes = new()
    {
        Method = HttpMethod.Get,
        Route = "/chat/emotes/user",
    };

    public static readonly HelixEndpoint GetUnbanRequests = new()
    {
        Method = HttpMethod.Get,
        Route = "/moderation/unban_requests",
    };

    public static readonly HelixEndpoint ResolveUnbanRequests = new()
    {
        Method = HttpMethod.Patch,
        Route = "/moderation/unban_requests",
    };

    public static readonly HelixEndpoint WarnChatUser = new()
    {
        Method = HttpMethod.Post,
        Route = "/moderation/warnings",
    };

    public static readonly HelixEndpoint GetSharedChatSession = new()
    {
        Method = HttpMethod.Get,
        Route = "/shared_chat/session"
    };

    public static readonly HelixEndpoint GetHypeTrainStatus = new()
    {
        Method = HttpMethod.Get,
        Route = "/hypetrain/status",
    };
}
