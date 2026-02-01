using System;
using System.Globalization;

using Lumina.Data;
using Lumina.Excel;
using Lumina.Excel.Sheets;

using LuminaSupplemental.Excel.Model;

namespace LuminaSupplemental.Excel.DataShare.Sheets;

public struct QuestRequiredItem : ISupplementalRow<Tuple<uint, uint, uint, bool>>, ICsv
{
    private Tuple<uint, uint, uint, bool> data;

    public uint ItemId => this.data.Item1;

    public uint QuestId => this.data.Item2;

    public uint Quantity => this.data.Item3;

    public bool IsHq => this.data.Item4;

    public int RowId { get; }

    public RowRef<Item> Item;

    public RowRef<Quest> Quest;

    public QuestRequiredItem(Tuple<uint, uint, uint, bool> data, int rowId)
    {
        this.data = data;
        this.RowId = rowId;
    }

    public QuestRequiredItem()
    {
    }

    public void FromCsv(string[] lineData)
    {
        this.data = new Tuple<uint, uint, uint, bool>(
            uint.Parse(lineData[0], CultureInfo.InvariantCulture),
            uint.Parse(lineData[1], CultureInfo.InvariantCulture),
            uint.Parse(lineData[2], CultureInfo.InvariantCulture),
            bool.Parse(lineData[3])
        );
    }

    public string[] ToCsv()
    {
        return
        [
            this.ItemId.ToString(),
            this.QuestId.ToString(),
            this.Quantity.ToString(),
            this.IsHq.ToString()
        ];
    }

    public bool IncludeInCsv()
    {
        return true;
    }

    public void PopulateData(ExcelModule module, Language language)
    {
        this.Item = new RowRef<Item>(module, this.ItemId);
        this.Quest = new RowRef<Quest>(module, this.QuestId);
    }
}
