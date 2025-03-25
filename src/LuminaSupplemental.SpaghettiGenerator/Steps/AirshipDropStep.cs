using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Lumina.Excel;
using Lumina.Excel.Sheets;

using LuminaSupplemental.Excel.Model;
using LuminaSupplemental.SpaghettiGenerator.Generator;
using LuminaSupplemental.SpaghettiGenerator.Steps.Parsers;

using Serilog;

namespace LuminaSupplemental.SpaghettiGenerator.Steps;

public partial class AirshipDropStep : GeneratorStep
{
    private readonly DataCacher dataCacher;
    private readonly GubalApi gubalApi;
    private readonly ExcelSheet<AirshipExplorationPoint> airshipExplorationPointSheet;
    private readonly ExcelSheet<Item> itemSheet;
    private readonly ILogger logger;
    private readonly Dictionary<string,uint> airshipsByName;
    private readonly Dictionary<string,uint> itemsByName;

    public override Type OutputType => typeof(AirshipDrop);

    public override string FileName => "AirshipDrop.csv";

    public override string Name => "Airship Drops";


    public AirshipDropStep(DataCacher dataCacher, GubalApi gubalApi, ExcelSheet<AirshipExplorationPoint> airshipExplorationPointSheet, ExcelSheet<Item> itemSheet, ILogger logger)
    {
        this.dataCacher = dataCacher;
        this.gubalApi = gubalApi;
        this.airshipExplorationPointSheet = airshipExplorationPointSheet;
        this.itemSheet = itemSheet;
        this.logger = logger;
        var bannedItems = new HashSet< uint >()
        {
            0,
            24225
        };
        this.airshipsByName = this.dataCacher.ByName<AirshipExplorationPoint>(item => item.NameShort.ToString().ToParseable());
        this.itemsByName = this.dataCacher.ByName<Item>(item => item.Name.ToString().ToParseable(), item => !bannedItems.Contains(item.RowId));
    }


    public override List<ICsv> Run()
    {
        List<AirshipDrop> items = new();
        items.AddRange(this.Process());
        items.AddRange(this.ProcessGubalData());
        items = items.DistinctBy(c => (c.ItemId, c.AirshipExplorationPointId)).ToList();
        return [..items.Select(c => c).OrderBy(c => c.AirshipExplorationPointId)];
    }

    private List<AirshipDrop> Process()
    {
        List<AirshipDrop> airshipDrops = new();

        var reader = CSVFile.CSVReader.FromFile(Path.Combine("ManualData", "AirshipUnlocks.csv"));

        foreach (var line in reader.Lines())
        {
            var sector = line[0];
            var items = line[4] + "," + line[5];

            sector = "Sea of Clouds " + $"{int.Parse(sector):D2}";
            sector = sector.ToParseable();
            //Sectors are stored as numbers
            if (airshipsByName.ContainsKey(sector))
            {
                var actualSector = airshipExplorationPointSheet.GetRow(airshipsByName[sector]);

                var items1List = items.Split(",");
                foreach (var itemName in items1List)
                {
                    var parseableItemName = itemName.Trim().ToParseable();
                    Item? outputItem = this.itemsByName.TryGetValue(parseableItemName, out var value) ? itemSheet.GetRow(value) : null;
                    if (outputItem != null)
                    {
                        airshipDrops.Add(
                            new AirshipDrop()
                            {
                                AirshipExplorationPointId = actualSector.RowId,
                                ItemId = outputItem.Value.RowId
                            });
                    }
                    else
                    {
                        Console.WriteLine("Could not find item with name " + itemName.Trim() + " in the sector " + actualSector.NameShort);
                    }
                }
            }
            else
            {
                Console.WriteLine("Could not find the airship point with name " + sector);
            }
        }

        return airshipDrops;
    }
}
