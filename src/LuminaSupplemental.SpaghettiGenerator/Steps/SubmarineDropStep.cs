using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using CSVFile;

using Lumina;
using Lumina.Excel;
using Lumina.Excel.Sheets;

using LuminaSupplemental.Excel.Model;
using LuminaSupplemental.SpaghettiGenerator.Generator;
using LuminaSupplemental.SpaghettiGenerator.Steps.Parsers;

using Newtonsoft.Json;

using SupabaseExporter;
using SupabaseExporter.Processing.Submarines;
using SupabaseExporter.Structures.Exports;

using ILogger = Serilog.ILogger;

namespace LuminaSupplemental.SpaghettiGenerator.Steps;

public partial class SubmarineDropStep : GeneratorStep
{
    private readonly DataCacher dataCacher;
    private readonly GubalApi gubalApi;
    private readonly ExcelSheet<SubmarineExploration> submarineExplorationSheet;
    private readonly Dictionary<string,uint> submarinesByName;
    private readonly Dictionary<string,uint> itemsByName;
    private readonly ILogger logger;
    private readonly ExcelSheet<Item> itemSheet;

    public override Type OutputType => typeof(SubmarineDrop);

    public override string FileName => "SubmarineDrop.csv";

    public override string Name => "Submarine Drops";

    public SubmarineDropStep(DataCacher dataCacher, GubalApi gubalApi, ExcelSheet<SubmarineExploration> submarineExplorationSheet, ILogger logger, ExcelSheet<Item> itemSheet)
    {
        this.dataCacher = dataCacher;
        this.gubalApi = gubalApi;
        this.submarineExplorationSheet = submarineExplorationSheet;
        this.logger = logger;
        this.itemSheet = itemSheet;
        var bannedItems = new HashSet< uint >()
        {
            0,
            24225
        };
        this.submarinesByName = this.dataCacher.ByName<SubmarineExploration>(item => item.Destination.ExtractText().ToParseable());
        this.itemsByName = this.dataCacher.ByName<Item>(item => item.Name.ToString().ToParseable(), item => !bannedItems.Contains(item.RowId));
    }


    public override List<ICsv> Run(Dictionary<Type, List<ICsv>> stepData)
    {
        List<SubmarineDrop> items = new ();
        items.AddRange(this.Process());
        items = items.DistinctBy(c => (c.ItemId, c.SubmarineExplorationId)).ToList();

        return [..items.Select(c => c).OrderBy(c => c.SubmarineExplorationId).ThenBy(c => c.ItemId)];
    }

    private List<SubmarineDrop> Process()
    {
        List<SubmarineDrop> submarineDrops = new();

        var filePath = "../../../../FFXIVGachaSpreadsheet/website/static/data/Submarines.json";
        var json = File.ReadAllText(filePath);
        var subLoot = JsonConvert.DeserializeObject<SubLoot>(json)!;

        foreach (var sector in subLoot.Sectors)
        {
            foreach (var pool in sector.Value.Pools)
            {
                foreach (var reward in pool.Value.Rewards)
                {
                    var sectorId = sector.Key;
                    var itemId = reward.Value.Id;
                    var lootTier = pool.Key;

                    submarineDrops.Add(
                        new SubmarineDrop()
                        {
                            SubmarineExplorationId = Convert.ToUInt32(sectorId),
                            ItemId = Convert.ToUInt32(itemId),
                            LootTier = Convert.ToByte(lootTier),
                            PoorMin = (uint)reward.Value.MinMax[RetTier.Poor][0],
                            PoorMax = (uint)reward.Value.MinMax[RetTier.Poor][1],
                            NormalMin = (uint)reward.Value.MinMax[RetTier.Normal][0],
                            NormalMax = (uint)reward.Value.MinMax[RetTier.Normal][1],
                            OptimalMin = (uint)reward.Value.MinMax[RetTier.Optimal][0],
                            OptimalMax = (uint)reward.Value.MinMax[RetTier.Optimal][1],
                        });
                }
            }
        }

        return submarineDrops;
    }
}
