﻿using System.Text.Json.Serialization;
using MiniTwitch.PubSub.Interfaces;

namespace MiniTwitch.PubSub.Models.Payloads;

public readonly struct SubscribeEvents : ISubEvent, ISubGiftEvent, IAnonSubGiftEvent
{
    [JsonPropertyName("channel_name")]
    public string ChannelName { get; init; } = "";
    [JsonPropertyName("channel_id")]
    public long ChannelId { get; init; } = default;
    [JsonPropertyName("time")]
    public DateTime Time { get; init; } = default;
    [JsonPropertyName("sub_plan")]
    public string SubPlan { get; init; } = "";
    [JsonPropertyName("sub_plan_name")]
    public string SubPlanName { get; init; } = "";
    [JsonPropertyName("context")]
    public string Context { get; init; } = "";
    [JsonPropertyName("is_gift")]
    public bool IsGift { get; init; } = default;
    [JsonPropertyName("sub_message")]
    public SubMessage SubMessage { get; init; } = default;
    [JsonPropertyName("recipient_id")]
    public long RecipientId { get; init; } = default;
    [JsonPropertyName("recipient_user_name")]
    public string RecipientUsername { get; init; } = "";
    [JsonPropertyName("recipient_display_name")]
    public string RecipientDisplayName { get; init; } = "";
    [JsonPropertyName("user_name")]
    public string Username { get; init; } = "";
    [JsonPropertyName("display_name")]
    public string DisplayName { get; init; } = "";
    [JsonPropertyName("user_id")]
    public long UserId { get; init; } = 0;
    [JsonPropertyName("months")]
    public int Months { get; init; } = 0;
    [JsonPropertyName("streak_months")]
    public int StreakMonths { get; init; } = 0;
    [JsonPropertyName("multi_month_duration")]
    public int MultiMonthDuration { get; init; } = 1;
    [JsonPropertyName("cumulative_months")]
    public int CumulativeMonths { get; init; } = 0;

    internal SubscribeEvents(object? any) { }
}

public readonly record struct Emote(
    [property: JsonPropertyName("start")] int Start,
    [property: JsonPropertyName("end")] int End,
    [property: JsonPropertyName("id")] int Id
);

public readonly record struct SubMessage(
    [property: JsonPropertyName("message")] string Message = "",
    [property: JsonPropertyName("emotes")] IReadOnlyList<Emote>? Emotes = default
);