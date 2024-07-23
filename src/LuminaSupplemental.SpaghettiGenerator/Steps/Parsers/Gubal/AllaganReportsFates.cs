using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LuminaSupplemental.SpaghettiGenerator.Steps.Parsers;

[Serializable]
public class AllaganReportsFateContainer
{
    [JsonPropertyName("allagan_reports")]
    public List<AllaganReportsFateItem> AllaganReports { get; set; }
}

[Serializable]
public class AllaganReportsFateItem
{
    [JsonPropertyName("itemId")]
    public uint ItemId { get; set; }
        
    [JsonConverter(typeof(GubalFatesConverter))]
    [JsonPropertyName("data")]
    public AllaganReportsFateData Data { get; set; }
}

public class AllaganReportsFateData
{
    [JsonPropertyName("fateId")]
    public uint FateId { get; set; }
}

public class GubalFatesConverter : JsonConverter<AllaganReportsFateData>
{
    public override AllaganReportsFateData Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // Read the JSON string
        try
        {
            if (reader.TokenType == JsonTokenType.StartObject)
            {
                //The server returns both encoded json and non-encoded json?
                return JsonSerializer.Deserialize<AllaganReportsFateData>(ref reader, options);
            }
            string jsonString = reader.GetString();

            // Deserialize the JSON string into the Data object
            return JsonSerializer.Deserialize<AllaganReportsFateData>(jsonString, options);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new AllaganReportsFateData();
        }

    }

    public override void Write(Utf8JsonWriter writer, AllaganReportsFateData value, JsonSerializerOptions options)
    {
        // Serialize the Data object back into a JSON string
        string jsonString = JsonSerializer.Serialize(value, options);
        writer.WriteStringValue(jsonString);
    }
}
