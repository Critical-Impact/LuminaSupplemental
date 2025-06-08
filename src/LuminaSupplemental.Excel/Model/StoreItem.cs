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

        [Name("StoreId")] public uint StoreId { get; set; }

        public RowRef< Item > Item;

        public RowRef< FittingShopItemSet > FittingShopItemSet;

        public string StoreUrl => $"https://store.finalfantasyxiv.com/ffxivstore/en-us/product/{StoreId}";

        public StoreItem(uint itemId, uint fittingShopItemSetId, uint priceCentsUSD, uint storeId )
        {
            ItemId = itemId;
            FittingShopItemSetId = fittingShopItemSetId;
            PriceCentsUSD = priceCentsUSD;
            this.StoreId = storeId;
        }

        public StoreItem()
        {

        }

        public void FromCsv(string[] lineData)
        {
            ItemId = uint.Parse( lineData[ 0 ], CultureInfo.InvariantCulture );
            FittingShopItemSetId = uint.Parse( lineData[ 1 ], CultureInfo.InvariantCulture );
            PriceCentsUSD = uint.Parse( lineData[ 2 ], CultureInfo.InvariantCulture );
            StoreId = uint.Parse( lineData[ 3 ], CultureInfo.InvariantCulture );
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
