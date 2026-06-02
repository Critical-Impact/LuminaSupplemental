using System.Collections.Generic;

using LuminaSupplemental.Excel.Model;
using LuminaSupplemental.SpaghettiGenerator.Steps.Parsers;

namespace LuminaSupplemental.SpaghettiGenerator.Steps;

public partial class AirshipDropStep
{
    private List<AirshipDrop> ProcessGubalData()
    {
        var drops = new List<AirshipDrop>();
        var gubalData = this.gubalApi.GetAllaganReportVoyageItems();
        foreach (var gubalItem in gubalData)
        {
            if (gubalItem.Data == null || gubalItem.ItemId == 0 || gubalItem.Data.VoyageId == 0 || gubalItem.Data.VoyageId == null)
            {
                continue;
            }
            if (gubalItem.Data.VoyageType == AllaganReportVoyageType.Airship)
            {
                if (!this.airshipExplorationPointSheet.HasRow(gubalItem.Data.VoyageId.Value))
                {
                    this.logger.Error($"Airship Point {gubalItem.Data.VoyageId} retrieved from gubal api does not exist in game.");
                    continue;
                }

                drops.Add(
                    new AirshipDrop()
                    {
                        ItemId = gubalItem.ItemId,
                        AirshipExplorationPointId = gubalItem.Data.VoyageId.Value
                    });
            }
        }


        return drops;
    }
}
