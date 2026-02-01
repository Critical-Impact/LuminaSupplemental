using System;
using System.Globalization;

using Lumina.Data;
using Lumina.Excel;
using Lumina.Excel.Sheets;

using LuminaSupplemental.Excel.Model;

namespace LuminaSupplemental.Excel.DataShare.Sheets;

public struct DungeonChestItem : ISupplementalRow<Tuple<uint, uint, uint?, uint?, decimal?>>, ICsv
{
    private Tuple<uint, uint, uint?, uint?, decimal?> data;
    
    public uint ItemId => this.data.Item1;
    
    public uint ChestId => this.data.Item2;
    
    public uint? Min => this.data.Item3;
    
    public uint? Max => this.data.Item4;
    
    public decimal? Probability => this.data.Item5;
    
    public int RowId { get; }
    
    public RowRef<Item> Item;
    
    public DungeonChestItem(Tuple<uint, uint, uint?, uint?, decimal?> data, int rowId)
    {
        this.data = data;
        this.RowId = rowId;
    }
    
    public DungeonChestItem()
    {
    }
    
    public void FromCsv(string[] lineData)
    {
        this.data = new Tuple<uint, uint, uint?, uint?, decimal?>(
            uint.Parse(lineData[0]),
            uint.Parse(lineData[1]),
            lineData[2] != string.Empty ? uint.Parse(lineData[2], CultureInfo.InvariantCulture) : null,
            lineData[3] != string.Empty ? uint.Parse(lineData[3], CultureInfo.InvariantCulture) : null,
            lineData[4] != string.Empty ? decimal.Parse(lineData[4], CultureInfo.InvariantCulture) : null
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
    }
}
