using System;
using System.Collections.Generic;

using LuminaSupplemental.Excel.Services;

namespace LuminaSupplemental.Excel.DataShare.Sheets;

public class SubmarineDropSheet : SupplementalSheet<SubmarineDrop, Tuple<uint, uint, byte, IReadOnlyList<uint>>>
{
    public override string SheetName => "SubmarineDrop";

    public override int Version => 1;

    public override string ResourceName => CsvLoader.SubmarineDropResourceName;

    public override Tuple<uint, uint, byte, IReadOnlyList<uint>> ToBackedData(SubmarineDrop row)
    {
        return new Tuple<uint, uint, byte, IReadOnlyList<uint>>(
            row.ItemId,
            row.SubmarineExplorationId,
            row.LootTier,
            row.MinMaxValues
        );
    }

    public override SubmarineDrop FromBackedData(Tuple<uint, uint, byte, IReadOnlyList<uint>> backedData, int rowIndex)
    {
        return new SubmarineDrop(backedData, rowIndex);
    }
}
