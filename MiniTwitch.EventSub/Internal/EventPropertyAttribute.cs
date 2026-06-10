namespace MiniTwitch.EventSub.Internal;

[AttributeUsage(AttributeTargets.Struct)]
internal class EventPropertyAttribute : Attribute
{
    public EventPropertyAttribute(bool optional = false, bool array = false) { }
}

