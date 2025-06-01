using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Lumina.Excel;
using Lumina.Excel.Sheets;

using LuminaSupplemental.Excel.Model;
using LuminaSupplemental.SpaghettiGenerator.Generator;

using Newtonsoft.Json;

using Serilog;

using SupabaseExporter.Structures;

namespace LuminaSupplemental.SpaghettiGenerator.Steps;

public class FieldOpCofferStep : GeneratorStep
{
    private readonly ExcelSheet<Item> itemSheet;
    private readonly ILogger logger;
    private readonly DataCacher dataCacher;
    private readonly Dictionary<string,uint> itemsByName;

    public override Type OutputType => typeof(FieldOpCoffer);

    public override string FileName => "FieldOpCoffer.csv";

    public override string Name => "Field Operation Coffers";

    public FieldOpCofferStep(ExcelSheet<Item> itemSheet, ILogger logger, DataCacher dataCacher)
    {
        this.itemSheet = itemSheet;
        this.logger = logger;
        this.dataCacher = dataCacher;
    }

    public override List<ICsv> Run()
    {
        var fieldOpCoffers = new List<FieldOpCoffer>();
        fieldOpCoffers.AddRange(this.ProcessData());
        fieldOpCoffers.AddRange(this.ProcessOccult());

        return fieldOpCoffers.OrderBy(c => c.Type).ThenBy(c => c.CofferType).ThenBy(c => c.ItemId).Cast<ICsv>().ToList();
    }

    private List<FieldOpCoffer> ProcessOccult()
    {
        var filePath = "../../../../FFXIVGachaSpreadsheet/Website/assets/data/Occult.json";

        var drops = new List<FieldOpCoffer>();
        var json = File.ReadAllText(filePath);
        var cofferDataList = JsonConvert.DeserializeObject<List<CofferData>>(json);

        for (var index = 0; index < cofferDataList.Count; index++)
        {
            FieldOpType fieldOpType;
            var cofferData = cofferDataList[index];
            switch (index)
            {
                case 0:
                    fieldOpType = FieldOpType.OccultTreasure;
                    break;
                case 1:
                    fieldOpType = FieldOpType.OccultPot;
                    break;
                case 2:
                    fieldOpType = FieldOpType.OccultGoldenCoffer;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();

            }
            foreach (var coffer in cofferData.Coffers)
            {
                if(!Enum.TryParse(coffer.CofferName, out FieldOpCofferType chest))
                {
                    throw new Exception("Could not parse coffer: " + coffer.CofferName);
                }
                var cofferContent = coffer.Patches["All"];
                foreach (var item in cofferContent.Items)
                {
                    if (item.Id == 0)
                    {
                        continue;
                    }

                    var min = item.Min;
                    var max = item.Max;
                    var probability = Math.Round(item.Percentage * 100, 2);

                    drops.Add(
                        new FieldOpCoffer()
                        {
                            ItemId = item.Id,
                            Type = fieldOpType,
                            CofferType = chest,
                            Min = (uint?)min,
                            Max = (uint?)max,
                            Probability = (decimal?)probability
                        });
                }
            }
        }

        return drops;
    }

    private List<FieldOpCoffer> ProcessData()
    {
        var filePath = "../../../../FFXIVGachaSpreadsheet/Website/assets/data/BunnyData.json";

        var drops = new List<FieldOpCoffer>();
        var json = File.ReadAllText(filePath);
        var cofferDataList = JsonConvert.DeserializeObject<List<CofferData>>(json);

        for (var index = 0; index < cofferDataList.Count; index++)
        {
            FieldOpType fieldOpType;
            var cofferData = cofferDataList[index];
            switch (index)
            {
                case 0:
                    fieldOpType = FieldOpType.Pagos;
                    break;
                case 1:
                    fieldOpType = FieldOpType.Pyros;
                    break;
                case 2:
                    fieldOpType = FieldOpType.Hydatos;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();

            }
            foreach (var coffer in cofferData.Coffers)
            {
                if(!Enum.TryParse(coffer.CofferName, out FieldOpCofferType chest))
                {
                    throw new Exception("Could not parse coffer: " + coffer.CofferName);
                }
                var cofferContent = coffer.Patches["All"];
                foreach (var item in cofferContent.Items)
                {
                    if (item.Id == 0)
                    {
                        continue;
                    }

                    var min = item.Min;
                    var max = item.Max;
                    var probability = Math.Round(item.Percentage * 100, 2);

                    drops.Add(
                        new FieldOpCoffer()
                        {
                            ItemId = item.Id,
                            Type = fieldOpType,
                            CofferType = chest,
                            Min = (uint?)min,
                            Max = (uint?)max,
                            Probability = (decimal?)probability
                        });
                }
            }
        }

        return drops;
    }
}
