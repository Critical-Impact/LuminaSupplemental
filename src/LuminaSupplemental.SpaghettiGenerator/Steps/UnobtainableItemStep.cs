using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Lumina.Excel;
using Lumina.Excel.Sheets;

using LuminaSupplemental.Excel.Model;
using LuminaSupplemental.SpaghettiGenerator.Generator;

namespace LuminaSupplemental.SpaghettiGenerator.Steps;

public class UnobtainableItemStep : GeneratorStep
{
    private readonly DataCacher dataCacher;
    private readonly ExcelSheet<Item> itemSheet;
    private readonly Dictionary<string, uint> itemsByName;

    public override Type OutputType => typeof(UnobtainableItem);

    public override string FileName => "UnobtainableItem.csv";

    public override string Name => "Unobtainable Items";

    public UnobtainableItemStep(DataCacher dataCacher, ExcelSheet<Item> itemSheet)
    {
        this.dataCacher = dataCacher;
        this.itemSheet = itemSheet;
        this.itemsByName = this.dataCacher.ByName<Item>(item => item.Name.ToString().ToParseable());
    }

    public override List<ICsv> Run(Dictionary<Type, List<ICsv>> stepData)
    {
        List<UnobtainableItem> items = new();
        items.AddRange(this.Process());

        return [..items.Select(c => c).OrderBy(c => c.ItemId)];
    }

    private List<UnobtainableItem> Process()
    {
        HashSet<uint> itemIds = new();

        foreach (var item in this.itemSheet)
        {
            if (IsUnobtainable(item))
            {
                itemIds.Add(item.RowId);
            }
        }

        var reader = CSVFile.CSVReader.FromFile(Path.Combine("ManualData", "Obsolete.csv"));

        foreach (var line in reader.Lines())
        {
            var itemName = line[0];
            var parseableItemName = itemName.Trim().ToParseable();
            if (this.itemsByName.TryGetValue(parseableItemName, out var value))
            {
                itemIds.Add(value);
            }
            else
            {
                Console.WriteLine("Could not find item with name " + itemName.Trim() + " for unobtainable items");
            }
        }

        return itemIds.Select(itemId => new UnobtainableItem() { ItemId = itemId }).ToList();
    }

    private static bool IsUnobtainable(Item item)
    {
        //Dated items
        if (item.RowId >= 100 && item.RowId <= 1600)
        {
            return true;
        }

        // Unobtainable category
        if (item.ItemUICategory.RowId == 39)
        {
            return true;
        }

        return false;
    }
}
