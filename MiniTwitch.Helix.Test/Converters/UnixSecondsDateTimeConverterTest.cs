using System.Text.Json;
using MiniTwitch.Helix.Internal.Json;

namespace MiniTwitch.Helix.Test.Converters;

public class UnixSecondsDateTimeConverterTest
{
    static readonly JsonSerializerOptions _options = new()
    {
        Converters = { new UnixSecondsDateTimeConverter() }
    };

    [Fact]
    public void Read_UnixSeconds_ReturnsUtcDateTime()
    {
        DateTime? value = JsonSerializer.Deserialize<DateTime?>("1690931298", _options);

        Assert.NotNull(value);
        Assert.Equal(DateTimeOffset.FromUnixTimeSeconds(1690931298).UtcDateTime, value.Value);
        Assert.Equal(DateTimeKind.Utc, value.Value.Kind);
    }

    [Fact]
    public void Read_Zero_ReturnsNull()
    {
        DateTime? value = JsonSerializer.Deserialize<DateTime?>("0", _options);

        Assert.Null(value);
    }

    [Fact]
    public void Read_NumericString_ReturnsDateTime()
    {
        DateTime? value = JsonSerializer.Deserialize<DateTime?>("\"1690931298\"", _options);

        Assert.NotNull(value);
        Assert.Equal(DateTimeOffset.FromUnixTimeSeconds(1690931298).UtcDateTime, value.Value);
        Assert.Equal(DateTimeKind.Utc, value.Value.Kind);
    }

    [Fact]
    public void Read_Rfc3339String_ReturnsDateTime()
    {
        DateTime? value = JsonSerializer.Deserialize<DateTime?>("\"2020-11-10T20:08:33Z\"", _options);

        Assert.NotNull(value);
        Assert.Equal(new DateTime(2020, 11, 10, 20, 8, 33, DateTimeKind.Utc), value.Value);
        Assert.Equal(DateTimeKind.Utc, value.Value.Kind);
    }

    [Fact]
    public void Read_Rfc3339OffsetString_ReturnsUtcDateTime()
    {
        DateTime? value = JsonSerializer.Deserialize<DateTime?>("\"2020-11-10T22:08:33+02:00\"", _options);

        Assert.NotNull(value);
        Assert.Equal(new DateTime(2020, 11, 10, 20, 8, 33, DateTimeKind.Utc), value.Value);
        Assert.Equal(DateTimeKind.Utc, value.Value.Kind);
    }

    [Fact]
    public void Read_EmptyString_ReturnsNull()
    {
        DateTime? value = JsonSerializer.Deserialize<DateTime?>("\"\"", _options);

        Assert.Null(value);
    }

    [Fact]
    public void Read_Null_ReturnsNull()
    {
        DateTime? value = JsonSerializer.Deserialize<DateTime?>("null", _options);

        Assert.Null(value);
    }

    [Fact]
    public void Read_InvalidValue_Throws()
    {
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<DateTime?>("\"not-a-date\"", _options));
    }
}
