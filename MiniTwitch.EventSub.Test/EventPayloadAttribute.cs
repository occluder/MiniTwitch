namespace MiniTwitch.EventSub.Test;

[AttributeUsage(AttributeTargets.Field)]
internal class EventPayloadAttribute : Attribute
{
    public readonly Type OutType;
    public EventPayloadAttribute(Type outType) => OutType = outType;
}
