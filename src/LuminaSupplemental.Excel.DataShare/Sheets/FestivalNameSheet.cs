using System;

using LuminaSupplemental.Excel.Services;

namespace LuminaSupplemental.Excel.DataShare.Sheets;

public class FestivalNameSheet : SupplementalSheet<FestivalName, Tuple<uint, string>>
{
    public override string SheetName => "FestivalName";
    
    public override int Version => 1;
    
    public override string ResourceName => CsvLoader.FestivalNameResourceName;
    
    public override Tuple<uint, string> ToBackedData(FestivalName row)
    {
        return new Tuple<uint, string>(row.FestivalId, row.Name);
    }
    
    public override FestivalName FromBackedData(Tuple<uint, string> backedData, int rowIndex)
    {
        return new FestivalName(backedData, rowIndex);
    }
}
