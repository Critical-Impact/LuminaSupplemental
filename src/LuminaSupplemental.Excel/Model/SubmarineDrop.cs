using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using CsvHelper;
using CsvHelper.Configuration.Attributes;

namespace LuminaSupplemental.Excel.Model
{
    public struct SubmarineDrop : ICsv
    {
        [Name("RowId")] public uint RowId { get; set; }
        [Name("ItemId")] public uint ItemId { get; set; }
        [Name("SubmarineExplorationId")] public uint SubmarineExplorationId { get; set; }

        public SubmarineDrop(uint rowId, uint itemId, uint submarineExplorationId )
        {
            RowId = rowId;
            ItemId = itemId;
            SubmarineExplorationId = submarineExplorationId;
        }

        public void FromCsv(string[] lineData)
        {
            RowId = uint.Parse( lineData[ 0 ] );
            ItemId = uint.Parse( lineData[ 1 ] );
            SubmarineExplorationId = uint.Parse( lineData[ 2 ] );
        }
    }
}