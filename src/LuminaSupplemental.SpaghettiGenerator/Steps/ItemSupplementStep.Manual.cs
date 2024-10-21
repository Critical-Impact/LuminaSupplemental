using System;
using System.Collections.Generic;
using System.IO;

using CSVFile;

using LuminaSupplemental.Excel.Model;

namespace LuminaSupplemental.SpaghettiGenerator.Steps;

public partial class ItemSupplementStep
{
    public List<ItemSupplement> ProcessManualItems()
    {
        List<ItemSupplement> itemSupplements = new();
        var reader = CSVFile.CSVReader.FromFile(Path.Combine("ManualData", "Items.csv"), CSVSettings.CSV);

        foreach (var line in reader.Lines())
        {
            var sourceItemId = uint.Parse(line[0]);
            var outputItemId = uint.Parse(line[1]);
            var source = Enum.Parse<ItemSupplementSource>(line[2]);
            itemSupplements.Add(new ItemSupplement((uint)(itemSupplements.Count + 1), outputItemId, sourceItemId, source));
        }

        reader = CSVFile.CSVReader.FromFile(Path.Combine("ManualData", "ItemsScraped.csv"), CSVSettings.CSV);

        foreach (var line in reader.Lines())
        {
            var s1 = line[0];
            var s2 = line[1];

            var sourceItemName = s1.ToParseable();
            var rewardItemName = s2.ToParseable();

            if (itemsByName.ContainsKey(sourceItemName) && itemsByName.ContainsKey(rewardItemName))
            {
                var sourceItem = this.itemSheet.GetRow(itemsByName[sourceItemName]);
                var rewardItem = this.itemSheet.GetRow(itemsByName[rewardItemName]);
                itemSupplements.Add(new ItemSupplement((uint)(itemSupplements.Count + 1), rewardItem.RowId, sourceItem.RowId, ItemSupplementSource.Loot));
            }
            else if (itemsByName.ContainsKey(sourceItemName))
            {
                Console.WriteLine("Could not find item with matching name: " + s1);
            }
            else if (itemsByName.ContainsKey(rewardItemName))
            {
                Console.WriteLine("Could not find item with matching name: " + s2);
            }
        }

        return itemSupplements;
    }
}
