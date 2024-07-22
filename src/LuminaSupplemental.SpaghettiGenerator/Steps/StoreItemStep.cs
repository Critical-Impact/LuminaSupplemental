using System;
using System.Collections.Generic;
using System.Linq;

using Lumina.Excel.GeneratedSheets;

using LuminaSupplemental.Excel.Model;
using LuminaSupplemental.SpaghettiGenerator.Generator;
using LuminaSupplemental.SpaghettiGenerator.Steps.Parsers;

namespace LuminaSupplemental.SpaghettiGenerator.Steps;

public partial class StoreItemStep : GeneratorStep
{
    private readonly DataCacher dataCacher;
    private readonly AppConfig appConfig;
    private readonly Dictionary<string,Item> itemsByName;
    private readonly Dictionary<string,FittingShopItemSet> fittingShopItemSetByName;

    public override Type OutputType => typeof(StoreItem);

    public override string FileName => "StoreItem.csv";

    public override string Name => "Store Item";
    

    public StoreItemStep(DataCacher dataCacher, AppConfig appConfig)
    {
        this.dataCacher = dataCacher;
        this.appConfig = appConfig;
        var bannedItems = new HashSet< uint >()
        {
            0,
            24225
        };
        this.itemsByName = this.dataCacher.ByName<Item>(item => item.Name.ToString().ToParseable(), item => !bannedItems.Contains(item.RowId));
        this.fittingShopItemSetByName = this.dataCacher.ByName<FittingShopItemSet>(item => item.Unknown6.ToString().ToParseable(), item => !bannedItems.Contains(item.RowId));
    }

    public override bool ShouldRun()
    {
        return this.appConfig.Parsing.ParseOnlineSources;
    }

    public override List<ICsv> Run()
    {
        List<StoreItem> items = new ();
        items.AddRange(this.Process());
        for (var index = 0; index < items.Count; index++)
        {
            var item = items[index];
            item.RowId = (uint)(index + 1);
        }

        return [..items.Select(c => c)];
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
                fittingShopItemSet = fittingShopItemSetByName[ parsedShopItemName ];
            }
            else if(product.Value.Items.Count != 1)
            {
                Console.WriteLine("Could not find a fitting shop item set with the name " + fittingShopItemName + ", it has more than 1 item so the assumption is it's a set.");
            }
            foreach( var item in product.Value.Items )
            {
                var itemName = item.Name;
                var parsedItemName = itemName.Trim().ToParseable();
                if( itemsByName.ContainsKey( parsedItemName ) )
                {
                    var outputItem = itemsByName[ parsedItemName ];
                    storeItems.Add( new StoreItem()
                    {
                        RowId = (uint)(storeItems.Count + 1),
                        FittingShopItemSetId = fittingShopItemSet?.RowId ?? 0,
                        ItemId = outputItem.RowId
                    });
                }
                else
                {
                    Console.WriteLine("Could not find an item with the name " + itemName + " while parsing store data.");
                }
            }
        }
        
        return storeItems;
    }
}
