namespace MiniTwitch.EventSub.Models;

public struct EventSubMessage<T> where T : struct
{
    public EventSubscription Subscription { get; set; }
    public T Event { get; set; }
}

