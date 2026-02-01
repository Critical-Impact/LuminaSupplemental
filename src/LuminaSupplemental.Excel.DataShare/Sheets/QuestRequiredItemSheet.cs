using System;

using LuminaSupplemental.Excel.Services;

namespace LuminaSupplemental.Excel.DataShare.Sheets;

public class QuestRequiredItemSheet : SupplementalSheet<QuestRequiredItem, Tuple<uint, uint, uint, bool>>
{
    public override string SheetName => "QuestRequiredItem";

    public override int Version => 1;

    public override string ResourceName => CsvLoader.QuestRequiredItemResourceName;

    public override Tuple<uint, uint, uint, bool> ToBackedData(QuestRequiredItem row)
    {
        return new Tuple<uint, uint, uint, bool>(
            row.ItemId,
            row.QuestId,
            row.Quantity,
            row.IsHq
        );
    }

    public override QuestRequiredItem FromBackedData(Tuple<uint, uint, uint, bool> backedData, int rowIndex)
    {
        return new QuestRequiredItem(backedData, rowIndex);
    }
}
