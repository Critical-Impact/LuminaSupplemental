using System;
using System.Collections.Generic;
using System.Linq;

using Lumina.Excel.GeneratedSheets;

using LuminaSupplemental.Excel.Model;
using LuminaSupplemental.SpaghettiGenerator.Generator;
using LuminaSupplemental.SpaghettiGenerator.Steps.Parsers;

namespace LuminaSupplemental.SpaghettiGenerator.Steps;

public partial class DungeonChestItemStep : GeneratorStep
{
    private readonly DataCacher dataCacher;
    private readonly GTParser gtParser;
    private readonly Dictionary<string,ContentFinderCondition> dutiesByString;
    private readonly Dictionary<string,Item> itemsByName;

    public override Type OutputType => typeof(DungeonChestItem);

    public override string FileName => "DungeonChestItem.csv";

    public override string Name => "Dungeon Chest Items";

    public DungeonChestItemStep(DataCacher dataCacher, GTParser gtParser)
    {
        this.dataCacher = dataCacher;
        this.gtParser = gtParser;
        var bannedItems = new HashSet<uint>() { 0, 24225 };
        this.dutiesByString = this.dataCacher.ByName<ContentFinderCondition>(item => item.Name.ToString().ToParseable());
        this.itemsByName = this.dataCacher.ByName<Item>(item => item.Name.ToString().ToParseable(), item => !bannedItems.Contains(item.RowId));
    }

    public override List<ICsv> Run()
    {
        List<DungeonChestItem> items = new();
        this.gtParser.ProcessDutiesJson();
        items.AddRange(this.gtParser.DungeonChestItems);
        return [..items.Select(c => c)];
    }
}
