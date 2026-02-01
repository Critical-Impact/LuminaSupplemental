using System;
using System.Collections.Generic;

using Lumina.Data;
using Lumina.Excel;
using Lumina.Excel.Sheets;

using LuminaSupplemental.Excel.Model;

namespace LuminaSupplemental.Excel.DataShare.Sheets;

public struct Gearset : ISupplementalRow<Tuple<string, string, IReadOnlyList<uint>>>, ICsv
{
    private Tuple<string, string, IReadOnlyList<uint>> data;

    public string Key => this.data.Item1;

    public string Name => this.data.Item2;

    public IReadOnlyList<uint> ItemIds => this.data.Item3;

    public uint ItemId1 => this.ItemIds[0];

    public uint ItemId2 => this.ItemIds[1];

    public uint ItemId3 => this.ItemIds[2];

    public uint ItemId4 => this.ItemIds[3];

    public uint ItemId5 => this.ItemIds[4];

    public uint ItemId6 => this.ItemIds[5];

    public uint ItemId7 => this.ItemIds[6];

    public uint ItemId8 => this.ItemIds[7];

    public uint ItemId9 => this.ItemIds[8];

    public uint ItemId10 => this.ItemIds[9];

    public uint ItemId11 => this.ItemIds[10];

    public uint ItemId12 => this.ItemIds[11];

    public uint ItemId13 => this.ItemIds[12];

    public uint ItemId14 => this.ItemIds[13];

    public int RowId { get; }

    public RowRef<Item> Item1;
    public RowRef<Item> Item2;
    public RowRef<Item> Item3;
    public RowRef<Item> Item4;
    public RowRef<Item> Item5;
    public RowRef<Item> Item6;
    public RowRef<Item> Item7;
    public RowRef<Item> Item8;
    public RowRef<Item> Item9;
    public RowRef<Item> Item10;
    public RowRef<Item> Item11;
    public RowRef<Item> Item12;
    public RowRef<Item> Item13;
    public RowRef<Item> Item14;

    public List<RowRef<Item>> Items =>
    [
        this.Item1, this.Item2, this.Item3, this.Item4, this.Item5, this.Item6, this.Item7, this.Item8, this.Item9, this.Item10, this.Item11, this.Item12,
        this.Item13, this.Item14
    ];

    public Gearset(Tuple<string, string, IReadOnlyList<uint>> data, int rowId)
    {
        this.data = data;
        this.RowId = rowId;
    }

    public Gearset()
    {
    }

    public void FromCsv(string[] lineData)
    {
        this.data = new Tuple<string, string, IReadOnlyList<uint>>(
            lineData[0],
            lineData[1],
            new List<uint>
            {
                uint.Parse(lineData[2]),
                uint.Parse(lineData[3]),
                uint.Parse(lineData[4]),
                uint.Parse(lineData[5]),
                uint.Parse(lineData[6]),
                uint.Parse(lineData[7]),
                uint.Parse(lineData[8]),
                uint.Parse(lineData[9]),
                uint.Parse(lineData[10]),
                uint.Parse(lineData[11]),
                uint.Parse(lineData[12]),
                uint.Parse(lineData[13]),
                uint.Parse(lineData[14]),
                uint.Parse(lineData[15])
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
        this.Item1 = new RowRef<Item>(module, this.ItemId1);
        this.Item2 = new RowRef<Item>(module, this.ItemId2);
        this.Item3 = new RowRef<Item>(module, this.ItemId3);
        this.Item4 = new RowRef<Item>(module, this.ItemId4);
        this.Item5 = new RowRef<Item>(module, this.ItemId5);
        this.Item6 = new RowRef<Item>(module, this.ItemId6);
        this.Item7 = new RowRef<Item>(module, this.ItemId7);
        this.Item8 = new RowRef<Item>(module, this.ItemId8);
        this.Item9 = new RowRef<Item>(module, this.ItemId9);
        this.Item10 = new RowRef<Item>(module, this.ItemId10);
        this.Item11 = new RowRef<Item>(module, this.ItemId11);
        this.Item12 = new RowRef<Item>(module, this.ItemId12);
        this.Item13 = new RowRef<Item>(module, this.ItemId13);
        this.Item14 = new RowRef<Item>(module, this.ItemId14);
    }
}
