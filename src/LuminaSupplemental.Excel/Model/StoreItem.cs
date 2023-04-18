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
using Lumina.Excel.GeneratedSheets;

namespace LuminaSupplemental.Excel.Model
{
    public class StoreItem : ICsv
    {
        [Name("RowId")] public uint RowId { get; set; }
        [Name("ItemId")] public uint ItemId { get; set; }
        [Name("FittingShopItemSetId")] public uint FittingShopItemSetId { get; set; }
        
        public LazyRow< Item > Item;
        
        public LazyRow< FittingShopItemSet > FittingShopItemSet;

        public StoreItem(uint rowId, uint itemId, uint fittingShopItemSetId )
        {
            RowId = rowId;
            ItemId = itemId;
            FittingShopItemSetId = fittingShopItemSetId;
        }

        public StoreItem()
        {
            
        }

        public void FromCsv(string[] lineData)
        {
            RowId = uint.Parse( lineData[ 0 ] );
            ItemId = uint.Parse( lineData[ 1 ] );
            FittingShopItemSetId = uint.Parse( lineData[ 2 ] );
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
            Item = new LazyRow< Item >( gameData, ItemId, language );
            FittingShopItemSet = new LazyRow< FittingShopItemSet >( gameData, FittingShopItemSetId, language );
        }
    }
}