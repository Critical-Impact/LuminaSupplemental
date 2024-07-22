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
            drops.Add(new MobDrop()
            {
                ItemId = gubalDrop.ItemId,
                BNpcNameId = gubalDrop.Data.MonsterId
            });
        }

        return drops;
    }
}
