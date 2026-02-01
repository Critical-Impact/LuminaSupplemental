using System;
using System.Numerics;

using LuminaSupplemental.Excel.Services;

namespace LuminaSupplemental.Excel.DataShare.Sheets;

public class ENpcPlaceSheet : SupplementalSheet<ENpcPlace, Tuple<uint, uint, uint, uint, Vector2>>
{
    public override string SheetName => "ENpcPlace";
    
    public override int Version => 1;
    
    public override string ResourceName => CsvLoader.ENpcPlaceResourceName;
    
    public override Tuple<uint, uint, uint, uint, Vector2> ToBackedData(ENpcPlace row)
    {
        return new Tuple<uint, uint, uint, uint, Vector2>(
            row.ENpcResidentId,
            row.TerritoryTypeId,
            row.MapId,
            row.PlaceNameId,
            row.Position
        );
    }
    
    public override ENpcPlace FromBackedData(Tuple<uint, uint, uint, uint, Vector2> backedData, int rowIndex)
    {
        return new ENpcPlace(backedData, rowIndex);
    }
}
