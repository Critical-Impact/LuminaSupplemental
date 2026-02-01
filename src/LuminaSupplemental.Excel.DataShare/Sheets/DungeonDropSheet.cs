using System;

using LuminaSupplemental.Excel.Services;

namespace LuminaSupplemental.Excel.DataShare.Sheets;

public class DungeonDropSheet : SupplementalSheet<DungeonDrop, Tuple<uint, uint>>
{
    public override string SheetName => "DungeonDrop";
    
    public override int Version => 1;
    
    public override string ResourceName => CsvLoader.DungeonDropItemResourceName;
    
    public override Tuple<uint, uint> ToBackedData(DungeonDrop row)
    {
        return new Tuple<uint, uint>(row.ItemId, row.ContentFinderConditionId);
    }
    
    public override DungeonDrop FromBackedData(Tuple<uint, uint> backedData, int rowIndex)
    {
        return new DungeonDrop(backedData, rowIndex);
    }
}
