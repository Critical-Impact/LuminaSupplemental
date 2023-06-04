using System.Linq;

namespace LuminaSupplemental.SpaghettiGenerator;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Lumina.Data;
using Lumina.Excel.GeneratedSheets;
using Newtonsoft.Json;

public class MappyParser {

    public static List<MappyEntry> MappyEntries = new();

    public class MappyEntry {
        [JsonProperty("BNpcBaseID")]
        public uint BNpcBaseID;

        [JsonProperty("BNpcNameID")]
        public uint BNpcNameID;

        [JsonProperty("CoordinateX")]
        public double CoordinateX;

        [JsonProperty("CoordinateY")]
        public double CoordinateY;

        [JsonProperty("CoordinateZ")]
        public double CoordinateZ;

        [JsonProperty("MapTerritoryID")]
        public double MapTerritoryID;

        [JsonProperty("MapID")]
        public double MapID;

        [JsonProperty("PosX")]
        public double PosX;

        [JsonProperty("PosY")]
        public double PosY;

        [JsonProperty("PosZ")]
        public double PosZ;

        [JsonProperty("Type")]
        public string Type;
    }


    public static void UpdateMappyData() {
        using var wc = new WebClient();
        var json = wc.DownloadString("https://xivapi.com/mappy/json");
        var mappyEntries = JsonConvert.DeserializeObject<List<MappyEntry>>(json);
        if (mappyEntries == null)
        {
            throw new Exception( "Could not deserialize mappy json into MappyEntry." );
        }

        MappyEntries = mappyEntries;
    }
}
