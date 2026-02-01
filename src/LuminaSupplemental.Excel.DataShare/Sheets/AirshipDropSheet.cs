using System;

using LuminaSupplemental.Excel.Services;

namespace LuminaSupplemental.Excel.DataShare.Sheets;

public class AirshipDropSheet : SupplementalSheet<AirshipDrop, Tuple<uint, uint>>
{
    public override string SheetName => "AirshipDrop";
    
    public override int Version => 1;
    
    public override string ResourceName => CsvLoader.AirshipDropResourceName;
    
    public override Tuple<uint, uint> ToBackedData(AirshipDrop row)
    {
        return new Tuple<uint, uint>(row.ItemId, row.AirshipExplorationPointId);
    }
    
    public override AirshipDrop FromBackedData(Tuple<uint, uint> backedData, int rowIndex)
    {
        return new AirshipDrop(backedData, rowIndex);
    }
}
