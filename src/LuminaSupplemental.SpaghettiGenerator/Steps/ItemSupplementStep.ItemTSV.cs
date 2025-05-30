using System.Collections.Generic;
using System.IO;

using CSVFile;

using LuminaSupplemental.Excel.Model;

namespace LuminaSupplemental.SpaghettiGenerator.Steps;

public partial class ItemSupplementStep
{
    private List<ItemSupplement> ProcessItemsTSV()
    {
        List<ItemSupplement> items = new List<ItemSupplement>();

        var reader = CSVFile.CSVReader.FromFile(Path.Join("ManualData", "FFXIV Data - Items.tsv"), CSVSettings.TSV);

        foreach (var line in reader.Lines())
        {
            if (line.Length < 2)
            {
                continue;
            }
            var outputItemId = line[0];
            var method = line[1];
            if (method == "")
            {
                continue;
            }

            var sources = new List<string>();
            for (var i = 2; i < 13; i++)
            {
                if (i >= 0 && i < line.Length && line[i] != "")
                {
                    sources.Add(line[i]);
                }
            }

            ItemSupplementSource? source;
            switch (method)
            {
                case "Desynth":
                    source = ItemSupplementSource.Desynth;
                    items.AddRange(GenerateItemSupplement(source, outputItemId, sources));
                    break;
                case "Reduce":
                    source = ItemSupplementSource.Reduction;
                    items.AddRange(GenerateItemSupplement(source, outputItemId, sources));
                    break;
                case "Loot":
                    source = ItemSupplementSource.Loot;
                    items.AddRange(GenerateItemSupplement(source, outputItemId, sources));
                    break;
                case "Gardening":
                    source = ItemSupplementSource.Gardening;
                    items.AddRange(GenerateItemSupplement(source, outputItemId, sources));
                    break;
            }
        }

        return items;
    }
}
