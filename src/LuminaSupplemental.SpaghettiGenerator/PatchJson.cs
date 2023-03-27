using Newtonsoft.Json;

namespace LuminaSupplemental.SpaghettiGenerator;

public class PatchJson
{
    [JsonProperty("type")]
    public string Type { get; set; }    
    
    [JsonProperty("id")]
    public uint Id { get; set; }
    
    [JsonProperty("patch")]
    public decimal Patch { get; set; }
}