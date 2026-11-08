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

public class FateRewardStep : GeneratorStep
{
    private const byte FateTypeFate = 0;

    private const uint GilItemId = 1;
    private const uint GrandCompanySealsItemId = 20;

    private readonly ExcelSheet<Fate> fateSheet;
    private readonly ILogger logger;

    public override Type OutputType => typeof(FateReward);

    public override string FileName => "FateReward.csv";

    public override string Name => "Fate Rewards";


    public FateRewardStep(ExcelSheet<Fate> fateSheet, ILogger logger)
    {
        this.fateSheet = fateSheet;
        this.logger = logger;
    }


    public override List<ICsv> Run(Dictionary<Type, List<ICsv>> stepData)
    {
        var filePath = "../../../../FFXIVGachaSpreadsheet/website/static/data/FateReward.json";

        var rewards = new List<FateReward>();
        var json = File.ReadAllText(filePath);
        var fateReward = JsonConvert.DeserializeObject<SupabaseExporter.Structures.Exports.FateReward>(json)!;

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

            var gil = fate.Rewards.FirstOrDefault(c => c.Id == GilItemId);
            var seals = fate.Rewards.FirstOrDefault(c => c.Id == GrandCompanySealsItemId);
            rewards.Add(new FateReward(
                fate.Id,
                (uint?)gil?.Min,
                (uint?)gil?.Max,
                (uint?)seals?.Min,
                (uint?)seals?.Max));
        }

        return [..rewards.DistinctBy(c => c.FateId).OrderBy(c => c.FateId)];
    }
}
