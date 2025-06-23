using System.Text.Json.Serialization;

namespace MiniTwitch.PubSub.Payloads;

public readonly struct ClipsLeaderboard
{
    public string Type { get; init; }
    [JsonPropertyName("broadcaster_id")]
    public long ChannelId { get; init; }
    public string TimeUnit { get; init; }
    public long EndTime { get; init; }
    public IReadOnlyList<LeaderboardEntry> NewLeaderboard { get; init; }

    public readonly struct LeaderboardEntry
    {
        public int Rank { get; init; }
        public long CuratorId { get; init; }
        [JsonPropertyName("curator_login")]
        public string CuratorName { get; init; }
        [JsonPropertyName("curator_display_name")]
        public string CuratorDisplayName { get; init; }
        public string ClipId { get; init; }
        public string ClipSlug { get; init; }
        public string ClipAssetId { get; init; }
        public string ClipTitle { get; init; }
        public string ClipThumbnailUrl { get; init; }
        public string ClipUrl { get; init; }
        public int Score { get; init; }
    }
}
