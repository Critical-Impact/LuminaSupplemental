using System;
using System.ComponentModel;
using System.Globalization;
using System.Numerics;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LuminaSupplemental.SpaghettiGenerator.Converters;


public sealed class Vector3JsonConverter : JsonConverter<Vector3>
{
    public override Vector3 ReadJson(
        JsonReader reader,
        Type objectType,
        Vector3 existingValue,
        bool hasExistingValue,
        JsonSerializer serializer)
    {
        var obj = JObject.Load(reader);

        return new Vector3(
            obj["X"]!.Value<float>(),
            obj["Y"]!.Value<float>(),
            obj["Z"]!.Value<float>()
        );
    }

    public override void WriteJson(
        JsonWriter writer,
        Vector3 value,
        JsonSerializer serializer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("X");
        writer.WriteValue(value.X);
        writer.WritePropertyName("Y");
        writer.WriteValue(value.Y);
        writer.WritePropertyName("Z");
        writer.WriteValue(value.Z);
        writer.WriteEndObject();
    }
}
