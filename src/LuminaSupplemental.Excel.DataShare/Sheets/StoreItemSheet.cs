using System;

using LuminaSupplemental.Excel.Services;

namespace LuminaSupplemental.Excel.DataShare.Sheets;

public class StoreItemSheet : SupplementalSheet<StoreItem, Tuple<uint, uint, uint, uint>>
{
    public override string SheetName => "StoreItem";

    public override int Version => 1;

    public override string ResourceName => CsvLoader.StoreItemResourceName;

    public override Tuple<uint, uint, uint, uint> ToBackedData(StoreItem row)
    {
        return new Tuple<uint, uint, uint, uint>(
            row.ItemId,
            row.FittingShopItemSetId,
            row.PriceCentsUSD,
            row.StoreId
        );
    }

    public override StoreItem FromBackedData(Tuple<uint, uint, uint, uint> backedData, int rowIndex)
    {
        return new StoreItem(backedData, rowIndex);
    }
}
