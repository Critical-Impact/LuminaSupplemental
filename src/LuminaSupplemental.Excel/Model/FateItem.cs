using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using CsvHelper;
using CsvHelper.Configuration.Attributes;
using Lumina;
using Lumina.Data;
using Lumina.Excel;

using Lumina.Excel.Sheets;

namespace LuminaSupplemental.Excel.Model
{
    public class FateItem : ICsv
    {
        [Name("ItemId")] public uint ItemId { get; set; }
        [Name("FateId")] public uint FateId { get; set; }

        public RowRef<Fate> Fate;

        public RowRef<Item> Item;

        public FateItem(uint itemId, uint fateId )
        {
            ItemId = itemId;
            FateId = fateId;
        }

        public FateItem()
        {

        }

        public void FromCsv(string[] lineData)
        {
            ItemId = uint.Parse( lineData[ 0 ] );
            FateId = uint.Parse( lineData[ 1 ] );
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
            Fate = new RowRef<Fate>( module, FateId);
            Item = new RowRef<Item>( module, ItemId);
        }
    }
}
