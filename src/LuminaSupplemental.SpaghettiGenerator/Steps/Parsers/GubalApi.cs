using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LuminaSupplemental.SpaghettiGenerator.Steps.Parsers;

public class GubalApi
{
    private readonly HttpClient httpClient;

    public GubalApi(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }
    
    public List<AllaganReportsDropItem> GetAllaganReportDrops()
    {
        var message = new HttpRequestMessage(HttpMethod.Post, "https://gubal.ffxivteamcraft.com/graphql");
        message.Content = new StringContent(JsonSerializer.Serialize(new
        {
            query = "{ allagan_reports( where: {applied: {_eq: true}, source: {_in: [\"DROP\"]}}) { itemId data }}",
        }));
        message.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        var response = this.httpClient.Send(message);
        var json     = response.Content.ReadAsStringAsync().Result;
        var result   = JsonSerializer.Deserialize<GraphqlContainer<AllaganReportsDropContainer>>(json)!.data.AllaganReports;

        return result;
        }
        
        public List<AllaganReportsLootItem> GetAllaganReportLoot()
        {
            var message = new HttpRequestMessage(HttpMethod.Post, "https://gubal.ffxivteamcraft.com/graphql");
            message.Content = new StringContent(JsonSerializer.Serialize(new
            {
                query = "{ allagan_reports( where: {applied: {_eq: true}, source: {_in: [\"LOOT\"]}}) { itemId data }}",
            }));
            message.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = this.httpClient.Send(message);
            var json     = response.Content.ReadAsStringAsync().Result;
            var result   = JsonSerializer.Deserialize<GraphqlContainer<AllaganReportsLootContainer>>(json)!.data.AllaganReports;

            return result;
        }
        
        public List<AllaganReportsLootItem> GetAllaganReportReductionItems()
        {
            var message = new HttpRequestMessage(HttpMethod.Post, "https://gubal.ffxivteamcraft.com/graphql");
            message.Content = new StringContent(JsonSerializer.Serialize(new
            {
                query = "{ allagan_reports( where: {applied: {_eq: true}, source: {_in: [\"REDUCTION\"]}}) { itemId data }}",
            }));
            message.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = this.httpClient.Send(message);
            var json     = response.Content.ReadAsStringAsync().Result;
            var result   = JsonSerializer.Deserialize<GraphqlContainer<AllaganReportsLootContainer>>(json)!.data.AllaganReports;

            return result;
        }
        
        public List<AllaganReportsLootItem> GetAllaganReportGardeningItems()
        {
            var message = new HttpRequestMessage(HttpMethod.Post, "https://gubal.ffxivteamcraft.com/graphql");
            message.Content = new StringContent(JsonSerializer.Serialize(new
            {
                query = "{ allagan_reports( where: {applied: {_eq: true}, source: {_in: [\"GARDENING\"]}}) { itemId data }}",
            }));
            message.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = this.httpClient.Send(message);
            var json     = response.Content.ReadAsStringAsync().Result;
            var result   = JsonSerializer.Deserialize<GraphqlContainer<AllaganReportsLootContainer>>(json)!.data.AllaganReports;

            return result;
        }
        
        public List<AllaganReportsLootItem> GetAllaganReportDesynthItems()
        {
            var message = new HttpRequestMessage(HttpMethod.Post, "https://gubal.ffxivteamcraft.com/graphql");
            message.Content = new StringContent(JsonSerializer.Serialize(new
            {
                query = "{ allagan_reports( where: {applied: {_eq: true}, source: {_in: [\"DESYNTH\"]}}) { itemId data }}",
            }));
            message.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = this.httpClient.Send(message);
            var json     = response.Content.ReadAsStringAsync().Result;
            var result   = JsonSerializer.Deserialize<GraphqlContainer<AllaganReportsLootContainer>>(json)!.data.AllaganReports;

            return result;
        }
}

