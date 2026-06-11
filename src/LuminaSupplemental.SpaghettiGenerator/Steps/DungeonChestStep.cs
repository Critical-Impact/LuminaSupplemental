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

public partial class DungeonChestStep : GeneratorStep
{
    private readonly DataCacher dataCacher;

    public override Type OutputType => typeof(DungeonChest);

    public override string FileName => "DungeonChest.csv";

    public override string Name => "Dungeon Chests";

    public override List<Type>? PrerequisiteSteps => [typeof(DungeonChestItemStep), typeof(DungeonBossChestStep), typeof(DungeonBossStep)];

    public DungeonChestStep(DataCacher dataCacher)
    {
        this.dataCacher = dataCacher;
    }

    public override List<ICsv> Run(Dictionary<Type, List<ICsv>> stepData)
    {
        var chests = this.ProcessChests();
        this.AssignBossIds(chests, stepData);
        return [..chests.Select(c => c)];
    }

    private List<DungeonChest> ProcessChests()
    {
        var filePath = "../../../../FFXIVGachaSpreadsheet/website/static/data/ChestDrops.json";

        var chests = new List<DungeonChest>();
        var json = File.ReadAllText(filePath);
        var chestList = JsonConvert.DeserializeObject<List<ChestDrop>>(json)!;
        uint chestId = 1;
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
                            chests.Add(
                                new DungeonChest()
                                {
                                    RowId = chestId,
                                    ChestNo = (byte)(index + 1),
                                    ContentFinderConditionId = duty.Id,
                                    MapId = chest.MapId,
                                    TerritoryTypeId = chest.TerritoryId,
                                    TreasureId = chest.Id
                                });
                            chestId++;
                        }
                    }
                }
            }
        }

        return chests;
    }

    private void AssignBossIds(List<DungeonChest> chests, Dictionary<Type, List<ICsv>> stepData)
    {
        var chestItemsByChestId = stepData[typeof(DungeonChestItemStep)]
            .Cast<DungeonChestItem>()
            .GroupBy(i => i.ChestId)
            .ToDictionary(g => g.Key, g => g.Select(i => i.ItemId).ToHashSet());

        var bossChestGroups = stepData[typeof(DungeonBossChestStep)]
            .Cast<DungeonBossChest>()
            .GroupBy(b => (b.ContentFinderConditionId, b.FightNo))
            .ToDictionary(g => g.Key, g => g.Select(b => b.ItemId).ToHashSet());

        var bossesByFight = stepData[typeof(DungeonBossStep)]
            .Cast<DungeonBoss>()
            .GroupBy(b => (b.ContentFinderConditionId, b.FightNo))
            .ToDictionary(g => g.Key, g => g.Min(b => b.RowId));

        foreach (var chest in chests)
        {
            if (!chestItemsByChestId.TryGetValue(chest.RowId, out var itemIds))
                continue;

            var matchedFight = bossChestGroups
                .Where(kvp => kvp.Key.ContentFinderConditionId == chest.ContentFinderConditionId)
                .Where(kvp => kvp.Value.SetEquals(itemIds))
                .Select(kvp => kvp.Key)
                .FirstOrDefault();

            if (bossesByFight.TryGetValue(matchedFight, out var bossId))
                chest.DungeonBossId = bossId;
        }
    }
}
