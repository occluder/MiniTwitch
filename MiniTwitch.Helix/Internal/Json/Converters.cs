using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using MiniTwitch.Helix.Responses;

namespace MiniTwitch.Helix.Internal.Json;

/// <summary>
/// Reads long? from string. Writes long as string. <br/>
/// NOTE: This does not do any validation.
/// </summary>
internal class OptionalLongConverter : JsonConverter<long?>
{
    public override long? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return ReadLong(reader.ValueSpan);
    }

    internal static long? ReadLong(ReadOnlySpan<byte> bytes)
    {
        if (bytes.Length == 0)
        {
            return null;
        }

        Span<char> chars = stackalloc char[bytes.Length];
        int written = Encoding.UTF8.GetChars(bytes, chars);
        return long.TryParse(chars, out var l) ? l : null;
    }

    public override void Write(Utf8JsonWriter writer, long? value, JsonSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStringValue(value.ToString());
    }
}

/// <summary>
/// Reads long from string. Writes long as string. <br/>
/// NOTE: This does not do any validation.
/// </summary>
internal class LongConverter : JsonConverter<long>
{
    public override long Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return ReadLong(reader.ValueSpan);
    }

    internal static long ReadLong(ReadOnlySpan<byte> bytes)
    {
        if (bytes.Length == 0)
        {
            return default;
        }

        Span<char> chars = stackalloc char[bytes.Length];
        int written = Encoding.UTF8.GetChars(bytes, chars);
        return long.TryParse(chars, out var l) ? l : default;
    }

    public override void Write(Utf8JsonWriter writer, long value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value.ToString());
}

/// <summary>
/// Reads int from string. Writes int as string. <br/>
/// NOTE: This does not do any validation.
/// </summary>
internal class IntConverter : JsonConverter<int>
{
    public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return ReadInt(reader.ValueSpan);
    }

    internal static int ReadInt(ReadOnlySpan<byte> bytes)
    {
        if (bytes.Length == 0)
        {
            return default;
        }

        Span<char> chars = stackalloc char[bytes.Length];
        int written = Encoding.UTF8.GetChars(bytes, chars);
        return int.TryParse(chars, out var i) ? i : default;
    }

    public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value.ToString());
}

/// <summary>
/// Deserializes snake_case strings to PascalCase enum members. Serialization goes in reverse
/// </summary>
internal class EnumConverter<TEnum> : JsonConverter<TEnum>
{
    public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            return ReadEnum(reader.GetString());
        }

        throw new JsonException($"Expected string token but got {reader.TokenType}");
    }

    internal static TEnum ReadEnum(string? enumString)
    {
        if (Enum.TryParse(typeof(TEnum), SnakeCase.Instance.ConvertFromCase(enumString), out object? enumMember))
        {
            return (TEnum)enumMember;
        }

        return default!;
    }

    public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
    {
        if (value is null)
            writer.WriteNullValue();
        else
            writer.WriteStringValue(SnakeCase.Instance.ConvertToCase(value.ToString()));
    }
}

/// <summary>
/// Converts TimeSpans to int (seconds) in serialization. Deserialization not supported.
/// </summary>
internal class TimeSpanToSeconds : JsonConverter<TimeSpan>
{
    public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => throw new NotSupportedException("Converter should only be used for writing");

    public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options) =>
        writer.WriteNumberValue(AsSeconds(value));

    internal static int AsSeconds(TimeSpan ts) => (int)ts.TotalSeconds;
}

/// <summary>
/// Converts <see cref="ConduitTransport"/>
/// </summary>
public class ConduitTransportConverter : JsonConverter<ConduitTransport>
{
    public override void Write(Utf8JsonWriter writer, ConduitTransport value, JsonSerializerOptions options)
        => throw new NotImplementedException("This converter is read-only");

    public override ConduitTransport? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var root = JsonDocument.ParseValue(ref reader).RootElement;
        return ReadTransport(root, options);
    }

    internal static ConduitTransport? ReadTransport(JsonElement root, JsonSerializerOptions options)
    {
        if (root.TryGetProperty("method", out var method))
        {
            return method.GetString() switch
            {
                "webhook" => root.Deserialize<ConduitTransport.Webhook>(options),
                "websocket" => root.Deserialize<ConduitTransport.WebSocket>(options),
                _ => throw new JsonException($"Unknown method: {method.GetString()}")
            };
        }

        return null;
    }
}

/// <summary>
/// Reads DateTime? from string. Writes DateTime as string. <br/>
/// NOTE: This does not do any validation.
/// </summary>
internal class OptionalDateTimeConverter : JsonConverter<DateTime?>
{
    public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TryGetDateTime(out var dateTime))
        {
            return dateTime;
        }

        if (reader.TokenType == JsonTokenType.Null || reader.ValueSpan.Length == 0)
        {
            return null;
        }

        var str = reader.GetString();
        if (string.IsNullOrEmpty(str))
        {
            return null;
        }

        throw new JsonException($"Cannot convert value {str} to DateTime?");
    }

    public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStringValue($"{value:O}");
    }
}