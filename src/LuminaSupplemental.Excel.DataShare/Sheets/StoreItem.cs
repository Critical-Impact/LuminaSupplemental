using System;
using System.Globalization;

using Lumina.Data;
using Lumina.Excel;
using Lumina.Excel.Sheets;

using LuminaSupplemental.Excel.Model;

namespace LuminaSupplemental.Excel.DataShare.Sheets;

public struct StoreItem : ISupplementalRow<Tuple<uint, uint, uint, uint>>, ICsv
{
    private Tuple<uint, uint, uint, uint> data;

    public uint ItemId => this.data.Item1;

    public uint FittingShopItemSetId => this.data.Item2;

    public uint PriceCentsUSD => this.data.Item3;

    public uint StoreId => this.data.Item4;

    public int RowId { get; }

    public RowRef<Item> Item;

    public RowRef<FittingShopItemSet> FittingShopItemSet;

    public string StoreUrl => $"https://store.finalfantasyxiv.com/ffxivstore/en-us/product/{this.StoreId}";

    public StoreItem(Tuple<uint, uint, uint, uint> data, int rowId)
    {
        this.data = data;
        this.RowId = rowId;
    }

    public StoreItem()
    {
    }

    public void FromCsv(string[] lineData)
    {
        this.data = new Tuple<uint, uint, uint, uint>(
            uint.Parse(lineData[0], CultureInfo.InvariantCulture),
            uint.Parse(lineData[1], CultureInfo.InvariantCulture),
            uint.Parse(lineData[2], CultureInfo.InvariantCulture),
            uint.Parse(lineData[3], CultureInfo.InvariantCulture)
        );
    }

    public string[] ToCsv()
    {
        return Array.Empty<string>();
    }

    public bool IncludeInCsv()
    {
        return false;
    }

    public void PopulateData(ExcelModule module, Language language)
    {
        this.Item = new RowRef<Item>(module, this.ItemId);
        this.FittingShopItemSet = new RowRef<FittingShopItemSet>(module, this.FittingShopItemSetId);
    }
}
