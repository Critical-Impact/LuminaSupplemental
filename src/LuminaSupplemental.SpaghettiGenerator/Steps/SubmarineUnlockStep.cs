using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Lumina;
using Lumina.Excel;
using Lumina.Excel.Sheets;

using LuminaSupplemental.Excel.Model;
using LuminaSupplemental.SpaghettiGenerator.Generator;

using Newtonsoft.Json;

using SupabaseExporter.Structures.Exports;

namespace LuminaSupplemental.SpaghettiGenerator.Steps;

public partial class SubmarineUnlockStep : GeneratorStep
{
    private readonly DataCacher dataCacher;
    private readonly Dictionary<string,uint> submarinesByName;
    private readonly ExcelSheet<SubmarineExploration> submarineExplorationSheet;

    public override Type OutputType => typeof(SubmarineUnlock);

    public override string FileName => "SubmarineUnlock.csv";

    public override string Name => "Submarine Unlocks";


    public SubmarineUnlockStep(DataCacher dataCacher, ExcelSheet<SubmarineExploration> submarineExplorationSheet)
    {
        this.dataCacher = dataCacher;
        var bannedItems = new HashSet< uint >()
        {
            0,
            24225
        };
        this.submarinesByName = this.dataCacher.ByName<SubmarineExploration>(item => item.Destination.ToString().ToParseable());
        this.submarineExplorationSheet = submarineExplorationSheet;
    }



    public override List<ICsv> Run(Dictionary<Type, List<ICsv>> stepData)
    {
        List<SubmarineUnlock> items = new ();
        items.AddRange(this.Process());
        return [..items.Select(c => c).OrderBy(c => c.SubmarineExplorationId).ThenBy(c => c.SubmarineExplorationUnlockId)];
    }

    private List<SubmarineUnlock> Process()
    {
        List<SubmarineUnlock> submarineUnlocks = new();

        var filePath = "../../../../FFXIVGachaSpreadsheet/website/static/data/Submarines.json";
        var json = File.ReadAllText(filePath);
        var subLoot = JsonConvert.DeserializeObject<SubLoot>(json)!;

        foreach (var sector in subLoot.Sectors)
        {
            submarineUnlocks.Add(new SubmarineUnlock()
            {
                SubmarineExplorationId = sector.Value.Id,
                SubmarineExplorationUnlockId = sector.Value.UnlockedFrom,
                RankRequired = sector.Value.Rank
            });
        }

        return submarineUnlocks;
    }
}
