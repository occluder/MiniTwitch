﻿using System.Text.Json;
using System.Text.Json.Serialization;

namespace MiniTwitch.Helix.Internal.Json;

internal class IntToString : JsonConverter<int>
{
    public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => throw new NotSupportedException("Converter should only be used for writing");

    public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}
