using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Lumina.Excel;
using Lumina.Excel.Sheets;

using LuminaSupplemental.Excel.Model;
using LuminaSupplemental.SpaghettiGenerator.Generator;

using Newtonsoft.Json;

using SupabaseExporter.Structures.Exports;

namespace LuminaSupplemental.SpaghettiGenerator.Steps;

public partial class RetainerVentureItemStep : GeneratorStep
{
    private readonly DataCacher dataCacher;
    private readonly ExcelSheet<Item> itemSheet;
    private readonly ExcelSheet<RetainerTaskRandom> retainerTaskRandomSheet;
    private readonly ExcelSheet<RetainerTask> retainerTaskSheet;
    private readonly Dictionary<string,uint> itemsByName;
    private readonly Dictionary<string,uint> retainerTaskRandomByName;

    public override Type OutputType => typeof(RetainerVentureItem);

    public override string FileName => "RetainerVentureItem.csv";

    public override string Name => "Retainer Venture Items";


    public RetainerVentureItemStep(DataCacher dataCacher, ExcelSheet<Item> itemSheet, ExcelSheet<RetainerTaskRandom> retainerTaskRandomSheet, ExcelSheet<RetainerTask> retainerTaskSheet)
    {
        this.dataCacher = dataCacher;
        this.itemSheet = itemSheet;
        this.retainerTaskRandomSheet = retainerTaskRandomSheet;
        this.retainerTaskSheet = retainerTaskSheet;
        var bannedItems = new HashSet< uint >()
        {
            0,
            24225
        };
        this.itemsByName = this.dataCacher.ByName<Item>(item => item.Name.ToString().ToParseable(), item => !bannedItems.Contains(item.RowId));
        this.retainerTaskRandomByName = this.dataCacher.ByName<RetainerTaskRandom>(item => item.Name.ToString().ToParseable());
    }


    public override List<ICsv> Run(Dictionary<Type, List<ICsv>> stepData)
    {
        List<RetainerVentureItem> items = new ();
        items.AddRange(this.ProcessTrackyData());
        items.AddRange(this.Process());
        // Tracky rows come first so they keep their drop rates when a manual row matches
        items = items.DistinctBy(c => (c.ItemId, c.RetainerTaskRandomId)).ToList();

        return [..items.Select(c => c).OrderBy(c => c.RetainerTaskRandomId).ThenBy(c => c.ItemId)];
    }

    private List<RetainerVentureItem> ProcessTrackyData()
    {
        var filePath = "../../../../FFXIVGachaSpreadsheet/website/static/data/Ventures.json";

        var json = File.ReadAllText(filePath);
        var ventures = JsonConvert.DeserializeObject<List<Venture>>(json)!;

        // Tasks are split by patch, these are merged into one rate per venture and item
        // Rates use 4 decimal places as many quick exploration items are below 0.01%
        var totals = new Dictionary<uint, long>();
        var rewards = new Dictionary<uint, List<Reward>>();
        foreach (var task in ventures.SelectMany(c => c.Tasks))
        {
            // The exporter adds pseudo tasks that split quick exploration by level, these have no matching retainer task
            if (!this.retainerTaskSheet.TryGetRow(task.Type, out var retainerTask) || !retainerTask.Task.TryGetValue<RetainerTaskRandom>(out var retainerTaskRandom))
            {
                continue;
            }

            var retainerTaskRandomId = retainerTaskRandom.RowId;
            totals[retainerTaskRandomId] = totals.GetValueOrDefault(retainerTaskRandomId) + task.Patches.Values.Sum(c => (long)c.Total);
            if (!rewards.TryGetValue(retainerTaskRandomId, out var taskRewards))
            {
                taskRewards = new List<Reward>();
                rewards[retainerTaskRandomId] = taskRewards;
            }

            taskRewards.AddRange(task.Patches.Values.SelectMany(c => c.Primaries.Concat(c.Secondaries)));
        }

        List<RetainerVentureItem> retainerVentureItems = new();
        foreach (var (retainerTaskRandomId, taskRewards) in rewards)
        {
            var total = totals[retainerTaskRandomId];
            if (total == 0)
            {
                continue;
            }

            foreach (var reward in taskRewards.Where(c => c.Id != 0).GroupBy(c => c.Id))
            {
                retainerVentureItems.Add(new RetainerVentureItem(
                    reward.Key,
                    retainerTaskRandomId,
                    (uint)reward.Min(c => c.Min),
                    (uint)reward.Max(c => c.Max),
                    (decimal)Math.Round(reward.Sum(c => c.Amount) / (double)total * 100, 4)));
            }
        }

        return retainerVentureItems;
    }

    private List<RetainerVentureItem> Process()
    {
        List<RetainerVentureItem> retainerVentureItems = new();

        var reader = CSVFile.CSVReader.FromFile(Path.Combine( "ManualData","RetainerVentures.csv"));

        foreach( var line in reader.Lines() )
        {
            var ventureName = line[ 0 ];
            var items = line[1].Split( "," );

            var parsedVentureName = ventureName.ToParseable();
            if( retainerTaskRandomByName.ContainsKey( parsedVentureName ) )
            {
                var retainerTaskRandom = this.retainerTaskRandomSheet.GetRow(retainerTaskRandomByName[ parsedVentureName ]);

                foreach( var itemName in items )
                {
                    var parseableItemName = itemName.Trim().ToParseable();
                    Item? outputItem = itemsByName.ContainsKey( parseableItemName ) ? this.itemSheet.GetRow(itemsByName[ parseableItemName ]) : null;
                    if( outputItem != null )
                    {
                        retainerVentureItems.Add( new RetainerVentureItem()
                        {
                            RetainerTaskRandomId = retainerTaskRandom.RowId,
                            ItemId = outputItem.Value.RowId
                        });
                    }
                    else
                    {
                        Console.WriteLine("Could not find item with name " + itemName.Trim() + " in the retainer task random with name " + ventureName);
                    }
                }
            }
            else
            {
                Console.WriteLine("Could not find the retainer task random with name " + ventureName);
            }
        }

        return retainerVentureItems;
    }
}
