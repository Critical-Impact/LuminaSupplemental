using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LuminaSupplemental.SpaghettiGenerator.Steps.Parsers;

[Serializable]

public class GubalBNpcLinkData
{
    [JsonPropertyName("bnpc")]
    public List<GubalBNpcLink> BNpc { get; set; }
}

public class GubalBNpcLink
{
    [JsonPropertyName("bnpcBase")]
    public uint BNpcBase { get; set; }

    [JsonPropertyName("bnpcName")]
    public uint BNpcName { get; set; }
}

[Serializable]
public class AllaganReportsDropContainer
{
    [JsonPropertyName("allagan_reports")]
    public List<AllaganReportsDropItem> AllaganReports { get; set; }
}

[Serializable]
public class AllaganReportsDropItem
{
    [JsonPropertyName("itemId")]
    public uint ItemId { get; set; }

    [JsonConverter(typeof(GubalDropsConverter))]
    [JsonPropertyName("data")]
    public AllaganReportsDropData Data { get; set; }
}

public class AllaganReportsDropData
{
    [JsonPropertyName("monsterId")]
    public uint MonsterId { get; set; }
}

public class GubalDropsConverter : JsonConverter<AllaganReportsDropData>
{
    public override AllaganReportsDropData Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // Read the JSON string
        try
        {
            if (reader.TokenType == JsonTokenType.StartObject)
            {
                //The server returns both encoded json and non-encoded json?
                return JsonSerializer.Deserialize<AllaganReportsDropData>(ref reader, options);
            }
            string jsonString = reader.GetString();

            // Deserialize the JSON string into the Data object
            return JsonSerializer.Deserialize<AllaganReportsDropData>(jsonString, options);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new AllaganReportsDropData();
        }

    }

    public override void Write(Utf8JsonWriter writer, AllaganReportsDropData value, JsonSerializerOptions options)
    {
        // Serialize the Data object back into a JSON string
        string jsonString = JsonSerializer.Serialize(value, options);
        writer.WriteStringValue(jsonString);
    }
}
