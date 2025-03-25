using System;
using System.Collections.Generic;
using System.Linq;

using Lumina;

using LuminaSupplemental.Excel.Model;
using LuminaSupplemental.SpaghettiGenerator.Generator;
using LuminaSupplemental.SpaghettiGenerator.Steps.Parsers;

namespace LuminaSupplemental.SpaghettiGenerator.Steps;

public partial class MobDropStep : GeneratorStep
{
    private readonly AppConfig appConfig;
    private readonly GameData gameData;
    private readonly GubalApi gubalApi;

    public override Type OutputType => typeof(MobDrop);

    public override string FileName => "MobDrop.csv";

    public override string Name => "Mob Drops";

    public MobDropStep(AppConfig appConfig, GameData gameData, GubalApi gubalApi)
    {
        this.appConfig = appConfig;
        this.gameData = gameData;
        this.gubalApi = gubalApi;
    }

    public override bool ShouldRun()
    {
        return appConfig.Parsing.ProcessMobSpawnHTML;
    }

    public override List<ICsv> Run()
    {
        List<MobDrop> items = new List<MobDrop>();
        items.AddRange(this.ProcessLodestoneDrops());
        items.AddRange(this.ProcessGubalData());
        items = items.DistinctBy(c => (c.ItemId, c.BNpcNameId)).ToList();
        return [..items.Select(c => c).OrderBy(c => c.BNpcNameId).ThenBy(c => c.ItemId)];
    }
}
