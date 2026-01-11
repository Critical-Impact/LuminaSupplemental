using System;
using System.Collections.Generic;
using System.Linq;

using Lumina.Excel;
using Lumina.Excel.Sheets;

using LuminaSupplemental.Excel.Model;
using LuminaSupplemental.SpaghettiGenerator.Generator;
using LuminaSupplemental.SpaghettiGenerator.Steps.Parsers;

namespace LuminaSupplemental.SpaghettiGenerator.Steps;

public partial class StoreItemStep : GeneratorStep
{
    private readonly DataCacher dataCacher;
    private readonly AppConfig appConfig;
    private readonly ExcelSheet<Item> itemSheet;
    private readonly ExcelSheet<FittingShopItemSet> fittingShopItemSetSheet;
    private readonly Dictionary<string,uint> itemsByName;
    private readonly Dictionary<string,uint> fittingShopItemSetByName;

    public override Type OutputType => typeof(StoreItem);

    public override string FileName => "StoreItem.csv";

    public override string Name => "Store Item";


    public StoreItemStep(DataCacher dataCacher, AppConfig appConfig, ExcelSheet<Item> itemSheet, ExcelSheet<FittingShopItemSet> fittingShopItemSetSheet)
    {
        this.dataCacher = dataCacher;
        this.appConfig = appConfig;
        this.itemSheet = itemSheet;
        this.fittingShopItemSetSheet = fittingShopItemSetSheet;
        var bannedItems = new HashSet< uint >()
        {
            0,
            24225
        };
        this.itemsByName = this.dataCacher.ByName<Item>(item => item.Name.ToString().ToParseable(), item => !bannedItems.Contains(item.RowId));
        this.fittingShopItemSetByName = this.dataCacher.ByName<FittingShopItemSet>(item => item.Name.ToString().ToParseable(), item => !bannedItems.Contains(item.RowId));
    }

    public override bool ShouldRun()
    {
        return this.appConfig.Parsing.ParseOnlineSources;
    }

    public override List<ICsv> Run(Dictionary<Type, List<ICsv>> stepData)
    {
        List<StoreItem> items = new ();
        items.AddRange(this.Process());

        return [..items.Select(c => c).OrderBy(c => c.ItemId).ThenBy(c => c.FittingShopItemSetId)];
    }

    private List<StoreItem> Process()
    {
        List<StoreItem> storeItems = new();

        foreach( var product in StoreParser.StoreProducts )
        {
            var fittingShopItemName = product.Value.Name;
            var parsedShopItemName = fittingShopItemName.Trim().ToParseable();
            FittingShopItemSet? fittingShopItemSet = null;
            if( this.fittingShopItemSetByName.ContainsKey( parsedShopItemName ) )
            {
                fittingShopItemSet = this.fittingShopItemSetSheet.GetRow(fittingShopItemSetByName[ parsedShopItemName ]);
            }
            else if(product.Value.Items.Count != 1)
            {
                Console.WriteLine("Could not find a fitting shop item set with the name " + fittingShopItemName + ", it has more than 1 item so the assumption is it's a set.");
            }
            foreach( var item in product.Value.Items )
            {
                var itemName = item.Name;
                if (StoreParser.replacements.TryGetValue(itemName, out var replacement))
                {
                    itemName = replacement;
                }
                var parsedItemName = itemName.Trim().ToParseable();
                if(!this.itemsByName.TryGetValue(parsedItemName, out var value) )
                {
                    if (!StoreParser.idReplacements.TryGetValue(itemName, out value))
                    {
                        Console.WriteLine("Could not find an item with the name " + itemName + " while parsing store data.");
                        continue;
                    }
                }
                var outputItem = this.itemSheet.GetRow(value);
                storeItems.Add( new StoreItem()
                {
                    FittingShopItemSetId = fittingShopItemSet?.RowId ?? 0,
                    ItemId = outputItem.RowId,
                    PriceCentsUSD = product.Value.PriceText.Contains(".") ? (uint)(float.Parse(product.Value.PriceText.Replace("$", "").Replace("USD", "")) * 100) : uint.Parse(product.Value.PriceText),
                    StoreId = product.Value.ID
                });
            }
        }

        return storeItems;
    }
}
