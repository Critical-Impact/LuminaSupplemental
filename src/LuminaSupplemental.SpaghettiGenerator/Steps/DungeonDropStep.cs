using System;
using System.Collections.Generic;
using System.IO;
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


    public override List<ICsv> Run(Dictionary<Type, List<ICsv>> stepData)
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

        var reader = CSVFile.CSVReader.FromFile(Path.Join("ManualData", "FFXIV Data - Items.tsv"), CSVSettings.TSV);

        foreach (var line in reader.Lines())
        {
            if (line.Length < 2)
            {
                continue;
            }
            var outputItemId = line[0];
            var method = line[1];
            if (method == "")
            {
                continue;
            }
            var sources = new List<string>();
            for (var i = 2; i < 13; i++)
            {
                if (i >= 0 && i < line.Length && line[i] != "")
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

        this.ProcessDungeonDropsCSV(dungeonDrops);

        return dungeonDrops;
    }

    private void ProcessDungeonDropsCSV(List<DungeonDrop> dungeonDrops)
    {
        var csvPath = Path.Join("ManualData", "DungeonDrops.csv");
        if (!File.Exists(csvPath))
        {
            return;
        }

        var reader = CSVFile.CSVReader.FromFile(csvPath);
        bool isFirstLine = true;

        foreach (var line in reader.Lines())
        {
            if (isFirstLine)
            {
                isFirstLine = false;
                continue;
            }

            if (line.Length < 2 || string.IsNullOrWhiteSpace(line[0]) || string.IsNullOrWhiteSpace(line[1]))
            {
                continue;
            }

            var itemName = line[0].ToParseable();
            var dungeonName = line[1].ToParseable();

            Item? outputItem = itemsByName.ContainsKey(itemName) ? this.itemSheet.GetRow(itemsByName[itemName]) : null;
            ContentFinderCondition? duty = dutiesByName.ContainsKey(dungeonName) ? this.contentFinderConditionSheet.GetRow(dutiesByName[dungeonName]) : null;

            if (outputItem == null)
            {
                Console.WriteLine($"DungeonDrops.csv: Could not find a match for item: {line[0]}");
                continue;
            }

            if (duty == null)
            {
                Console.WriteLine($"DungeonDrops.csv: Could not find a match for dungeon: {line[1]} (item: {line[0]})");
                continue;
            }

            dungeonDrops.Add(new DungeonDrop((uint)dungeonDrops.Count + 1, outputItem.Value.RowId, duty.Value.RowId));
        }
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
