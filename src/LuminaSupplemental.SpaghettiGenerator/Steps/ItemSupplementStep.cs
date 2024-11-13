using System;
using System.Collections.Generic;
using System.Linq;

using Lumina.Excel;
using Lumina.Excel.Sheets;

using LuminaSupplemental.Excel.Model;
using LuminaSupplemental.SpaghettiGenerator.Generator;
using LuminaSupplemental.SpaghettiGenerator.Steps.Parsers;

using Serilog;

namespace LuminaSupplemental.SpaghettiGenerator.Steps;

public partial class ItemSupplementStep : GeneratorStep
{
    private readonly DataCacher dataCacher;
    private readonly ILogger logger;
    private readonly ExcelSheet<Item> itemSheet;
    private readonly GubalApi gubalApi;
    private readonly Dictionary<string, uint> itemsByName;

    public override Type OutputType => typeof(ItemSupplement);

    public override string FileName => "ItemSupplement.csv";

    public override string Name => "Item Supplement";

    public HashSet<uint> bannedItems { get; set; }

    public ItemSupplementStep(DataCacher dataCacher, ILogger logger, ExcelSheet<Item> itemSheet, GubalApi gubalApi)
    {
        this.dataCacher = dataCacher;
        this.logger = logger;
        this.itemSheet = itemSheet;
        this.gubalApi = gubalApi;
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
        items.AddRange(this.ProcessItemsTSV());
        items.AddRange(this.ProcessItemSets());
        items.AddRange(this.ProcessManualItems());
        items.AddRange(this.ProcessSkybuilderItems());
        items.AddRange(this.ProcessGubalData());
        items = items.DistinctBy(c => (c.ItemId, c.SourceItemId, c.ItemSupplementSource)).ToList();
        for (var index = 0; index < items.Count; index++)
        {
            var item = items[index];
            item.RowId = (uint)(index + 1);
        }

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
                    itemSupplements.Add(new ItemSupplement((uint)itemSupplements.Count + 1, outputItem.Value.RowId, actualItem.Value.RowId, source.Value));
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
