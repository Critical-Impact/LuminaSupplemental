using System;
using System.Globalization;

using CsvHelper.Configuration.Attributes;

using Lumina.Data;
using Lumina.Excel;
using Lumina.Excel.Sheets;

namespace LuminaSupplemental.Excel.Model;

public class FieldOpCoffer : ICsv
{
    [Name("ItemId")] public uint ItemId { get; set; }
    [Name("Type")] public FieldOpType Type { get; set; }
    [Name("ChestType")] public FieldOpCofferType CofferType { get; set; }
    [Name("Min")] public uint? Min { get; set; }
    [Name("Max")] public uint? Max { get; set; }
    [Name("Probability")] public decimal? Probability { get; set; }

    public RowRef< Item > Item;

    public FieldOpCoffer(uint itemId, FieldOpType type, FieldOpCofferType cofferType, uint? min = null, uint? max = null, decimal? probability = null )
    {
        ItemId = itemId;
        Type = type;
        this.CofferType = cofferType;
        Min = min;
        Max = max;
        Probability = probability;
    }

    public FieldOpCoffer()
    {

    }

    public void FromCsv(string[] lineData)
    {
        ItemId = uint.Parse( lineData[ 0 ] );

        if( Enum.TryParse<FieldOpType>( lineData[ 1 ], out var fieldOpType ) )
        {
            this.Type = fieldOpType;
        }

        if( Enum.TryParse<FieldOpCofferType>( lineData[ 2 ], out var fieldOpCofferType ) )
        {
            this.CofferType = fieldOpCofferType;
        }

        if (lineData[3] != string.Empty)
        {
            Min = uint.Parse( lineData[ 3 ], CultureInfo.InvariantCulture );
        }

        if (lineData[4] != string.Empty)
        {
            Max = uint.Parse( lineData[ 4 ], CultureInfo.InvariantCulture );
        }

        if (lineData[5] != string.Empty)
        {
            Probability = decimal.Parse( lineData[ 5 ], CultureInfo.InvariantCulture );
        }
    }

    public string[] ToCsv()
    {
        return Array.Empty<string>();
    }

    public bool IncludeInCsv()
    {
        return false;
    }

    public virtual void PopulateData( ExcelModule module, Language language )
    {
        Item = new RowRef< Item >( module, ItemId);
    }
}
