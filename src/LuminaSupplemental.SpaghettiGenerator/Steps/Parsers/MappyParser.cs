using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;

using Newtonsoft.Json;

namespace LuminaSupplemental.SpaghettiGenerator.Steps.Parsers;

public class MappyParser
{
    private readonly AppConfig appConfig;
    private readonly HttpClient httpClient;

    public MappyParser(AppConfig appConfig, HttpClient httpClient)
    {
        this.appConfig = appConfig;
        this.httpClient = httpClient;
    }

    public class MappyEntry
    {
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


    public List<MappyEntry> RetrieveMappyCache()
    {
        string cacheFileName = "mappy.json.cache";
        string cacheDirectory = this.appConfig.Parsing.OnlineCacheDirectory;
        string cacheFilePath = Path.Combine(cacheDirectory, cacheFileName);

        // Check if the cached file exists
        if (File.Exists(cacheFilePath))
        {
            // Check the age of the cached file
            DateTime lastWriteTime = File.GetLastWriteTime(cacheFilePath);
            DateTime oneMonthAgo = DateTime.Now.AddDays(-5);

            if (lastWriteTime >= oneMonthAgo)
            {
                // If file is less than a month old, parse from the cache
                var json = File.ReadAllText(cacheFilePath);
                var mappyEntries = JsonConvert.DeserializeObject<List<MappyEntry>>(json);
                if (mappyEntries == null)
                {
                    throw new Exception("Could not deserialize cached mappy json into MappyEntry.");
                }

                return mappyEntries;
            }
        }

        // If cache is either older than a month or doesn't exist, download fresh data
        try
        {
            HttpResponseMessage response = httpClient.GetAsync("https://us-central1-ffxivteamcraft.cloudfunctions.net/getMappyData").Result;

            if (response.IsSuccessStatusCode)
            {
                var json = response.Content.ReadAsStringAsync().Result;
                var mappyEntries = JsonConvert.DeserializeObject<List<MappyEntry>>(json);
                if (mappyEntries == null)
                {
                    throw new Exception("Could not deserialize fresh mappy json into MappyEntry.");
                }
                File.WriteAllTextAsync(cacheFilePath, json);

                return mappyEntries;
            }
            else
            {
                throw new Exception($"Failed to download data. Status code: {response.StatusCode}");
            }
        }
        catch (HttpRequestException ex)
        {
            throw new Exception("HTTP request failed.", ex);
        }
    }
}
