using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;

namespace MiniTwitch.EventSub.Internal.Parsing;

public static class JsonMemoryExtensions
{
    static bool TryFindProperty(ReadOnlySpan<byte> json, ReadOnlySpan<byte> propertyName, ref Utf8JsonReader reader)
    {
        reader = new Utf8JsonReader(json);
        while (reader.Read())
        {
            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                continue;
            }

            if (!reader.ValueSpan.SequenceEqual(propertyName))
            {
                reader.Skip();
                continue;
            }

            reader.Read();
            return true;
        }

        return false;
    }

    public static ReadOnlyMemory<byte> GetChild(this ReadOnlyMemory<byte> mem, ReadOnlySpan<byte> property)
    {
        Utf8JsonReader reader = default;
        if (!TryFindProperty(mem.Span, property, ref reader))
        {
            return Array.Empty<byte>();
        }

        if (reader.TokenType == JsonTokenType.Null)
        {
            return Array.Empty<byte>();
        }

        Debug.Assert(reader.TokenType == JsonTokenType.StartObject);
        int outerDepth = reader.CurrentDepth;
        var startIdx = reader.TokenStartIndex;
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject && reader.CurrentDepth == outerDepth)
            {
                return mem[(int)startIdx..(int)(reader.TokenStartIndex + 1)];
            }

        }

        return Array.Empty<byte>();
    }

    public static string? GetString(this ReadOnlyMemory<byte> mem, ReadOnlySpan<byte> property, bool intern = false)
    {
        Utf8JsonReader reader = default;
        if (!TryFindProperty(mem.Span, property, ref reader))
        {
            return null;
        }

        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        Debug.Assert(reader.TokenType == JsonTokenType.String);
        var s = Encoding.UTF8.GetString(reader.ValueSpan);
        return intern ? string.Intern(s) : s;
    }

    public static DateTimeOffset? GetTime(this ReadOnlyMemory<byte> mem, ReadOnlySpan<byte> property)
    {
        Utf8JsonReader reader = default;
        if (!TryFindProperty(mem.Span, property, ref reader))
        {
            return null;
        }

        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        Debug.Assert(reader.TokenType == JsonTokenType.String);
        Span<char> chars = stackalloc char[Encoding.UTF8.GetCharCount(reader.ValueSpan)];
        int written = Encoding.UTF8.GetChars(reader.ValueSpan, chars);
        return DateTimeOffset.Parse(chars[..written]);
    }

    public static Guid? GetGuid(this ReadOnlyMemory<byte> mem, ReadOnlySpan<byte> property)
    {
        Utf8JsonReader reader = default;
        if (!TryFindProperty(mem.Span, property, ref reader))
        {
            return null;
        }

        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        Debug.Assert(reader.TokenType == JsonTokenType.String);
        Span<char> chars = stackalloc char[Encoding.UTF8.GetCharCount(reader.ValueSpan)];
        int written = Encoding.UTF8.GetChars(reader.ValueSpan, chars);
        return Guid.Parse(chars[..written]);
    }

    public static bool? GetBool(this ReadOnlyMemory<byte> mem, ReadOnlySpan<byte> property)
    {
        Utf8JsonReader reader = default;
        if (!TryFindProperty(mem.Span, property, ref reader))
        {
            return null;
        }

        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        Debug.Assert(reader.TokenType is JsonTokenType.True or JsonTokenType.False);
        return reader.TokenType == JsonTokenType.True;
    }

    public static int? GetInt(this ReadOnlyMemory<byte> mem, ReadOnlySpan<byte> property)
    {
        Utf8JsonReader reader = default;
        if (!TryFindProperty(mem.Span, property, ref reader))
        {
            return null;
        }

        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        Debug.Assert(reader.TokenType is JsonTokenType.Number or JsonTokenType.String);
        return ParseInt(reader.ValueSpan);
    }

    public static long? GetLong(this ReadOnlyMemory<byte> mem, ReadOnlySpan<byte> property)
    {
        Utf8JsonReader reader = default;
        if (!TryFindProperty(mem.Span, property, ref reader))
        {
            return null;
        }

        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        Debug.Assert(reader.TokenType is JsonTokenType.Number or JsonTokenType.String);
        return ParseLong(reader.ValueSpan);
    }

    public static string[]? GetStringArray(this ReadOnlyMemory<byte> mem, ReadOnlySpan<byte> property, bool intern = false)
    {
        Utf8JsonReader reader = default;
        if (!TryFindProperty(mem.Span, property, ref reader))
        {
            return null;
        }

        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        Debug.Assert(reader.TokenType == JsonTokenType.StartArray);
        var list = new List<string>();
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndArray)
                break;

            Debug.Assert(reader.TokenType == JsonTokenType.String);
            var s = Encoding.UTF8.GetString(reader.ValueSpan);
            list.Add(intern ? string.Intern(s) : s);
        }
        return [.. list];
    }

    public static int[]? GetIntArray(this ReadOnlyMemory<byte> mem, ReadOnlySpan<byte> property)
    {
        Utf8JsonReader reader = default;
        if (!TryFindProperty(mem.Span, property, ref reader))
        {
            return null;
        }

        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        Debug.Assert(reader.TokenType == JsonTokenType.StartArray);
        var list = new List<int>();
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndArray)
                break;

            Debug.Assert(reader.TokenType is JsonTokenType.Number or JsonTokenType.String);
            list.Add(ParseInt(reader.ValueSpan));
        }
        return [.. list];
    }

    public static long[]? GetLongArray(this ReadOnlyMemory<byte> mem, ReadOnlySpan<byte> property)
    {
        Utf8JsonReader reader = default;
        if (!TryFindProperty(mem.Span, property, ref reader))
        {
            return null;
        }

        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        Debug.Assert(reader.TokenType == JsonTokenType.StartArray);
        var list = new List<long>();
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndArray)
                break;

            Debug.Assert(reader.TokenType is JsonTokenType.Number or JsonTokenType.String);
            list.Add(ParseLong(reader.ValueSpan));
        }
        return [.. list];
    }

    public static ReadOnlyMemory<byte>[]? GetChildrenArray(this ReadOnlyMemory<byte> mem, ReadOnlySpan<byte> property)
    {
        Utf8JsonReader reader = default;
        if (!TryFindProperty(mem.Span, property, ref reader))
        {
            return null;
        }

        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        Debug.Assert(reader.TokenType == JsonTokenType.StartArray);
        var list = new List<ReadOnlyMemory<byte>>();
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndArray)
                break;

            Debug.Assert(reader.TokenType == JsonTokenType.StartObject);
            int outerDepth = reader.CurrentDepth;
            var startIdx = reader.TokenStartIndex;
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject && reader.CurrentDepth == outerDepth)
                {
                    list.Add(mem[(int)startIdx..(int)(reader.TokenStartIndex + 1)]);
                    break;
                }
            }
        }
        return [.. list];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static int ParseInt(ReadOnlySpan<byte> span)
    {
        const byte numBase = (byte)'0';

        int result = 0;
        foreach (byte b in span)
        {
            Debug.Assert(b is >= (byte)'0' and <= (byte)'9');
            result *= 10;
            result += b - numBase;
        }

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static long ParseLong(ReadOnlySpan<byte> span)
    {
        const byte numBase = (byte)'0';

        long result = 0;
        foreach (byte b in span)
        {
            Debug.Assert(b is >= (byte)'0' and <= (byte)'9');
            result *= 10L;
            result += b - numBase;
        }

        return result;
    }
}

