using System;
using System.Collections.Generic;

using LuminaSupplemental.Excel.Services;

namespace LuminaSupplemental.Excel.DataShare.Sheets;

public class GearsetSheet : SupplementalSheet<Gearset, Tuple<string, string, List<uint>>>
{
    public override string SheetName => "Gearset";
    
    public override int Version => 1;
    
    public override string ResourceName => CsvLoader.GearsetResourceName;
    
    public override Tuple<string, string, List<uint>> ToBackedData(Gearset row)
    {
        return new Tuple<string, string, List<uint>>(
            row.Key,
            row.Name,
            row.ItemIds
        );
    }
    
    public override Gearset FromBackedData(Tuple<string, string, List<uint>> backedData, int rowIndex)
    {
        return new Gearset(backedData, rowIndex);
    }
}
