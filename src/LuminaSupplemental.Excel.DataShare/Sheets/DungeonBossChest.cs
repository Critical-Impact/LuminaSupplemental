using System;

using Lumina.Data;
using Lumina.Excel;
using Lumina.Excel.Sheets;

using LuminaSupplemental.Excel.Model;

namespace LuminaSupplemental.Excel.DataShare.Sheets;

public struct DungeonBossChest : ISupplementalRow<Tuple<uint, uint, uint, uint, uint>>, ICsv
{
    private Tuple<uint, uint, uint, uint, uint> data;
    
    public uint ItemId => this.data.Item1;
    
    public uint ContentFinderConditionId => this.data.Item2;
    
    public uint Quantity => this.data.Item3;
    
    public uint FightNo => this.data.Item4;
    
    public uint CofferNo => this.data.Item5;
    
    public int RowId { get; }
    
    public RowRef<Item> Item;
    
    public RowRef<ContentFinderCondition> ContentFinderCondition;
    
    public DungeonBossChest(Tuple<uint, uint, uint, uint, uint> data, int rowId)
    {
        this.data = data;
        this.RowId = rowId;
    }
    
    public DungeonBossChest()
    {
    }
    
    public void FromCsv(string[] lineData)
    {
        this.data = new Tuple<uint, uint, uint, uint, uint>(
            uint.Parse(lineData[0]),
            uint.Parse(lineData[1]),
            uint.Parse(lineData[2]),
            uint.Parse(lineData[3]),
            uint.Parse(lineData[4])
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
