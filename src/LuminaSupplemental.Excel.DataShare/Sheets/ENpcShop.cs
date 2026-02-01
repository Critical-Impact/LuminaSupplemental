using System;

using Lumina.Data;
using Lumina.Excel;
using Lumina.Excel.Sheets;

using LuminaSupplemental.Excel.Model;

namespace LuminaSupplemental.Excel.DataShare.Sheets;

public struct ENpcShop : ISupplementalRow<Tuple<uint, uint>>, ICsv
{
    private Tuple<uint, uint> data;
    
    public uint ENpcResidentId => this.data.Item1;
    
    public uint ShopId => this.data.Item2;
    
    public int RowId { get; }
    
    public RowRef<ENpcResident> ENpcResident;
    
    public ENpcShop(Tuple<uint, uint> data, int rowId)
    {
        this.data = data;
        this.RowId = rowId;
    }
    
    public ENpcShop()
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
        this.ENpcResident = new RowRef<ENpcResident>(module, this.ENpcResidentId);
    }
}
