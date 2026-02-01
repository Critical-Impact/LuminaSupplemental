using System;

using LuminaSupplemental.Excel.Services;

namespace LuminaSupplemental.Excel.DataShare.Sheets;

public class ShopNameSheet : SupplementalSheet<ShopName, Tuple<uint, string>>
{
    public override string SheetName => "ShopName";

    public override int Version => 1;

    public override string ResourceName => CsvLoader.ShopNameResourceName;

    public override Tuple<uint, string> ToBackedData(ShopName row)
    {
        return new Tuple<uint, string>(row.ShopId, row.Name);
    }

    public override ShopName FromBackedData(Tuple<uint, string> backedData, int rowIndex)
    {
        return new ShopName(backedData, rowIndex);
    }
}
