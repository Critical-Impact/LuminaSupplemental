using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using CsvHelper;
using CsvHelper.Configuration.Attributes;

namespace LuminaSupplemental.Excel.Model
{
    public struct VentureDrop : ICsv
    {
        [Name("RowId")] public uint RowId { get; set; }
        [Name("ItemId")] public uint ItemId { get; set; }
        [Name("RetainerTaskRandomId")] public uint RetainerTaskRandomId { get; set; }

        public VentureDrop(uint rowId, uint itemId, uint retainerTaskRandomId )
        {
            RowId = rowId;
            ItemId = itemId;
            RetainerTaskRandomId = retainerTaskRandomId;
        }

        public void FromCsv(string[] lineData)
        {
            RowId = uint.Parse( lineData[ 0 ] );
            ItemId = uint.Parse( lineData[ 1 ] );
            RetainerTaskRandomId = uint.Parse( lineData[ 2 ] );
        }
    }
}