using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;

using Lumina;
using Lumina.Excel;
using Lumina.Excel.Sheets;

using LuminaSupplemental.Excel.Model;
using LuminaSupplemental.SpaghettiGenerator.Generator;
using LuminaSupplemental.SpaghettiGenerator.Steps.Parsers;

namespace LuminaSupplemental.SpaghettiGenerator.Steps;

public partial class BNpcLinkStep : GeneratorStep
{
    private readonly GubalApi gubalApi;
    private readonly ExcelSheet<BNpcName> nameSheet;
    private readonly ExcelSheet<BNpcBase> baseSheet;

    public BNpcLinkStep(GubalApi gubalApi, GameData gameData)
    {
        this.nameSheet = gameData.GetExcelSheet<BNpcName>()!;
        this.baseSheet = gameData.GetExcelSheet<BNpcBase>()!;
        this.gubalApi = gubalApi;
    }
    public override Type OutputType => typeof(BNpcLink);

    public override string FileName => "BNpcLink.csv";

    public override string Name => "BNPC Links";

    public override List<Type>? PrerequisiteSteps => [typeof(MobSpawnStep)];

    public override List<ICsv> Run(Dictionary<Type, List<ICsv>> stepData)
    {
        List<BNpcLink> items = new ();
        if (stepData.ContainsKey(typeof(MobSpawnStep)))
        {
            var mobSpawnPositions = stepData[typeof(MobSpawnStep)].Cast<MobSpawnPosition>().ToList();
            items.AddRange(this.Process(mobSpawnPositions));
        }

        items.AddRange(this.ProcessGubalData());

        return [..items.Where(c => this.nameSheet.HasRow(c.BNpcNameId) && this.baseSheet.HasRow(c.BNpcBaseId)).DistinctBy(c => (c.BNpcBaseId, c.BNpcNameId)).OrderBy(c => c.BNpcNameId).ThenBy(c => c.BNpcBaseId).Select(c => c)];
    }

    private List<BNpcLink> Process(List<MobSpawnPosition> positions)
    {
        return positions.Select(e => (e.BNpcBaseId, e.BNpcNameId)).Distinct().Where(c => c.BNpcBaseId != 0 && c.BNpcNameId != 0).Select(c => new BNpcLink(c.BNpcNameId, c.BNpcBaseId)).OrderBy(c => c.BNpcNameId).ThenBy(c => c.BNpcBaseId).ToList();
    }

    private List<BNpcLink> ProcessGubalData()
    {
        var drops = new List<BNpcLink>();
        var bNpcLinks = this.gubalApi.GetGubalBNpcLinks();
        foreach (var bNpcLink in bNpcLinks)
        {
            if (bNpcLink.BNpcBase == 0 || bNpcLink.BNpcName == 0)
            {
                continue;
            }
            drops.Add(new BNpcLink()
            {
                BNpcBaseId = bNpcLink.BNpcBase,
                BNpcNameId = bNpcLink.BNpcName
            });
        }

        return drops;
    }
}
