using System;

using Lumina.Data;
using Lumina.Excel;
using Lumina.Excel.Sheets;

using LuminaSupplemental.Excel.Model;

namespace LuminaSupplemental.Excel.DataShare.Sheets;

public struct FestivalName : ISupplementalRow<Tuple<uint, string>>, ICsv
{
    private Tuple<uint, string> data;
    
    public uint FestivalId => this.data.Item1;
    
    public string Name => this.data.Item2;
    
    public int RowId { get; }
    
    public RowRef<Festival> Festival;
    
    public FestivalName(Tuple<uint, string> data, int rowId)
    {
        this.data = data;
        this.RowId = rowId;
    }
    
    public FestivalName()
    {
    }
    
    public void FromCsv(string[] lineData)
    {
        this.data = new Tuple<uint, string>(
            uint.Parse(lineData[0]),
            lineData[1]
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
        this.Festival = new RowRef<Festival>(module, this.FestivalId);
    }
}
