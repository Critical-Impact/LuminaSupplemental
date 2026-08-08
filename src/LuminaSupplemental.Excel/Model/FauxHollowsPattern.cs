using System;
using System.Collections.Generic;
using System.Globalization;

using CsvHelper.Configuration.Attributes;

using Lumina.Data;
using Lumina.Excel;

namespace LuminaSupplemental.Excel.Model;

/// <summary>One known Faux Hollows board layout from the community-maintained pattern set.</summary>
public sealed class FauxHollowsPattern : ICsv
{
    [Name("Identifier")]
    public string Identifier { get; set; } = string.Empty;

    [Name("BlockedTiles")]
    public string BlockedTilesCsv { get; set; } = string.Empty;

    [Name("Present")]
    public int Present { get; set; }

    [Name("Sword")]
    public int Sword { get; set; }

    [Name("Sword3x2")]
    public bool Sword3x2 { get; set; }

    [Name("ConfirmedFoxes")]
    public string ConfirmedFoxesCsv { get; set; } = string.Empty;

    [Ignore]
    public IReadOnlyList<int> BlockedTiles => ParseTileList(BlockedTilesCsv);

    [Ignore]
    public IReadOnlyList<int> ConfirmedFoxes => ParseTileList(ConfirmedFoxesCsv);

    public FauxHollowsPattern()
    {
    }

    public FauxHollowsPattern(string identifier, string blockedTilesCsv, int present, int sword, bool sword3x2, string confirmedFoxesCsv)
    {
        Identifier = identifier;
        BlockedTilesCsv = blockedTilesCsv;
        Present = present;
        Sword = sword;
        Sword3x2 = sword3x2;
        ConfirmedFoxesCsv = confirmedFoxesCsv;
    }

    public void FromCsv(string[] lineData)
    {
        Identifier = lineData[0];
        BlockedTilesCsv = lineData[1];
        Present = int.Parse(lineData[2], CultureInfo.InvariantCulture);
        Sword = int.Parse(lineData[3], CultureInfo.InvariantCulture);
        Sword3x2 = bool.Parse(lineData[4]);
        ConfirmedFoxesCsv = lineData[5];
    }

    public string[] ToCsv()
        => [Identifier, BlockedTilesCsv, Present.ToString(CultureInfo.InvariantCulture), Sword.ToString(CultureInfo.InvariantCulture), Sword3x2.ToString(CultureInfo.InvariantCulture), ConfirmedFoxesCsv];

    public bool IncludeInCsv() => true;

    public void PopulateData(ExcelModule module, Language language)
    {
    }

    private static IReadOnlyList<int> ParseTileList(string value)
    {
        var values = value.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var result = new int[values.Length];
        for (var i = 0; i < values.Length; i++)
            result[i] = int.Parse(values[i], CultureInfo.InvariantCulture);
        return result;
    }
}
