using System;
using System.Collections.Generic;
using System.Globalization;

using Lumina.Data;
using Lumina.Excel;

using LuminaSupplemental.Excel.Model;

namespace LuminaSupplemental.Excel.DataShare.Sheets;

public struct ItemPatch : ISupplementalRow<Tuple<uint, uint, decimal>>, ICsv
{
    private Tuple<uint, uint, decimal> data;
    
    public uint StartItemId => this.data.Item1;
    
    public uint EndItemId => this.data.Item2;
    
    public decimal PatchNo => this.data.Item3;
    
    public int RowId { get; }
    
    public ItemPatch(Tuple<uint, uint, decimal> data, int rowId)
    {
        this.data = data;
        this.RowId = rowId;
    }
    
    public ItemPatch()
    {
    }
    
    public void FromCsv(string[] lineData)
    {
        this.data = new Tuple<uint, uint, decimal>(
            uint.Parse(lineData[0]),
            uint.Parse(lineData[1]),
            decimal.Parse(lineData[2], CultureInfo.InvariantCulture)
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
    }
    
    public static Dictionary<uint, decimal> ToItemLookup(List<ItemPatch> itemPatches)
    {
        Dictionary<uint, decimal> lookup = new();
        foreach (var itemPatch in itemPatches)
            for (var i = itemPatch.StartItemId; i <= itemPatch.EndItemId; i++)
                lookup.Add(i, itemPatch.PatchNo);
        
        return lookup;
    }
}
