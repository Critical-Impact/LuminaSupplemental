using System;

using LuminaSupplemental.Excel.Services;

namespace LuminaSupplemental.Excel.DataShare.Sheets;

public class SubmarineUnlockSheet : SupplementalSheet<SubmarineUnlock, Tuple<uint, uint, uint>>
{
    public override string SheetName => "SubmarineUnlock";

    public override int Version => 1;

    public override string ResourceName => CsvLoader.SubmarineUnlockResourceName;

    public override Tuple<uint, uint, uint> ToBackedData(SubmarineUnlock row)
    {
        return new Tuple<uint, uint, uint>(
            row.SubmarineExplorationId,
            row.SubmarineExplorationUnlockId,
            row.RankRequired
        );
    }

    public override SubmarineUnlock FromBackedData(Tuple<uint, uint, uint> backedData, int rowIndex)
    {
        return new SubmarineUnlock(backedData, rowIndex);
    }
}
