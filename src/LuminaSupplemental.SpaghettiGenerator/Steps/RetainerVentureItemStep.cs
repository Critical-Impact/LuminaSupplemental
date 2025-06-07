using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Lumina.Excel;
using Lumina.Excel.Sheets;

using LuminaSupplemental.Excel.Model;
using LuminaSupplemental.SpaghettiGenerator.Generator;

namespace LuminaSupplemental.SpaghettiGenerator.Steps;

public partial class RetainerVentureItemStep : GeneratorStep
{
    private readonly DataCacher dataCacher;
    private readonly ExcelSheet<Item> itemSheet;
    private readonly ExcelSheet<RetainerTaskRandom> retainerTaskRandomSheet;
    private readonly Dictionary<string,uint> itemsByName;
    private readonly Dictionary<string,uint> retainerTaskRandomByName;

    public override Type OutputType => typeof(RetainerVentureItem);

    public override string FileName => "RetainerVentureItem.csv";

    public override string Name => "Retainer Venture Items";


    public RetainerVentureItemStep(DataCacher dataCacher, ExcelSheet<Item> itemSheet, ExcelSheet<RetainerTaskRandom> retainerTaskRandomSheet)
    {
        this.dataCacher = dataCacher;
        this.itemSheet = itemSheet;
        this.retainerTaskRandomSheet = retainerTaskRandomSheet;
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
        items.AddRange(this.Process());

        return [..items.Select(c => c).OrderBy(c => c.RetainerTaskRandomId).ThenBy(c => c.ItemId)];
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
