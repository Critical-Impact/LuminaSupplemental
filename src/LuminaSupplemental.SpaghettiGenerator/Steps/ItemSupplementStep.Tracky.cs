using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using LuminaSupplemental.Excel.Model;

using Newtonsoft.Json;

using SupabaseExporter.Structures;

namespace LuminaSupplemental.SpaghettiGenerator.Steps;

public partial class ItemSupplementStep
{
    private List<ItemSupplement> ProcessCards()
    {
        var supplementType = ItemSupplementSource.CardPacks;
        var filePath = "../../../../FFXIVGachaSpreadsheet/Website/assets/data/Cards.json";

        return ProcessCofferJson(filePath, supplementType);
    }

    private List<ItemSupplement> ProcessCoffers()
    {
        var supplementType = ItemSupplementSource.Coffer;
        var filePath = "../../../../FFXIVGachaSpreadsheet/Website/assets/data/CofferData.json";

        return ProcessCofferJson(filePath, supplementType);
    }

    private List<ItemSupplement> ProcessLogograms()
    {
        var supplementType = ItemSupplementSource.Logogram;
        var filePath = "../../../../FFXIVGachaSpreadsheet/Website/assets/data/LogoFrag.json";

        return ProcessCofferJson(filePath, supplementType);
    }

    private List<ItemSupplement> ProcessDesynth()
    {
        var filePath = "../../../../FFXIVGachaSpreadsheet/Website/assets/data/DesynthesisData.json";

        var drops = new List<ItemSupplement>();
        var json = File.ReadAllText(filePath);
        var desynthData = JsonConvert.DeserializeObject<DesynthData>(json);
        var supplementType = ItemSupplementSource.Desynth;

        foreach(var history in desynthData.Sources)
        {
            var sourceItemId = history.Key;
            foreach (var coffer in history.Value.Results)
            {

                var itemId = coffer.Id;
                if (itemId == 0 || sourceItemId == 0)
                {
                    continue;
                }

                var min = coffer.Min;
                var max = coffer.Max;
                var probability = Math.Round(coffer.Percentage * 100, 2);

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
        var filePath = "../../../../FFXIVGachaSpreadsheet/Website/assets/data/DeepDungeonData.json";

        var drops = new List<ItemSupplement>();
        var json = File.ReadAllText(filePath);
        var cofferDataList = JsonConvert.DeserializeObject<List<CofferData>>(json);
        for (var index = 0; index < cofferDataList.Count; index++)
        {
            ItemSupplementSource supplementType;

            var cofferData = cofferDataList[index];
            switch (index)
            {
                case 0:
                    supplementType = ItemSupplementSource.PalaceOfTheDead;
                    break;
                case 1:
                    supplementType = ItemSupplementSource.HeavenOnHigh;
                    break;
                case 2:
                    supplementType = ItemSupplementSource.EurekaOrthos;
                    break;
                default:
                    continue;
            }

            var coffers = cofferData.Coffers;
            foreach (var coffer in coffers)
            {
                var sourceItemId = coffer.CofferId;
                foreach (var cofferItem in coffer.Patches["All"].Items)
                {
                    var itemId = cofferItem.Id;
                    if (itemId == 0 || sourceItemId == 0)
                    {
                        continue;
                    }

                    var min = cofferItem.Min;
                    var max = cofferItem.Max;
                    var probability = Math.Round(cofferItem.Percentage * 100, 2);

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
        var filePath = "../../../../FFXIVGachaSpreadsheet/Website/assets/data/LockboxData.json";

        var drops = new List<ItemSupplement>();
        var json = File.ReadAllText(filePath);
        var cofferDataList = JsonConvert.DeserializeObject<List<CofferData>>(json);
        for (var index = 0; index < cofferDataList.Count; index++)
        {
            ItemSupplementSource supplementType;

            var cofferData = cofferDataList[index];
            switch (index)
            {
                case 0:
                    supplementType = ItemSupplementSource.Anemos;
                    break;
                case 1:
                    supplementType = ItemSupplementSource.Pagos;
                    break;
                case 2:
                    supplementType = ItemSupplementSource.Pyros;
                    break;
                case 3:
                    supplementType = ItemSupplementSource.Hydatos;
                    break;
                case 4:
                    supplementType = ItemSupplementSource.Bozja;
                    break;
                default:
                    continue;
            }

            var coffers = cofferData.Coffers;
            foreach (var coffer in coffers)
            {
                var sourceItemId = coffer.CofferId;
                foreach (var cofferItem in coffer.Patches["All"].Items)
                {
                    var itemId = cofferItem.Id;
                    if (itemId == 0 || sourceItemId == 0)
                    {
                        continue;
                    }

                    var min = cofferItem.Min;
                    var max = cofferItem.Max;
                    var probability = Math.Round(cofferItem.Percentage * 100, 2);

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
        var cofferDataList = JsonConvert.DeserializeObject<List<CofferData>>(cardJson);
        var coffers = cofferDataList.SelectMany(c => c.Coffers);
        foreach (var coffer in coffers)
        {
            var sourceItemId = coffer.CofferId;
            foreach (var cofferItem in coffer.Patches["All"].Items)
            {
                var itemId = cofferItem.Id;
                if (itemId == 0 || sourceItemId == 0)
                {
                    continue;
                }

                var min = cofferItem.Min;
                var max = cofferItem.Max;
                var probability = Math.Round(cofferItem.Percentage * 100, 2);

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
