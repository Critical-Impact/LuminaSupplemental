using System;

using LuminaSupplemental.Excel.Services;

namespace LuminaSupplemental.Excel.DataShare.Sheets;

public class VentureDropSheet : SupplementalSheet<VentureDrop, Tuple<uint, uint>>
{
    public override string SheetName => "VentureDrop";

    public override int Version => 1;

    public override string ResourceName => CsvLoader.RetainerVentureItemResourceName;

    public override Tuple<uint, uint> ToBackedData(VentureDrop row)
    {
        return new Tuple<uint, uint>(row.ItemId, row.RetainerTaskRandomId);
    }

    public override VentureDrop FromBackedData(Tuple<uint, uint> backedData, int rowIndex)
    {
        return new VentureDrop(backedData, rowIndex);
    }
}
