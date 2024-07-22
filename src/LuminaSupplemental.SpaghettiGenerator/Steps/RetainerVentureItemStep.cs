using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Lumina.Excel.GeneratedSheets;

using LuminaSupplemental.Excel.Model;
using LuminaSupplemental.SpaghettiGenerator.Generator;

namespace LuminaSupplemental.SpaghettiGenerator.Steps;

public partial class RetainerVentureItemStep : GeneratorStep
{
    private readonly DataCacher dataCacher;
    private readonly Dictionary<string,Item> itemsByName;
    private readonly Dictionary<string,RetainerTaskRandom> retainerTaskRandomByName;

    public override Type OutputType => typeof(RetainerVentureItem);

    public override string FileName => "RetainerVentureItem.csv";

    public override string Name => "Retainer Venture Items";
    

    public RetainerVentureItemStep(DataCacher dataCacher)
    {
        this.dataCacher = dataCacher;
        var bannedItems = new HashSet< uint >()
        {
            0,
            24225
        };
        this.itemsByName = this.dataCacher.ByName<Item>(item => item.Name.ToString().ToParseable(), item => !bannedItems.Contains(item.RowId));
        this.retainerTaskRandomByName = this.dataCacher.ByName<RetainerTaskRandom>(item => item.Name.ToString().ToParseable());
    }


    public override List<ICsv> Run()
    {
        List<RetainerVentureItem> items = new ();
        items.AddRange(this.Process());
        for (var index = 0; index < items.Count; index++)
        {
            var item = items[index];
            item.RowId = (uint)(index + 1);
        }

        return [..items.Select(c => c)];
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
                var retainerTaskRandom = retainerTaskRandomByName[ parsedVentureName ];

                foreach( var itemName in items )
                {
                    var parseableItemName = itemName.Trim().ToParseable();
                    var outputItem = itemsByName.ContainsKey( parseableItemName ) ? itemsByName[ parseableItemName ] : null;
                    if( outputItem != null )
                    {
                        retainerVentureItems.Add( new RetainerVentureItem()
                        {
                            RowId = (uint)(retainerVentureItems.Count + 1),
                            RetainerTaskRandomId = retainerTaskRandom.RowId,
                            ItemId = outputItem.RowId
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
