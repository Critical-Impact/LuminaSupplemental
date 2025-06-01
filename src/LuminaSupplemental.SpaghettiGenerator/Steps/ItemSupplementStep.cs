using System;
using System.Collections.Generic;
using System.Linq;

using Lumina;
using Lumina.Excel;
using Lumina.Excel.Sheets;

using LuminaSupplemental.Excel.Model;
using LuminaSupplemental.SpaghettiGenerator.Generator;
using LuminaSupplemental.SpaghettiGenerator.Steps.Parsers;

using ILogger = Serilog.ILogger;

namespace LuminaSupplemental.SpaghettiGenerator.Steps;

public partial class ItemSupplementStep : GeneratorStep
{
    private readonly DataCacher dataCacher;
    private readonly ILogger logger;
    private readonly ExcelSheet<Item> itemSheet;
    private readonly GubalApi gubalApi;
    private readonly GameData gameData;
    private readonly AppConfig appConfig;
    private readonly Dictionary<string, uint> itemsByName;

    public override Type OutputType => typeof(ItemSupplement);

    public override string FileName => "ItemSupplement.csv";

    public override string Name => "Item Supplement";

    public HashSet<uint> bannedItems { get; set; }

    public ItemSupplementStep(DataCacher dataCacher, ILogger logger, ExcelSheet<Item> itemSheet, GubalApi gubalApi, GameData gameData, AppConfig appConfig)
    {
        this.dataCacher = dataCacher;
        this.logger = logger;
        this.itemSheet = itemSheet;
        this.gubalApi = gubalApi;
        this.gameData = gameData;
        this.appConfig = appConfig;
        bannedItems = new HashSet< uint >()
        {
            0,
            24225
        };
        this.itemsByName = this.dataCacher.ByName<Item>(item => item.Name.ToString().ToParseable(), item => !bannedItems.Contains(item.RowId));
    }


    public override List<ICsv> Run()
    {
        List<ItemSupplement> items = new List<ItemSupplement>();
        items.AddRange(this.ProcessLodestoneItems());
        items.AddRange(this.ProcessItemsTSV());
        items.AddRange(this.ProcessItemSets());
        items.AddRange(this.ProcessManualItems());
        items.AddRange(this.ProcessSkybuilderItems());
        items.AddRange(this.ProcessGubalData());

        items.AddRange(this.ProcessCards());
        items.AddRange(this.ProcessCoffers());
        items.AddRange(this.ProcessLogograms());
        items.AddRange(this.ProcessDesynth());
        items.AddRange(this.ProcessDeepDungeons());
        items.AddRange(this.ProcessLockboxes());

        items.AddRange(this.AutoMatchMissingCoffers(items));
        items = items.DistinctBy(c => (c.ItemId, c.SourceItemId, c.ItemSupplementSource)).OrderBy(c => c.ItemId).ThenBy(c => c.SourceItemId).ToList();

        //If we detect that there is more detailed drop information about an item, remove the generic loot entry
        HashSet<uint> removeLootItemIds = new HashSet<uint>();
        foreach (var item in items.Where(c=> c.ItemSupplementSource == ItemSupplementSource.Loot))
        {
            if (items.Any(c => c.ItemId == item.ItemId && (int)c.ItemSupplementSource >= 6 && (int)c.ItemSupplementSource <= 15))
            {
                removeLootItemIds.Add(item.ItemId);
            }
        }

        items = items.Where(c => (c.ItemSupplementSource == ItemSupplementSource.Loot && !removeLootItemIds.Contains(c.ItemId)) || c.ItemSupplementSource != ItemSupplementSource.Loot).ToList();

        return [..items.Select(c => c)];
    }


    private List<ItemSupplement> GenerateItemSupplement(ItemSupplementSource? source, string outputItemId, List<string> sources)
    {
        if (source == null)
        {
            return new();
        }

        var itemSupplements = new List<ItemSupplement>();
        outputItemId = outputItemId.ToParseable();
        Item? outputItem = this.itemsByName.ContainsKey(outputItemId) ? this.itemSheet.GetRow(this.itemsByName[outputItemId]) : null;
        if (outputItem != null)
        {
            foreach (var sourceItem in sources)
            {
                var sourceName = sourceItem.ToParseable();
                Item? actualItem = this.itemsByName.ContainsKey(sourceName) ? this.itemSheet.GetRow(this.itemsByName[sourceName]) : null;
                if (actualItem != null)
                {
                    itemSupplements.Add(new ItemSupplement(outputItem.Value.RowId, actualItem.Value.RowId, source.Value));
                }
                else
                {
                    logger.Error("Could not find a match for input item: " + outputItemId + " and source " + sourceName);
                }
            }
        }
        else
        {
            logger.Error("Could not find a match for output item: " + outputItemId);
        }

        return itemSupplements;
    }
}
