using System;
using System.Collections.Generic;
using System.Linq;

using LuminaSupplemental.Excel.Model;

namespace LuminaSupplemental.SpaghettiGenerator.Steps;

public partial class ItemSupplementStep
{
    public List<ItemSupplement> ProcessItemSets()
    {
        var itemSupplements = new List<ItemSupplement>();

        var cofferNames = new Dictionary<string[], string>
        {
            { new[] { "Crystarium", "Coffer" }, "Augmented Crystarium" },
            { new[] { "Abyssos", "Coffer" }, "Abyssos" },
            { new[] { "Alexandrian", "Coffer" }, "Alexandrian" },
            { new[] { "Allagan", "Coffer" }, "Allagan" },
            { new[] { "High Allagan", "Coffer" }, "High Allagan" },
            { new[] { "Asphodelos", "Coffer" }, "Asphodelos" },
            { new[] { "Bluefeather", "Coffer" }, "Bluefeather" },
            { new[] { "Byakko", "Coffer" }, "Byakko" },
            { new[] { "Archfiend Attire Coffer" }, "Archfiend" },
            { new[] { "Bookwyrm's Attire Coffer" }, "Bookwyrm's" },
            { new[] { "False Monarchy Attire Coffer" }, "False Monarchy" },
            { new[] { "Muzhik Attire Coffer" }, "Muzhik" },
            { new[] { "Spotted Attire Coffer" }, "Spotted" },
            { new[] { "Street Attire Coffer" }, "Street" },
            { new[] { "Chondrite", "Coffer" }, "Star Quartz" },
            { new[] { "Chondrite", "Coffer" }, "Chondrite" },
            { new[] { "Chondrite", "Coffer" }, "AR-Caean Velvet" },
            { new[] { "Chondrite", "Coffer" }, "Ophiotauroskin" },
            { new[] { "Crag", "Coffer" }, "Crags" },
            { new[] { "Crag", "Coffer" }, "Key of Titan" },
            { new[] { "Cryptlurker", "Coffer" }, "Cryptlurker's" },
            { new[] { "Diamond", "Coffer" }, "Diamond" },
            { new[] { "Divine Light", "Coffer" }, "Divine Light" },
            { new[] { "Dreadwyrm", "Coffer" }, "Dreadwyrm" },
            { new[] { "Emerald", "Coffer" }, "Emerald" },
            { new[] { "Expanse", "Coffer" }, "Expanse" },
            { new[] { "Flamecloaked", "Coffer" }, "Flamecloaked" },
            { new[] { "Genji Armo", "Coffer" }, "Genji" },
            { new[] { "Zurvanite", "Coffer" }, "Zurvanite" },
            { new[] { "Windswept", "Coffer" }, "Windswept" },
            { new[] { "Vortex", "Coffer" }, "Garuda's" },
            { new[] { "Tsukuyomi", "Coffer" }, "Tsukuyomi" },
            { new[] { "Titania", "Coffer" }, "The King's" },
            { new[] { "Tidal", "Coffer" }, "Wave " },
            { new[] { "Suzaku", "Coffer" }, "Suzaku's" },
            { new[] { "Susano", "Coffer" }, "Susano's" },
            { new[] { "Splendorous", "Coffer" }, "Splendorous " },
            { new[] { "Sophic", "Coffer" }, "Sophic " },
            { new[] { "Shinryu", "Coffer" }, "Shinryu's" },
            { new[] { "Sephirotic", "Coffer" }, "of the Sephirot" },
            { new[] { "Seiryu ", "Coffer" }, "Seiryu's" },
            { new[] { "Ruby ", "Coffer" }, "Ruby" },
            { new[] { "Pewter ", "Coffer" }, "Palm" },
            { new[] { "Omega ", "Coffer" }, "Omega" },
            { new[] { "Voidcast ", "Coffer" }, "Voidcast" },
            { new[] { "Ascension ", "Coffer" }, "Ascension" },
            { new[] { "Ascension ", "Coffer" }, "of Ascension" },
        };
        foreach (var cofferName in cofferNames)
        {
            var coffers = itemSheet.Where(
                c =>
                {
                    return !bannedItems.Contains(c.RowId) && c.Name.ToString().StartsWith(cofferName.Key[0]) &&
                           cofferName.Key.Skip(1).All(d => c.Name.ToString().Contains(d));
                }).ToList();
            foreach (var coffer in coffers)
            {
                var fullName = String.Join(" ", cofferName.Key);
                if (coffer.Name.ExtractText() == fullName)
                {
                    var items = itemSheet.Where(
                        c => !bannedItems.Contains(c.RowId) &&
                             c.Name.ToString().StartsWith(cofferName.Value) &&
                             ((c.EquipSlotCategory.ValueNullable?.MainHand ?? 0) == 1 || (c.EquipSlotCategory.ValueNullable?.OffHand ?? 0) == 1));
                    foreach (var item in items)
                    {
                        itemSupplements.Add(new ItemSupplement(item.RowId, coffer.RowId, ItemSupplementSource.Loot));
                    }
                }

                if (coffer.Name.ToString().Contains("Attire Coffer"))
                {
                    var potentialItems = new string[]
                    {
                        "Armor",
                        "Breeches",
                        "Gauntlets",
                        "Helm",
                        "Sabatons",
                        "Cap",
                        "Jacket",
                        "Gloves",
                        "Slacks",
                        "Shoes",
                        "Mask",
                        "Culottes",
                        "Hat",
                        "Dress",
                        "Blinder",
                        "Boots",
                        "Coat",
                        "Field Dressing",
                        "Trousers",
                        "Fedora",
                        "Spencer",
                        "Cargo Trousers",
                        "Handwear",
                        "High-top Shoes",
                        "Top",
                        "Spectacles",
                        "Chasuble",
                        "Waistwrap"
                    };
                    foreach (var potentialItem in potentialItems)
                    {
                        var items = itemSheet.Where(
                            c => !bannedItems.Contains(c.RowId) && c.Name.ToString().Contains(cofferName.Value) && c.Name.ToString().Contains(potentialItem) &&
                                 (
                                     (c.EquipSlotCategory.ValueNullable?.Body ?? 0) == 1 ||
                                     (c.EquipSlotCategory.ValueNullable?.Feet ?? 0) == 1 ||
                                     (c.EquipSlotCategory.ValueNullable?.Head ?? 0) == 1 ||
                                     (c.EquipSlotCategory.ValueNullable?.Gloves ?? 0) == 1 ||
                                     (c.EquipSlotCategory.ValueNullable?.Legs ?? 0) == 1
                                 ));
                        foreach (var item in items)
                        {
                            itemSupplements.Add(new ItemSupplement(item.RowId, coffer.RowId, ItemSupplementSource.Loot));
                        }
                    }
                }

                //Weapon, Gear, Accessories
                if (coffer.Name.ToString().Contains("Weapon"))
                {
                    var items = itemSheet.Where(
                        c => !bannedItems.Contains(c.RowId) &&
                             c.Name.ToString().Contains(cofferName.Value) &&
                             ((c.EquipSlotCategory.ValueNullable?.MainHand ?? 0) == 1 || (c.EquipSlotCategory.ValueNullable?.OffHand ?? 0) == 1));
                    foreach (var item in items)
                    {
                        itemSupplements.Add(new ItemSupplement(item.RowId, coffer.RowId, ItemSupplementSource.Loot));
                    }
                }

                if (coffer.Name.ToString().Contains("Gear"))
                {
                    var armourTypes = new string[]
                    {
                        "Striking",
                        "Maiming",
                        "Fending",
                        "Aiming",
                        "Scouting",
                        "Healing",
                        "Casting",
                        "Slaying"
                    };
                    foreach (var armourType in armourTypes)
                    {
                        if (coffer.Name.ToString().Contains(armourType))
                        {
                            var items = itemSheet.Where(
                                c => !bannedItems.Contains(c.RowId) &&
                                     c.Name.ToString().Contains(cofferName.Value) && c.Name.ToString().Contains(armourType) &&
                                     (
                                         (c.EquipSlotCategory.ValueNullable?.Body ?? 0) == 1 ||
                                         (c.EquipSlotCategory.ValueNullable?.Feet ?? 0) == 1 ||
                                         (c.EquipSlotCategory.ValueNullable?.Head ?? 0) == 1 ||
                                         (c.EquipSlotCategory.ValueNullable?.Gloves ?? 0) == 1 ||
                                         (c.EquipSlotCategory.ValueNullable?.Legs ?? 0) == 1
                                     ));
                            foreach (var item in items)
                            {
                                itemSupplements.Add(new ItemSupplement(item.RowId, coffer.RowId, ItemSupplementSource.Loot));
                            }
                        }
                    }
                }

                if (coffer.Name.ToString().Contains("Head Gear"))
                {
                    var items = itemSheet.Where(
                        c => !bannedItems.Contains(c.RowId) &&
                             c.Name.ToString().StartsWith(cofferName.Value) && c.Name.ToString().Contains(" of ") &&
                             (c.EquipSlotCategory.ValueNullable?.Head ?? 0) == 1);
                    foreach (var item in items)
                    {
                        itemSupplements.Add(new ItemSupplement(item.RowId, coffer.RowId, ItemSupplementSource.Loot));
                    }
                }

                if (coffer.Name.ToString().Contains("Chest Gear"))
                {
                    var items = itemSheet.Where(
                        c => !bannedItems.Contains(c.RowId) &&
                             c.Name.ToString().StartsWith(cofferName.Value) && c.Name.ToString().Contains(" of ") &&
                             (c.EquipSlotCategory.ValueNullable?.Body ?? 0) == 1);
                    foreach (var item in items)
                    {
                        itemSupplements.Add(new ItemSupplement(item.RowId, coffer.RowId, ItemSupplementSource.Loot));
                    }
                }

                if (coffer.Name.ToString().Contains("Hand Gear"))
                {
                    var items = itemSheet.Where(
                        c => !bannedItems.Contains(c.RowId) &&
                             c.Name.ToString().StartsWith(cofferName.Value) && c.Name.ToString().Contains(" of ") &&
                             (c.EquipSlotCategory.ValueNullable?.Gloves ?? 0) == 1);
                    foreach (var item in items)
                    {
                        itemSupplements.Add(new ItemSupplement(item.RowId, coffer.RowId, ItemSupplementSource.Loot));
                    }
                }

                if (coffer.Name.ToString().Contains("Leg Gear"))
                {
                    var items = itemSheet.Where(
                        c => !bannedItems.Contains(c.RowId) &&
                             c.Name.ToString().StartsWith(cofferName.Value) && c.Name.ToString().Contains(" of ") &&
                             (c.EquipSlotCategory.ValueNullable?.Legs ?? 0) == 1);
                    foreach (var item in items)
                    {
                        itemSupplements.Add(new ItemSupplement(item.RowId, coffer.RowId, ItemSupplementSource.Loot));
                    }
                }

                if (coffer.Name.ToString().Contains("Foot Gear"))
                {
                    var items = itemSheet.Where(
                        c => !bannedItems.Contains(c.RowId) &&
                             c.Name.ToString().StartsWith(cofferName.Value) && c.Name.ToString().Contains(" of ") &&
                             (c.EquipSlotCategory.ValueNullable?.Feet ?? 0) == 1);
                    foreach (var item in items)
                    {
                        itemSupplements.Add(new ItemSupplement(item.RowId, coffer.RowId, ItemSupplementSource.Loot));
                    }
                }

                if (coffer.Name.ToString().Contains("Earring Coffer"))
                {
                    var items = itemSheet.Where(
                        c => !bannedItems.Contains(c.RowId) &&
                             c.Name.ToString().StartsWith(cofferName.Value) && c.Name.ToString().Contains(" of ") &&
                             (c.EquipSlotCategory.ValueNullable?.Ears ?? 0) == 1);
                    foreach (var item in items)
                    {
                        itemSupplements.Add(new ItemSupplement(item.RowId, coffer.RowId, ItemSupplementSource.Loot));
                    }
                }

                if (coffer.Name.ToString().Contains("Necklace Coffer"))
                {
                    var items = itemSheet.Where(
                        c => !bannedItems.Contains(c.RowId) &&
                             c.Name.ToString().StartsWith(cofferName.Value) && c.Name.ToString().Contains(" of ") &&
                             (c.EquipSlotCategory.ValueNullable?.Neck ?? 0) == 1);
                    foreach (var item in items)
                    {
                        itemSupplements.Add(new ItemSupplement(item.RowId, coffer.RowId, ItemSupplementSource.Loot));
                    }
                }

                if (coffer.Name.ToString().Contains("Bracelet Coffer"))
                {
                    var items = itemSheet.Where(
                        c => !bannedItems.Contains(c.RowId) &&
                             c.Name.ToString().StartsWith(cofferName.Value) && c.Name.ToString().Contains(" of ") &&
                             (c.EquipSlotCategory.ValueNullable?.Wrists ?? 0) == 1);
                    foreach (var item in items)
                    {
                        itemSupplements.Add(new ItemSupplement(item.RowId, coffer.RowId, ItemSupplementSource.Loot));
                    }
                }

                if (coffer.Name.ToString().Contains("Ring Coffer"))
                {
                    var items = itemSheet.Where(
                        c => !bannedItems.Contains(c.RowId) &&
                             c.Name.ToString().StartsWith(cofferName.Value) && c.Name.ToString().Contains(" of ") &&
                             (c.EquipSlotCategory.ValueNullable?.FingerL ?? 0) == 1);
                    foreach (var item in items)
                    {
                        itemSupplements.Add(new ItemSupplement(item.RowId, coffer.RowId, ItemSupplementSource.Loot));
                    }
                }

                if (coffer.Name.ToString().Contains("Accessories"))
                {
                    var armourTypes = new string[]
                    {
                        "Striking",
                        "Maiming",
                        "Fending",
                        "Aiming",
                        "Scouting",
                        "Healing",
                        "Casting",
                        "Slaying"
                    };
                    foreach (var armourType in armourTypes)
                    {
                        if (coffer.Name.ToString().Contains(armourType))
                        {
                            var items = itemSheet.Where(
                                c => !bannedItems.Contains(c.RowId) && c.Name.ToString().Contains(cofferName.Value) &&
                                     c.Name.ToString().Contains(armourType) &&
                                     (
                                         (c.EquipSlotCategory.ValueNullable?.Ears ?? 0) == 1 ||
                                         (c.EquipSlotCategory.ValueNullable?.Neck ?? 0) == 1 ||
                                         (c.EquipSlotCategory.ValueNullable?.FingerL ?? 0) == 1 ||
                                         (c.EquipSlotCategory.ValueNullable?.FingerR ?? 0) == 1 ||
                                         (c.EquipSlotCategory.ValueNullable?.Wrists ?? 0) == 1
                                     ));
                            foreach (var item in items)
                            {
                                itemSupplements.Add(new ItemSupplement(item.RowId, coffer.RowId, ItemSupplementSource.Loot));
                            }
                        }
                    }
                }
            }
        }

        return itemSupplements;
    }
}
