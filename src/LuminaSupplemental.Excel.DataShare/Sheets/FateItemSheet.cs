using System;

using LuminaSupplemental.Excel.Services;

namespace LuminaSupplemental.Excel.DataShare.Sheets;

public class FateItemSheet : SupplementalSheet<FateItem, Tuple<uint, uint>>
{
    public override string SheetName => "FateItem";
    
    public override int Version => 1;
    
    public override string ResourceName => CsvLoader.FateItemResourceName;
    
    public override Tuple<uint, uint> ToBackedData(FateItem row)
    {
        return new Tuple<uint, uint>(row.ItemId, row.FateId);
    }
    
    public override FateItem FromBackedData(Tuple<uint, uint> backedData, int rowIndex)
    {
        return new FateItem(backedData, rowIndex);
    }
}
