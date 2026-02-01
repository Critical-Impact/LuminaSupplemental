using System;

using LuminaSupplemental.Excel.Services;

namespace LuminaSupplemental.Excel.DataShare.Sheets;

public class DungeonChestSheet : SupplementalSheet<DungeonChest, Tuple<byte, uint, uint, uint, uint>>
{
    public override string SheetName => "DungeonChest";
    
    public override int Version => 1;
    
    public override string ResourceName => CsvLoader.DungeonChestResourceName;
    
    public override Tuple<byte, uint, uint, uint, uint> ToBackedData(DungeonChest row)
    {
        return new Tuple<byte, uint, uint, uint, uint>(
            row.ChestNo,
            row.ContentFinderConditionId,
            row.MapId,
            row.TerritoryTypeId,
            row.ChestId
        );
    }
    
    public override DungeonChest FromBackedData(Tuple<byte, uint, uint, uint, uint> backedData, int rowIndex)
    {
        return new DungeonChest(backedData, rowIndex);
    }
}
