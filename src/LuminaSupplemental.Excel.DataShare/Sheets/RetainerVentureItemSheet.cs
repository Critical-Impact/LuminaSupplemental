using System;

using LuminaSupplemental.Excel.Services;

namespace LuminaSupplemental.Excel.DataShare.Sheets;

public class RetainerVentureItemSheet : SupplementalSheet<RetainerVentureItem, Tuple<uint, uint>>
{
    public override string SheetName => "RetainerVentureItem";

    public override int Version => 1;

    public override string ResourceName => CsvLoader.RetainerVentureItemResourceName;

    public override Tuple<uint, uint> ToBackedData(RetainerVentureItem row)
    {
        return new Tuple<uint, uint>(row.ItemId, row.RetainerTaskRandomId);
    }

    public override RetainerVentureItem FromBackedData(Tuple<uint, uint> backedData, int rowIndex)
    {
        return new RetainerVentureItem(backedData, rowIndex);
    }
}
