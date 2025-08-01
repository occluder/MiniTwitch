using System.Web;
using Microsoft.Extensions.Logging;
using MiniTwitch.Common;
using MiniTwitch.Helix.Enums;
using MiniTwitch.Helix.Internal;
using MiniTwitch.Helix.Internal.Json;
using MiniTwitch.Helix.Internal.Models;
using MiniTwitch.Helix.Models;
using MiniTwitch.Helix.Requests;
using MiniTwitch.Helix.Responses;

namespace MiniTwitch.Helix;

/// <summary>
/// Wraps all Helix endpoints and exposes them directly as methods.
/// </summary>
public class HelixWrapper
{
    /// <summary>
    /// The default logger for <see cref="HelixWrapper"/>, only used when <see cref="ILogger"/> is not provided in the constructor
    /// <para>Can be toggled with <see cref="DefaultMiniTwitchLogger{T}.Enabled"/></para>
    /// </summary>
    public DefaultMiniTwitchLogger<HelixWrapper> DefaultLogger => Client.Logger;
    /// <summary>
    /// Gets the user ID associated with this <see cref="HelixWrapper"/> instance
    /// </summary>
    public long UserId { get; }
    public HelixApiClient Client { get; }

    private readonly string _baseUrl;

    public HelixWrapper(
        string accessToken,
        long userId,
        ILogger? logger = null,
        string helixBaseUrl = "https://api.twitch.tv/helix",
        string tokenValidationUrl = "https://id.twitch.tv/oauth2/validate")
    {
        _baseUrl = helixBaseUrl;
        Client = new HelixApiClient(accessToken, userId, logger, tokenValidationUrl);
        this.UserId = userId;
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#start-commercial">API Reference</see>
    ///</summary>
    public Task<HelixResult<Commercial>> StartCommercial(
        NewCommercial body,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.StartCommercial;
        var request = new RequestData(_baseUrl, endpoint)
        {
            Body = body
        };

        return HelixResultFactory.Create<Commercial>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#get-extension-analytics">API Reference</see>
    ///</summary>
    public Task<HelixResult<ExtensionAnalytics>> GetExtensionAnalytics(
        string? extensionId = null,
        AnalyticsType? type = null,
        DateTime? startedAt = null,
        DateTime? endedAt = null,
        int? first = null,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.GetExtensionAnalytics;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.ExtensionId, extensionId)
            .AddParam(QueryParams.Type, type?.ToString().ToLower())
            .AddParam(QueryParams.StartedAt, startedAt)
            .AddParam(QueryParams.EndedAt, endedAt)
            .AddParam(QueryParams.First, first);

        return HelixResultFactory.Create<ExtensionAnalytics>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#get-game-analytics">API Reference</see>
    ///</summary>
    public Task<HelixResult<GameAnalytics>> GetGameAnalytics(
        string? gameId = null,
        AnalyticsType? type = null,
        DateTime? startedAt = null,
        DateTime? endedAt = null,
        int? first = null,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.GetGameAnalytics;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.GameId, gameId)
            .AddParam(QueryParams.Type, type?.ToString().ToLower())
            .AddParam(QueryParams.StartedAt, startedAt)
            .AddParam(QueryParams.EndedAt, endedAt)
            .AddParam(QueryParams.First, first);

        return HelixResultFactory.Create<GameAnalytics>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#get-bits-leaderboard">API Reference</see>
    ///</summary>
    public Task<HelixResult<BitsLeaderboard>> GetBitsLeaderboard(
        int? count = null,
        BitsLeaderboardPeriod? period = null,
        DateTime? startedAt = null,
        long? UserId = null,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.GetBitsLeaderboard;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.Count, count)
            .AddParam(QueryParams.Period, period?.ToString().ToLower())
            .AddParam(QueryParams.StartedAt, startedAt)
            .AddParam(QueryParams.UserId, UserId);

        return HelixResultFactory.Create<BitsLeaderboard>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#get-cheermotes">API Reference</see>
    ///</summary>
    public Task<HelixResult<Cheermotes>> GetCheermotes(
        long? broadcasterId = null,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.GetCheermotes;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.BroadcasterId, broadcasterId);

        return HelixResultFactory.Create<Cheermotes>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#get-extension-transactions">API Reference</see>
    ///</summary>
    public Task<HelixResult<ExtensionTransactions>> GetExtensionTransactions(
        string extensionId,
        string? id = null,
        int? first = null,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.GetExtensionTransactions;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.ExtensionId, extensionId)
            .AddParam(QueryParams.Id, id)
            .AddParam(QueryParams.First, first);

        return HelixResultFactory.Create<ExtensionTransactions>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#get-extension-transactions">API Reference</see>
    ///</summary>
    public Task<HelixResult<ExtensionTransactions>> GetExtensionTransactions(
        string extensionId,
        IEnumerable<string>? ids = null,
        int? first = null,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.GetExtensionTransactions;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.ExtensionId, extensionId)
            .AddMultiParam(QueryParams.Id, ids)
            .AddParam(QueryParams.First, first);

        return HelixResultFactory.Create<ExtensionTransactions>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#get-channel-information">API Reference</see>
    ///</summary>
    public Task<HelixResult<ChannelsInformation>> GetChannelInformation(
        long broadcasterId,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.GetChannelInformation;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.BroadcasterId, broadcasterId);

        return HelixResultFactory.Create<ChannelsInformation>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#get-channel-information">API Reference</see>
    ///</summary>
    public Task<HelixResult<ChannelsInformation>> GetChannelInformation(
        IEnumerable<long> broadcasterIds,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.GetChannelInformation;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddMultiParam(QueryParams.BroadcasterId, broadcasterIds);

        return HelixResultFactory.Create<ChannelsInformation>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#modify-channel-information">API Reference</see>
    ///</summary>
    public Task<HelixResult> ModifyChannelInformation(
        NewChannelInformation body,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.ModifyChannelInformation;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.BroadcasterId, this.UserId);

        request.Body = body;
        return HelixResultFactory.Create(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#get-channel-editors">API Reference</see>
    ///</summary>
    public Task<HelixResult<ChannelEditors>> GetChannelEditors(CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.GetChannelEditors;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.BroadcasterId, this.UserId);

        return HelixResultFactory.Create<ChannelEditors>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#get-followed-channels">API Reference</see>
    ///</summary>
    public Task<HelixResult<FollowedChannels>> GetFollowedChannels(
        long? broadcasterId = null,
        int? first = null,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.GetFollowedChannels;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.UserId, this.UserId)
            .AddParam(QueryParams.BroadcasterId, broadcasterId)
            .AddParam(QueryParams.First, first);

        return HelixResultFactory.Create<FollowedChannels>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#get-channel-followers">API Reference</see>
    ///</summary>
    public Task<HelixResult<ChannelFollowers>> GetChannelFollowers(
        long? userId = null,
        int? first = null,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.GetChannelFollowers;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.BroadcasterId, this.UserId)
            .AddParam(QueryParams.UserId, userId)
            .AddParam(QueryParams.First, first);

        return HelixResultFactory.Create<ChannelFollowers>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#create-custom-rewards">API Reference</see>
    ///</summary>
    public Task<HelixResult<CustomReward>> CreateCustomReward(
        NewCustomReward body,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.CreateCustomRewards;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.BroadcasterId, this.UserId);

        request.Body = body;
        return HelixResultFactory.Create<CustomReward>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#delete-custom-reward">API Reference</see>
    ///</summary>
    public Task<HelixResult> DeleteCustomReward(
        string id,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.DeleteCustomReward;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.BroadcasterId, this.UserId)
            .AddParam(QueryParams.Id, id);

        return HelixResultFactory.Create(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#get-custom-reward">API Reference</see>
    ///</summary>
    public Task<HelixResult<CustomReward>> GetCustomReward(
        string? id = null,
        bool onlyManageableRewards = false,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.GetCustomReward;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.BroadcasterId, this.UserId)
            .AddParam(QueryParams.Id, id)
            .AddParam(QueryParams.OnlyManageableRewards, onlyManageableRewards);

        return HelixResultFactory.Create<CustomReward>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#get-custom-reward">API Reference</see>
    ///</summary>
    public Task<HelixResult<CustomReward>> GetCustomReward(
        IEnumerable<string>? ids = null,
        bool onlyManageableRewards = false,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.GetCustomReward;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.BroadcasterId, this.UserId)
            .AddMultiParam(QueryParams.Id, ids)
            .AddParam(QueryParams.OnlyManageableRewards, onlyManageableRewards);

        return HelixResultFactory.Create<CustomReward>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#get-custom-reward-redemption">API Reference</see>
    ///</summary>
    public Task<HelixResult<CustomRewardRedemptions>> GetCustomRewardRedemption(
        string rewardId,
        RewardRedemptionStatus status,
        string? id = null,
        SortMethod? sort = null,
        int? first = null,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.GetCustomRewardRedemption;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.BroadcasterId, this.UserId)
            .AddParam(QueryParams.RewardId, rewardId)
            .AddParam(QueryParams.Id, id)
            .AddParam(QueryParams.Status, status.ToString())
            .AddParam(QueryParams.Sort, sort?.ToString())
            .AddParam(QueryParams.First, first);

        return HelixResultFactory.Create<CustomRewardRedemptions>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#get-custom-reward-redemption">API Reference</see>
    ///</summary>
    public Task<HelixResult<CustomRewardRedemptions>> GetCustomRewardRedemption(
        string rewardId,
        RewardRedemptionStatus status,
        IEnumerable<string>? ids = null,
        SortMethod? sort = null,
        int? first = null,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.GetCustomRewardRedemption;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.BroadcasterId, this.UserId)
            .AddParam(QueryParams.RewardId, rewardId)
            .AddMultiParam(QueryParams.Id, ids)
            .AddParam(QueryParams.Status, status.ToString())
            .AddParam(QueryParams.Sort, sort?.ToString())
            .AddParam(QueryParams.First, first);

        return HelixResultFactory.Create<CustomRewardRedemptions>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#update-custom-reward">API Reference</see>
    ///</summary>
    public Task<HelixResult<CustomReward>> UpdateCustomReward(
        string id,
        UpdatedCustomReward body,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.UpdateCustomReward;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.BroadcasterId, this.UserId)
            .AddParam(QueryParams.Id, id);

        request.Body = body;
        return HelixResultFactory.Create<CustomReward>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#update-redemption-status">API Reference</see>
    ///</summary>
    public Task<HelixResult<CustomRewardRedemption>> UpdateRedemptionStatus(
        string id,
        string rewardId,
        RewardRedemptionStatus status,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.UpdateRedemptionStatus;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.BroadcasterId, this.UserId)
            .AddParam(QueryParams.Id, id)
            .AddParam(QueryParams.RewardId, rewardId);

        request.Body = new
        {
            status = status.ToString()
        };

        return HelixResultFactory.Create<CustomRewardRedemption>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#update-redemption-status">API Reference</see>
    ///</summary>
    public Task<HelixResult<CustomRewardRedemptions>> UpdateRedemptionStatus(
        IEnumerable<string> ids,
        string rewardId,
        RewardRedemptionStatus status,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.UpdateRedemptionStatus;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.BroadcasterId, this.UserId)
            .AddMultiParam(QueryParams.Id, ids)
            .AddParam(QueryParams.RewardId, rewardId);

        request.Body = new
        {
            status = status.ToString()
        };

        return HelixResultFactory.Create<CustomRewardRedemptions>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#get-charity-campaign">API Reference</see>
    ///</summary>
    public Task<HelixResult<CharityCampaign>> GetCharityCampaign(CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.GetCharityCampaign;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.BroadcasterId, this.UserId);

        return HelixResultFactory.Create<CharityCampaign>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#get-charity-campaign-donations">API Reference</see>
    ///</summary>
    public Task<HelixResult<CharityCampaignDonations>> GetCharityCampaignDonations(
        int? first = null,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.GetCharityCampaignDonations;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.BroadcasterId, this.UserId)
            .AddParam(QueryParams.First, first);

        return HelixResultFactory.Create<CharityCampaignDonations>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#get-chatters">API Reference</see>
    ///</summary>
    public Task<HelixResult<Chatters>> GetChatters(
        long broadcasterId,
        int? first = null,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.GetChatters;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.BroadcasterId, broadcasterId)
            .AddParam(QueryParams.ModeratorId, this.UserId)
            .AddParam(QueryParams.First, first);

        return HelixResultFactory.Create<Chatters>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#get-channel-emotes">API Reference</see>
    ///</summary>
    public Task<HelixResult<Emotes>> GetChannelEmotes(
        long broadcasterId,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.GetChannelEmotes;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.BroadcasterId, broadcasterId);

        return HelixResultFactory.Create<Emotes>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#get-global-emotes">API Reference</see>
    ///</summary>
    public Task<HelixResult<Emotes>> GetGlobalEmotes(CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.GetGlobalEmotes;
        var request = new RequestData(_baseUrl, endpoint);
        return HelixResultFactory.Create<Emotes>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#get-emote-sets">API Reference</see>
    ///</summary>
    public Task<HelixResult<EmoteSets>> GetEmoteSets(
        string emoteSetId,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.GetEmoteSets;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam("emote_set_id", emoteSetId);

        return HelixResultFactory.Create<EmoteSets>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#get-emote-sets">API Reference</see>
    ///</summary>
    public Task<HelixResult<EmoteSets>> GetEmoteSets(
        IEnumerable<string> emoteSetIds,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.GetEmoteSets;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddMultiParam("emote_set_id", emoteSetIds);

        return HelixResultFactory.Create<EmoteSets>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#get-chat-settings">API Reference</see>
    ///</summary>
    public Task<HelixResult<ChatSettings>> GetChatSettings(
        long broadcasterId,
        long? moderatorId = null,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.GetChatSettings;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.BroadcasterId, broadcasterId)
            .AddParam(QueryParams.ModeratorId, moderatorId);

        return HelixResultFactory.Create<ChatSettings>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#update-chat-settings">API Reference</see>
    ///</summary>
    public Task<HelixResult<ChatSettings>> UpdateChatSettings(
        long broadcasterId,
        NewChatSettings body,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.UpdateChatSettings;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.BroadcasterId, broadcasterId)
            .AddParam(QueryParams.ModeratorId, this.UserId);

        request.Body = body;
        return HelixResultFactory.Create<ChatSettings>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#get-user-block-list">API Reference</see>
    ///</summary>
    public Task<HelixResult<BlockList>> GetUserBlockList(
        int? first = null,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.GetUserBlockList;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.BroadcasterId, this.UserId)
            .AddParam(QueryParams.First, first);

        return HelixResultFactory.Create<BlockList>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#send-chat-announcement">API Reference</see>
    ///</summary>
    public Task<HelixResult> SendChatAnnouncement(
        long broadcasterId,
        Announcement body,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.SendChatAnnouncement;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.BroadcasterId, broadcasterId)
            .AddParam(QueryParams.ModeratorId, this.UserId);

        request.Body = body;
        return HelixResultFactory.Create(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#send-a-shoutout">API Reference</see>
    ///</summary>
    public Task<HelixResult> SendAShoutout(
        long fromBroadcasterId,
        long toBroadcasterId,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.SendAShoutout;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.FromBroadcasterId, fromBroadcasterId)
            .AddParam(QueryParams.ToBroadcasterId, toBroadcasterId)
            .AddParam(QueryParams.ModeratorId, this.UserId);

        return HelixResultFactory.Create(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#get-user-chat-color">API Reference</see>
    ///</summary>
    public Task<HelixResult<UsersChatColor>> GetUserChatColor(
        long userId,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.GetUserChatColor;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.UserId, userId);

        return HelixResultFactory.Create<UsersChatColor>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#get-user-chat-color">API Reference</see>
    ///</summary>
    public Task<HelixResult<UsersChatColor>> GetUserChatColor(
        IEnumerable<long> userIds,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.GetUserChatColor;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddMultiParam(QueryParams.UserId, userIds);

        return HelixResultFactory.Create<UsersChatColor>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#update-user-chat-color">API Reference</see>
    ///</summary>
    public Task<HelixResult> UpdateUserChatColor(
        ChatColor color,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.UpdateUserChatColor;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.UserId, this.UserId)
            .AddParam(QueryParams.Color, SnakeCase.Instance.ConvertToCase(color.ToString()));

        return HelixResultFactory.Create(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#update-user-chat-color">API Reference</see>
    ///</summary>
    public Task<HelixResult> UpdateUserChatColor(
        string hexColor,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.UpdateUserChatColor;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.UserId, this.UserId)
            .AddParam(QueryParams.Color, HttpUtility.UrlEncode(hexColor));

        return HelixResultFactory.Create(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#create-clip">API Reference</see>
    ///</summary>
    public Task<HelixResult<Clip>> CreateClip(
        long broadcasterId,
        bool? hasDelay = null,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.CreateClip;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.BroadcasterId, broadcasterId)
            .AddParam(QueryParams.HasDelay, hasDelay);

        return HelixResultFactory.Create<Clip>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#get-clips">API Reference</see>
    ///</summary>
    public Task<HelixResult<Clips>> GetClips(
        long broadcasterId,
        long gameId,
        string id,
        DateTime? startedAt = null,
        DateTime? endedAt = null,
        int? first = null,
        bool? isFeatured = null,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.GetClips;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.BroadcasterId, broadcasterId)
            .AddParam(QueryParams.GameId, gameId)
            .AddParam(QueryParams.Id, id)
            .AddParam(QueryParams.StartedAt, startedAt)
            .AddParam(QueryParams.EndedAt, endedAt)
            .AddParam(QueryParams.First, first)
            .AddParam(QueryParams.IsFeatured, isFeatured);

        return HelixResultFactory.Create<Clips>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#get-clips">API Reference</see>
    ///</summary>
    public Task<HelixResult<Clips>> GetClips(
        long broadcasterId,
        long gameId,
        IEnumerable<string> ids,
        DateTime? startedAt = null,
        DateTime? endedAt = null,
        int? first = null,
        bool? isFeatured = null,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.GetClips;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.BroadcasterId, broadcasterId)
            .AddParam(QueryParams.GameId, gameId)
            .AddMultiParam(QueryParams.Id, ids)
            .AddParam(QueryParams.StartedAt, startedAt)
            .AddParam(QueryParams.EndedAt, endedAt)
            .AddParam(QueryParams.First, first)
            .AddParam(QueryParams.IsFeatured, isFeatured);

        return HelixResultFactory.Create<Clips>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#get-conduits">API Reference</see>
    ///</summary>
    public Task<HelixResult<Conduits>> GetConduits(
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.GetConduits;
        RequestData request = new RequestData(_baseUrl, endpoint);

        return HelixResultFactory.Create<Conduits>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#create-conduits">API Reference</see>
    ///</summary>
    public Task<HelixResult<Conduits>> CreateConduits(
        int shardCount,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.CreateConduits;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.ShardCount, shardCount);

        return HelixResultFactory.Create<Conduits>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#update-conduits">API Reference</see>
    ///</summary>
    public Task<HelixResult<Conduits>> UpdateConduits(
        string id,
        int shardCount,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.UpdateConduits;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.Id, id)
            .AddParam(QueryParams.ShardCount, shardCount);

        return HelixResultFactory.Create<Conduits>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#delete-conduit">API Reference</see>
    ///</summary>
    public Task<HelixResult> DeleteConduits(
        string id,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.DeleteConduits;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.Id, id);

        return HelixResultFactory.Create(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#get-conduit-shards">API Reference</see>
    ///</summary>
    public Task<HelixResult<ConduitShards>> GetConduitShards(
        string conduitId,
        ConduitShardStatus? status = null,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.GetConduitShards;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.ConduitId, conduitId)
            .AddParam(QueryParams.Status, status?.ToString().ToLower());

        return HelixResultFactory.Create<ConduitShards>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#update-conduit-shards">API Reference</see>
    ///</summary>
    public Task<HelixResult<UpdatedConduitShards>> UpdateConduitShards(
        string conduitId,
        UpdateConduitRequest body,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.UpdateConduitShards;
        var request = new RequestData(_baseUrl, endpoint)
        {
            Body = new
            {
                conduit_id = conduitId,
                shards = new[] { body }
            }
        };

        return HelixResultFactory.Create<UpdatedConduitShards>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#update-conduit-shards">API Reference</see>
    ///</summary>
    public Task<HelixResult<UpdatedConduitShards>> UpdateConduitShards(
        string conduitId,
        IEnumerable<UpdateConduitRequest> body,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.UpdateConduitShards;
        var request = new RequestData(_baseUrl, endpoint)
        {
            Body = new
            {
                conduit_id = conduitId,
                shards = body
            }
        };

        return HelixResultFactory.Create<UpdatedConduitShards>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#get-content-classification-labels">API Reference</see>
    ///</summary>
    public Task<HelixResult<ContentClassificationLabels>> GetContentClassificationLabels(
        LabelLocale? locale = null,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.GetContentClassificationLabels;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.Locale, locale?.ToString().Replace('_', '-'));

        return HelixResultFactory.Create<ContentClassificationLabels>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#get-drops-entitlements">API Reference</see>
    ///</summary>
    public Task<HelixResult<DropsEntitlements>> GetDropsEntitlements(
        string? id = null,
        long? userId = null,
        string? gameId = null,
        FulfillmentStatus? fulfillmentStatus = null,
        int? first = null,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.GetDropsEntitlements;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.Id, id)
            .AddParam(QueryParams.UserId, userId)
            .AddParam(QueryParams.GameId, gameId)
            .AddParam(QueryParams.FulfillmentStatus, fulfillmentStatus?.ToString())
            .AddParam(QueryParams.First, first);

        return HelixResultFactory.Create<DropsEntitlements>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#get-drops-entitlements">API Reference</see>
    ///</summary>
    public Task<HelixResult<DropsEntitlements>> GetDropsEntitlements(
        IEnumerable<string>? ids = null,
        long? userId = null,
        string? gameId = null,
        FulfillmentStatus? fulfillmentStatus = null,
        int? first = null,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.GetDropsEntitlements;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddMultiParam(QueryParams.Id, ids)
            .AddParam(QueryParams.UserId, userId)
            .AddParam(QueryParams.GameId, gameId)
            .AddParam(QueryParams.FulfillmentStatus, fulfillmentStatus?.ToString())
            .AddParam(QueryParams.First, first);

        return HelixResultFactory.Create<DropsEntitlements>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#update-drops-entitlements">API Reference</see>
    ///</summary>
    public Task<HelixResult<UpdatedDropsEntitlements>> UpdateDropsEntitlements(
        IEnumerable<string>? entitlementIds = null,
        FulfillmentStatus? fulfillmentStatus = null,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.UpdateDropsEntitlements;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddMultiParam(QueryParams.EntitlementIds, entitlementIds)
            .AddParam(QueryParams.FulfillmentStatus, fulfillmentStatus?.ToString());


        return HelixResultFactory.Create<UpdatedDropsEntitlements>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#get-extension-configuration-segment">API Reference</see>
    ///</summary>
    public Task<HelixResult<Responses.ExtensionConfigurationSegment>> GetExtensionConfigurationSegment(
        string extensionId,
        ConfigSegmentType segment,
        long? broadcasterId = null,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.GetExtensionConfigurationSegment;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.ExtensionId, extensionId)
            .AddParam(QueryParams.Segment, segment.ToString().ToLower())
            .AddParam(QueryParams.BroadcasterId, broadcasterId);


        return HelixResultFactory.Create<Responses.ExtensionConfigurationSegment>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#get-extension-configuration-segment">API Reference</see>
    ///</summary>
    public Task<HelixResult<Responses.ExtensionConfigurationSegment>> GetExtensionConfigurationSegment(
        string extensionId,
        IEnumerable<ConfigSegmentType> segments,
        long? broadcasterId = null,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.GetExtensionConfigurationSegment;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.ExtensionId, extensionId)
            .AddMultiParam(QueryParams.Segment, segments.Select(x => x.ToString().ToLower()))
            .AddParam(QueryParams.BroadcasterId, broadcasterId);


        return HelixResultFactory.Create<Responses.ExtensionConfigurationSegment>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#set-extension-configuration-segment">API Reference</see>
    ///</summary>
    public Task<HelixResult> SetExtensionConfigurationSegment(
        ConfigurationSegment body,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.SetExtensionConfigurationSegment;
        var request = new RequestData(_baseUrl, endpoint)
        {
            Body = body
        };
        return HelixResultFactory.Create(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#set-extension-required-configuration">API Reference</see>
    ///</summary>
    public Task<HelixResult> SetExtensionRequiredConfiguration(
        long broadcasterId,
        ExtensionRequiredConfiguration body,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.SetExtensionRequiredConfiguration;
        var request = new RequestData(_baseUrl, endpoint)
        {
            Body = body
        };
        return HelixResultFactory.Create(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#send-extension-pubsub-message">API Reference</see>
    ///</summary>
    public Task<HelixResult> SendExtensionPubSubMessage(
        ExtensionPubSubMessage body,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.SendExtensionPubSubMessage;
        var request = new RequestData(_baseUrl, endpoint)
        {
            Body = body
        };
        return HelixResultFactory.Create(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#get-extension-live-channels">API Reference</see>
    ///</summary>
    public Task<HelixResult<ExtensionLiveChannels>> GetExtensionLiveChannels(
        string extensionId,
        int? first = null,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.GetExtensionLiveChannels;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.ExtensionId, extensionId)
            .AddParam(QueryParams.First, first);

        return HelixResultFactory.Create<ExtensionLiveChannels>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#get-extension-secrets">API Reference</see>
    ///</summary>
    public Task<HelixResult<ExtensionSecrets>> GetExtensionSecrets(
        string extensionId,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.GetExtensionSecrets;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.ExtensionId, extensionId);

        return HelixResultFactory.Create<ExtensionSecrets>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#create-extension-secret">API Reference</see>
    ///</summary>
    public Task<HelixResult<ExtensionSecrets>> CreateExtensionSecret(
        string extensionId,
        int? delay = null,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.CreateExtensionSecret;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.ExtensionId, extensionId)
            .AddParam(QueryParams.Delay, delay);

        return HelixResultFactory.Create<ExtensionSecrets>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#send-extension-chat-message">API Reference</see>
    ///</summary>
    public Task<HelixResult> SendExtensionChatMessage(
        long broadcasterId,
        ExtensionChatMessage body,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.SendExtensionChatMessage;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.BroadcasterId, broadcasterId);

        request.Body = body;
        return HelixResultFactory.Create(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#get-extensions">API Reference</see>
    ///</summary>
    public Task<HelixResult<ChannelExtensions>> GetExtensions(
        string extensionId,
        string? extensionVersion = null,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.GetExtensions;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.ExtensionId, extensionId)
            .AddParam(QueryParams.ExtensionVersion, extensionVersion);

        return HelixResultFactory.Create<ChannelExtensions>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#get-released-extensions">API Reference</see>
    ///</summary>
    public Task<HelixResult<ReleasedExtensions>> GetReleasedExtensions(
        string extensionId,
        string? extensionVersion = null,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.GetReleasedExtensions;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.ExtensionId, extensionId)
            .AddParam(QueryParams.ExtensionVersion, extensionVersion);

        return HelixResultFactory.Create<ReleasedExtensions>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#get-extension-bits-products">API Reference</see>
    ///</summary>
    public Task<HelixResult<ExtensionBitsProducts>> GetExtensionBitsProducts(
        bool? shouldIncludeAll = null,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.GetExtensionBitsProducts;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.ShouldIncludeAll, shouldIncludeAll);

        return HelixResultFactory.Create<ExtensionBitsProducts>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#update-extension-bits-product">API Reference</see>
    ///</summary>
    public Task<HelixResult<ExtensionBitsProducts>> UpdateExtensionBitsProduct(
        UpdatedBitsProduct body,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.UpdateExtensionBitsProduct;
        var request = new RequestData(_baseUrl, endpoint)
        {
            Body = body
        };
        return HelixResultFactory.Create<ExtensionBitsProducts>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#create-eventsub-subscription">API Reference</see>
    ///</summary>
    public Task<HelixResult<CreatedSubscription>> CreateEventSubSubscription(
        NewSubscription body,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.CreateEventSubSubscription;
        var request = new RequestData(_baseUrl, endpoint)
        {
            Body = body
        };
        return HelixResultFactory.Create<CreatedSubscription>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#delete-eventsub-subscription">API Reference</see>
    ///</summary>
    public Task<HelixResult> DeleteEventSubSubscription(
        string id,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.DeleteEventSubSubscription;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.Id, id);

        return HelixResultFactory.Create(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#get-eventsub-subscriptions">API Reference</see>
    ///</summary>
    public Task<HelixResult<EventSubSubscriptions>> GetEventSubSubscriptions(
        EventSubStatus? status = null,
        string? type = null,
        long? userId = null,
        string? subscriptionId = null,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.GetEventSubSubscriptions;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.Status, SnakeCase.Instance.ConvertToCase(status.ToString()))
            .AddParam(QueryParams.Type, type)
            .AddParam(QueryParams.UserId, userId)
            .AddParam(QueryParams.SubscriptionId, subscriptionId);

        return HelixResultFactory.Create<EventSubSubscriptions>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#get-top-games">API Reference</see>
    ///</summary>
    public Task<HelixResult<Games>> GetTopGames(
        int? first = null,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.GetTopGames;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.First, first);

        return HelixResultFactory.Create<Games>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#get-games">API Reference</see>
    ///</summary>
    public Task<HelixResult<Games>> GetGames(
        string? id = null,
        string? name = null,
        string? igdbId = null,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.GetGames;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.Id, id)
            .AddParam(QueryParams.Name, name)
            .AddParam(QueryParams.IgdbId, igdbId);

        return HelixResultFactory.Create<Games>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#get-games">API Reference</see>
    ///</summary>
    public Task<HelixResult<Games>> GetGames(
        IEnumerable<string>? ids = null,
        IEnumerable<string>? names = null,
        IEnumerable<string>? igdbIds = null,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.GetGames;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddMultiParam(QueryParams.Id, ids)
            .AddMultiParam(QueryParams.Name, names)
            .AddMultiParam(QueryParams.IgdbId, igdbIds);

        return HelixResultFactory.Create<Games>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#get-creator-goals">API Reference</see>
    ///</summary>
    public Task<HelixResult<CreatorGoals>> GetCreatorGoals(CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.GetCreatorGoals;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.BroadcasterId, this.UserId);

        return HelixResultFactory.Create<CreatorGoals>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#get-channel-guest-star-settings">API Reference</see>
    ///</summary>
    public Task<HelixResult<ChannelGuestStarSettings>> GetChannelGuestStarSettings(
        long broadcasterId,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.GetChannelGuestStarSettings;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.BroadcasterId, broadcasterId)
            .AddParam(QueryParams.ModeratorId, this.UserId);

        return HelixResultFactory.Create<ChannelGuestStarSettings>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#update-channel-guest-star-settings">API Reference</see>
    ///</summary>
    public Task<HelixResult> UpdateChannelGuestStarSettings(
        NewGuestStarSettings body,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.UpdateChannelGuestStarSettings;
        RequestData request = new RequestData(_baseUrl, endpoint)
        {
            Body = body
        }.AddParam(QueryParams.BroadcasterId, this.UserId);

        return HelixResultFactory.Create(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#get-guest-star-session">API Reference</see>
    ///</summary>
    public Task<HelixResult<GuestStarSession>> GetGuestStarSession(
        long broadcasterId,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.GetGuestStarSession;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.BroadcasterId, broadcasterId)
            .AddParam(QueryParams.ModeratorId, this.UserId);

        return HelixResultFactory.Create<GuestStarSession>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#create-guest-star-session">API Reference</see>
    ///</summary>
    public Task<HelixResult<GuestStarSession>> CreateGuestStarSession(CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.CreateGuestStarSession;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.BroadcasterId, this.UserId);

        return HelixResultFactory.Create<GuestStarSession>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#end-guest-star-session">API Reference</see>
    ///</summary>
    public Task<HelixResult<GuestStarSession>> EndGuestStarSession(
        string sessionId,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.EndGuestStarSession;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.BroadcasterId, this.UserId)
            .AddParam(QueryParams.SessionId, sessionId);

        return HelixResultFactory.Create<GuestStarSession>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#get-guest-star-invites">API Reference</see>
    ///</summary>
    public Task<HelixResult<GuestStarInvites>> GetGuestStarInvites(
        long broadcasterId,
        string sessionId,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.GetGuestStarInvites;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.BroadcasterId, broadcasterId)
            .AddParam(QueryParams.ModeratorId, this.UserId)
            .AddParam(sessionId, sessionId);

        return HelixResultFactory.Create<GuestStarInvites>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#send-guest-star-invite">API Reference</see>
    ///</summary>
    public Task<HelixResult> SendGuestStarInvite(
        long broadcasterId,
        string sessionId,
        long guestId,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.SendGuestStarInvite;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.BroadcasterId, broadcasterId)
            .AddParam(QueryParams.ModeratorId, this.UserId)
            .AddParam(QueryParams.GuestId, guestId);

        return HelixResultFactory.Create(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#delete-guest-star-invite">API Reference</see>
    ///</summary>
    public Task<HelixResult> DeleteGuestStarInvite(
        long broadcasterId,
        string sessionId,
        long guestId,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.DeleteGuestStarInvite;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.BroadcasterId, broadcasterId)
            .AddParam(QueryParams.ModeratorId, this.UserId)
            .AddParam(QueryParams.SessionId, sessionId)
            .AddParam(QueryParams.GuestId, guestId);

        return HelixResultFactory.Create(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#assign-guest-star-slot">API Reference</see>
    ///</summary>
    public Task<HelixResult> AssignGuestStarSlot(
        long broadcasterId,
        string sessionId,
        long guestId,
        string slotId,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.AssignGuestStarSlot;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.BroadcasterId, broadcasterId)
            .AddParam(QueryParams.ModeratorId, this.UserId)
            .AddParam(QueryParams.SessionId, sessionId)
            .AddParam(QueryParams.GuestId, guestId)
            .AddParam(QueryParams.SlotId, slotId);

        return HelixResultFactory.Create(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#update-guest-star-slot">API Reference</see>
    ///</summary>
    public Task<HelixResult> UpdateGuestStarSlot(
        long broadcasterId,
        string sessionId,
        string sourceSlotId,
        string? destinationSlotId = null,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.UpdateGuestStarSlot;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.BroadcasterId, broadcasterId)
            .AddParam(QueryParams.ModeratorId, this.UserId)
            .AddParam(QueryParams.SessionId, sessionId)
            .AddParam(QueryParams.SourceSlotId, sourceSlotId)
            .AddParam(QueryParams.DestinationSlotId, destinationSlotId);

        return HelixResultFactory.Create(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#delete-guest-star-slot">API Reference</see>
    ///</summary>
    public Task<HelixResult> DeleteGuestStarSlot(
        long broadcasterId,
        string sessionId,
        long guestId,
        string slotId,
        string? shouldReinviteGuest = null,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.DeleteGuestStarSlot;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.BroadcasterId, broadcasterId)
            .AddParam(QueryParams.ModeratorId, this.UserId)
            .AddParam(QueryParams.SessionId, sessionId)
            .AddParam(QueryParams.GuestId, guestId)
            .AddParam(QueryParams.SlotId, slotId)
            .AddParam(QueryParams.ShouldReinviteGuest, shouldReinviteGuest);

        return HelixResultFactory.Create(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#update-guest-star-slot-settings">API Reference</see>
    ///</summary>
    public Task<HelixResult> UpdateGuestStarSlotSettings(
        long broadcasterId,
        string sessionId,
        string slotId,
        bool? isAudioEnabled = null,
        bool? isVideoEnabled = null,
        bool? isLive = null,
        int? volume = null,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.UpdateGuestStarSlotSettings;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.BroadcasterId, broadcasterId)
            .AddParam(QueryParams.ModeratorId, this.UserId)
            .AddParam(QueryParams.SessionId, sessionId)
            .AddParam(QueryParams.SlotId, slotId)
            .AddParam(QueryParams.IsAudioEnabled, isAudioEnabled)
            .AddParam(QueryParams.IsVideoEnabled, isVideoEnabled)
            .AddParam(QueryParams.IsLive, isLive)
            .AddParam(QueryParams.Volume, volume);

        return HelixResultFactory.Create(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#get-hype-train-events">API Reference</see>
    ///</summary>
    public Task<HelixResult<HypeTrainEvents>> GetHypeTrainEvents(
        int? first = null,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.GetHypeTrainEvents;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.BroadcasterId, this.UserId)
            .AddParam(QueryParams.First, first);

        return HelixResultFactory.Create<HypeTrainEvents>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#check-automod-status">API Reference</see>
    ///</summary>
    public Task<HelixResult<AutoModStatus>> CheckAutoModStatus(
        MessageToCheck body,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.CheckAutoModStatus;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.BroadcasterId, this.UserId);

        request.Body = body;
        return HelixResultFactory.Create<AutoModStatus>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#manage-held-automod-messages">API Reference</see>
    ///</summary>
    public Task<HelixResult> ManageHeldAutoModMessages(
        string msgId,
        string action,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.ManageHeldAutoModMessages;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.UserId, this.UserId)
            .AddParam(QueryParams.MsgId, msgId)
            .AddParam(QueryParams.Action, action);

        return HelixResultFactory.Create(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#get-automod-settings">API Reference</see>
    ///</summary>
    public Task<HelixResult<AutoModSettings>> GetAutoModSettings(
        long broadcasterId,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.GetAutoModSettings;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.BroadcasterId, broadcasterId)
            .AddParam(QueryParams.ModeratorId, this.UserId);

        return HelixResultFactory.Create<AutoModSettings>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#update-automod-settings">API Reference</see>
    ///</summary>
    public Task<HelixResult<AutoModSettings>> UpdateAutoModSettings(
        long broadcasterId,
        NewAutoModSettings body,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.UpdateAutoModSettings;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.BroadcasterId, broadcasterId)
            .AddParam(QueryParams.ModeratorId, this.UserId);

        request.Body = body;
        return HelixResultFactory.Create<AutoModSettings>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#get-banned-users">API Reference</see>
    ///</summary>
    public Task<HelixResult<BannedUsers>> GetBannedUsers(
        long? userId = null,
        int? first = null,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.GetBannedUsers;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.BroadcasterId, this.UserId)
            .AddParam(QueryParams.UserId, userId)
            .AddParam(QueryParams.First, first);

        return HelixResultFactory.Create<BannedUsers>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#get-banned-users">API Reference</see>
    ///</summary>
    public Task<HelixResult<BannedUsers>> GetBannedUsers(
        IEnumerable<long>? userIds = null,
        int? first = null,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.GetBannedUsers;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.BroadcasterId, this.UserId)
            .AddMultiParam(QueryParams.UserId, userIds)
            .AddParam(QueryParams.First, first);

        return HelixResultFactory.Create<BannedUsers>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#ban-user">API Reference</see>
    ///</summary>
    public Task<HelixResult<BannedUser>> BanUser(
        long broadcasterId,
        UserToBan body,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.BanUser;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.BroadcasterId, broadcasterId)
            .AddParam(QueryParams.ModeratorId, this.UserId);

        request.Body = new
        {
            data = body
        };

        return HelixResultFactory.Create<BannedUser>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#unban-user">API Reference</see>
    ///</summary>
    public Task<HelixResult> UnbanUser(
        long broadcasterId,
        long userId,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.UnbanUser;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.BroadcasterId, broadcasterId)
            .AddParam(QueryParams.ModeratorId, this.UserId)
            .AddParam(QueryParams.UserId, userId);

        return HelixResultFactory.Create(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#get-blocked-terms">API Reference</see>
    ///</summary>
    public Task<HelixResult<BlockedTerms>> GetBlockedTerms(
        long broadcasterId,
        int? first = null,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.GetBlockedTerms;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.BroadcasterId, broadcasterId)
            .AddParam(QueryParams.ModeratorId, this.UserId)
            .AddParam(QueryParams.First, first);

        return HelixResultFactory.Create<BlockedTerms>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#add-blocked-term">API Reference</see>
    ///</summary>
    public Task<HelixResult<BlockedTerms>> AddBlockedTerm(
        long broadcasterId,
        string text,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.AddBlockedTerm;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.BroadcasterId, broadcasterId)
            .AddParam(QueryParams.ModeratorId, this.UserId);

        request.Body = new { text };
        return HelixResultFactory.Create<BlockedTerms>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#remove-blocked-term">API Reference</see>
    ///</summary>
    public Task<HelixResult> RemoveBlockedTerm(
        long broadcasterId,
        string id,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.RemoveBlockedTerm;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.BroadcasterId, broadcasterId)
            .AddParam(QueryParams.ModeratorId, this.UserId)
            .AddParam(QueryParams.Id, id);

        return HelixResultFactory.Create(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#delete-chat-messages">API Reference</see>
    ///</summary>
    public Task<HelixResult> DeleteChatMessages(
        long broadcasterId,
        string? messageId = null,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.DeleteChatMessages;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.BroadcasterId, broadcasterId)
            .AddParam(QueryParams.ModeratorId, this.UserId)
            .AddParam(QueryParams.MessageId, messageId);

        return HelixResultFactory.Create(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#get-moderators">API Reference</see>
    ///</summary>
    public Task<HelixResult<Moderators>> GetModerators(
        long? userId = null,
        int? first = null,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.GetModerators;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.BroadcasterId, this.UserId)
            .AddParam(QueryParams.UserId, userId)
            .AddParam(QueryParams.First, first);

        return HelixResultFactory.Create<Moderators>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#get-moderators">API Reference</see>
    ///</summary>
    public Task<HelixResult<Moderators>> GetModerators(
        IEnumerable<long>? userIds = null,
        int? first = null,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.GetModerators;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.BroadcasterId, this.UserId)
            .AddMultiParam(QueryParams.UserId, userIds)
            .AddParam(QueryParams.First, first);

        return HelixResultFactory.Create<Moderators>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#add-channel-moderator">API Reference</see>
    ///</summary>
    public Task<HelixResult> AddChannelModerator(
        long userId,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.AddChannelModerator;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.BroadcasterId, this.UserId)
            .AddParam(QueryParams.UserId, userId);

        return HelixResultFactory.Create(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#remove-channel-moderator">API Reference</see>
    ///</summary>
    public Task<HelixResult> RemoveChannelModerator(
        long userId,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.RemoveChannelModerator;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.BroadcasterId, this.UserId)
            .AddParam(QueryParams.UserId, userId);

        return HelixResultFactory.Create(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#get-vi-ps">API Reference</see>
    ///</summary>
    public Task<HelixResult<VIPs>> GetVIPs(
        long? userId = null,
        int? first = null,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.GetVIPs;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.BroadcasterId, this.UserId)
            .AddParam(QueryParams.UserId, userId)
            .AddParam(QueryParams.First, first);

        return HelixResultFactory.Create<VIPs>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#get-vi-ps">API Reference</see>
    ///</summary>
    public Task<HelixResult<VIPs>> GetVIPs(
        IEnumerable<long>? userIds = null,
        int? first = null,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.GetVIPs;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.BroadcasterId, this.UserId)
            .AddMultiParam(QueryParams.UserId, userIds)
            .AddParam(QueryParams.First, first);

        return HelixResultFactory.Create<VIPs>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#add-channel-vip">API Reference</see>
    ///</summary>
    public Task<HelixResult> AddChannelVIP(
        long userId,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.AddChannelVIP;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.BroadcasterId, this.UserId)
            .AddParam(QueryParams.UserId, userId);

        return HelixResultFactory.Create(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#remove-channel-vip">API Reference</see>
    ///</summary>
    public Task<HelixResult> RemoveChannelVIP(
        long userId,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.RemoveChannelVIP;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.BroadcasterId, this.UserId)
            .AddParam(QueryParams.UserId, userId);

        return HelixResultFactory.Create(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#update-shield-mode-status">API Reference</see>
    ///</summary>
    public Task<HelixResult<ShieldModeStatus>> UpdateShieldModeStatus(
        long broadcasterId,
        bool isActive,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.UpdateShieldModeStatus;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.BroadcasterId, broadcasterId)
            .AddParam(QueryParams.ModeratorId, this.UserId);

        request.Body = new { is_active = isActive };
        return HelixResultFactory.Create<ShieldModeStatus>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#get-shield-mode-status">API Reference</see>
    ///</summary>
    public Task<HelixResult<ShieldModeStatus>> GetShieldModeStatus(
        long broadcasterId,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.GetShieldModeStatus;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.BroadcasterId, broadcasterId)
            .AddParam(QueryParams.ModeratorId, this.UserId);

        return HelixResultFactory.Create<ShieldModeStatus>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#get-polls">API Reference</see>
    ///</summary>
    public Task<HelixResult<Polls>> GetPolls(
        string? id = null,
        int? first = null,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.GetPolls;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.BroadcasterId, this.UserId)
            .AddParam(QueryParams.Id, id)
            .AddParam(QueryParams.First, first);

        return HelixResultFactory.Create<Polls>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#get-polls">API Reference</see>
    ///</summary>
    public Task<HelixResult<Polls>> GetPolls(
        IEnumerable<string>? ids = null,
        int? first = null,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.GetPolls;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.BroadcasterId, this.UserId)
            .AddMultiParam(QueryParams.Id, ids)
            .AddParam(QueryParams.First, first);

        return HelixResultFactory.Create<Polls>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#create-poll">API Reference</see>
    ///</summary>
    public Task<HelixResult<Poll>> CreatePoll(
        NewPoll body,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.CreatePoll;
        var request = new RequestData(_baseUrl, endpoint)
        {
            Body = body
        };

        return HelixResultFactory.Create<Poll>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#end-poll">API Reference</see>
    ///</summary>
    public Task<HelixResult<Poll>> EndPoll(
        string id,
        string status,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.EndPoll;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.BroadcasterId, this.UserId)
            .AddParam(QueryParams.Id, id)
            .AddParam(QueryParams.Status, status);

        return HelixResultFactory.Create<Poll>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#get-predictions">API Reference</see>
    ///</summary>
    public Task<HelixResult<Predictions>> GetPredictions(
        string? id = null,
        int? first = null,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.GetPredictions;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.BroadcasterId, this.UserId)
            .AddParam(QueryParams.Id, id)
            .AddParam(QueryParams.First, first);

        return HelixResultFactory.Create<Predictions>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#get-predictions">API Reference</see>
    ///</summary>
    public Task<HelixResult<Predictions>> GetPredictions(
        IEnumerable<string>? ids = null,
        int? first = null,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.GetPredictions;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.BroadcasterId, this.UserId)
            .AddMultiParam(QueryParams.Id, ids)
            .AddParam(QueryParams.First, first);

        return HelixResultFactory.Create<Predictions>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#create-prediction">API Reference</see>
    ///</summary>
    public Task<HelixResult<Prediction>> CreatePrediction(
        NewPrediction body,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.CreatePrediction;
        var request = new RequestData(_baseUrl, endpoint)
        {
            Body = body
        };

        return HelixResultFactory.Create<Prediction>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#end-prediction">API Reference</see>
    ///</summary>
    public Task<HelixResult<Prediction>> EndPrediction(
        PredictionToEnd body,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.EndPrediction;
        var request = new RequestData(_baseUrl, endpoint)
        {
            Body = body
        };

        return HelixResultFactory.Create<Prediction>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#start-a-raid">API Reference</see>
    ///</summary>
    public Task<HelixResult<Raid>> StartRaid(
        long toBroadcasterId,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.StartARaid;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.FromBroadcasterId, this.UserId)
            .AddParam(QueryParams.ToBroadcasterId, toBroadcasterId);

        return HelixResultFactory.Create<Raid>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#cancel-a-raid">API Reference</see>
    ///</summary>
    public Task<HelixResult> CancelRaid(CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.CancelARaid;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.BroadcasterId, this.UserId);

        return HelixResultFactory.Create(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#get-channel-stream-schedule">API Reference</see>
    ///</summary>
    public Task<HelixResult<StreamSchedule>> GetChannelStreamSchedule(
        long broadcasterId,
        string? id = null,
        DateTime? startTime = null,
        int? first = null,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.GetChannelStreamSchedule;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.BroadcasterId, broadcasterId)
            .AddParam(QueryParams.Id, id)
            .AddParam(QueryParams.StartTime, startTime)
            .AddParam(QueryParams.First, first);

        return HelixResultFactory.Create<StreamSchedule>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#get-channel-stream-schedule">API Reference</see>
    ///</summary>
    public Task<HelixResult<StreamSchedule>> GetChannelStreamSchedule(
        long broadcasterId,
        IEnumerable<string>? ids = null,
        DateTime? startTime = null,
        int? first = null,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.GetChannelStreamSchedule;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.BroadcasterId, broadcasterId)
            .AddMultiParam(QueryParams.Id, ids)
            .AddParam(QueryParams.StartTime, startTime)
            .AddParam(QueryParams.First, first);

        return HelixResultFactory.Create<StreamSchedule>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#update-channel-stream-schedule">API Reference</see>
    ///</summary>
    public Task<HelixResult> UpdateChannelStreamSchedule(
        bool? isVacationEnabled = null,
        DateTime? vacationStartTime = null,
        DateTime? vacationEndTime = null,
        string? timezone = null,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.UpdateChannelStreamSchedule;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.BroadcasterId, this.UserId)
            .AddParam(QueryParams.IsVacationEnabled, isVacationEnabled)
            .AddParam(QueryParams.VacationStartTime, vacationStartTime)
            .AddParam(QueryParams.VacationEndTime, vacationEndTime)
            .AddParam(QueryParams.Timezone, timezone);

        return HelixResultFactory.Create(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#create-channel-stream-schedule-segment">API Reference</see>
    ///</summary>
    public Task<HelixResult<ScheduleSegment>> CreateChannelStreamScheduleSegment(
        NewScheduleSegment body,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.CreateChannelStreamScheduleSegment;
        RequestData request = new RequestData(_baseUrl, endpoint)
        {
            Body = body
        }.AddParam(QueryParams.BroadcasterId, this.UserId);

        return HelixResultFactory.Create<ScheduleSegment>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#update-channel-stream-schedule-segment">API Reference</see>
    ///</summary>
    public Task<HelixResult<Responses.UpdatedScheduleSegment>> UpdateChannelStreamScheduleSegment(
        string id,
        Requests.UpdatedScheduleSegment Body,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.UpdateChannelStreamScheduleSegment;
        RequestData request = new RequestData(_baseUrl, endpoint)
        {
            Body = Body
        }
        .AddParam(QueryParams.BroadcasterId, this.UserId)
        .AddParam(QueryParams.Id, id);

        return HelixResultFactory.Create<Responses.UpdatedScheduleSegment>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#delete-channel-stream-schedule-segment">API Reference</see>
    ///</summary>
    public Task<HelixResult> DeleteChannelStreamScheduleSegment(
        string id,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.DeleteChannelStreamScheduleSegment;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.BroadcasterId, this.UserId)
            .AddParam(QueryParams.Id, id);

        return HelixResultFactory.Create(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#search-categories">API Reference</see>
    ///</summary>
    public Task<HelixResult<Categories>> SearchCategories(
        string query,
        int? first = null,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.SearchCategories;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.Query, HttpUtility.UrlEncode(query))
            .AddParam(QueryParams.First, first);

        return HelixResultFactory.Create<Categories>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#search-channels">API Reference</see>
    ///</summary>
    public Task<HelixResult<Channels>> SearchChannels(
        string query,
        bool? liveOnly = null,
        int? first = null,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.SearchChannels;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.Query, query)
            .AddParam(QueryParams.LiveOnly, liveOnly)
            .AddParam(QueryParams.First, first);

        return HelixResultFactory.Create<Channels>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#get-stream-key">API Reference</see>
    ///</summary>
    public Task<HelixResult<StreamKey>> GetStreamKey(CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.GetStreamKey;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.BroadcasterId, this.UserId);

        return HelixResultFactory.Create<StreamKey>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#get-streams">API Reference</see>
    ///</summary>
    public Task<HelixResult<Streams>> GetStreams(
        long? userId = null,
        string? userLogin = null,
        string? gameId = null,
        StreamTypes? type = null,
        string? language = null,
        int? first = null,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.GetStreams;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.UserId, userId)
            .AddParam(QueryParams.UserLogin, userLogin)
            .AddParam(QueryParams.GameId, gameId)
            .AddParam(QueryParams.Type, type?.ToString().ToLower())
            .AddParam(QueryParams.Language, language)
            .AddParam(QueryParams.First, first);

        return HelixResultFactory.Create<Streams>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#get-streams">API Reference</see>
    ///</summary>
    public Task<HelixResult<Streams>> GetStreams(
        IEnumerable<long>? userIds = null,
        IEnumerable<string>? userLogins = null,
        IEnumerable<string>? gameIds = null,
        StreamTypes? type = null,
        IEnumerable<string>? languages = null,
        int? first = null,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.GetStreams;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddMultiParam(QueryParams.UserId, userIds)
            .AddMultiParam(QueryParams.UserLogin, userLogins)
            .AddMultiParam(QueryParams.GameId, gameIds)
            .AddParam(QueryParams.Type, type?.ToString().ToLower())
            .AddMultiParam(QueryParams.Language, languages)
            .AddParam(QueryParams.First, first);

        return HelixResultFactory.Create<Streams>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#get-followed-streams">API Reference</see>
    ///</summary>
    public Task<HelixResult<FollowedStreams>> GetFollowedStreams(
        int? first = null,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.GetFollowedStreams;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.UserId, this.UserId)
            .AddParam(QueryParams.First, first);

        return HelixResultFactory.Create<FollowedStreams>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#create-stream-marker">API Reference</see>
    ///</summary>
    public Task<HelixResult<StreamMarker>> CreateStreamMarker(
        string? description = null,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.CreateStreamMarker;
        var request = new RequestData(_baseUrl, endpoint)
        {
            Body = new
            {
                user_id = this.UserId,
                description
            }
        };

        return HelixResultFactory.Create<StreamMarker>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#get-stream-markers">API Reference</see>
    ///</summary>
    public Task<HelixResult<StreamMarkers>> GetStreamMarkers(
        string? videoId,
        int? first = null,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.GetStreamMarkers;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.UserId, this.UserId)
            .AddParam(QueryParams.VideoId, videoId)
            .AddParam(QueryParams.First, first);

        return HelixResultFactory.Create<StreamMarkers>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#get-broadcaster-subscriptions">API Reference</see>
    ///</summary>
    public Task<HelixResult<BroadcasterSubscriptions>> GetBroadcasterSubscriptions(
        long? userId = null,
        int? first = null,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.GetBroadcasterSubscriptions;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.BroadcasterId, this.UserId)
            .AddParam(QueryParams.UserId, userId)
            .AddParam(QueryParams.First, first);

        return HelixResultFactory.Create<BroadcasterSubscriptions>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#get-broadcaster-subscriptions">API Reference</see>
    ///</summary>
    public Task<HelixResult<BroadcasterSubscriptions>> GetBroadcasterSubscriptions(
        IEnumerable<long>? userIds = null,
        int? first = null,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.GetBroadcasterSubscriptions;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.BroadcasterId, this.UserId)
            .AddMultiParam(QueryParams.UserId, userIds)
            .AddParam(QueryParams.First, first);

        return HelixResultFactory.Create<BroadcasterSubscriptions>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#check-user-subscription">API Reference</see>
    ///</summary>
    public Task<HelixResult<UserSubscription>> CheckUserSubscription(
        long broadcasterId,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.CheckUserSubscription;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.BroadcasterId, broadcasterId)
            .AddParam(QueryParams.UserId, this.UserId);

        return HelixResultFactory.Create<UserSubscription>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#get-channel-teams">API Reference</see>
    ///</summary>
    public Task<HelixResult<ChannelTeams>> GetChannelTeams(
        long broadcasterId,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.GetChannelTeams;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.BroadcasterId, broadcasterId);

        return HelixResultFactory.Create<ChannelTeams>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#get-teams">API Reference</see>
    ///</summary>
    public Task<HelixResult<Teams>> GetTeams(
        string name,
        string id,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.GetTeams;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.Name, name)
            .AddParam(QueryParams.Id, id);

        return HelixResultFactory.Create<Teams>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#get-users">API Reference</see>
    ///</summary>
    public Task<HelixResult<Users>> GetUsers(
        long? id = null,
        string? login = null,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.GetUsers;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.Id, id)
            .AddParam(QueryParams.Login, login);

        return HelixResultFactory.Create<Users>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#get-users">API Reference</see>
    ///</summary>
    public Task<HelixResult<Users>> GetUsers(
        IEnumerable<long>? id = null,
        IEnumerable<string>? login = null,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.GetUsers;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddMultiParam(QueryParams.Id, id)
            .AddMultiParam(QueryParams.Login, login);

        return HelixResultFactory.Create<Users>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#update-user">API Reference</see>
    ///</summary>
    public Task<HelixResult<UpdatedUser>> UpdateUser(
        string? description = null,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.UpdateUser;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam("description", description);

        return HelixResultFactory.Create<UpdatedUser>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#block-user">API Reference</see>
    ///</summary>
    public Task<HelixResult> BlockUser(
        long targetUserId,
        string? sourceContext = null,
        string? reason = null,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.BlockUser;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.TargetUserId, targetUserId)
            .AddParam(QueryParams.SourceContext, sourceContext)
            .AddParam(QueryParams.Reason, reason);

        return HelixResultFactory.Create(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#unblock-user">API Reference</see>
    ///</summary>
    public Task<HelixResult> UnblockUser(
        long targetUserId,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.UnblockUser;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.TargetUserId, targetUserId);

        return HelixResultFactory.Create(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#get-user-extensions">API Reference</see>
    ///</summary>
    public Task<HelixResult<UserExtensions>> GetUserExtensions(CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.GetUserExtensions;
        var request = new RequestData(_baseUrl, endpoint);
        return HelixResultFactory.Create<UserExtensions>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#get-user-active-extensions">API Reference</see>
    ///</summary>
    public Task<HelixResult<ActiveExtensions>> GetUserActiveExtensions(CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.GetUserActiveExtensions;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.UserId, this.UserId);

        return HelixResultFactory.Create<ActiveExtensions>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#get-videos">API Reference</see>
    ///</summary>
    public Task<HelixResult<Videos>> GetVideos(
        string? id = null,
        long? userId = null,
        string? gameId = null,
        string? language = null,
        VideoPeriod? period = null,
        VideoSortMethod? sort = null,
        VideoType? type = null,
        int? first = null,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.GetVideos;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.Id, id)
            .AddParam(QueryParams.UserId, userId)
            .AddParam(QueryParams.GameId, gameId)
            .AddParam(QueryParams.Language, language)
            .AddParam(QueryParams.Period, period?.ToString().ToLower())
            .AddParam(QueryParams.Sort, sort?.ToString().ToLower())
            .AddParam(QueryParams.Type, type?.ToString().ToLower())
            .AddParam(QueryParams.First, first);

        return HelixResultFactory.Create<Videos>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#get-videos">API Reference</see>
    ///</summary>
    public Task<HelixResult<Videos>> GetVideos(
        IEnumerable<string>? ids = null,
        long? userId = null,
        string? gameId = null,
        string? language = null,
        VideoPeriod? period = null,
        VideoSortMethod? sort = null,
        VideoType? type = null,
        int? first = null,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.GetVideos;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddMultiParam(QueryParams.Id, ids)
            .AddParam(QueryParams.UserId, userId)
            .AddParam(QueryParams.GameId, gameId)
            .AddParam(QueryParams.Language, language)
            .AddParam(QueryParams.Period, period?.ToString().ToLower())
            .AddParam(QueryParams.Sort, sort?.ToString().ToLower())
            .AddParam(QueryParams.Type, type?.ToString().ToLower())
            .AddParam(QueryParams.First, first);

        return HelixResultFactory.Create<Videos>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#delete-videos">API Reference</see>
    ///</summary>
    public Task<HelixResult> DeleteVideos(
        string id,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.DeleteVideos;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.Id, id);

        return HelixResultFactory.Create(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#delete-videos">API Reference</see>
    ///</summary>
    public Task<HelixResult> DeleteVideos(
        IEnumerable<string> ids,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.DeleteVideos;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddMultiParam(QueryParams.Id, ids);

        return HelixResultFactory.Create(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#send-whisper">API Reference</see>
    ///</summary>
    public Task<HelixResult> SendWhisper(
        long toUserId,
        string message,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.SendWhisper;
        RequestData request = new RequestData(_baseUrl, endpoint)
        {
            Body = new
            {
                message
            }
        }
        .AddParam(QueryParams.FromUserId, this.UserId)
        .AddParam(QueryParams.ToUserId, toUserId);

        return HelixResultFactory.Create(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#get-ad-schedule">API Reference</see>
    ///</summary>
    public Task<HelixResult<AdSchedule>> GetAdSchedule(
        long broadcasterId,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.GetAdSchedule;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.BroadcasterId, broadcasterId);

        return HelixResultFactory.Create<AdSchedule>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#snooze-next-ad">API Reference</see>
    ///</summary>
    public Task<HelixResult<SnoozedAd>> SnoozeNextAd(
        long broadcasterId,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.SnoozeNextAd;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.BroadcasterId, broadcasterId);

        return HelixResultFactory.Create<SnoozedAd>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#get-global-chat-badges">API Reference</see>
    ///</summary>
    public Task<HelixResult<ChatBadges>> GetGlobalChatBadges(CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.GetGlobalChatBadges;
        var request = new RequestData(_baseUrl, endpoint);
        return HelixResultFactory.Create<ChatBadges>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#get-channel-chat-badges">API Reference</see>
    ///</summary>
    public Task<HelixResult<ChatBadges>> GetChannelChatBadges(
        long broadcasterId,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.GetChannelChatBadges;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.BroadcasterId, broadcasterId);

        return HelixResultFactory.Create<ChatBadges>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#get-moderated-channels">API Reference</see>
    ///</summary>
    public Task<HelixResult<ModeratedChannels>> GetModeratedChannels(
        int? first = null,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.GetModeratedChannels;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.UserId, this.UserId)
            .AddParam(QueryParams.First, first);

        return HelixResultFactory.Create<ModeratedChannels>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#send-chat-message">API Reference</see>
    ///</summary>
    public Task<HelixResult<SentMessage>> SendChatMessage(
        ChatMessage message,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.SendChatMessage;
        var request = new RequestData(_baseUrl, endpoint);
        message.SenderId = this.UserId;
        request.Body = message;
        return HelixResultFactory.Create<SentMessage>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#get-user-emotes">API Reference</see>
    ///</summary>
    public Task<HelixResult<UserEmotes>> GetUserEmotes(
        long? broadcasterId = null,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.GetUserEmotes;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.UserId, this.UserId)
            .AddParam(QueryParams.BroadcasterId, broadcasterId);

        return HelixResultFactory.Create<UserEmotes>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#get-unban-requests">API Reference</see>
    ///</summary>
    public Task<HelixResult<UnbanRequests>> GetUnbanRequests(
        long broadcasterId,
        UnbanRequestStatus status,
        long? userId = null,
        int? first = null,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.GetUnbanRequests;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.BroadcasterId, broadcasterId)
            .AddParam(QueryParams.ModeratorId, this.UserId)
            .AddParam(QueryParams.Status, status.ToString().ToLower())
            .AddParam(QueryParams.UserId, userId)
            .AddParam(QueryParams.First, first);

        return HelixResultFactory.Create<UnbanRequests>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#resolve-unban-requests">API Reference</see>
    ///</summary>
    public Task<HelixResult<UnbanRequests>> ResolveUnbanRequests(
        long broadcasterId,
        string unbanRequestId,
        UnbanRequestStatus status,
        string? resolutionText = null,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.ResolveUnbanRequests;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.BroadcasterId, broadcasterId)
            .AddParam(QueryParams.ModeratorId, this.UserId)
            .AddParam(QueryParams.UnbanRequestId, unbanRequestId)
            .AddParam(QueryParams.Status, status.ToString().ToLower())
            .AddParam(QueryParams.ResolutionText, resolutionText);

        return HelixResultFactory.Create<UnbanRequests>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#warn-chat-user">API Reference</see>
    ///</summary>
    public Task<HelixResult<WarnInfo>> WarnChatUser(
        long broadcasterId,
        long userId,
        string reason,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.WarnChatUser;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.BroadcasterId, broadcasterId)
            .AddParam(QueryParams.ModeratorId, this.UserId);

        request.Body = new
        {
            data = new Warning(userId, reason)
        };

        return HelixResultFactory.Create<WarnInfo>(Client, request, endpoint, cancellationToken);
    }

    ///<summary>
    ///<see href="https://dev.twitch.tv/docs/api/reference/#get-shared-chat-session">API Reference</see>
    ///</summary>
    public Task<HelixResult<SharedChatSession>> GetSharedChatSession(
        long broadcasterId,
        CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.GetSharedChatSession;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.BroadcasterId, broadcasterId);

        return HelixResultFactory.Create<SharedChatSession>(Client, request, endpoint, cancellationToken);
    }

    /// <summary>
    /// <see href="https://dev.twitch.tv/docs/api/reference/#get-hype-train-status">API Reference</see>
    /// <para>The broadcaster_id parameter is omitted because it must match the caller's user id. The <see cref="UserId"/> property is used for the parameter.</para>
    /// </summary>
    public Task<HelixResult<HypeTrainStatus>> GetHypeTrainStatus(CancellationToken cancellationToken = default)
    {
        HelixEndpoint endpoint = Endpoints.GetHypeTrainStatus;
        RequestData request = new RequestData(_baseUrl, endpoint)
            .AddParam(QueryParams.BroadcasterId, this.UserId);

        return HelixResultFactory.Create<HypeTrainStatus>(Client, request, endpoint, cancellationToken);
    }
}
