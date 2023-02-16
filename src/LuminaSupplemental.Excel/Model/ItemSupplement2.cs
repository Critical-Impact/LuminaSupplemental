using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using CsvHelper;
using CsvHelper.Configuration.Attributes;
using CsvHelper.TypeConversion;

namespace LuminaSupplemental.Excel.Model
{
    public struct ItemSupplement : ICsv
    {
        [Name("RowId")] public uint RowId { get; set; }
        [Name("ItemId")] public uint ItemId { get; set; }
        [Name("SourceItemId")] public uint SourceItemId { get; set; }
        [Name("ItemSupplementSource"), TypeConverter(typeof( EnumConverter ))] public ItemSupplementSource ItemSupplementSource { get; set; }

        public ItemSupplement(uint rowId, uint itemId, uint sourceItemId, ItemSupplementSource itemSupplementSource )
        {
            RowId = rowId;
            ItemId = itemId;
            SourceItemId = sourceItemId;
            ItemSupplementSource = itemSupplementSource;
        }

        public void FromCsv(string[] lineData)
        {
            RowId = uint.Parse( lineData[ 0 ] );
            ItemId = uint.Parse( lineData[ 1 ] );
            SourceItemId = uint.Parse( lineData[ 2 ] );
            if( Enum.TryParse<ItemSupplementSource>( lineData[ 3 ], out var itemSupplementSource ) )
            {
                ItemSupplementSource = itemSupplementSource;
            }
        }
    }
}