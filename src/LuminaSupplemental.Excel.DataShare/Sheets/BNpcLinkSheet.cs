using System;

using LuminaSupplemental.Excel.Services;

namespace LuminaSupplemental.Excel.DataShare.Sheets;

public class BNpcLinkSheet : SupplementalSheet<BNpcLink, Tuple<uint, uint>>
{
    public override string SheetName => "BNpcLink";
    
    public override int Version => 1;
    
    public override string ResourceName => CsvLoader.BNpcLinkResourceName;
    
    public override Tuple<uint, uint> ToBackedData(BNpcLink row)
    {
        return new Tuple<uint, uint>(row.BNpcNameId, row.BNpcBaseId);
    }
    
    public override BNpcLink FromBackedData(Tuple<uint, uint> backedData, int rowIndex)
    {
        return new BNpcLink(backedData, rowIndex);
    }
}
