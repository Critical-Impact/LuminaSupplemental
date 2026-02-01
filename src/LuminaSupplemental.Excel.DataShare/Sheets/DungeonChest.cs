using System;

using Lumina.Data;
using Lumina.Excel;
using Lumina.Excel.Sheets;

using LuminaSupplemental.Excel.Model;

namespace LuminaSupplemental.Excel.DataShare.Sheets;

public struct DungeonChest : ISupplementalRow<Tuple<byte, uint, uint, uint, uint>>, ICsv
{
    private Tuple<byte, uint, uint, uint, uint> data;
    
    public byte ChestNo => this.data.Item1;
    
    public uint ContentFinderConditionId => this.data.Item2;
    
    public uint MapId => this.data.Item3;
    
    public uint TerritoryTypeId => this.data.Item4;
    
    public uint ChestId => this.data.Item5;
    
    public int RowId { get; }
    
    public RowRef<ContentFinderCondition> ContentFinderCondition;
    
    public RowRef<Map> Map;
    
    public RowRef<TerritoryType> TerritoryType;
    
    public DungeonChest(Tuple<byte, uint, uint, uint, uint> data, int rowId)
    {
        this.data = data;
        this.RowId = rowId;
    }
    
    public DungeonChest()
    {
    }
    
    public void FromCsv(string[] lineData)
    {
        this.data = new Tuple<byte, uint, uint, uint, uint>(
            byte.Parse(lineData[0]),
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
        this.ContentFinderCondition = new RowRef<ContentFinderCondition>(module, this.ContentFinderConditionId);
        this.Map = new RowRef<Map>(module, this.MapId);
        this.TerritoryType = new RowRef<TerritoryType>(module, this.TerritoryTypeId);
    }
}
