using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LuminaSupplemental.SpaghettiGenerator.Steps.Parsers;

[Serializable]
public class AllaganReportsVoyageContainer
{
    [JsonPropertyName("allagan_reports")]
    public List<AllaganReportsVoyageItem> AllaganReports { get; set; }
}

[Serializable]
public class AllaganReportsVoyageItem
{
    [JsonPropertyName("itemId")]
    public uint ItemId { get; set; }
        
    [JsonConverter(typeof(GubalVoyagesConverter))]
    [JsonPropertyName("data")]
    public AllaganReportsVoyageData? Data { get; set; }
}

public class AllaganReportsVoyageData
{
    [JsonPropertyName("voyageId")]
    public uint? VoyageId { get; set; }

    [JsonPropertyName("voyageType")]
    public AllaganReportVoyageType VoyageType { get; set; }
}

public enum AllaganReportVoyageType
{
    Airship = 0,
    Submarine = 1
}

public class GubalVoyagesConverter : JsonConverter<AllaganReportsVoyageData>
{
    public override AllaganReportsVoyageData? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        AllaganReportsVoyageData? result;
        if (reader.TokenType == JsonTokenType.StartObject)
        {
            result = JsonSerializer.Deserialize<AllaganReportsVoyageData>(ref reader, options);
        }
        else if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }
        else
        {
            var jsonString = reader.GetString();
            if (jsonString == null) return null;
            result = JsonSerializer.Deserialize<AllaganReportsVoyageData>(jsonString, options);
        }

        return result?.VoyageId == null ? null : result;
    }

    public override void Write(Utf8JsonWriter writer, AllaganReportsVoyageData value, JsonSerializerOptions options)
    {
        // Serialize the Data object back into a JSON string
        string jsonString = JsonSerializer.Serialize(value, options);
        writer.WriteStringValue(jsonString);
    }
}
