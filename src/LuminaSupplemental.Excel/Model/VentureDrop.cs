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
    public class VentureDrop : ICsv
    {
        [Name("ItemId")] public uint ItemId { get; set; }
        [Name("RetainerTaskRandomId")] public uint RetainerTaskRandomId { get; set; }

        public RowRef< Item > Item;

        public RowRef< RetainerTaskRandom > RetainerTaskRandom;

        public VentureDrop(uint itemId, uint retainerTaskRandomId )
        {
            ItemId = itemId;
            RetainerTaskRandomId = retainerTaskRandomId;
        }

        public void FromCsv(string[] lineData)
        {
            ItemId = uint.Parse( lineData[ 0 ] );
            RetainerTaskRandomId = uint.Parse( lineData[ 1 ] );
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
            RetainerTaskRandom = new RowRef< RetainerTaskRandom >( module, RetainerTaskRandomId);
            Item = new RowRef< Item >( module, ItemId);
        }
    }
}
