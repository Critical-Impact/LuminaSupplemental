using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using CsvHelper;
using CsvHelper.Configuration.Attributes;
using CsvHelper.TypeConversion;
using Lumina;
using Lumina.Data;
using Lumina.Excel;

using Lumina.Excel.Sheets;

namespace LuminaSupplemental.Excel.Model
{
    public class ItemSupplement : ICsv
    {
        [Name("ItemId")] public uint ItemId { get; set; }
        [Name("SourceItemId")] public uint SourceItemId { get; set; }
        [Name("ItemSupplementSource"), TypeConverter(typeof( EnumConverter ))] public ItemSupplementSource ItemSupplementSource { get; set; }
        [Name("Min")] public uint? Min { get; set; }
        [Name("Max")] public uint? Max { get; set; }
        [Name("Probability")] public decimal? Probability { get; set; }

        public RowRef< Item > Item;

        public RowRef< Item > SourceItem;

        public ItemSupplement(uint itemId, uint sourceItemId,ItemSupplementSource itemSupplementSource,  uint? min = null, uint? max = null, decimal? probability = null )
        {
            ItemId = itemId;
            SourceItemId = sourceItemId;
            ItemSupplementSource = itemSupplementSource;
            Min = min;
            Max = max;
            Probability = probability;
        }

        public ItemSupplement()
        {

        }

        public void FromCsv(string[] lineData)
        {
            ItemId = uint.Parse( lineData[ 0 ] );
            SourceItemId = uint.Parse( lineData[ 1 ] );
            if( Enum.TryParse<ItemSupplementSource>( lineData[ 2 ], out var itemSupplementSource ) )
            {
                ItemSupplementSource = itemSupplementSource;
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
            SourceItem = new RowRef< Item >( module, SourceItemId);
        }
    }
}
