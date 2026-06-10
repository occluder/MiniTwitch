namespace MiniTwitch.EventSub.CodeGen;

internal class JsonSnakeCaseLowerNamingPolicy() : JsonSeparatorNamingPolicy(lowercase: true, separator: '_')
{
    public static JsonSnakeCaseLowerNamingPolicy Instance { get; } = new();
}

