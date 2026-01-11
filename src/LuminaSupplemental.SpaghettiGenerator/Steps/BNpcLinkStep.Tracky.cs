using System.Collections.Generic;
using System.IO;

using LuminaSupplemental.Excel.Model;

using Newtonsoft.Json;

using SupabaseExporter.Processing.BnpcPairs;
using SupabaseExporter.Structures.Exports;

namespace LuminaSupplemental.SpaghettiGenerator.Steps;

public partial class BNpcLinkStep
{
    protected List<BNpcLink> ProcessTrackyData()
    {
        var filePath = "../../../../FFXIVGachaSpreadsheet/website/static/data/BnpcPairsSimple.json";

        var npcLinks = new List<BNpcLink>();
        var json = File.ReadAllText(filePath);
        var simplePairs = JsonConvert.DeserializeObject<List<BnpcSimple>>(json)!;
        foreach (var pairing in simplePairs)
        {
            foreach (var name in pairing.Names)
            {
                npcLinks.Add(new BNpcLink(name, pairing.Base));
            }
        }

        return npcLinks;
    }

}
