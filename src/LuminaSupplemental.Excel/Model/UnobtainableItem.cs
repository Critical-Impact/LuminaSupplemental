using System;

using CsvHelper.Configuration.Attributes;

using Lumina.Data;
using Lumina.Excel;
using Lumina.Excel.Sheets;

namespace LuminaSupplemental.Excel.Model;

public class UnobtainableItem : ICsv
{
    [Name("ItemId")]
    public uint ItemId { get; set; }

    public RowRef<Item> Item;

    public UnobtainableItem(uint itemId)
    {
        ItemId = itemId;
    }

    public UnobtainableItem()
    {
    }

    public void FromCsv(string[] lineData)
    {
        ItemId = uint.Parse(lineData[0]);
    }

    public string[] ToCsv()
    {
        return Array.Empty<string>();
    }

    public bool IncludeInCsv()
    {
        return false;
    }

    public virtual void PopulateData(ExcelModule module, Language language)
    {
        Item = new RowRef<Item>(module, ItemId);
    }
}
