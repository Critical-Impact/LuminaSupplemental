using System;

using LuminaSupplemental.Excel.Services;

namespace LuminaSupplemental.Excel.DataShare.Sheets;

public class HouseVendorSheet : SupplementalSheet<HouseVendor, Tuple<uint, uint>>
{
    public override string SheetName => "HouseVendor";
    
    public override int Version => 1;
    
    public override string ResourceName => CsvLoader.HouseVendorResourceName;
    
    public override Tuple<uint, uint> ToBackedData(HouseVendor row)
    {
        return new Tuple<uint, uint>(row.ENpcResidentId, row.ParentId);
    }
    
    public override HouseVendor FromBackedData(Tuple<uint, uint> backedData, int rowIndex)
    {
        return new HouseVendor(backedData, rowIndex);
    }
}
