using System;
using System.Globalization;

using Lumina.Data;
using Lumina.Excel;
using Lumina.Excel.Sheets;

using LuminaSupplemental.Excel.Model;

namespace LuminaSupplemental.Excel.DataShare.Sheets;

public struct ItemSupplement : ISupplementalRow<Tuple<uint, uint, byte, uint?, uint?, decimal?>>, ICsv
{
    private Tuple<uint, uint, byte, uint?, uint?, decimal?> data;

    public uint ItemId => this.data.Item1;

    public uint SourceItemId => this.data.Item2;

    public byte ItemSupplementSourceRaw => this.data.Item3;

    public ItemSupplementSource ItemSupplementSource => (ItemSupplementSource)this.ItemSupplementSourceRaw;

    public uint? Min => this.data.Item4;

    public uint? Max => this.data.Item5;

    public decimal? Probability => this.data.Item6;

    public int RowId { get; }

    public RowRef<Item> Item;

    public RowRef<Item> SourceItem;

    public ItemSupplement(Tuple<uint, uint, byte, uint?, uint?, decimal?> data, int rowId)
    {
        this.data = data;
        this.RowId = rowId;
    }

    public ItemSupplement()
    {
    }

    public void FromCsv(string[] lineData)
    {
        byte itemSupplementSourceRaw = 0;

        if (Enum.TryParse<ItemSupplementSource>(lineData[2], out var itemSupplementSource))
        {
            itemSupplementSourceRaw = (byte)itemSupplementSource;
        }

        this.data = new Tuple<uint, uint, byte, uint?, uint?, decimal?>(
            uint.Parse(lineData[0]),
            uint.Parse(lineData[1]),
            itemSupplementSourceRaw,
            lineData[3] != string.Empty ? uint.Parse(lineData[3], CultureInfo.InvariantCulture) : null,
            lineData[4] != string.Empty ? uint.Parse(lineData[4], CultureInfo.InvariantCulture) : null,
            lineData[5] != string.Empty ? decimal.Parse(lineData[5], CultureInfo.InvariantCulture) : null
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
        this.Item = new RowRef<Item>(module, this.ItemId);
        this.SourceItem = new RowRef<Item>(module, this.SourceItemId);
    }
}
