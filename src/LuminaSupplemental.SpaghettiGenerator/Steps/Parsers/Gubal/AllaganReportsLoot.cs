using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LuminaSupplemental.SpaghettiGenerator.Steps.Parsers;

[Serializable]
public class AllaganReportsLootContainer
{
    [JsonPropertyName("allagan_reports")]
    public List<AllaganReportsLootItem> AllaganReports { get; set; }
}

[Serializable]
public class AllaganReportsLootItem
{
    [JsonPropertyName("itemId")]
    public uint ItemId { get; set; }
        
    [JsonConverter(typeof(GubalLootsConverter))]
    [JsonPropertyName("data")]
    public AllaganReportsLootData Data { get; set; }
}

public class AllaganReportsLootData
{
    [JsonPropertyName("itemId")]
    public uint ItemId { get; set; }
}

public class GubalLootsConverter : JsonConverter<AllaganReportsLootData>
{
    public override AllaganReportsLootData Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // Read the JSON string
        try
        {
            if (reader.TokenType == JsonTokenType.StartObject)
            {
                //The server returns both encoded json and non-encoded json?
                return JsonSerializer.Deserialize<AllaganReportsLootData>(ref reader, options);
            }
            string jsonString = reader.GetString();

            // Deserialize the JSON string into the Data object
            var deserializedData = JsonSerializer.Deserialize<AllaganReportsLootData>(jsonString, options);
            return deserializedData;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new AllaganReportsLootData();
        }

    }

    public override void Write(Utf8JsonWriter writer, AllaganReportsLootData value, JsonSerializerOptions options)
    {
        // Serialize the Data object back into a JSON string
        string jsonString = JsonSerializer.Serialize(value, options);
        writer.WriteStringValue(jsonString);
    }
}
