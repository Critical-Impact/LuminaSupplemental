using System;

using LuminaSupplemental.Excel.Services;

namespace LuminaSupplemental.Excel.DataShare.Sheets;

public class DungeonChestItemSheet : SupplementalSheet<DungeonChestItem, Tuple<uint, uint, uint?, uint?, decimal?>>
{
    public override string SheetName => "DungeonChestItem";
    
    public override int Version => 1;
    
    public override string ResourceName => CsvLoader.DungeonChestItemResourceName;
    
    public override Tuple<uint, uint, uint?, uint?, decimal?> ToBackedData(DungeonChestItem row)
    {
        return new Tuple<uint, uint, uint?, uint?, decimal?>(
            row.ItemId,
            row.ChestId,
            row.Min,
            row.Max,
            row.Probability
        );
    }
    
    public override DungeonChestItem FromBackedData(Tuple<uint, uint, uint?, uint?, decimal?> backedData, int rowIndex)
    {
        return new DungeonChestItem(backedData, rowIndex);
    }
}
