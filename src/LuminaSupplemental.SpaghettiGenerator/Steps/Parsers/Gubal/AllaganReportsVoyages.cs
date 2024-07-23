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
    public AllaganReportsVoyageData Data { get; set; }
}

public class AllaganReportsVoyageData
{
    [JsonPropertyName("voyageId")]
    public uint VoyageId { get; set; }
    
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
    public override AllaganReportsVoyageData Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // Read the JSON string
        try
        {
            if (reader.TokenType == JsonTokenType.StartObject)
            {
                //The server returns both encoded json and non-encoded json?
                return JsonSerializer.Deserialize<AllaganReportsVoyageData>(ref reader, options);
            }
            string jsonString = reader.GetString();

            // Deserialize the JSON string into the Data object
            return JsonSerializer.Deserialize<AllaganReportsVoyageData>(jsonString, options);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new AllaganReportsVoyageData();
        }

    }

    public override void Write(Utf8JsonWriter writer, AllaganReportsVoyageData value, JsonSerializerOptions options)
    {
        // Serialize the Data object back into a JSON string
        string jsonString = JsonSerializer.Serialize(value, options);
        writer.WriteStringValue(jsonString);
    }
}
