using System;
using System.Collections.Generic;
using System.Linq;

using Lumina.Excel.Sheets;

using LuminaSupplemental.Excel.Model;
using LuminaSupplemental.SpaghettiGenerator.Generator;
using LuminaSupplemental.SpaghettiGenerator.Steps.Parsers;

namespace LuminaSupplemental.SpaghettiGenerator.Steps;

public partial class DungeonChestItemStep : GeneratorStep
{
    private readonly DataCacher dataCacher;

    public override Type OutputType => typeof(DungeonChestItem);

    public override string FileName => "DungeonChestItem.csv";

    public override string Name => "Dungeon Chest Items";

    public DungeonChestItemStep(DataCacher dataCacher)
    {
        this.dataCacher = dataCacher;
    }

    public override List<ICsv> Run(Dictionary<Type, List<ICsv>> stepData)
    {
        List<DungeonChestItem> items = new();
        items.AddRange(this.ProcessChestItems());
        return [..items.Select(c => c)];
    }

    private List<DungeonChestItem> ProcessChestItems()
    {
        var chests = new List<DungeonChestItem>();
        var itemId = 1u;
        var chestId = 1u;
        foreach (var duty in ChestDropsV2Reader.Load())
        {
            foreach (var chest in duty.Chests)
            {
                foreach (var reward in chest.Rewards)
                {
                    chests.Add(
                        new DungeonChestItem()
                        {
                            RowId = itemId,
                            ChestId = chestId,
                            ItemId = reward.Id,
                            Min = (uint?)reward.Min,
                            Max = (uint?)reward.Max,
                            Probability = (decimal?)Math.Round(reward.Pct * 100, 2)
                        });
                    itemId++;
                }

                chestId++;
            }
        }

        return chests;
    }
}
