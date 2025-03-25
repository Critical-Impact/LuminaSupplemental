using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Lumina.Excel;
using Lumina.Excel.Sheets;

using LuminaSupplemental.Excel.Model;
using LuminaSupplemental.SpaghettiGenerator.Generator;

namespace LuminaSupplemental.SpaghettiGenerator.Steps;

public partial class AirshipUnlockStep : GeneratorStep
{
    private readonly DataCacher dataCacher;
    private readonly ExcelSheet<AirshipExplorationPoint> airshipExplorationPointSheet;
    private readonly Dictionary<string,uint> airshipsByName;

    public override Type OutputType => typeof(AirshipUnlock);

    public override string FileName => "AirshipUnlock.csv";

    public override string Name => "Airship Unlocks";


    public AirshipUnlockStep(DataCacher dataCacher, ExcelSheet<AirshipExplorationPoint> airshipExplorationPointSheet)
    {
        this.dataCacher = dataCacher;
        this.airshipExplorationPointSheet = airshipExplorationPointSheet;
        var bannedItems = new HashSet< uint >()
        {
            0,
            24225
        };
        this.airshipsByName = this.dataCacher.ByName<AirshipExplorationPoint>(item => item.NameShort.ToString().ToParseable());
    }


    public override List<ICsv> Run()
    {
        List<AirshipUnlock> items = new ();
        items.AddRange(this.Process());
        return [..items.Select(c => c).OrderBy(c => c.AirshipExplorationPointId).ThenBy(c => c.AirshipExplorationPointUnlockId)];
    }

    private List<AirshipUnlock> Process()
    {
        List<AirshipUnlock> airshipUnlocks = new();

        var reader = CSVFile.CSVReader.FromFile(Path.Combine("ManualData", "AirshipUnlocks.csv"));

        foreach (var line in reader.Lines())
        {
            var sector = line[0];
            var unlockSector = line[1];
            var rankRequired = uint.Parse(line[2]);
            var surveillanceRequired = line[3];

            sector = "Sea of Clouds " + $"{int.Parse(sector):D2}";
            unlockSector = unlockSector != "" ? "Sea of Clouds " + $"{int.Parse(unlockSector):D2}" : "";
            sector = sector.ToParseable();
            unlockSector = unlockSector.ToParseable();
            //Sectors are stored as numbers
            if (airshipsByName.ContainsKey(sector))
            {
                var actualSector = airshipExplorationPointSheet.GetRow(airshipsByName[sector]);
                AirshipExplorationPoint? actualUnlockSector = null;
                if (airshipsByName.ContainsKey(unlockSector))
                {
                    actualUnlockSector = airshipExplorationPointSheet.GetRow(airshipsByName[unlockSector]);
                }

                var actualSurveillanceRequired = uint.Parse(surveillanceRequired);
                airshipUnlocks.Add(
                    new AirshipUnlock()
                    {
                        AirshipExplorationPointId = actualSector.RowId,
                        AirshipExplorationPointUnlockId = actualUnlockSector?.RowId ?? 0,
                        SurveillanceRequired = actualSurveillanceRequired,
                        RankRequired = rankRequired
                    });
            }
            else
            {
                Console.WriteLine("Could not find the airship point with name " + sector);
            }
        }

        return airshipUnlocks;
    }
}
