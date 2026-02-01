using System;

using LuminaSupplemental.Excel.Services;

namespace LuminaSupplemental.Excel.DataShare.Sheets;

public class ItemPatchSheet : SupplementalSheet<ItemPatch, Tuple<uint, uint, decimal>>
{
    public override string SheetName => "ItemPatch";
    
    public override int Version => 1;
    
    public override string ResourceName => CsvLoader.ItemPatchResourceName;
    
    public override Tuple<uint, uint, decimal> ToBackedData(ItemPatch row)
    {
        return new Tuple<uint, uint, decimal>(
            row.StartItemId,
            row.EndItemId,
            row.PatchNo
        );
    }
    
    public override ItemPatch FromBackedData(Tuple<uint, uint, decimal> backedData, int rowIndex)
    {
        return new ItemPatch(backedData, rowIndex);
    }
}
