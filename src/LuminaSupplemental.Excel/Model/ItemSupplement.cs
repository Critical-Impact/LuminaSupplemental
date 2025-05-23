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

        public RowRef< Item > Item;

        public RowRef< Item > SourceItem;

        public ItemSupplement(uint itemId, uint sourceItemId, ItemSupplementSource itemSupplementSource )
        {
            ItemId = itemId;
            SourceItemId = sourceItemId;
            ItemSupplementSource = itemSupplementSource;
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
