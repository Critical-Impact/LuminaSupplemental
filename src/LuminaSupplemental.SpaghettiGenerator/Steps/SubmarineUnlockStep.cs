using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Lumina;
using Lumina.Excel;
using Lumina.Excel.Sheets;

using LuminaSupplemental.Excel.Model;
using LuminaSupplemental.SpaghettiGenerator.Generator;

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



    public override List<ICsv> Run()
    {
        List<SubmarineUnlock> items = new ();
        items.AddRange(this.Process());
        for (var index = 0; index < items.Count; index++)
        {
            var item = items[index];
            item.RowId = (uint)(index + 1);
        }

        return [..items.Select(c => c)];
    }

    private List<SubmarineUnlock> Process()
    {
        List<SubmarineUnlock> submarineUnlocks = new();


        var reader = CSVFile.CSVReader.FromFile(Path.Combine( "ManualData","SubmarineUnlocks.csv"));

        foreach( var line in reader.Lines() )
        {
            var sector = line[ 0 ];
            var unlockSector = line[ 1 ];
            var rankRequired = uint.Parse(line[ 2 ]);

            sector = sector.ToParseable();
            unlockSector = unlockSector.ToParseable();
            if( submarinesByName.ContainsKey( sector ) )
            {
                var actualSector =  this.submarineExplorationSheet.GetRow(submarinesByName[ sector ]);
                SubmarineExploration? actualUnlockSector = null;
                if( submarinesByName.ContainsKey( unlockSector ) )
                {
                    actualUnlockSector = this.submarineExplorationSheet.GetRow(submarinesByName[ unlockSector ]);
                }

                submarineUnlocks.Add(new SubmarineUnlock()
                {
                    RowId = (uint)( submarineUnlocks.Count + 1 ),
                    SubmarineExplorationId = actualSector.RowId,
                    SubmarineExplorationUnlockId = actualUnlockSector?.RowId ?? 0,
                    RankRequired = rankRequired
                });
            }
            else
            {
                Console.WriteLine("Could not find the submarine point with name " + sector);
            }
        }

        return submarineUnlocks;
    }
}
