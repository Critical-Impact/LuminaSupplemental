using System;

using LuminaSupplemental.Excel.Services;

namespace LuminaSupplemental.Excel.DataShare.Sheets;

public class AirshipUnlockSheet : SupplementalSheet<AirshipUnlock, Tuple<uint, uint, uint, uint>>
{
    public override string SheetName => "AirshipUnlock";
    
    public override int Version => 1;
    
    public override string ResourceName => CsvLoader.AirshipUnlockResourceName;
    
    public override Tuple<uint, uint, uint, uint> ToBackedData(AirshipUnlock row)
    {
        return new Tuple<uint, uint, uint, uint>(
            row.AirshipExplorationPointId,
            row.AirshipExplorationPointUnlockId,
            row.SurveillanceRequired,
            row.RankRequired
        );
    }
    
    public override AirshipUnlock FromBackedData(Tuple<uint, uint, uint, uint> backedData, int rowIndex)
    {
        return new AirshipUnlock(backedData, rowIndex);
    }
}
