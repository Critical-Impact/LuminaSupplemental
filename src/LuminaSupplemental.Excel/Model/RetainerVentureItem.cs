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
    public class RetainerVentureItem : ICsv
    {
        [Name("ItemId")] public uint ItemId { get; set; }
        [Name("RetainerTaskRandomId")] public uint RetainerTaskRandomId { get; set; }
        [Name("Min")] public uint? Min { get; set; }
        [Name("Max")] public uint? Max { get; set; }
        [Name("Probability")] public decimal? Probability { get; set; }

        public RowRef< Item > Item;

        public RowRef< RetainerTaskRandom > RetainerTaskRandom;

        public RetainerVentureItem(uint itemId, uint retainerTaskRandomId, uint? min = null, uint? max = null, decimal? probability = null )
        {
            ItemId = itemId;
            RetainerTaskRandomId = retainerTaskRandomId;
            Min = min;
            Max = max;
            Probability = probability;
        }

        public RetainerVentureItem()
        {

        }

        public void FromCsv(string[] lineData)
        {
            ItemId = uint.Parse( lineData[ 0 ] );
            RetainerTaskRandomId = uint.Parse( lineData[ 1 ] );

            if (lineData[2] != string.Empty)
            {
                Min = uint.Parse( lineData[ 2 ], CultureInfo.InvariantCulture );
            }

            if (lineData[3] != string.Empty)
            {
                Max = uint.Parse( lineData[ 3 ], CultureInfo.InvariantCulture );
            }

            if (lineData[4] != string.Empty)
            {
                Probability = decimal.Parse( lineData[ 4 ], CultureInfo.InvariantCulture );
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
            RetainerTaskRandom = new RowRef< RetainerTaskRandom >( module, RetainerTaskRandomId);
        }
    }
}
