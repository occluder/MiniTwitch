using System.Text.Json.Serialization;

namespace MiniTwitch.PubSub.Payloads;

/// <summary>
/// Payload for the undocumented Clips Leaderboard topic.
/// </summary>
public readonly struct ClipsLeaderboard
{
    /// <summary>
    /// Undocumented
    /// <para>Known values: "clips-leaderboard-update"</para>
    /// </summary>
    public string Type { get; init; }
    /// <summary>
    /// ID of the channel for which the leaderboard is being updated.
    /// </summary>
    [JsonPropertyName("broadcaster_id")]
    public long ChannelId { get; init; }
    /// <summary>
    /// The interval for which the leaderboard is being updated.
    /// </summary>
    public string TimeUnit { get; init; }
    /// <summary>
    /// The end time of the leaderboard interval, in Unix timestamp format (milliseconds since epoch), after which the leaderboard is reset.
    /// </summary>
    public long EndTime { get; init; }
    /// <summary>
    /// Gets the list of new/updated leaderboard entries.
    /// </summary>
    public IReadOnlyList<LeaderboardEntry> NewLeaderboard { get; init; }

    /// <summary>
    /// Represents an entry in a leaderboard, containing information about a curator and their associated clip.
    /// </summary>
    public readonly struct LeaderboardEntry
    {
        /// <summary>
        /// The clip's rank in the leaderboard.
        /// </summary>
        public int Rank { get; init; }
        /// <summary>
        /// ID of the curator who created the clip.
        /// </summary>
        public long CuratorId { get; init; }
        /// <summary>
        /// Gets the login name of the curator.
        /// </summary>
        [JsonPropertyName("curator_login")]
        public string CuratorName { get; init; }
        /// <summary>
        /// Gets the display name of the curator.
        /// </summary>
        [JsonPropertyName("curator_display_name")]
        public string CuratorDisplayName { get; init; }
        /// <summary>
        /// Gets the unique identifier for the clip.
        /// </summary>
        public string ClipId { get; init; }
        /// <summary>
        /// Gets the unique identifier or slug associated with the clip.
        /// </summary>
        public string ClipSlug { get; init; }
        /// <summary>
        /// Gets the unique identifier for the clip asset.
        /// </summary>
        public string ClipAssetId { get; init; }
        /// <summary>
        /// Gets the title of the clip.
        /// </summary>
        public string ClipTitle { get; init; }
        /// <summary>
        /// Gets the URL of the thumbnail image associated with the clip.
        /// </summary>
        public string ClipThumbnailUrl { get; init; }
        /// <summary>
        /// Gets the URL of the clip.
        /// </summary>
        public string ClipUrl { get; init; }
        /// <summary>
        /// Gets the score of the clip.
        /// </summary>
        public int Score { get; init; }
    }
}
