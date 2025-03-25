using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Lumina;
using Lumina.Excel;
using Lumina.Excel.Sheets;

using LuminaSupplemental.Excel.Model;
using LuminaSupplemental.SpaghettiGenerator.Generator;
using LuminaSupplemental.SpaghettiGenerator.Steps.Parsers;

using ILogger = Serilog.ILogger;

namespace LuminaSupplemental.SpaghettiGenerator.Steps;

public partial class SubmarineDropStep : GeneratorStep
{
    private readonly DataCacher dataCacher;
    private readonly GubalApi gubalApi;
    private readonly ExcelSheet<SubmarineExploration> submarineExplorationSheet;
    private readonly Dictionary<string,uint> submarinesByName;
    private readonly Dictionary<string,uint> itemsByName;
    private readonly ILogger logger;
    private readonly ExcelSheet<Item> itemSheet;

    public override Type OutputType => typeof(SubmarineDrop);

    public override string FileName => "SubmarineDrop.csv";

    public override string Name => "Submarine Drops";

    public SubmarineDropStep(DataCacher dataCacher, GubalApi gubalApi, ExcelSheet<SubmarineExploration> submarineExplorationSheet, ILogger logger, ExcelSheet<Item> itemSheet)
    {
        this.dataCacher = dataCacher;
        this.gubalApi = gubalApi;
        this.submarineExplorationSheet = submarineExplorationSheet;
        this.logger = logger;
        this.itemSheet = itemSheet;
        var bannedItems = new HashSet< uint >()
        {
            0,
            24225
        };
        this.submarinesByName = this.dataCacher.ByName<SubmarineExploration>(item => item.Destination.ExtractText().ToParseable());
        this.itemsByName = this.dataCacher.ByName<Item>(item => item.Name.ToString().ToParseable(), item => !bannedItems.Contains(item.RowId));
    }


    public override List<ICsv> Run()
    {
        List<SubmarineDrop> items = new ();
        items.AddRange(this.Process());
        //items.AddRange(this.ProcessGubalData()); // No longer needed as the data from the CSV is up to date
        items = items.DistinctBy(c => (c.ItemId, c.SubmarineExplorationId)).ToList();

        return [..items.Select(c => c).OrderBy(c => c.SubmarineExplorationId).ThenBy(c => c.ItemId)];
    }

    private Dictionary<string, string> MateriaNames = new()
    {
        { "Direct Hit Rate", "Heavens' Eye Materia" },
        { "Skill Speed", "Quickarm" },
        { "Spell Speed", "Quicktongue" },
        { "Tenacity", "Battledance" },
        { "Control", "Craftsman's Command" },
        { "Craftsmanship", "Craftsman's Competence" },
        { "GP", "Gatherer's Grasp" },
        { "Perception", "Gatherer's Guile" },
        { "Determination", "Savage Might" },
        { "Piety", "Piety" },
        { "CP", "Craftsman's Cunning" },
        { "Critical Hit", "Savage Aim" },
        { "Gathering", "Gatherer's Guerdon" },
    };

    private List<SubmarineDrop> Process()
    {
        List<SubmarineDrop> submarineDrops = new();

        var reader = CSVFile.CSVReader.FromFile(Path.Combine( "ManualData","SubmarineUnlocks.csv"));

        foreach( var line in reader.Lines() )
        {
            var sector = line[ 0 ];
            var items = line[ 3 ] + "," + line[ 4 ] + "," + line[ 5 ];
            items = items.ReplaceLineEndings(",");

            sector = sector.ToParseable();
            if( submarinesByName.ContainsKey( sector ) )
            {
                var actualSector = this.submarineExplorationSheet.GetRow(submarinesByName[ sector ]);

                var items1List = items.Split( "," );
                foreach( var tempName in items1List )
                {
                    var itemName = tempName;
                    if (MateriaNames.Any(c => itemName.Contains(c.Key + " ")))
                    {
                        var nameMap = MateriaNames.First(c => itemName.Contains(c.Key + " "));
                        itemName = itemName.Replace(nameMap.Key, nameMap.Value + " Materia").Replace("Materia Materia", "Materia");//whyyyy
                    }
                    var parseableItemName = itemName.Trim().ToParseable();
                    Item? outputItem = itemsByName.ContainsKey( parseableItemName ) ? this.itemSheet.GetRow(itemsByName[ parseableItemName ]) : null;
                    if( outputItem != null )
                    {
                        submarineDrops.Add( new SubmarineDrop()
                        {
                            SubmarineExplorationId = actualSector.RowId,
                            ItemId = outputItem.Value.RowId
                        });
                    }
                    else
                    {
                        Console.WriteLine("Could not find item with name " + itemName.Trim() + " in the sector " + actualSector.Destination.ToString());
                    }
                }
            }
            else
            {
                Console.WriteLine("Could not find the submarine point with name " + sector);
            }
        }

        return submarineDrops;
    }
}
