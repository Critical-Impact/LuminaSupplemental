using System;

using LuminaSupplemental.Excel.Services;

namespace LuminaSupplemental.Excel.DataShare.Sheets;

public class ENpcShopSheet : SupplementalSheet<ENpcShop, Tuple<uint, uint>>
{
    public override string SheetName => "ENpcShop";
    
    public override int Version => 1;
    
    public override string ResourceName => CsvLoader.ENpcShopResourceName;
    
    public override Tuple<uint, uint> ToBackedData(ENpcShop row)
    {
        return new Tuple<uint, uint>(row.ENpcResidentId, row.ShopId);
    }
    
    public override ENpcShop FromBackedData(Tuple<uint, uint> backedData, int rowIndex)
    {
        return new ENpcShop(backedData, rowIndex);
    }
}
