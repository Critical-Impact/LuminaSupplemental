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

    public DungeonChestStep(DataCacher dataCacher)
    {
        this.dataCacher = dataCacher;
    }

    public override List<ICsv> Run(Dictionary<Type, List<ICsv>> stepData)
    {
        List<DungeonChest> items = new();
        items.AddRange(this.ProcessChests());
        return [..items.Select(c => c)];
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
                                    ChestId = chest.Id
                                });
                            chestId++;
                        }
                    }
                }
            }
        }

        return chests;
    }
}
