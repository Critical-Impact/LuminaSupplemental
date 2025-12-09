using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Lumina.Excel.Sheets;

using LuminaSupplemental.Excel.Model;
using LuminaSupplemental.SpaghettiGenerator.Generator;
using LuminaSupplemental.SpaghettiGenerator.Steps.Parsers;

using Newtonsoft.Json;

using SupabaseExporter.Structures.Exports;

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
        var filePath = "../../../../FFXIVGachaSpreadsheet/website/static/data/ChestDrops.json";

        var chests = new List<DungeonChestItem>();
        var json = File.ReadAllText(filePath);
        var chestList = JsonConvert.DeserializeObject<List<ChestDrop>>(json)!;
        var itemId = 1u;
        var chestId = 1u;
        foreach (var chestDrop in chestList)
        {
            foreach (var expansion in chestDrop.Expansions)
            {
                foreach (var header in expansion.Headers)
                {
                    foreach (var duty in header.Duties)
                    {
                        for (var index = 0; index < duty.Chests.Count; index++)
                        {
                            var chest = duty.Chests[index];
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
                }
            }
        }

        return chests;
    }
}
