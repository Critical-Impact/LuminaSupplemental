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
    public class VentureDrop : ICsv
    {
        [Name("RowId")] public uint RowId { get; set; }
        [Name("ItemId")] public uint ItemId { get; set; }
        [Name("RetainerTaskRandomId")] public uint RetainerTaskRandomId { get; set; }
        
        public LazyRow< Item > Item;
        
        public LazyRow< RetainerTaskRandom > RetainerTaskRandom;

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
            RetainerTaskRandom = new LazyRow< RetainerTaskRandom >( gameData, RetainerTaskRandomId, language );
            Item = new LazyRow< Item >( gameData, ItemId, language );
        }
    }
}