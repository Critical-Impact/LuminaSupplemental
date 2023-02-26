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
using Lumina.Excel.GeneratedSheets;

namespace LuminaSupplemental.Excel.Model
{
    public class SubmarineDrop : ICsv
    {
        [Name("RowId")] public uint RowId { get; set; }
        [Name("ItemId")] public uint ItemId { get; set; }
        [Name("SubmarineExplorationId")] public uint SubmarineExplorationId { get; set; }
        
        public LazyRow< SubmarineExploration > SubmarineExploration;
        
        public LazyRow< Item > Item;

        public SubmarineDrop(uint rowId, uint itemId, uint submarineExplorationId )
        {
            RowId = rowId;
            ItemId = itemId;
            SubmarineExplorationId = submarineExplorationId;
        }

        public SubmarineDrop()
        {
            
        }

        public void FromCsv(string[] lineData)
        {
            RowId = uint.Parse( lineData[ 0 ] );
            ItemId = uint.Parse( lineData[ 1 ] );
            SubmarineExplorationId = uint.Parse( lineData[ 2 ] );
        }

        public string[] ToCsv()
        {
            return Array.Empty<string>();
        }

        public bool IncludeInCsv()
        {
            return false;
        }

        public virtual void PopulateData( GameData gameData, Language language )
        {
            SubmarineExploration = new LazyRow< SubmarineExploration >( gameData, SubmarineExplorationId, language );
            Item = new LazyRow< Item >( gameData, ItemId, language );
        }
    }
}