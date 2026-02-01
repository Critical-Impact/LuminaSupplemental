using System;

using LuminaSupplemental.Excel.Services;

namespace LuminaSupplemental.Excel.DataShare.Sheets;

public class DungeonBossDropSheet : SupplementalSheet<DungeonBossDrop, Tuple<uint, uint, uint, uint>>
{
    public override string SheetName => "DungeonBossDrop";
    
    public override int Version => 1;
    
    public override string ResourceName => CsvLoader.DungeonBossDropResourceName;
    
    public override Tuple<uint, uint, uint, uint> ToBackedData(DungeonBossDrop row)
    {
        return new Tuple<uint, uint, uint, uint>(
            row.ContentFinderConditionId,
            row.FightNo,
            row.ItemId,
            row.Quantity
        );
    }
    
    public override DungeonBossDrop FromBackedData(Tuple<uint, uint, uint, uint> backedData, int rowIndex)
    {
        return new DungeonBossDrop(backedData, rowIndex);
    }
}
