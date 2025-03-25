using System;
using System.Collections.Generic;
using System.Linq;

using Lumina;
using Lumina.Excel;
using Lumina.Excel.Sheets;

using LuminaSupplemental.Excel.Model;
using LuminaSupplemental.SpaghettiGenerator.Generator;

namespace LuminaSupplemental.SpaghettiGenerator.Steps;

public partial class ENpcPlaceStep : GeneratorStep
{
    private readonly ExcelSheet<Item> itemSheet;
    private readonly ExcelSheet<TerritoryType> territoryTypeSheet;
    private readonly GameData gameData;
    private readonly Dictionary<string, uint> itemsByName;

    public override Type OutputType => typeof(ENpcPlace);

    public override string FileName => "ENpcPlace.csv";

    public override string Name => "Event NPC Place";


    public ENpcPlaceStep(ExcelSheet<TerritoryType> territoryTypeSheet, GameData gameData)
    {
        this.territoryTypeSheet = territoryTypeSheet;
        this.gameData = gameData;
    }


    public override List<ICsv> Run()
    {
        List<ENpcPlace> items = new ();
        items.AddRange(this.ProcessManualData());

        return [..items.Select(c => c).OrderBy(c => c.ENpcResidentId).ThenBy(c => c.TerritoryTypeId)];
    }
}
