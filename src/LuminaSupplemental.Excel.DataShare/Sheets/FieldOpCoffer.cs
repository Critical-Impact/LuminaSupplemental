using System;
using System.Globalization;

using Lumina.Data;
using Lumina.Excel;
using Lumina.Excel.Sheets;

using LuminaSupplemental.Excel.Model;

namespace LuminaSupplemental.Excel.DataShare.Sheets;

public struct FieldOpCoffer : ISupplementalRow<Tuple<uint, byte, byte, uint?, uint?, decimal?>>, ICsv
{
    private Tuple<uint, byte, byte, uint?, uint?, decimal?> data;

    public uint ItemId => this.data.Item1;

    public byte TypeRaw => this.data.Item2;

    public byte CofferTypeRaw => this.data.Item3;

    public FieldOpType Type => (FieldOpType)this.TypeRaw;

    public FieldOpCofferType CofferType => (FieldOpCofferType)this.CofferTypeRaw;

    public uint? Min => this.data.Item4;

    public uint? Max => this.data.Item5;

    public decimal? Probability => this.data.Item6;

    public int RowId { get; }

    public RowRef<Item> Item;

    public FieldOpCoffer(Tuple<uint, byte, byte, uint?, uint?, decimal?> data, int rowId)
    {
        this.data = data;
        this.RowId = rowId;
    }

    public FieldOpCoffer()
    {
    }

    public void FromCsv(string[] lineData)
    {
        byte typeRaw = 0;
        byte cofferTypeRaw = 0;

        if (Enum.TryParse<FieldOpType>(lineData[1], out var fieldOpType))
        {
            typeRaw = (byte)fieldOpType;
        }

        if (Enum.TryParse<FieldOpCofferType>(lineData[2], out var fieldOpCofferType))
        {
            cofferTypeRaw = (byte)fieldOpCofferType;
        }

        this.data = new Tuple<uint, byte, byte, uint?, uint?, decimal?>(
            uint.Parse(lineData[0]),
            typeRaw,
            cofferTypeRaw,
            lineData[3] != string.Empty ? uint.Parse(lineData[3], CultureInfo.InvariantCulture) : null,
            lineData[4] != string.Empty ? uint.Parse(lineData[4], CultureInfo.InvariantCulture) : null,
            lineData[5] != string.Empty ? decimal.Parse(lineData[5], CultureInfo.InvariantCulture) : null
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
        this.Item = new RowRef<Item>(module, this.ItemId);
    }
}
