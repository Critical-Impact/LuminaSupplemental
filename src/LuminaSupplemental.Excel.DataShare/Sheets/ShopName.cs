using System;

using Lumina.Data;
using Lumina.Excel;

using LuminaSupplemental.Excel.Model;

namespace LuminaSupplemental.Excel.DataShare.Sheets;

public struct ShopName : ISupplementalRow<Tuple<uint, string>>, ICsv
{
    private Tuple<uint, string> data;

    public uint ShopId => this.data.Item1;

    public string Name => this.data.Item2;

    public int RowId { get; }

    public ShopName(Tuple<uint, string> data, int rowId)
    {
        this.data = data;
        this.RowId = rowId;
    }

    public ShopName()
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
    }
}
