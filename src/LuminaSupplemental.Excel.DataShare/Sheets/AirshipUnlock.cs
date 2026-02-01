using System;

using Lumina.Data;
using Lumina.Excel;
using Lumina.Excel.Sheets;

using LuminaSupplemental.Excel.Model;

namespace LuminaSupplemental.Excel.DataShare.Sheets;

public struct AirshipUnlock : ISupplementalRow<Tuple<uint, uint, uint, uint>>, ICsv
{
    private Tuple<uint, uint, uint, uint> data;
    
    public uint AirshipExplorationPointId => this.data.Item1;
    
    public uint AirshipExplorationPointUnlockId => this.data.Item2;
    
    public uint SurveillanceRequired => this.data.Item3;
    
    public uint RankRequired => this.data.Item4;
    
    public int RowId { get; }
    
    public RowRef<AirshipExplorationPoint> AirshipExplorationPoint;
    
    public RowRef<AirshipExplorationPoint> AirshipExplorationPointUnlock;
    
    public AirshipUnlock(Tuple<uint, uint, uint, uint> data, int rowId)
    {
        this.data = data;
        this.RowId = rowId;
    }
    
    public AirshipUnlock()
    {
    }
    
    public void FromCsv(string[] lineData)
    {
        this.data = new Tuple<uint, uint, uint, uint>(
            uint.Parse(lineData[0]),
            uint.Parse(lineData[1]),
            uint.Parse(lineData[2]),
            uint.Parse(lineData[3])
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
        this.AirshipExplorationPoint = new RowRef<AirshipExplorationPoint>(module, this.AirshipExplorationPointId);
        this.AirshipExplorationPointUnlock = new RowRef<AirshipExplorationPoint>(module, this.AirshipExplorationPointUnlockId);
    }
}
