using System;

using LuminaSupplemental.Excel.Services;

namespace LuminaSupplemental.Excel.DataShare.Sheets;

public class DungeonBossSheet : SupplementalSheet<DungeonBoss, Tuple<uint, uint, uint>>
{
    public override string SheetName => "DungeonBoss";
    
    public override int Version => 1;
    
    public override string ResourceName => CsvLoader.DungeonBossResourceName;
    
    public override Tuple<uint, uint, uint> ToBackedData(DungeonBoss row)
    {
        return new Tuple<uint, uint, uint>(
            row.BNpcNameId,
            row.ContentFinderConditionId,
            row.FightNo
        );
    }
    
    public override DungeonBoss FromBackedData(Tuple<uint, uint, uint> backedData, int rowIndex)
    {
        return new DungeonBoss(backedData, rowIndex);
    }
}
