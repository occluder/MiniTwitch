using MiniTwitch.EventSub.Internal;

namespace MiniTwitch.EventSub.Models;

[EventProperty]
public partial struct EventSubscription
{
    public partial Guid Id { get; }
    public partial string Status { get; }
    public partial string Type { get; }
    public partial string Version { get; }
    public partial SubscriptionCondition Condition { get; }
    public partial DateTimeOffset CreatedAt { get; }
    public partial int Cost { get; }
}

