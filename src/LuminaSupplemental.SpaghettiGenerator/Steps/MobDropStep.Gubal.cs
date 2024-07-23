using System.Collections.Generic;

using LuminaSupplemental.Excel.Model;

namespace LuminaSupplemental.SpaghettiGenerator.Steps;

public partial class MobDropStep
{
    private List<MobDrop> ProcessGubalData()
    {
        var drops = new List<MobDrop>();
        var gubalDrops = this.gubalApi.GetAllaganReportDrops();
        foreach (var gubalDrop in gubalDrops)
        {
            if (gubalDrop.ItemId == 0 || gubalDrop.Data.MonsterId == 0)
            {
                continue;
            }
            drops.Add(new MobDrop()
            {
                ItemId = gubalDrop.ItemId,
                BNpcNameId = gubalDrop.Data.MonsterId
            });
        }

        return drops;
    }
}
