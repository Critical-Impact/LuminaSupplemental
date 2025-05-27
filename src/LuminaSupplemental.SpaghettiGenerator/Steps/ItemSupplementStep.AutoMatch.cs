using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using Dalamud.Utility;

using Lumina.Excel.Sheets;

using LuminaSupplemental.Excel.Model;

namespace LuminaSupplemental.SpaghettiGenerator.Steps;

public partial class ItemSupplementStep
{
    //Taken from https://github.com/RajahOmen/BisBuddy/blob/91112b805087401a99a8530e18953561c9b0fc98/BisBuddy/Items/ItemData.Generate.cs#L4 - thank you!
    private static readonly Regex ItemNameIlvlRegex = new(@"\(IL ([0-9]*)\)");
    private static readonly Regex AugmentedTomestoneGearNameRegex = new(@"\AAugmented");

    private static readonly Dictionary<ushort, List<uint>> CofferIconToEquipSlotCategory = new()
    {
        { 26557, [1, 2, 13] }, // general weapon coffer
        { 26632, [1, 2] },     // PLD arms 1
        { 26633, [1, 2] },     // PLD arms 2
        { 26634, [1, 2] },     // PLD arms 3
        { 26635, [1, 2] },     // PLD arms 4
        { 26558, [3] },        // head
        { 26559, [4] },        // body
        { 26560, [5] },        // gloves
        { 26561, [7] },        // pants
        { 26562, [8] },        // shoes
        // { 26573, [6] }        // belt/waist, ignore
        { 26564, [9] },        // earrings
        { 26565, [10] },       // necklace
        { 26566, [11] },       // bracelet
        { 26567, [12] },       // ring
    };

    private List<ItemSupplement> AutoMatchMissingCoffers(List<ItemSupplement> existingCofferSources)
    {
        List<ItemSupplement> newItems = new List<ItemSupplement>();

        // list of ids of coffer-like items with known contents
        var knownCoffers = existingCofferSources
            .Select(item => item.SourceItemId)
            .ToHashSet();

        // get list of coffer-like items with unknown contents
        var unknownCoffers = new HashSet<Item>();
        var unknownCofferIlvls = new HashSet<uint>();
        var candidateItems = new Dictionary<uint, List<Item>>();
        var candidateCoffers = new Dictionary<uint, List<Item>>();
        foreach (var item in itemSheet.Where(item => !knownCoffers.Contains(item.RowId)))
        {
            var itemName = item.Name.ToDalamudString().TextValue;
            if (CofferIconToEquipSlotCategory.ContainsKey(item.Icon))
            {
                var match = ItemNameIlvlRegex.Match(itemName);
                if (!match.Success)
                    continue;

                var itemLevel = uint.Parse(match.Groups[1].Value);
                unknownCoffers.Add(item);
                unknownCofferIlvls.Add(itemLevel);
                candidateItems[itemLevel] = [];
                if (!candidateCoffers.TryGetValue(itemLevel, out var coffersAtIlvl))
                    candidateCoffers[itemLevel] = [item];
                else
                    coffersAtIlvl.Add(item);
            }
        }

        // no coffers with unknown contents
        if (unknownCoffers.Count == 0)
        {
            return newItems;
        }

        // retrieve items that may feasibly come from unknown coffers
        foreach (var item in itemSheet)
        {
            // a coffer, ignore
            if (unknownCoffers.Contains(item) || knownCoffers.Contains(item.RowId))
                continue;

            // cannot be equipped, not a gearpiece
            if (item.EquipSlotCategory.RowId == 0)
                continue;

            // this is a augmented tomestone gearpiece, cannot be from coffer
            var itemName = item.Name.ToDalamudString().TextValue;
            if (AugmentedTomestoneGearNameRegex.IsMatch(itemName))
                continue;

            // potential candidate item
            if (unknownCofferIlvls.Contains(item.LevelItem.RowId))
                candidateItems[item.LevelItem.RowId].Add(item);
        }

        // solve per relevant ilvl
        foreach (var ilvl in unknownCofferIlvls)
        {
            var coffersAtIlvl = candidateCoffers[ilvl];
            var itemsAtIlvl = candidateItems[ilvl];
            // group items by EquipSlotCategory/ClassJobCategory pair, since cannot contain multiple from each grouping from 1 coffer
            var groupedItemsAtIlvl = itemsAtIlvl
                .GroupBy(
                    item => new
                    {
                        equipSlotCategory = item.EquipSlotCategory.RowId,
                        classJobCategory = item.ClassJobCategory.RowId,
                    });

            foreach (var coffer in coffersAtIlvl)
            {
                var cofferName = coffer.Name.ToDalamudString().TextValue;
                var cofferNameLev = new Fastenshtein.Levenshtein(cofferName);
                var cofferEquipSlot = CofferIconToEquipSlotCategory.GetValueOrDefault(coffer.Icon) ?? [];

                foreach (var itemGroup in groupedItemsAtIlvl.Where(g => cofferEquipSlot.Contains(g.Key.equipSlotCategory)))
                {
                    // get item whos name most closely matches name of coffer
                    var bestMatch = itemGroup
                        .OrderBy(item => cofferNameLev.DistanceFrom(item.Name.ToDalamudString().TextValue))
                        .First();

                    var newItemSupplement = new ItemSupplement(bestMatch.RowId, coffer.RowId, ItemSupplementSource.Loot);
                    newItems.Add(newItemSupplement);
                }
            }
        }

        return newItems;
    }
}
