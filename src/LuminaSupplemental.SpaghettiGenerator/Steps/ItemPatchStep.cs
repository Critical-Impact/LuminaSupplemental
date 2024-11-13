using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

using CSVFile;

using Lumina.Excel;
using Lumina.Excel.Sheets;

using LuminaSupplemental.Excel.Model;
using LuminaSupplemental.SpaghettiGenerator.Generator;
using LuminaSupplemental.SpaghettiGenerator.Model;

using Newtonsoft.Json;

namespace LuminaSupplemental.SpaghettiGenerator.Steps;

public partial class ItemPatchStep : GeneratorStep
{
    private readonly ExcelSheet<Item> itemSheet;

    public override Type OutputType => typeof(ItemPatch);

    public override string FileName => "ItemPatch.csv";

    public override string Name => "Item Patches";


    public ItemPatchStep(ExcelSheet<Item> itemSheet)
    {
        this.itemSheet = itemSheet;
    }


    public override List<ICsv> Run()
    {
        List<ItemPatch> items = new ();
        items.AddRange(this.Process());
        for (var index = 0; index < items.Count; index++)
        {
            var item = items[index];
            item.RowId = (uint)(index + 1);
        }

        return [..items.Select(c => c)];
    }

    private List<ItemPatch> Process()
    {
        List<ItemPatch> itemPatches = new();
        var patchText = File.ReadAllText( @"patches.json" );
        var patches = JsonConvert.DeserializeObject<PatchJson[]>(patchText)!.ToList();
        var knownItemIds = patches.Where( c => c.Type == "item" ).Select( c => c.Id ).Distinct().ToHashSet();

        //Process extra data not in GT's jsons(no idea why)

        var reader = CSVFile.CSVReader.FromFile(Path.Combine( "ManualData","ExtraPatchData.csv"), CSVSettings.CSV);

        foreach( var line in reader.Lines() )
        {
            var sourceItemId = uint.Parse(line[ 0 ]);
            if( knownItemIds.Contains( sourceItemId ) )
            {
                continue;
            }
            var patchId = decimal.Parse(line[ 1 ], CultureInfo.InvariantCulture);
            patches.Add( new PatchJson() {Patch = patchId, Id = sourceItemId, Type = "item"} );
        }
        var patchWhereItemKnown = patches.Select( c => c.Id ).Distinct().ToHashSet();
        var unknownPatchItems = itemSheet.Where( c => !patchWhereItemKnown.Contains( c.RowId ) ).ToList();
        foreach( var unknownPatchItem in unknownPatchItems )
        {
            patches.Add( new PatchJson() {Patch = new(9.9), Id = unknownPatchItem.RowId, Type = "item"} );
            Console.WriteLine(unknownPatchItem.RowId);
        }
        var orderedPatches = patches.Where( c => c.Type == "item" ).OrderBy( c => c.Id );
        decimal? currentPatch = null;
        uint? firstItem = null;
        uint? lastItem = null;
        var patchData = new List< (decimal, uint, uint) >();
        foreach( var orderedPatch in orderedPatches )
        {
            if( currentPatch == null )
            {
                currentPatch = orderedPatch.Patch;
            }

            if( firstItem == null )
            {
                firstItem = orderedPatch.Id;
            }


            if( currentPatch != orderedPatch.Patch && lastItem != null)
            {
                patchData.Add( (currentPatch.Value, firstItem.Value, lastItem.Value) );
                firstItem = orderedPatch.Id;
                currentPatch = orderedPatch.Patch;
            }

            lastItem = orderedPatch.Id;

        }
        if( currentPatch != null && firstItem != null && lastItem != null)
        {
            patchData.Add( (currentPatch.Value, firstItem.Value, lastItem.Value) );
        }
        foreach( var patch in patchData )
        {
            itemPatches.Add( new ItemPatch((uint)(itemPatches.Count + 1), patch.Item2, patch.Item3, patch.Item1) );
        }

        return itemPatches;
    }
}
