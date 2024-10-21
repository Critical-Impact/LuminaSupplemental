using System;
using System.Collections.Generic;
using System.Linq;

using Lumina.Excel.Sheets;

using LuminaSupplemental.Excel.Model;
using LuminaSupplemental.SpaghettiGenerator.Generator;
using LuminaSupplemental.SpaghettiGenerator.Steps.Parsers;

namespace LuminaSupplemental.SpaghettiGenerator.Steps;

public partial class DungeonBossDropStep : GeneratorStep
{
    private readonly DataCacher dataCacher;
    private readonly GTParser gtParser;
    private readonly Dictionary<string,uint> dutiesByString;

    public override Type OutputType => typeof(DungeonBossDrop);

    public override string FileName => "DungeonBossDrop.csv";

    public override string Name => "Dungeon Boss Drops";

    public DungeonBossDropStep(DataCacher dataCacher, GTParser gtParser)
    {
        this.dataCacher = dataCacher;
        this.gtParser = gtParser;
        this.dutiesByString = this.dataCacher.ByName<ContentFinderCondition>(item => item.Name.ToString().ToParseable());
    }

    public override List<ICsv> Run()
    {
        List<DungeonBossDrop> items = new();
        this.gtParser.ProcessDutiesJson();
        items.AddRange(this.gtParser.DungeonBossDrops);
        return [..items.Select(c => c)];
    }
}
