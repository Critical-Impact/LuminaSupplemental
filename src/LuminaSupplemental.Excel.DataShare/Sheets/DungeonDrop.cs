using System;

using Lumina.Data;
using Lumina.Excel;
using Lumina.Excel.Sheets;

using LuminaSupplemental.Excel.Model;

namespace LuminaSupplemental.Excel.DataShare.Sheets;

public struct DungeonDrop : ISupplementalRow<Tuple<uint, uint>>, ICsv
{
    private Tuple<uint, uint> data;
    
    public uint ItemId => this.data.Item1;
    
    public uint ContentFinderConditionId => this.data.Item2;
    
    public int RowId { get; }
    
    public RowRef<Item> Item;
    
    public RowRef<ContentFinderCondition> ContentFinderCondition;
    
    public DungeonDrop(Tuple<uint, uint> data, int rowId)
    {
        this.data = data;
        this.RowId = rowId;
    }
    
    public DungeonDrop()
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
        this.Item = new RowRef<Item>(module, this.ItemId);
        this.ContentFinderCondition = new RowRef<ContentFinderCondition>(module, this.ContentFinderConditionId);
    }
}
