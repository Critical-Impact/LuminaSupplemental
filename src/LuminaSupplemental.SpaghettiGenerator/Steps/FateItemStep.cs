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

using SupabaseExporter.Structures.Exports;

namespace LuminaSupplemental.SpaghettiGenerator.Steps;

public class FateItemStep : GeneratorStep
{
    // FateReward.json fate type ids, 1 is critical engagements which use DynamicEvent ids instead of Fate ids
    private const byte FateTypeFate = 0;

    // The exporter adds gil and grand company seals as fake rewards, these will move to their own sheet once fate token data is available
    private static readonly HashSet<uint> CurrencyItemIds = [1, 20];

    private readonly ExcelSheet<Fate> fateSheet;
    private readonly ILogger logger;

    public override Type OutputType => typeof(FateItem);

    public override string FileName => "FateItem.csv";

    public override string Name => "Fate Items";


    public FateItemStep(ExcelSheet<Fate> fateSheet, ILogger logger)
    {
        this.fateSheet = fateSheet;
        this.logger = logger;
    }


    public override List<ICsv> Run(Dictionary<Type, List<ICsv>> stepData)
    {
        List<FateItem> items = new List<FateItem>();
        items.AddRange(this.ProcessTrackyData());
        items.AddRange(this.ProcessManualData());
        // Tracky rows come first so they keep their drop rates when a manual row matches
        items = items.DistinctBy(c => (c.ItemId, c.FateId)).ToList();

        return [..items.Select(c => c).OrderBy(c => c.FateId).ThenBy(c => c.ItemId)];
    }

    private List<FateItem> ProcessTrackyData()
    {
        var filePath = "../../../../FFXIVGachaSpreadsheet/website/static/data/FateReward.json";

        var drops = new List<FateItem>();
        var json = File.ReadAllText(filePath);
        var fateReward = JsonConvert.DeserializeObject<FateReward>(json)!;

        var fates = fateReward.Expansions
            .SelectMany(expansion => expansion.Territories)
            .SelectMany(territory => territory.FateTypes)
            .Where(fateType => fateType.Id == FateTypeFate)
            .SelectMany(fateType => fateType.Fates);

        foreach (var fate in fates)
        {
            if (!this.fateSheet.HasRow(fate.Id))
            {
                this.logger.Error($"Fate {fate.Id} retrieved from tracky data does not exist in game.");
                continue;
            }

            foreach (var reward in fate.Rewards)
            {
                if (reward.Id == 0 || CurrencyItemIds.Contains(reward.Id))
                {
                    continue;
                }

                drops.Add(new FateItem(reward.Id, fate.Id, (uint)reward.Min, (uint)reward.Max, (decimal)Math.Round(reward.Pct * 100, 2)));
            }
        }

        return drops;
    }

    private List<FateItem> ProcessManualData()
    {
        var drops = new List<FateItem>();
        var reader = CSVFile.CSVReader.FromFile(Path.Combine("ManualData", "FateManualItems.csv"));

        foreach (var line in reader.Lines())
        {
            var itemId = uint.Parse(line[0]);
            var fateId = uint.Parse(line[1]);
            if (!this.fateSheet.HasRow(fateId))
            {
                this.logger.Error($"Fate {fateId} from the manual fate data does not exist in game.");
                continue;
            }

            drops.Add(new FateItem(itemId, fateId));
        }

        return drops;
    }
}
