using System;
using System.Globalization;
using System.Numerics;
using CsvHelper.Configuration.Attributes;
using Lumina;
using Lumina.Data;
using Lumina.Excel;

using Lumina.Excel.Sheets;

namespace LuminaSupplemental.Excel.Model
{
    public class DungeonChestItem : ICsv
    {
        [Name("RowId")] public uint RowId { get; set; }
        [Name("ItemId")] public uint ItemId { get; set; }
        [Name("ChestId")] public uint ChestId { get; set; }
        [Name("Min")] public uint? Min { get; set; }
        [Name("Max")] public uint? Max { get; set; }
        [Name("Probability")] public decimal? Probability { get; set; }

        public RowRef< Item > Item;

        public DungeonChestItem( uint rowId, uint itemId, uint chestId, uint? min = null, uint? max = null, decimal? probability = null )
        {
            RowId = rowId;
            ItemId = itemId;
            ChestId = chestId;
            Min = min;
            Max = max;
            Probability = probability;
        }

        public DungeonChestItem()
        {

        }

        public void FromCsv( string[] lineData )
        {
            RowId = uint.Parse( lineData[ 0 ] );
            ItemId = uint.Parse( lineData[ 1 ] );
            ChestId = uint.Parse( lineData[ 2 ] );

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
}
