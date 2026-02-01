using System;

using LuminaSupplemental.Excel.Services;

namespace LuminaSupplemental.Excel.DataShare.Sheets;

public class DungeonBossChestSheet : SupplementalSheet<DungeonBossChest, Tuple<uint, uint, uint, uint, uint>>
{
    public override string SheetName => "DungeonBossChest";
    
    public override int Version => 1;
    
    public override string ResourceName => CsvLoader.DungeonBossChestResourceName;
    
    public override Tuple<uint, uint, uint, uint, uint> ToBackedData(DungeonBossChest row)
    {
        return new Tuple<uint, uint, uint, uint, uint>(
            row.ItemId,
            row.ContentFinderConditionId,
            row.Quantity,
            row.FightNo,
            row.CofferNo
        );
    }
    
    public override DungeonBossChest FromBackedData(Tuple<uint, uint, uint, uint, uint> backedData, int rowIndex)
    {
        return new DungeonBossChest(backedData, rowIndex);
    }
}
