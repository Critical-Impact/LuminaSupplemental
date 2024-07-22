using System.Collections.Generic;

using LuminaSupplemental.Excel.Model;

namespace LuminaSupplemental.SpaghettiGenerator.Steps;

public partial class ItemSupplementStep
{
    private List<ItemSupplement> ProcessGubalData()
    {
        var drops = new List<ItemSupplement>();
        var gubalData = this.gubalApi.GetAllaganReportLoot();
        foreach (var gubalItem in gubalData)
        {
            drops.Add(new ItemSupplement()
            {
                ItemId = gubalItem.ItemId,
                SourceItemId = gubalItem.Data.ItemId,
                ItemSupplementSource = ItemSupplementSource.Loot
            });
        }
        var reductionData = this.gubalApi.GetAllaganReportReductionItems();
        foreach (var reductionItem in reductionData)
        {
            drops.Add(new ItemSupplement()
            {
                ItemId = reductionItem.ItemId,
                SourceItemId = reductionItem.Data.ItemId,
                ItemSupplementSource = ItemSupplementSource.Reduction
            });
        }
        var gardeningData = this.gubalApi.GetAllaganReportGardeningItems();
        foreach (var gardeningItem in gardeningData)
        {
            drops.Add(new ItemSupplement()
            {
                ItemId = gardeningItem.ItemId,
                SourceItemId = gardeningItem.Data.ItemId,
                ItemSupplementSource = ItemSupplementSource.Gardening
            });
        }
        var desynthData = this.gubalApi.GetAllaganReportDesynthItems();
        foreach (var desynthItem in desynthData)
        {
            drops.Add(new ItemSupplement()
            {
                ItemId = desynthItem.ItemId,
                SourceItemId = desynthItem.Data.ItemId,
                ItemSupplementSource = ItemSupplementSource.Desynth
            });
        }

        return drops;
    }   
}
