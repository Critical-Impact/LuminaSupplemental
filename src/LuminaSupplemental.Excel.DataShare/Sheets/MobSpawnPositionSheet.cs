using System;
using System.Numerics;

using LuminaSupplemental.Excel.Services;

namespace LuminaSupplemental.Excel.DataShare.Sheets;

public class MobSpawnPositionSheet : SupplementalSheet<MobSpawnPosition, Tuple<uint, uint, uint, Vector3, byte>>
{
    public override string SheetName => "MobSpawnPosition";

    public override int Version => 1;

    public override string ResourceName => CsvLoader.MobSpawnResourceName;

    public override Tuple<uint, uint, uint, Vector3, byte> ToBackedData(MobSpawnPosition row)
    {
        return new Tuple<uint, uint, uint, Vector3, byte>(
            row.BNpcBaseId,
            row.BNpcNameId,
            row.TerritoryTypeId,
            row.Position,
            row.Subtype
        );
    }

    public override MobSpawnPosition FromBackedData(Tuple<uint, uint, uint, Vector3, byte> backedData, int rowIndex)
    {
        return new MobSpawnPosition(backedData, rowIndex);
    }
}
