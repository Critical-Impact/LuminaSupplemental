using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

using CSVFile;

using Lumina.Excel;
using Lumina.Excel.Sheets;

using LuminaSupplemental.Excel.Model;
using LuminaSupplemental.SpaghettiGenerator.Extensions;
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


    public override List<ICsv> Run(Dictionary<Type, List<ICsv>> stepData)
    {
        List<ItemPatch> items = new ();
        items.AddRange(this.Process());

        return [..items.Select(c => c).OrderBy(c => c.PatchNo).ThenBy(c => c.StartItemId).ThenBy(c => c.EndItemId)];
    }



    private List<ItemPatch> Process()
    {
        List<ItemPatch> itemPatches = new();
        var patchText = File.ReadAllText( @"Item.json" );
        var patchListText = File.ReadAllText( @"patchlist.json" );
        var patchList = JsonConvert.DeserializeObject<List<PatchListItem>>(patchListText)!;
        var patchMap = patchList.ToDictionary(c => c.ID, c => decimal.Parse(c.Version.GetNumbers()));

        var unmappedIds = JsonConvert.DeserializeObject<Dictionary<uint, uint>>(patchText);
        var unsortedIds = unmappedIds!.ToDictionary(c => c.Key, c => patchMap.TryGetValue(c.Value, out var v) ? v : c.Value);

        var orderedPatches = unsortedIds.OrderBy( c => c.Key );
        decimal? currentPatch = null;
        uint? firstItem = null;
        uint? lastItem = null;
        var patchData = new List< (decimal, uint, uint) >();
        foreach( var orderedPatch in orderedPatches )
        {
            if( currentPatch == null )
            {
                currentPatch = orderedPatch.Value;
            }

            if( firstItem == null )
            {
                firstItem = orderedPatch.Key;
            }


            if( currentPatch != orderedPatch.Value && lastItem != null)
            {
                patchData.Add( (currentPatch.Value, firstItem.Value, lastItem.Value) );
                firstItem = orderedPatch.Key;
                currentPatch = orderedPatch.Value;
            }

            lastItem = orderedPatch.Key;

        }
        if( currentPatch != null && firstItem != null && lastItem != null)
        {
            patchData.Add( (currentPatch.Value, firstItem.Value, lastItem.Value) );
        }
        foreach( var patch in patchData )
        {
            itemPatches.Add( new ItemPatch(patch.Item2, patch.Item3, patch.Item1) );
        }

        return itemPatches;
    }
}
