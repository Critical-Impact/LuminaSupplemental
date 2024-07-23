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
    public class FateItem : ICsv
    {
        [Name("RowId")] public uint RowId { get; set; }
        [Name("ItemId")] public uint ItemId { get; set; }
        [Name("FateId")] public uint FateId { get; set; }

        public LazyRow<Fate> Fate;
        
        public LazyRow<Item> Item;
        
        public FateItem(uint rowId, uint itemId, uint fateId )
        {
            RowId = rowId;
            ItemId = itemId;
            FateId = fateId;
        }

        public FateItem()
        {
            
        }

        public void FromCsv(string[] lineData)
        {
            RowId = uint.Parse( lineData[ 0 ] );
            ItemId = uint.Parse( lineData[ 1 ] );
            FateId = uint.Parse( lineData[ 2 ] );
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
            Fate = new LazyRow<Fate>( gameData, FateId, language );
            Item = new LazyRow<Item>( gameData, ItemId, language );
        }
    }
}
