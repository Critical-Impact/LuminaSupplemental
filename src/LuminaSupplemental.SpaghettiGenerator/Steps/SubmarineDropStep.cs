using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Lumina.Excel.GeneratedSheets;

using LuminaSupplemental.Excel.Model;
using LuminaSupplemental.SpaghettiGenerator.Generator;

namespace LuminaSupplemental.SpaghettiGenerator.Steps;

public partial class SubmarineDropStep : GeneratorStep
{
    private readonly DataCacher dataCacher;
    private readonly Dictionary<string,SubmarineExploration> submarinesByName;
    private readonly Dictionary<string,Item> itemsByName;

    public override Type OutputType => typeof(SubmarineDrop);

    public override string FileName => "SubmarineDrop.csv";

    public override string Name => "Submarine Drops";
    
    public SubmarineDropStep(DataCacher dataCacher)
    {
        this.dataCacher = dataCacher;
        var bannedItems = new HashSet< uint >()
        {
            0,
            24225
        };
        this.submarinesByName = this.dataCacher.ByName<SubmarineExploration>(item => item.Destination.ToString().ToParseable());
        this.itemsByName = this.dataCacher.ByName<Item>(item => item.Name.ToString().ToParseable(), item => !bannedItems.Contains(item.RowId));
    }


    public override List<ICsv> Run()
    {
        List<SubmarineDrop> items = new ();
        items.AddRange(this.Process());
        for (var index = 0; index < items.Count; index++)
        {
            var item = items[index];
            item.RowId = (uint)(index + 1);
        }

        return [..items.Select(c => c)];
    }
    
    private List<SubmarineDrop> Process()
    {
        List<SubmarineDrop> submarineDrops = new();
        
        var reader = CSVFile.CSVReader.FromFile(Path.Combine( "ManualData","SubmarineUnlocks.csv"));

        foreach( var line in reader.Lines() )
        {
            var sector = line[ 0 ];
            var items = line[ 3 ] + "," + line[ 4 ];
            
            sector = sector.ToParseable();
            if( submarinesByName.ContainsKey( sector ) )
            {
                var actualSector = submarinesByName[ sector ];

                var items1List = items.Split( "," );
                foreach( var itemName in items1List )
                {
                    var parseableItemName = itemName.Trim().ToParseable();
                    var outputItem = itemsByName.ContainsKey( parseableItemName ) ? itemsByName[ parseableItemName ] : null;
                    if( outputItem != null )
                    {
                        submarineDrops.Add( new SubmarineDrop()
                        {
                            RowId = (uint)(submarineDrops.Count + 1),
                            SubmarineExplorationId = actualSector.RowId,
                            ItemId = outputItem.RowId
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
