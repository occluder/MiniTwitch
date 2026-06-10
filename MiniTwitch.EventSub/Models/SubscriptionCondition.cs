using MiniTwitch.EventSub.Internal.Parsing;

namespace MiniTwitch.EventSub.Models;

public struct SubscriptionCondition(ReadOnlyMemory<byte> slice)
{
    internal readonly bool IsInitialized => !slice.IsEmpty;
    public long BroadcasterId => field == default ? field = slice.GetLong("broadcaster_user_id"u8).GetValueOrDefault() : field;
    public long UserId => field == default ? field = slice.GetLong("user_id"u8).GetValueOrDefault() : field;
}
