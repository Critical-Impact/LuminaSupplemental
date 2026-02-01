using System;

using LuminaSupplemental.Excel.Services;

namespace LuminaSupplemental.Excel.DataShare.Sheets;

public class FieldOpCofferSheet : SupplementalSheet<FieldOpCoffer, Tuple<uint, byte, byte, uint?, uint?, decimal?>>
{
    public override string SheetName => "FieldOpCoffer";

    public override int Version => 1;

    public override string ResourceName => CsvLoader.FieldOpCofferResourceName;

    public override Tuple<uint, byte, byte, uint?, uint?, decimal?> ToBackedData(FieldOpCoffer row)
    {
        return new Tuple<uint, byte, byte, uint?, uint?, decimal?>(
            row.ItemId,
            row.TypeRaw,
            row.CofferTypeRaw,
            row.Min,
            row.Max,
            row.Probability
        );
    }

    public override FieldOpCoffer FromBackedData(Tuple<uint, byte, byte, uint?, uint?, decimal?> backedData, int rowIndex)
    {
        return new FieldOpCoffer(backedData, rowIndex);
    }
}
