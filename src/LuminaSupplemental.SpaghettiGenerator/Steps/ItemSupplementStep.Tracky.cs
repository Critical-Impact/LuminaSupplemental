using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using LuminaSupplemental.Excel.Model;

using Newtonsoft.Json;

using SupabaseExporter.Structures;
using SupabaseExporter.Structures.Exports;

namespace LuminaSupplemental.SpaghettiGenerator.Steps;

public partial class ItemSupplementStep
{
    private List<ItemSupplement> ProcessCards()
    {
        var supplementType = ItemSupplementSource.CardPacks;
        var filePath = "../../../../FFXIVGachaSpreadsheet/website/static/data/TripleTriadPacks.json";

        return ProcessCofferJson(filePath, supplementType);
    }

    private List<ItemSupplement> ProcessCoffers()
    {
        var supplementType = ItemSupplementSource.Coffer;
        var filePath = "../../../../FFXIVGachaSpreadsheet/website/static/data/RandomCoffers.json";

        return ProcessCofferJson(filePath, supplementType);
    }

    private List<ItemSupplement> ProcessLogograms()
    {
        var supplementType = ItemSupplementSource.Logogram;
        var filePath = "../../../../FFXIVGachaSpreadsheet/website/static/data/FieldOpContainers.json";
        //TODO: split into logograms and fragments

        return ProcessCofferJson(filePath, supplementType);
    }

    public List<Reward> CollectUniqueCoffersInReverse<TKey>(
        IDictionary<TKey, Coffer.Content> contents)
    {
        var result = new List<Reward>();
        var seenIds = new HashSet<uint>();

        var values = contents.Values.ToList();

        for (int i = values.Count - 1; i >= 0; i--)
        {
            var content = values[i];

            foreach (var item in content.Items)
            {
                if (seenIds.Add(item.Id))
                {
                    result.Add(item);
                }
            }
        }

        return result;
    }

    private List<ItemSupplement> ProcessDesynth()
    {
        var filePath = "../../../../FFXIVGachaSpreadsheet/website/static/data/Desynthesis.json";

        var drops = new List<ItemSupplement>();
        var json = File.ReadAllText(filePath);
        var desynthData = JsonConvert.DeserializeObject<Desynth>(json)!;
        var supplementType = ItemSupplementSource.Desynth;

        foreach(var history in desynthData.Sources)
        {
            var sourceItemId = history.Key;
            foreach (var coffer in history.Value.Rewards)
            {

                var itemId = coffer.Id;
                if (itemId == 0 || sourceItemId == 0)
                {
                    continue;
                }

                var min = coffer.Min;
                var max = coffer.Max;
                var probability = Math.Round(coffer.Pct * 100, 2);

                drops.Add(
                    new ItemSupplement()
                    {
                        ItemId = itemId,
                        SourceItemId = sourceItemId,
                        ItemSupplementSource = supplementType,
                        Min = (uint?)min,
                        Max = (uint?)max,
                        Probability = (decimal?)probability
                    });
            }
        }

        return drops;
    }

    private List<ItemSupplement> ProcessDeepDungeons()
    {
        var filePath = "../../../../FFXIVGachaSpreadsheet/website/static/data/DeepDungeonSacks.json";

        var drops = new List<ItemSupplement>();
        var json = File.ReadAllText(filePath);
        var cofferDataList = JsonConvert.DeserializeObject<List<Coffer>>(json)!;
        for (var index = 0; index < cofferDataList.Count; index++)
        {
            ItemSupplementSource supplementType;

            var cofferData = cofferDataList[index];
            switch (cofferData.Name)
            {
                case "Palace of the Dead":
                    supplementType = ItemSupplementSource.PalaceOfTheDead;
                    break;
                case "Heaven-on-High":
                    supplementType = ItemSupplementSource.HeavenOnHigh;
                    break;
                case "Eureka Orthos":
                    supplementType = ItemSupplementSource.EurekaOrthos;
                    break;
                case "Pilgrim's Traverse":
                    supplementType = ItemSupplementSource.PilgrimsTraverse;
                    break;
                default:
                    throw new Exception("Unhandled supplement type");
            }

            var coffers = cofferData.Variants;
            foreach (var coffer in coffers)
            {
                var sourceItemId = coffer.Id;
                foreach (var cofferItem in CollectUniqueCoffersInReverse(coffer.Patches))
                {
                    var itemId = cofferItem.Id;
                    if (itemId == 0 || sourceItemId == 0)
                    {
                        continue;
                    }

                    var min = cofferItem.Min;
                    var max = cofferItem.Max;
                    var probability = Math.Round(cofferItem.Pct * 100, 2);

                    drops.Add(
                        new ItemSupplement()
                        {
                            ItemId = itemId,
                            SourceItemId = sourceItemId,
                            ItemSupplementSource = supplementType,
                            Min = (uint?)min,
                            Max = (uint?)max,
                            Probability = (decimal?)probability
                        });
                }
            }
        }

        return drops;
    }

