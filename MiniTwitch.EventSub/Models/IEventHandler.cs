namespace MiniTwitch.EventSub.Models;

public interface IEventHandler<T> where T : struct
{
    ValueTask HandleAsync(EventSubMessage<T> message);
}
