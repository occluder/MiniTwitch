﻿using System.Text.Json.Serialization;
using MiniTwitch.Helix.Enums;

namespace MiniTwitch.Helix.Requests;

public readonly struct Announcement
{
    public required string Message { get; init; }
    [JsonIgnore]
    public AnnouncementColor Color { get; init; }
    [JsonInclude, JsonPropertyName("color")]
    private string Color_ => this.Color.ToString();
}