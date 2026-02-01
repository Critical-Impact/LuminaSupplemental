using System;
using System.Globalization;
using System.Linq;
using System.Numerics;

using Lumina.Data;
using Lumina.Excel;
using Lumina.Excel.Sheets;

using LuminaSupplemental.Excel.Model;

namespace LuminaSupplemental.Excel.DataShare.Sheets;

public struct MobSpawnPosition : ISupplementalRow<Tuple<uint, uint, uint, Vector3, byte>>, ICsv
{
    private Tuple<uint, uint, uint, Vector3, byte> data;

    public uint BNpcBaseId => this.data.Item1;

    public uint BNpcNameId => this.data.Item2;

    public uint TerritoryTypeId => this.data.Item3;

    public Vector3 Position => this.data.Item4;

    public byte Subtype => this.data.Item5;

    public int RowId { get; }

    public RowRef<BNpcBase> BNpcBase;

    public RowRef<BNpcName> BNpcName;

    public RowRef<TerritoryType> TerritoryType;

    public MobSpawnPosition(Tuple<uint, uint, uint, Vector3, byte> data, int rowId)
    {
        this.data = data;
        this.RowId = rowId;
    }

    public MobSpawnPosition()
    {
    }

    public void FromCsv(string[] lineData)
    {
        var positionData = lineData[3].Split(";").Select(c => float.Parse(c, CultureInfo.InvariantCulture)).ToList();
        this.data = new Tuple<uint, uint, uint, Vector3, byte>(
            uint.Parse(lineData[0]),
            uint.Parse(lineData[1]),
            uint.Parse(lineData[2]),
            new Vector3(positionData[0], positionData[1], positionData[2]),
            byte.Parse(lineData[4])
        );
    }

    public string[] ToCsv()
    {
        return
        [
            this.BNpcBaseId.ToString(),
            this.BNpcNameId.ToString(),
            this.TerritoryTypeId.ToString(),
            this.Position.X.ToString(CultureInfo.InvariantCulture) + ";" + this.Position.Y.ToString(CultureInfo.InvariantCulture) + ";" +
            this.Position.Z.ToString(CultureInfo.InvariantCulture),
            this.Subtype.ToString()
        ];
    }

    public bool IncludeInCsv()
    {
        return true;
    }

    public void PopulateData(ExcelModule module, Language language)
    {
        this.BNpcBase = new RowRef<BNpcBase>(module, this.BNpcBaseId);
        this.BNpcName = new RowRef<BNpcName>(module, this.BNpcNameId);
        this.TerritoryType = new RowRef<TerritoryType>(module, this.TerritoryTypeId);
    }
}
