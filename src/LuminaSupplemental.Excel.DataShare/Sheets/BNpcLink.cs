using System;

using Lumina.Data;
using Lumina.Excel;
using Lumina.Excel.Sheets;

using LuminaSupplemental.Excel.Model;

namespace LuminaSupplemental.Excel.DataShare.Sheets;

public struct BNpcLink : ISupplementalRow<Tuple<uint, uint>>, ICsv
{
    private Tuple<uint, uint> data;
    
    public uint BNpcNameId => this.data.Item1;
    
    public uint BNpcBaseId => this.data.Item2;
    
    public int RowId { get; }
    
    public RowRef<BNpcName> BNpcName;
    
    public RowRef<BNpcBase> BNpcBase;
    
    public BNpcLink(Tuple<uint, uint> data, int rowId)
    {
        this.data = data;
        this.RowId = rowId;
    }
    
    public BNpcLink()
    {
    }
    
    public void FromCsv(string[] lineData)
    {
        this.data = new Tuple<uint, uint>(
            uint.Parse(lineData[0]),
            uint.Parse(lineData[1])
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
        this.BNpcName = new RowRef<BNpcName>(module, this.BNpcNameId);
        this.BNpcBase = new RowRef<BNpcBase>(module, this.BNpcBaseId);
    }
}
