using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;

namespace LuminaSupplemental.SpaghettiGenerator.Steps.Parsers;

public class GubalApi
{
    private readonly HttpClient httpClient;

    public GubalApi(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    private const int MaxAttempts = 3;

    private string Query(string query)
    {
        for (var attempt = 1; ; attempt++)
        {
            var message = new HttpRequestMessage(HttpMethod.Post, "https://gubal.ffxivteamcraft.com/graphql");
            message.Content = new StringContent(JsonSerializer.Serialize(new
            {
                query,
            }));
            message.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = this.httpClient.Send(message);
            var json     = response.Content.ReadAsStringAsync().Result;
            if (response.IsSuccessStatusCode && json.TrimStart().StartsWith('{'))
            {
                return json;
            }

            if (attempt == MaxAttempts)
            {
                var body = json.Length > 200 ? json[..200] : json;
                throw new HttpRequestException($"Gubal returned an invalid response after {MaxAttempts} attempts ({(int)response.StatusCode} {response.StatusCode}): {body}");
            }

            //Shit but good enough
            Thread.Sleep(TimeSpan.FromSeconds(5 * attempt));
        }
    }

    public List<GubalBNpcLink> GetGubalBNpcLinks()
    {
        var json     = this.Query("query{ bnpc  {bnpcBase   bnpcName  }}");
        var result   = JsonSerializer.Deserialize<GraphqlContainer<GubalBNpcLinkData>>(json)!.data.BNpc;

        return result;
    }

    public List<AllaganReportsDropItem> GetAllaganReportDrops()
    {
        var json     = this.Query("{ allagan_reports( where: {applied: {_eq: true}, source: {_in: [\"DROP\"]}}) { itemId data }}");
        var result   = JsonSerializer.Deserialize<GraphqlContainer<AllaganReportsDropContainer>>(json)!.data.AllaganReports;

        return result;
    }

    public List<AllaganReportsLootItem> GetAllaganReportLoot()
    {
        var json     = this.Query("{ allagan_reports( where: {applied: {_eq: true}, source: {_in: [\"LOOT\"]}}) { itemId data }}");
        var result   = JsonSerializer.Deserialize<GraphqlContainer<AllaganReportsLootContainer>>(json)!.data.AllaganReports;

        return result;
    }

    public List<AllaganReportsLootItem> GetAllaganReportReductionItems()
    {
        var json     = this.Query("{ allagan_reports( where: {applied: {_eq: true}, source: {_in: [\"REDUCTION\"]}}) { itemId data }}");
        var result   = JsonSerializer.Deserialize<GraphqlContainer<AllaganReportsLootContainer>>(json)!.data.AllaganReports;

        return result;
    }

    public List<AllaganReportsLootItem> GetAllaganReportGardeningItems()
    {
        var json     = this.Query("{ allagan_reports( where: {applied: {_eq: true}, source: {_in: [\"GARDENING\"]}}) { itemId data }}");
        var result   = JsonSerializer.Deserialize<GraphqlContainer<AllaganReportsLootContainer>>(json)!.data.AllaganReports;

        return result;
    }

    public List<AllaganReportsLootItem> GetAllaganReportDesynthItems()
    {
        var json     = this.Query("{ allagan_reports( where: {applied: {_eq: true}, source: {_in: [\"DESYNTH\"]}}) { itemId data }}");
        var result   = JsonSerializer.Deserialize<GraphqlContainer<AllaganReportsLootContainer>>(json)!.data.AllaganReports;

        return result;
    }

    public List<AllaganReportsVoyageItem> GetAllaganReportVoyageItems()
    {
        var json     = this.Query("{ allagan_reports( where: {applied: {_eq: true}, source: {_in: [\"VOYAGE\"]}}) { itemId data }}");
        var result   = JsonSerializer.Deserialize<GraphqlContainer<AllaganReportsVoyageContainer>>(json)!.data.AllaganReports;

        return result;
    }
}

