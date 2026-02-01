using System;
using System.Globalization;

using Lumina.Data;
using Lumina.Excel;
using Lumina.Excel.Sheets;

using LuminaSupplemental.Excel.Model;

namespace LuminaSupplemental.Excel.DataShare.Sheets;

public struct GardeningCrossbreed : ISupplementalRow<Tuple<uint, uint, uint>>, ICsv
{
    private Tuple<uint, uint, uint> data;
    
    public uint ItemResultId => this.data.Item1;
    
    public uint ItemRequirement1Id => this.data.Item2;
    
    public uint ItemRequirement2Id => this.data.Item3;
    
    public int RowId { get; }
    
    public RowRef<Item> ItemResult;
    
    public RowRef<Item> ItemRequirement1;
    
    public RowRef<Item> ItemRequirement2;
    
    public GardeningCrossbreed(Tuple<uint, uint, uint> data, int rowId)
    {
        this.data = data;
        this.RowId = rowId;
    }
    
    public GardeningCrossbreed()
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
        return
        [
            this.ItemResultId.ToString(CultureInfo.InvariantCulture),
            this.ItemRequirement1Id.ToString(CultureInfo.InvariantCulture),
            this.ItemRequirement2Id.ToString(CultureInfo.InvariantCulture)
        ];
    }
    
    public bool IncludeInCsv()
    {
        return true;
    }
    
    public void PopulateData(ExcelModule module, Language language)
    {
        this.ItemResult = new RowRef<Item>(module, this.ItemResultId, language);
        this.ItemRequirement1 = new RowRef<Item>(module, this.ItemRequirement1Id, language);
        this.ItemRequirement2 = new RowRef<Item>(module, this.ItemRequirement2Id, language);
    }
}
