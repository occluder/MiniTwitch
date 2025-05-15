using System.Text.Json.Serialization;
using MiniTwitch.PubSub.Enums;
using MiniTwitch.PubSub.Interfaces;

namespace MiniTwitch.PubSub.Payloads;

/// <inheritdoc/>
public readonly struct VideoPlayback : IStreamUp, IViewerCountUpdate, ICommercialBreak,
    IStreamDown
{
    /// <inheritdoc/>
    [JsonPropertyName("type")]
    public string Type { get; init; }
    /// <inheritdoc/>
    [JsonPropertyName("server_time")]
    public double ServerTime { get; init; }
    /// <inheritdoc/>
    [JsonPropertyName("play_delay")]
    public int PlayDelay { get; init; }
    /// <inheritdoc/>
    [JsonPropertyName("viewers")]
    public int Viewers { get; init; }
    /// <inheritdoc/>
    [JsonPropertyName("length")]
    public int Length { get; init; }
    /// <inheritdoc/>
    [JsonPropertyName("scheduled")]
    public bool Scheduled { get; init; }
    /// <inheritdoc/>
    public int CollaborationViewers { get; init; }
    /// <inheritdoc/>
    public CollaborationStatus CollaborationStatus { get; init; }
}
