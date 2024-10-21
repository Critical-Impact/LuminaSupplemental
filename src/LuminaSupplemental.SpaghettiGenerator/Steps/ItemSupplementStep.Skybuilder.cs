using System;
using System.Collections.Generic;
using System.Linq;

using LuminaSupplemental.Excel.Model;

namespace LuminaSupplemental.SpaghettiGenerator.Steps;

public partial class ItemSupplementStep
{
    private List<ItemSupplement> ProcessSkybuilderItems()
    {
        List<ItemSupplement> itemSupplements = new();
        var items = itemSheet.Where(c => !bannedItems.Contains(c.RowId) && c.Name.ToString().Contains("Approved")).ToList();
        foreach (var item in items)
        {
            var regularVersion = item.Name.ToString().Replace("Approved ", "").ToParseable();
            if (itemsByName.ContainsKey(regularVersion))
            {
                var sourceItem = this.itemSheet.GetRow(itemsByName[regularVersion]);
                itemSupplements.Add(
                    new ItemSupplement()
                    {
                        ItemSupplementSource = ItemSupplementSource.SkybuilderHandIn,
                        SourceItemId = sourceItem.RowId,
                        ItemId = item.RowId,
                        RowId = (uint)itemSupplements.Count + 1
                    });
            }
            else
            {
                Console.WriteLine("Could not find a non-approved version of " + regularVersion + " for matching skybuilder items.");
            }
        }

        return itemSupplements;
    }
}
