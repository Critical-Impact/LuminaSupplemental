using System;

using Lumina.Data;
using Lumina.Excel;
using Lumina.Excel.Sheets;

using LuminaSupplemental.Excel.Model;

namespace LuminaSupplemental.Excel.DataShare.Sheets;

public struct VentureDrop : ISupplementalRow<Tuple<uint, uint>>, ICsv
{
    private Tuple<uint, uint> data;

    public uint ItemId => this.data.Item1;

    public uint RetainerTaskRandomId => this.data.Item2;

    public int RowId { get; }

    public RowRef<Item> Item;

    public RowRef<RetainerTaskRandom> RetainerTaskRandom;

    public VentureDrop(Tuple<uint, uint> data, int rowId)
    {
        this.data = data;
        this.RowId = rowId;
    }

    public VentureDrop()
    {
    }

    public void FromCsv(string[] lineData)
    {
        this.data = new Tuple<uint, uint>(
            uint.Parse(lineData[0]),
            uint.Parse(lineData[1])
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
        this.RetainerTaskRandom = new RowRef<RetainerTaskRandom>(module, this.RetainerTaskRandomId);
        this.Item = new RowRef<Item>(module, this.ItemId);
    }
}
