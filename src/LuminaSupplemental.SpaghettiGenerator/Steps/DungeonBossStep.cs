using System;
using System.Collections.Generic;
using System.Linq;

using Lumina.Excel.Sheets;

using LuminaSupplemental.Excel.Model;
using LuminaSupplemental.SpaghettiGenerator.Generator;
using LuminaSupplemental.SpaghettiGenerator.Steps.Parsers;

namespace LuminaSupplemental.SpaghettiGenerator.Steps;

public partial class DungeonBossStep : GeneratorStep
{
    private readonly DataCacher dataCacher;
    private readonly GTParser gtParser;
    private readonly Dictionary<string,uint> dutiesByString;

    public override Type OutputType => typeof(DungeonBoss);

    public override string FileName => "DungeonBoss.csv";

    public override string Name => "Dungeon Bosses";

    public DungeonBossStep(DataCacher dataCacher, GTParser gtParser)
    {
        this.dataCacher = dataCacher;
        this.gtParser = gtParser;
        this.dutiesByString = this.dataCacher.ByName<ContentFinderCondition>(item => item.Name.ToString().ToParseable());
    }

    public override List<ICsv> Run(Dictionary<Type, List<ICsv>> stepData)
    {
        List<DungeonBoss> items = new();
        this.gtParser.ProcessDutiesJson();
        items.AddRange(this.gtParser.DungeonBosses);
        return [..items.Select(c => c)];
    }
}
