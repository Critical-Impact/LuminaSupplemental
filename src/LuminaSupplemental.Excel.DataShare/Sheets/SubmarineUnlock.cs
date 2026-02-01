using System;

using Lumina.Data;
using Lumina.Excel;
using Lumina.Excel.Sheets;

using LuminaSupplemental.Excel.Model;

namespace LuminaSupplemental.Excel.DataShare.Sheets;

public struct SubmarineUnlock : ISupplementalRow<Tuple<uint, uint, uint>>, ICsv
{
    private Tuple<uint, uint, uint> data;

    public uint SubmarineExplorationId => this.data.Item1;

    public uint SubmarineExplorationUnlockId => this.data.Item2;

    public uint RankRequired => this.data.Item3;

    public int RowId { get; }

    public RowRef<SubmarineExploration> SubmarineExploration;

    public RowRef<SubmarineExploration> SubmarineExplorationUnlock;

    public SubmarineUnlock(Tuple<uint, uint, uint> data, int rowId)
    {
        this.data = data;
        this.RowId = rowId;
    }

    public SubmarineUnlock()
    {
    }

    public void FromCsv(string[] lineData)
    {
        this.data = new Tuple<uint, uint, uint>(
            uint.Parse(lineData[0]),
            uint.Parse(lineData[1]),
            uint.Parse(lineData[2])
        );
    }

    public string[] ToCsv()
    {
        return Array.Empty<string>();
    }

    public bool IncludeInCsv()
    {
        return false;
    }

    public void PopulateData(ExcelModule module, Language language)
    {
        this.SubmarineExploration = new RowRef<SubmarineExploration>(module, this.SubmarineExplorationId);
        this.SubmarineExplorationUnlock = new RowRef<SubmarineExploration>(module, this.SubmarineExplorationUnlockId);
    }
}
