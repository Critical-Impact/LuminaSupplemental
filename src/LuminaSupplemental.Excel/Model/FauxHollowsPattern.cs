using System;
using System.Collections.Generic;
using System.Globalization;

using CsvHelper.Configuration.Attributes;

using Lumina.Data;
using Lumina.Excel;
using LuminaSupplemental.Excel.Converters;

namespace LuminaSupplemental.Excel.Model;

/// <summary>One known Faux Hollows board layout from the community-maintained pattern set.</summary>
public sealed class FauxHollowsPattern : ICsv
{
    [Name("Identifier")]
    public string Identifier { get; set; } = string.Empty;

    [Name("BlockedTiles"), TypeConverter(typeof(SpaceSeparatedIntListConverter))]
    public IReadOnlyList<int> BlockedTiles { get; set; } = Array.Empty<int>();

    [Name("Present")]
    public int Present { get; set; }

    [Name("Sword")]
    public int Sword { get; set; }

    [Name("Sword3x2")]
    public bool Sword3x2 { get; set; }

    [Name("ConfirmedFoxes"), TypeConverter(typeof(SpaceSeparatedIntListConverter))]
    public IReadOnlyList<int> ConfirmedFoxes { get; set; } = Array.Empty<int>();

    public FauxHollowsPattern()
    {
    }

    public FauxHollowsPattern(string identifier, IReadOnlyList<int> blockedTiles, int present, int sword, bool sword3x2, IReadOnlyList<int> confirmedFoxes)
    {
        Identifier = identifier;
        BlockedTiles = blockedTiles;
        Present = present;
        Sword = sword;
        Sword3x2 = sword3x2;
        ConfirmedFoxes = confirmedFoxes;
    }

    public void FromCsv(string[] lineData)
    {
        Identifier = lineData[0];
        BlockedTiles = SpaceSeparatedIntListConverter.Parse(lineData[1]);
        Present = int.Parse(lineData[2], CultureInfo.InvariantCulture);
        Sword = int.Parse(lineData[3], CultureInfo.InvariantCulture);
        Sword3x2 = bool.Parse(lineData[4]);
        ConfirmedFoxes = SpaceSeparatedIntListConverter.Parse(lineData[5]);
    }

    public string[] ToCsv()
        => [Identifier, SpaceSeparatedIntListConverter.Format(BlockedTiles), Present.ToString(CultureInfo.InvariantCulture), Sword.ToString(CultureInfo.InvariantCulture), Sword3x2.ToString(CultureInfo.InvariantCulture), SpaceSeparatedIntListConverter.Format(ConfirmedFoxes)];

    public bool IncludeInCsv() => true;

    public void PopulateData(ExcelModule module, Language language)
    {
    }
}
