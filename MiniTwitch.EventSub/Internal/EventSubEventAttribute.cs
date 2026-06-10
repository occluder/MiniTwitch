namespace MiniTwitch.EventSub.Internal;

[AttributeUsage(AttributeTargets.Struct)]
internal sealed class EventSubEventAttribute : Attribute
{
    public EventSubEventAttribute(string subscriptionType, string version) { }
}
