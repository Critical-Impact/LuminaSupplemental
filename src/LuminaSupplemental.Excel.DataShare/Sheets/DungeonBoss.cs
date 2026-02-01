using System;

using Lumina.Data;
using Lumina.Excel;
using Lumina.Excel.Sheets;

using LuminaSupplemental.Excel.Model;

namespace LuminaSupplemental.Excel.DataShare.Sheets;

public struct DungeonBoss : ISupplementalRow<Tuple<uint, uint, uint>>, ICsv
{
    private Tuple<uint, uint, uint> data;
    
    public uint BNpcNameId => this.data.Item1;
    
    public uint ContentFinderConditionId => this.data.Item2;
    
    public uint FightNo => this.data.Item3;
    
    public int RowId { get; }
    
    public RowRef<BNpcName> BNpcName;
    
    public RowRef<ContentFinderCondition> ContentFinderCondition;
    
    public DungeonBoss(Tuple<uint, uint, uint> data, int rowId)
    {
        this.data = data;
        this.RowId = rowId;
    }
    
    public DungeonBoss()
    {
    }
    
    public void FromCsv(string[] lineData)
    {
        this.data = new Tuple<uint, uint, uint>(
            uint.Parse(lineData[0]),
            uint.Parse(lineData[1]),
            uint.Parse(lineData[2])
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
        this.BNpcName = new RowRef<BNpcName>(module, this.BNpcNameId);
        this.ContentFinderCondition = new RowRef<ContentFinderCondition>(module, this.ContentFinderConditionId);
    }
}
