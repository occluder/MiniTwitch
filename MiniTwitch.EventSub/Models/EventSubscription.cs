using MiniTwitch.EventSub.Internal.Parsing;

namespace MiniTwitch.EventSub.Models;

public struct EventSubscription(ReadOnlyMemory<byte> slice)
{
    public Guid Id => field == default ? field = slice.GetGuid("id"u8).GetValueOrDefault() : field;
    public string Status => field ??= slice.GetString("status"u8, intern: true)!;
    public string Type { get; } = slice.GetString("type"u8, intern: true)!;
    public string Version { get; } = slice.GetString("version"u8, intern: true)!;
    public SubscriptionCondition Condition => field.IsInitialized ? field = new(slice.GetChild("condition"u8)) : field;
    public DateTimeOffset CreatedAt => field == default ? field = slice.GetTime("created_at"u8).GetValueOrDefault() : field;
    public int Cost => field == default ? field = slice.GetInt("cost"u8).GetValueOrDefault() : field;
}

