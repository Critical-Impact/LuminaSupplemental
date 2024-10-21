using System;
using System.Collections.Generic;
using System.Linq;

using CSVFile;

using Lumina.Excel;
using Lumina.Excel.Sheets;

using LuminaSupplemental.Excel.Model;
using LuminaSupplemental.SpaghettiGenerator.Generator;

namespace LuminaSupplemental.SpaghettiGenerator.Steps;

public partial class DungeonDropStep : GeneratorStep
{
    private readonly DataCacher dataCacher;
    private readonly ExcelSheet<Item> itemSheet;
    private readonly ExcelSheet<ContentFinderCondition> contentFinderConditionSheet;
    private readonly Dictionary<string,uint> itemsByName;
    private readonly Dictionary<string,uint> dutiesByName;

    public override Type OutputType => typeof(DungeonDrop);

    public override string FileName => "DungeonDrop.csv";

    public override string Name => "Dungeon Drops";


    public DungeonDropStep(DataCacher dataCacher, ExcelSheet<Item> itemSheet, ExcelSheet<ContentFinderCondition> contentFinderConditionSheet)
    {
        this.dataCacher = dataCacher;
        this.itemSheet = itemSheet;
        this.contentFinderConditionSheet = contentFinderConditionSheet;
        var bannedItems = new HashSet< uint >()
        {
            0,
            24225
        };
        this.itemsByName = this.dataCacher.ByName<Item>(item => item.Name.ToString().ToParseable(), item => !bannedItems.Contains(item.RowId));
        this.dutiesByName = this.dataCacher.ByName<ContentFinderCondition>(item => item.Name.ToString().ToParseable(), item => !bannedItems.Contains(item.RowId));
    }


    public override List<ICsv> Run()
    {
        List<DungeonDrop> items = new ();
        items.AddRange(this.Process());
        for (var index = 0; index < items.Count; index++)
        {
            var item = items[index];
            item.RowId = (uint)(index + 1);
        }

        return [..items.Select(c => c)];
    }

    private List<DungeonDrop> Process()
    {
        List<DungeonDrop> dungeonDrops = new();

        var reader = CSVFile.CSVReader.FromFile(@"FFXIV Data - Items.tsv", CSVSettings.TSV);

        foreach (var line in reader.Lines())
        {
            var outputItemId = line[0];
            var method = line[1];
            if (method == "")
            {
                continue;
            }
            var sources = new List<string>();
            for (var i = 2; i < 13; i++)
            {
                if (line[i] != "")
                {
                    sources.Add(line[i]);
                }
            }

            ItemSupplementSource? source;
            switch (method)
            {
                case "Instance":
                    GenerateDungeonDrops( outputItemId, sources, dungeonDrops );
                    break;
            }


        }

        return dungeonDrops;
    }

    private void GenerateDungeonDrops( string outputItemId, List< string > sources, List< DungeonDrop > dungeonDrops )
    {
        outputItemId = outputItemId.ToParseable();
        Item? outputItem = itemsByName.ContainsKey( outputItemId ) ? this.itemSheet.GetRow(itemsByName[ outputItemId ]) : null;
        if( outputItem != null )
        {
            foreach( var sourceItem in sources )
            {
                var sourceName = sourceItem.ToParseable();
                ContentFinderCondition? duty = dutiesByName.ContainsKey( sourceName ) ? this.contentFinderConditionSheet.GetRow(dutiesByName[ sourceName ]) : null;
                if( duty != null )
                {
                    dungeonDrops.Add( new DungeonDrop( (uint)dungeonDrops.Count + 1, outputItem.Value.RowId, duty.Value.RowId ) );
                }
                else
                {
                    Console.WriteLine( "Could not find a match for input item: " + outputItemId + " and duty " + sourceName );
                }
            }
        }
        else
        {
            Console.WriteLine( "Could not find a match for output item: " + outputItemId );
        }
    }

}