    private List<ItemSupplement> ProcessLockboxes()
    {
        var filePath = "../../../../FFXIVGachaSpreadsheet/website/static/data/FieldOpLockboxes.json";

        var drops = new List<ItemSupplement>();
        var json = File.ReadAllText(filePath);
        var cofferDataList = JsonConvert.DeserializeObject<List<Coffer>>(json)!;
        for (var index = 0; index < cofferDataList.Count; index++)
        {
            ItemSupplementSource supplementType;

            var cofferData = cofferDataList[index];
            switch (cofferData.Name)
            {
                case "Anemos":
                    supplementType = ItemSupplementSource.Anemos;
                    break;
                case "Pagos":
                    supplementType = ItemSupplementSource.Pagos;
                    break;
                case "Pyros":
                    supplementType = ItemSupplementSource.Pyros;
                    break;
                case "Hydatos":
                    supplementType = ItemSupplementSource.Hydatos;
                    break;
                case "Bozja":
                    supplementType = ItemSupplementSource.Bozja;
                    break;
                case "Oizys":
                    supplementType = ItemSupplementSource.Oizys;
                    break;
                default:
                    throw new Exception("Unhandled supplement type");
            }

            var coffers = cofferData.Variants;
            foreach (var coffer in coffers)
            {
                var sourceItemId = coffer.Id;
                foreach (var cofferItem in CollectUniqueCoffersInReverse(coffer.Patches))
                {
                    var itemId = cofferItem.Id;
                    if (itemId == 0 || sourceItemId == 0)
                    {
                        continue;
                    }

                    var min = cofferItem.Min;
                    var max = cofferItem.Max;
                    var probability = Math.Round(cofferItem.Pct * 100, 2);

                    drops.Add(
                        new ItemSupplement()
                        {
                            ItemId = itemId,
                            SourceItemId = sourceItemId,
                            ItemSupplementSource = supplementType,
                            Min = (uint?)min,
                            Max = (uint?)max,
                            Probability = (decimal?)probability
                        });
                }
            }
        }

        return drops;
    }

    private List<ItemSupplement> ProcessCofferJson(string filePath, ItemSupplementSource supplementType)
    {
        var drops = new List<ItemSupplement>();
        var cardJson = File.ReadAllText(filePath);
        var cofferDataList = JsonConvert.DeserializeObject<List<Coffer>>(cardJson);
        var coffers = cofferDataList.SelectMany(c => c.Variants);
        foreach (var coffer in coffers)
        {
            var sourceItemId = coffer.Id;
            foreach (var cofferItem in CollectUniqueCoffersInReverse(coffer.Patches))
            {
                var itemId = cofferItem.Id;
                if (itemId == 0 || sourceItemId == 0)
                {
                    continue;
                }

                var min = cofferItem.Min;
                var max = cofferItem.Max;
                var probability = Math.Round(cofferItem.Pct * 100, 2);

                drops.Add(
                    new ItemSupplement()
                    {
                        ItemId = itemId,
                        SourceItemId = sourceItemId,
                        ItemSupplementSource = supplementType,
                        Min = (uint?)min,
                        Max = (uint?)max,
                        Probability = (decimal?)probability
                    });
            }
        }
        return drops;
    }
}
