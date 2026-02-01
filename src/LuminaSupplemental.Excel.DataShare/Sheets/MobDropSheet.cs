using System;

using LuminaSupplemental.Excel.Services;

namespace LuminaSupplemental.Excel.DataShare.Sheets;

public class MobDropSheet : SupplementalSheet<MobDrop, Tuple<uint, uint>>
{
    public override string SheetName => "MobDrop";

    public override int Version => 1;

    public override string ResourceName => CsvLoader.MobDropResourceName;

    public override Tuple<uint, uint> ToBackedData(MobDrop row)
    {
        return new Tuple<uint, uint>(row.ItemId, row.BNpcNameId);
    }

    public override MobDrop FromBackedData(Tuple<uint, uint> backedData, int rowIndex)
    {
        return new MobDrop(backedData, rowIndex);
    }
}
