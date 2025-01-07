using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Lumina.Excel;
using Lumina.Excel.Sheets;

using LuminaSupplemental.Excel.Model;
using LuminaSupplemental.SpaghettiGenerator.Generator;

using Newtonsoft.Json;

using Serilog;

namespace LuminaSupplemental.SpaghettiGenerator.Steps;

public class GardeningCrossbreedsStep : GeneratorStep
{
    private readonly ExcelSheet<Item> itemSheet;
    private readonly ILogger logger;
    private readonly DataCacher dataCacher;
    private readonly Dictionary<string,uint> itemsByName;

    public override Type OutputType => typeof(GardeningCrossbreed);

    public override string FileName => "GardeningCrossbreed.csv";

    public override string Name => "Gardening Crossbreeds";

    public GardeningCrossbreedsStep(ExcelSheet<Item> itemSheet, ILogger logger, DataCacher dataCacher)
    {
        this.itemSheet = itemSheet;
        this.logger = logger;
        this.dataCacher = dataCacher;
        var bannedItems = new HashSet< uint >()
        {
            0,
            24225
        };
        this.itemsByName = this.dataCacher.ByName<Item>(item => item.Name.ToString().ToParseable(), item => !bannedItems.Contains(item.RowId));
    }

    public override List<ICsv> Run()
    {
        var gardeningCrossbreeds = new List<GardeningCrossbreed>();
        var patchText = File.ReadAllText( Path.Join("ManualData","Gardening","crossbreeding.json") );
        var crossBreeds = JsonConvert.DeserializeObject<Dictionary<string, List<string[]>>>(patchText)!.ToList();

        foreach (var crossBreed in crossBreeds)
        {
            var crossBreedName = crossBreed.Key.ToParseable();
            if (this.itemsByName.ContainsKey(crossBreedName))
            {
                var itemId = itemsByName[crossBreedName];

                foreach (var requirements in crossBreed.Value)
                {
                    if (requirements.Length == 2)
                    {
                        var requirement1Name = requirements[0].ToParseable();
                        var requirement2Name = requirements[1].ToParseable();

                        if (!this.itemsByName.TryGetValue(requirement1Name, out var requirement1Id))
                        {
                            break;
                        }

                        if (!this.itemsByName.TryGetValue(requirement2Name, out var requirement2Id))
                        {
                            break;
                        }
                        gardeningCrossbreeds.Add(new GardeningCrossbreed(itemId, requirement1Id, requirement2Id));

                    }
                    else
                    {
                        this.logger.Error("Incorrect requirements for gardening crossbreed for " + crossBreedName);
                    }
                }
            }
            else
            {
                this.logger.Error("Could not find the item with the name " + crossBreedName);
            }
        }

        return gardeningCrossbreeds.DistinctBy(c => (c.ItemResultId, c.ItemRequirement1Id, c.ItemRequirement2Id)).OrderBy(c => c.ItemResultId).ThenBy(c => c.ItemRequirement1Id).ThenBy(c => c.ItemRequirement2Id).Cast<ICsv>().ToList();
    }
}
