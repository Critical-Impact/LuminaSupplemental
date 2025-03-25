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
    public class StoreItem : ICsv
    {
        [Name("ItemId")] public uint ItemId { get; set; }
        [Name("FittingShopItemSetId")] public uint FittingShopItemSetId { get; set; }

        [Name("PriceCentsUSD")] public uint PriceCentsUSD { get; set; }

        public RowRef< Item > Item;

        public RowRef< FittingShopItemSet > FittingShopItemSet;

        public StoreItem(uint itemId, uint fittingShopItemSetId, uint priceCentsUSD )
        {
            ItemId = itemId;
            FittingShopItemSetId = fittingShopItemSetId;
            PriceCentsUSD = priceCentsUSD;
        }

        public StoreItem()
        {

        }

        public void FromCsv(string[] lineData)
        {
            ItemId = uint.Parse( lineData[ 0 ] );
            FittingShopItemSetId = uint.Parse( lineData[ 1 ] );
            PriceCentsUSD = uint.Parse( lineData[ 2 ] );
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
            FittingShopItemSet = new RowRef< FittingShopItemSet >( module, FittingShopItemSetId);
        }
    }
}
