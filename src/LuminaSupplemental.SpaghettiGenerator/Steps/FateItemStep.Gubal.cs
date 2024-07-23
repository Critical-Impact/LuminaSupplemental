using System.Collections.Generic;

using LuminaSupplemental.Excel.Model;

namespace LuminaSupplemental.SpaghettiGenerator.Steps;

public partial class FateItemStep
{
    private List<FateItem> ProcessGubalData()
    {
        var drops = new List<FateItem>();
        var gubalData = this.gubalApi.GetAllaganReportFateItems();
        foreach (var gubalItem in gubalData)
        {
            if (gubalItem.ItemId == 0 || gubalItem.Data.FateId == 0)
            {
                continue;
            }
            if (!this.fateSheet.HasRow(gubalItem.Data.FateId))
            {
                this.logger.Error($"Fate {gubalItem.Data.FateId} retrieved from gubal api does not exist in game.");
                continue;
            }

            drops.Add(new FateItem()
            {
                ItemId = gubalItem.ItemId,
                FateId = gubalItem.Data.FateId
            });
        }
        

        return drops;
    }   
}
