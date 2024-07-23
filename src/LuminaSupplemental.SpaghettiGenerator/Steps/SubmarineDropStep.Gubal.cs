using System.Collections.Generic;

using LuminaSupplemental.Excel.Model;
using LuminaSupplemental.SpaghettiGenerator.Steps.Parsers;

namespace LuminaSupplemental.SpaghettiGenerator.Steps;

public partial class SubmarineDropStep
{
    private List<SubmarineDrop> ProcessGubalData()
    {
        var drops = new List<SubmarineDrop>();
        var gubalData = this.gubalApi.GetAllaganReportVoyageItems();
        foreach (var gubalItem in gubalData)
        {
            if (gubalItem.ItemId == 0 || gubalItem.Data.VoyageId == 0)
            {
                continue;
            }
            if (gubalItem.Data.VoyageType == AllaganReportVoyageType.Submarine)
            {
                if (!this.submarineExplorationSheet.HasRow(gubalItem.Data.VoyageId))
                {
                    this.logger.Error($"Submarine Point {gubalItem.Data.VoyageId} retrieved from gubal api does not exist in game.");
                    continue;
                }

                drops.Add(
                    new SubmarineDrop()
                    {
                        ItemId = gubalItem.ItemId,
                        SubmarineExplorationId = gubalItem.Data.VoyageId
                    });
            }
        }
        

        return drops;
    }   
}
