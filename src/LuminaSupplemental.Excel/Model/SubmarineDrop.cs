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
    public class SubmarineDrop : ICsv
    {
        [Name("ItemId")] public uint ItemId { get; set; }
        [Name("SubmarineExplorationId")] public uint SubmarineExplorationId { get; set; }

        public RowRef< SubmarineExploration > SubmarineExploration;

        public RowRef< Item > Item;

        public SubmarineDrop(uint itemId, uint submarineExplorationId )
        {
            ItemId = itemId;
            SubmarineExplorationId = submarineExplorationId;
        }

        public SubmarineDrop()
        {

        }

        public void FromCsv(string[] lineData)
        {
            ItemId = uint.Parse( lineData[ 0 ] );
            SubmarineExplorationId = uint.Parse( lineData[ 1 ] );
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
            SubmarineExploration = new RowRef< SubmarineExploration >( module, SubmarineExplorationId);
            Item = new RowRef< Item >( module, ItemId);
        }
    }
}
