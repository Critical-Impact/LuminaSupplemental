using System;
using System.Collections.Generic;
using System.Globalization;

using Lumina.Data;
using Lumina.Excel;
using Lumina.Excel.Sheets;

using LuminaSupplemental.Excel.Model;

namespace LuminaSupplemental.Excel.DataShare.Sheets;

public struct SubmarineDrop : ISupplementalRow<Tuple<uint, uint, byte, List<uint>>>, ICsv
{
    private Tuple<uint, uint, byte, List<uint>> data;

    public uint ItemId => this.data.Item1;

    public uint SubmarineExplorationId => this.data.Item2;

    public byte LootTier => this.data.Item3;

    public List<uint> MinMaxValues => this.data.Item4;

    public uint NormalMin => this.MinMaxValues[0];

    public uint NormalMax => this.MinMaxValues[1];

    public uint PoorMin => this.MinMaxValues[2];

    public uint PoorMax => this.MinMaxValues[3];

    public uint OptimalMin => this.MinMaxValues[4];

    public uint OptimalMax => this.MinMaxValues[5];

    public int RowId { get; }

    public RowRef<SubmarineExploration> SubmarineExploration;

    public RowRef<Item> Item;

    public SubmarineDrop(Tuple<uint, uint, byte, List<uint>> data, int rowId)
    {
        this.data = data;
        this.RowId = rowId;
    }

    public SubmarineDrop()
    {
    }

    public void FromCsv(string[] lineData)
    {
        this.data = new Tuple<uint, uint, byte, List<uint>>(
            uint.Parse(lineData[0]),
            uint.Parse(lineData[1]),
            byte.Parse(lineData[2], CultureInfo.InvariantCulture),
            new List<uint>
            {
                uint.Parse(lineData[3], CultureInfo.InvariantCulture),
                uint.Parse(lineData[4], CultureInfo.InvariantCulture),
                uint.Parse(lineData[5], CultureInfo.InvariantCulture),
                uint.Parse(lineData[6], CultureInfo.InvariantCulture),
                uint.Parse(lineData[7], CultureInfo.InvariantCulture),
                uint.Parse(lineData[8], CultureInfo.InvariantCulture)
            }
        );
    }

    public string[] ToCsv()
    {
        return Array.Empty<string>();
    }

    public bool IncludeInCsv()
    {
        return false;
    }

    public void PopulateData(ExcelModule module, Language language)
    {
        this.SubmarineExploration = new RowRef<SubmarineExploration>(module, this.SubmarineExplorationId);
        this.Item = new RowRef<Item>(module, this.ItemId);
    }
}
