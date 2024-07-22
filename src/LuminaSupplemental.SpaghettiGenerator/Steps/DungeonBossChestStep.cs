using System;
using System.Collections.Generic;
using System.Linq;

using Lumina.Excel.GeneratedSheets;

using LuminaSupplemental.Excel.Model;
using LuminaSupplemental.SpaghettiGenerator.Generator;
using LuminaSupplemental.SpaghettiGenerator.Steps.Parsers;

namespace LuminaSupplemental.SpaghettiGenerator.Steps;

public partial class DungeonBossChestStep : GeneratorStep
{
    private readonly DataCacher dataCacher;
    private readonly GTParser gtParser;
    private readonly Dictionary<string,ContentFinderCondition> dutiesByString;

    public override Type OutputType => typeof(DungeonBossChest);

    public override string FileName => "DungeonBossChest.csv";

    public override string Name => "Dungeon Boss Chests";

    public DungeonBossChestStep(DataCacher dataCacher, GTParser gtParser)
    {
        this.dataCacher = dataCacher;
        this.gtParser = gtParser;
        this.dutiesByString = this.dataCacher.ByName<ContentFinderCondition>(item => item.Name.ToString().ToParseable());
    }

    public override List<ICsv> Run()
    {
        List<DungeonBossChest> items = new();
        this.gtParser.ProcessDutiesJson();
        items.AddRange(this.gtParser.DungeonBossChests);
        return [..items.Select(c => c)];
    }
}
