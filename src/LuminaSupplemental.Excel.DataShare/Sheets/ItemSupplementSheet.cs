using System;

using LuminaSupplemental.Excel.Services;

namespace LuminaSupplemental.Excel.DataShare.Sheets;

public class ItemSupplementSheet : SupplementalSheet<ItemSupplement, Tuple<uint, uint, byte, uint?, uint?, decimal?>>
{
    public override string SheetName => "ItemSupplement";

    public override int Version => 1;

    public override string ResourceName => CsvLoader.ItemSupplementResourceName;

    public override Tuple<uint, uint, byte, uint?, uint?, decimal?> ToBackedData(ItemSupplement row)
    {
        return new Tuple<uint, uint, byte, uint?, uint?, decimal?>(
            row.ItemId,
            row.SourceItemId,
            row.ItemSupplementSourceRaw,
            row.Min,
            row.Max,
            row.Probability
        );
    }

    public override ItemSupplement FromBackedData(Tuple<uint, uint, byte, uint?, uint?, decimal?> backedData, int rowIndex)
    {
        return new ItemSupplement(backedData, rowIndex);
    }
}
