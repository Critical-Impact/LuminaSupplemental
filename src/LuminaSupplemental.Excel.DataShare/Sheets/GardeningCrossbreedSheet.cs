using System;

using LuminaSupplemental.Excel.Services;

namespace LuminaSupplemental.Excel.DataShare.Sheets;

public class GardeningCrossbreedSheet : SupplementalSheet<GardeningCrossbreed, Tuple<uint, uint, uint>>
{
    public override string SheetName => "GardeningCrossbreed";
    
    public override int Version => 1;
    
    public override string ResourceName => CsvLoader.GardeningCrossbreedResourceName;
    
    public override Tuple<uint, uint, uint> ToBackedData(GardeningCrossbreed row)
    {
        return new Tuple<uint, uint, uint>(
            row.ItemResultId,
            row.ItemRequirement1Id,
            row.ItemRequirement2Id
        );
    }
    
    public override GardeningCrossbreed FromBackedData(Tuple<uint, uint, uint> backedData, int rowIndex)
    {
        return new GardeningCrossbreed(backedData, rowIndex);
    }
}
