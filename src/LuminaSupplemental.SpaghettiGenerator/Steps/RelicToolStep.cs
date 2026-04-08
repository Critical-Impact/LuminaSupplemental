using System;
using System.Collections.Generic;
using System.Linq;

using Lumina.Excel;
using Lumina.Excel.Sheets;
using Lumina.Extensions;

using LuminaSupplemental.Excel.Model;
using LuminaSupplemental.SpaghettiGenerator.Extensions;
using LuminaSupplemental.SpaghettiGenerator.Generator;

namespace LuminaSupplemental.SpaghettiGenerator.Steps;

public class RelicToolStep : GeneratorStep
{
    private readonly ClassJob CRP;
    private readonly ClassJob BSM;
    private readonly ClassJob ARM;
    private readonly ClassJob GSM;
    private readonly ClassJob LTW;
    private readonly ClassJob WVR;
    private readonly ClassJob ALC;
    private readonly ClassJob CUL;
    private readonly ClassJob MIN;
    private readonly ClassJob BTN;
    private readonly ClassJob FSH;

    private readonly Dictionary<string, uint> itemsByName;

    public override Type OutputType => typeof(RelicTool);

    public override string FileName => "RelicTool.csv";

    public override string Name => "Relic Tools";

    public RelicToolStep(ExcelSheet<ClassJob> classJobSheet, DataCacher dataCacher)
    {
        var bannedItems = new HashSet< uint >()
        {
            0,
            24225
        };
        this.itemsByName = dataCacher.ByName<Item>(item => item.Name.ToString().ToParseable(), item => !bannedItems.Contains(item.RowId));
        var classes = classJobSheet.Where(c => !c.Abbreviation.IsEmpty).ToDictionary(c => c.Abbreviation.ToImGuiString());

        CRP = classes["CRP"];
        BSM = classes["BSM"];
        ARM = classes["ARM"];
        GSM = classes["GSM"];
        LTW = classes["LTW"];
        WVR = classes["WVR"];
        ALC = classes["ALC"];
        CUL = classes["CUL"];
        MIN = classes["MIN"];
        BTN = classes["BTN"];
        FSH = classes["FSH"];
    }

    public List<RelicTool> ProcessTools()
    {
        var relicTools = new List<RelicTool>();

        var rowId = 1u;

        //Mastercraft Tools
        var relicToolType = (int)RelicToolType.MastercraftBase;
        var relicToolCategory = (int)RelicToolCategory.Mastercraft;

        var crafterGathererMap = new Dictionary<ClassJob, List<List<string>>>()
        {
            {
                this.CRP, new List<List<string>>
                {
                    new() { "Ullikummi" },
                    new() { "Ullikummi Supra" },
                    new() { "Ullikummi Lucis" },
                }
            },
            {
                this.BSM, new List<List<string>>
                {
                    new() { "Vulcan" },
                    new() { "Vulcan Supra" },
                    new() { "Vulcan Lucis" },
                }
            },
            {
                this.ARM, new List<List<string>>
                {
                    new() { "Kurdalegon" },
                    new() { "Kurdalegon Supra" },
                    new() { "Kurdalegon Lucis" },
                }
            },
            {
                this.GSM, new List<List<string>>
                {
                    new() { "Urcaguary" },
                    new() { "Urcaguary Supra" },
                    new() { "Urcaguary Lucis" },
                }
            },
            {
                this.LTW, new List<List<string>>
                {
                    new() { "Pinga" },
                    new() { "Pinga Supra" },
                    new() { "Pinga Lucis" },
                }
            },
            {
                this.WVR, new List<List<string>>
                {
                    new() { "Clotho" },
                    new() { "Clotho Supra" },
                    new() { "Clotho Lucis" },
                }
            },
            {
                this.ALC, new List<List<string>>
                {
                    new() { "Paracelsus" },
                    new() { "Paracelsus Supra" },
                    new() { "Paracelsus Lucis" },
                }
            },
            {
                this.CUL, new List<List<string>>
                {
                    new() { "Chantico" },
                    new() { "Chantico Supra" },
                    new() { "Chantico Lucis" },
                }
            },
            {
                this.MIN, new List<List<string>>
                {
                    new() { "Mammon" },
                    new() { "Mammon Supra" },
                    new() { "Mammon Lucis" },
                }
            },
            {
                this.BTN, new List<List<string>>
                {
                    new() { "Rauni" },
                    new() { "Rauni Supra" },
                    new() { "Rauni Lucis" },
                }
            },
            {
                this.FSH, new List<List<string>>
                {
                    new() { "Halcyon Rod" },
                    new() { "Halcyon Rod Supra" },
                    new() { "Halcyon Rod Lucis" },
                }
            }
        };

        var stageQuests = new Dictionary<RelicToolType, uint>()
        {
            { RelicToolType.MastercraftBase, 66959 },
        };

        foreach (var item in crafterGathererMap)
        {
            var stage = 1u;
            var originalRelicToolType = relicToolType;
            var previousWeapon = 0u;
            foreach(var items in item.Value)
            {
                uint item1;
                uint item2 = 0;
                if (!this.itemsByName.TryGetValue(items[0].ToParseable(), out item1))
                {
                    throw new Exception($"Unknown item: {items[0]}");
                }

                if (items.Count > 1)
                {
                    if (!this.itemsByName.TryGetValue(items[1].ToParseable(), out item2))
                    {
                        throw new Exception($"Unknown item: {items[1]}");
                    }
                }

                var weaponCategory = (RelicToolCategory)relicToolCategory;
                var weaponType = (RelicToolType)relicToolType;
                relicTools.Add(new RelicTool(rowId, item1, stage, item.Key.RowId, weaponCategory, weaponType, stageQuests.TryGetValue(weaponType, out var quest) ? quest : 0, previousWeapon));
                previousWeapon = rowId;
                relicToolType++;
                stage++;
                rowId++;
            }

            relicToolType = originalRelicToolType;
        }

        //Skysteel Tools
        relicToolType = (int)RelicToolType.SkysteelBase;
        relicToolCategory = (int)RelicToolCategory.Skysteel;

        crafterGathererMap = new Dictionary<ClassJob, List<List<string>>>()
        {
            {
                this.CRP, new List<List<string>>
                {
                    new() { "Skysteel Saw" },
                    new() { "Skysteel Saw +1" },
                    new() { "Dragonsung Saw" },
                    new() { "Augmented Dragonsung Saw" },
                    new() { "Skysung Saw" },
                    new() { "Skybuilders' Saw" },
                }
            },
            {
                this.BSM, new List<List<string>>
                {
                    new() { "Skysteel Cross-pein Hammer" },
                    new() { "Skysteel Cross-pein Hammer +1" },
                    new() { "Dragonsung Cross-pein Hammer" },
                    new() { "Augmented Dragonsung Cross-pein Hammer" },
                    new() { "Skysung Cross-pein Hammer" },
                    new() { "Skybuilders' Cross-pein Hammer" },
                }
            },
            {
                this.ARM, new List<List<string>>
                {
                    new() { "Skysteel Raising Hammer" },
                    new() { "Skysteel Raising Hammer +1" },
                    new() { "Dragonsung Raising Hammer" },
                    new() { "Augmented Dragonsung Raising Hammer" },
                    new() { "Skysung Raising Hammer" },
                    new() { "Skybuilders' Raising Hammer" },
                }
            },
            {
                this.GSM, new List<List<string>>
                {
                    new() { "Skysteel Lapidary Hammer" },
                    new() { "Skysteel Lapidary Hammer +1" },
                    new() { "Dragonsung Lapidary Hammer" },
                    new() { "Augmented Dragonsung Lapidary Hammer" },
                    new() { "Skysung Lapidary Hammer" },
                    new() { "Skybuilders' Lapidary Hammer" },
                }
            },
            {
                this.LTW, new List<List<string>>
                {
                    new() { "Skysteel Round Knife" },
                    new() { "Skysteel Round Knife +1" },
                    new() { "Dragonsung Round Knife" },
                    new() { "Augmented Dragonsung Round Knife" },
                    new() { "Skysung Round Knife" },
                    new() { "Skybuilders' Round Knife" },
                }
            },
            {
                this.WVR, new List<List<string>>
                {
                    new() { "Skysteel Needle" },
                    new() { "Skysteel Needle +1" },
                    new() { "Dragonsung Needle" },
                    new() { "Augmented Dragonsung Needle" },
                    new() { "Skysung Needle" },
                    new() { "Skybuilders' Needle" },
                }
            },
            {
                this.ALC, new List<List<string>>
                {
                    new() { "Skysteel Alembic" },
                    new() { "Skysteel Alembic +1" },
                    new() { "Dragonsung Alembic" },
                    new() { "Augmented Dragonsung Alembic" },
                    new() { "Skysung Alembic" },
                    new() { "Skybuilders' Alembic" },
                }
            },
            {
                this.CUL, new List<List<string>>
                {
                    new() { "Skysteel Frypan" },
                    new() { "Skysteel Frypan +1" },
                    new() { "Dragonsung Frypan" },
                    new() { "Augmented Dragonsung Frypan" },
                    new() { "Skysung Frypan" },
                    new() { "Skybuilders' Frypan" },
                }
            },
            {
                this.MIN, new List<List<string>>
                {
                    new() { "Skysteel Pickaxe" },
                    new() { "Skysteel Pickaxe +1" },
                    new() { "Dragonsung Pickaxe" },
                    new() { "Augmented Dragonsung Pickaxe" },
                    new() { "Skysung Pickaxe" },
                    new() { "Skybuilders' Pickaxe" },
                }
            },
            {
                this.BTN, new List<List<string>>
                {
                    new() { "Skysteel Hatchet" },
                    new() { "Skysteel Hatchet +1" },
                    new() { "Dragonsung Hatchet" },
                    new() { "Augmented Dragonsung Hatchet" },
                    new() { "Skysung Hatchet" },
                    new() { "Skybuilders' Hatchet" },
                }
            },
            {
                this.FSH, new List<List<string>>
                {
                    new() { "Skysteel Fishing Rod" },
                    new() { "Skysteel Fishing Rod +1" },
                    new() { "Dragonsung Fishing Rod" },
                    new() { "Augmented Dragonsung Fishing Rod" },
                    new() { "Skysung Fishing Rod" },
                    new() { "Skybuilders' Fishing Rod" },
                }
            }
        };

        stageQuests = new Dictionary<RelicToolType, uint>()
        {
            { RelicToolType.SkysteelBase, 69384 },
            { RelicToolType.SkysteelDragonsung, 69428 },
            { RelicToolType.SkysteelAugmentedDragonsung, 69429 },
            { RelicToolType.SkysteelSkysung, 69430 },
            { RelicToolType.SkysteelSkybuilders, 69519 },
        };

        foreach (var item in crafterGathererMap)
        {
            var stage = 1u;
            var originalRelicToolType = relicToolType;
            var previousWeapon = 0u;
            foreach(var items in item.Value)
            {
                uint item1;
                uint item2 = 0;
                if (!this.itemsByName.TryGetValue(items[0].ToParseable(), out item1))
                {
                    throw new Exception($"Unknown item: {items[0]}");
                }

                if (items.Count > 1)
                {
                    if (!this.itemsByName.TryGetValue(items[1].ToParseable(), out item2))
                    {
                        throw new Exception($"Unknown item: {items[1]}");
                    }
                }

                var weaponCategory = (RelicToolCategory)relicToolCategory;
                var weaponType = (RelicToolType)relicToolType;
                relicTools.Add(new RelicTool(rowId, item1, stage, item.Key.RowId, weaponCategory, weaponType, stageQuests.TryGetValue(weaponType, out var quest) ? quest : 0, previousWeapon));
                previousWeapon = rowId;
                relicToolType++;
                stage++;
                rowId++;
            }

            relicToolType = originalRelicToolType;
        }

        //Resplendent Tools
        relicToolType = (int)RelicToolType.Resplendent;
        relicToolCategory = (int)RelicToolCategory.Resplendent;

        crafterGathererMap = new Dictionary<ClassJob, List<List<string>>>()
        {
            {
                this.CRP, new List<List<string>>
                {
                    new() { "Resplendent Millfiend's Saw" },
                }
            },
            {
                this.BSM, new List<List<string>>
                {
                    new() { "Resplendent Forgefiend's Hammer" },
                }
            },
            {
                this.ARM, new List<List<string>>
                {
                    new() { "Resplendent Hammerfiend's Beetle" },
                }
            },
            {
                this.GSM, new List<List<string>>
                {
                    new() { "Resplendent Gemfiend's Mallet" },
                }
            },
            {
                this.LTW, new List<List<string>>
                {
                    new() { "Resplendent Hidefiend's Knife" },
                }
            },
            {
                this.WVR, new List<List<string>>
                {
                    new() { "Resplendent Boltfiend's Needle" },
                }
            },
            {
                this.ALC, new List<List<string>>
                {
                    new() { "Resplendent Cauldronfiend's Alembic" },
                }
            },
            {
                this.CUL, new List<List<string>>
                {
                    new() { "Resplendent Galleyfiend's Frypan" },
                }
            },
            {
                this.MIN, new List<List<string>>
                {
                    new() { "Resplendent Minefiend's Pickaxe" },
                }
            },
            {
                this.BTN, new List<List<string>>
                {
                    new() { "Resplendent Fieldfiend's Hatchet" },
                }
            },
            {
                this.FSH, new List<List<string>>
                {
                    new() { "Resplendent Tacklefiend's Rod" },
                }
            }
        };

        stageQuests = new Dictionary<RelicToolType, uint>()
        {
            { RelicToolType.Resplendent, 69139 },
        };

        foreach (var item in crafterGathererMap)
        {
            var stage = 1u;
            var originalRelicToolType = relicToolType;
            var previousWeapon = 0u;
            foreach(var items in item.Value)
            {
                uint item1;
                uint item2 = 0;
                if (!this.itemsByName.TryGetValue(items[0].ToParseable(), out item1))
                {
                    throw new Exception($"Unknown item: {items[0]}");
                }

                if (items.Count > 1)
                {
                    if (!this.itemsByName.TryGetValue(items[1].ToParseable(), out item2))
                    {
                        throw new Exception($"Unknown item: {items[1]}");
                    }
                }

                var weaponCategory = (RelicToolCategory)relicToolCategory;
                var weaponType = (RelicToolType)relicToolType;
                relicTools.Add(new RelicTool(rowId, item1, stage, item.Key.RowId, weaponCategory, weaponType, stageQuests.TryGetValue(weaponType, out var quest) ? quest : 0, previousWeapon));
                previousWeapon = rowId;
                relicToolType++;
                stage++;
                rowId++;
            }

            relicToolType = originalRelicToolType;
        }

        //Splendorous Tools
        relicToolType = (int)RelicToolType.SplendorousBase;
        relicToolCategory = (int)RelicToolCategory.Splendorous;

        crafterGathererMap = new Dictionary<ClassJob, List<List<string>>>()
        {
            {
                this.CRP, new List<List<string>>
                {
                    new() { "Splendorous Saw" },
                    new() { "Augmented Splendorous Saw" },
                    new() { "Crystalline Saw" },
                    new() { "Chora-Zoi's Crystalline Saw" },
                    new() { "Brilliant Saw" },
                    new() { "Vrandtic Visionary's Saw" },
                    new() { "Lodestar Saw" },
                }
            },
            {
                this.BSM, new List<List<string>>
                {
                    new() { "Splendorous Cross-pein Hammer" },
                    new() { "Augmented Splendorous Cross-pein Hammer" },
                    new() { "Crystalline Cross-pein Hammer" },
                    new() { "Chora-Zoi's Crystalline Cross-pein Hammer" },
                    new() { "Brilliant Cross-pein Hammer" },
                    new() { "Vrandtic Visionary's Cross-pein Hammer" },
                    new() { "Lodestar Cross-pein Hammer" },
                }
            },
            {
                this.ARM, new List<List<string>>
                {
                    new() { "Splendorous Raising Hammer" },
                    new() { "Augmented Splendorous Raising Hammer" },
                    new() { "Crystalline Raising Hammer" },
                    new() { "Chora-Zoi's Crystalline Raising Hammer" },
                    new() { "Brilliant Raising Hammer" },
                    new() { "Vrandtic Visionary's Raising Hammer" },
                    new() { "Lodestar Raising Hammer" },
                }
            },
            {
                this.GSM, new List<List<string>>
                {
                    new() { "Splendorous Mallet" },
                    new() { "Augmented Splendorous Mallet" },
                    new() { "Crystalline Mallet" },
                    new() { "Chora-Zoi's Crystalline Mallet" },
                    new() { "Brilliant Mallet" },
                    new() { "Vrandtic Visionary's Mallet" },
                    new() { "Lodestar Mallet" },
                }
            },
            {
                this.LTW, new List<List<string>>
                {
                    new() { "Splendorous Knife" },
                    new() { "Augmented Splendorous Knife" },
                    new() { "Crystalline Round Knife" },
                    new() { "Chora-Zoi's Crystalline Round Knife" },
                    new() { "Brilliant Round Knife" },
                    new() { "Vrandtic Visionary's Round Knife" },
                    new() { "Lodestar Round Knife" },
                }
            },
            {
                this.WVR, new List<List<string>>
                {
                    new() { "Splendorous Needle" },
                    new() { "Augmented Splendorous Needle" },
                    new() { "Crystalline Needle" },
                    new() { "Chora-Zoi's Crystalline Needle" },
                    new() { "Brilliant Needle" },
                    new() { "Vrandtic Visionary's Needle" },
                    new() { "Lodestar Needle" },
                }
            },
            {
                this.ALC, new List<List<string>>
                {
                    new() { "Splendorous Alembic" },
                    new() { "Augmented Splendorous Alembic" },
                    new() { "Crystalline Alembic" },
                    new() { "Chora-Zoi's Crystalline Alembic" },
                    new() { "Brilliant Alembic" },
                    new() { "Vrandtic Visionary's Alembic" },
                    new() { "Lodestar Alembic" },
                }
            },
            {
                this.CUL, new List<List<string>>
                {
                    new() { "Splendorous Frypan" },
                    new() { "Augmented Splendorous Frypan" },
                    new() { "Crystalline Frypan" },
                    new() { "Chora-Zoi's Crystalline Frypan" },
                    new() { "Brilliant Frypan" },
                    new() { "Vrandtic Visionary's Frypan" },
                    new() { "Lodestar Frypan" },
                }
            },
            {
                this.MIN, new List<List<string>>
                {
                    new() { "Splendorous Pickaxe" },
                    new() { "Augmented Splendorous Pickaxe" },
                    new() { "Crystalline Pickaxe" },
                    new() { "Chora-Zoi's Crystalline Pickaxe" },
                    new() { "Brilliant Pickaxe" },
                    new() { "Vrandtic Visionary's Pickaxe" },
                    new() { "Lodestar Pickaxe" },
                }
            },
            {
                this.BTN, new List<List<string>>
                {
                    new() { "Splendorous Hatchet" },
                    new() { "Augmented Splendorous Hatchet" },
                    new() { "Crystalline Hatchet" },
                    new() { "Chora-Zoi's Crystalline Hatchet" },
                    new() { "Brilliant Hatchet" },
                    new() { "Vrandtic Visionary's Hatchet" },
                    new() { "Lodestar Hatchet" },
                }
            },
            {
                this.FSH, new List<List<string>>
                {
                    new() { "Splendorous Fishing Rod" },
                    new() { "Augmented Splendorous Fishing Rod" },
                    new() { "Crystalline Fishing Rod" },
                    new() { "Chora-Zoi's Crystalline Fishing Rod" },
                    new() { "Brilliant Fishing Rod" },
                    new() { "Vrandtic Visionary's Fishing Rod" },
                    new() { "Lodestar Fishing Rod" },
                }
            }
        };

        stageQuests = new Dictionary<RelicToolType, uint>()
        {
            { RelicToolType.SplendorousBase, 70266 },
            { RelicToolType.SplendorousAugmented, 70267 },
            { RelicToolType.SplendorousCrystalline, 70268 },
            { RelicToolType.SplendorousChoraZoiCrystalline, 70304 },
            { RelicToolType.SplendorousBrilliant, 70305 },
            { RelicToolType.SplendorousVrandticVisionary, 70339 },
            { RelicToolType.SplendorousLodestar, 70340 }
        };

        foreach (var item in crafterGathererMap)
        {
            var stage = 1u;
            var originalRelicToolType = relicToolType;
            var previousWeapon = 0u;
            foreach(var items in item.Value)
            {
                uint item1;
                uint item2 = 0;
                if (!this.itemsByName.TryGetValue(items[0].ToParseable(), out item1))
                {
                    throw new Exception($"Unknown item: {items[0]}");
                }

                if (items.Count > 1)
                {
                    if (!this.itemsByName.TryGetValue(items[1].ToParseable(), out item2))
                    {
                        throw new Exception($"Unknown item: {items[1]}");
                    }
                }

                var weaponCategory = (RelicToolCategory)relicToolCategory;
                var weaponType = (RelicToolType)relicToolType;
                relicTools.Add(new RelicTool(rowId, item1, stage, item.Key.RowId, weaponCategory, weaponType, stageQuests.TryGetValue(weaponType, out var quest) ? quest : 0, previousWeapon));
                previousWeapon = rowId;
                relicToolType++;
                stage++;
                rowId++;
            }

            relicToolType = originalRelicToolType;
        }

        //Cosmic Tools
        relicToolType = (int)RelicToolType.CosmicPrototype01;
        relicToolCategory = (int)RelicToolCategory.Cosmic;

        crafterGathererMap = new Dictionary<ClassJob, List<List<string>>>()
        {
            {
                this.CRP, new List<List<string>>
                {
                    new() { "Cosmic Saw Prototype v0.1" },
                    new() { "Cosmic Saw Prototype v0.2" },
                    new() { "Cosmic Saw Prototype v0.3" },
                    new() { "Cosmic Saw Prototype v0.4" },
                    new() { "Cosmic Saw Prototype v0.5" },
                    new() { "Cosmic Saw Prototype v0.6" },
                    new() { "Cosmic Saw Prototype v0.7" },
                    new() { "Cosmic Saw Prototype v0.8" },
                    new() { "Cosmic Saw" },
                    new() { "Cosmic Saw v1.1" },
                    new() { "Cosmic Saw v1.2" },
                    new() { "Cosmic Saw v1.3" },
                    new() { "Cosmic Saw v1.4" },
                    new() { "Stellar Saw" },
                    new() { "Stellar Saw v1.1" },
                    new() { "Stellar Saw v1.2" },
                    new() { "Hypersaw" },
                }
            },
            {
                this.BSM, new List<List<string>>
                {
                    new() { "Cosmic Cross-pein Hammer Prototype v0.1" },
                    new() { "Cosmic Cross-pein Hammer Prototype v0.2" },
                    new() { "Cosmic Cross-pein Hammer Prototype v0.3" },
                    new() { "Cosmic Cross-pein Hammer Prototype v0.4" },
                    new() { "Cosmic Cross-pein Hammer Prototype v0.5" },
                    new() { "Cosmic Cross-pein Hammer Prototype v0.6" },
                    new() { "Cosmic Cross-pein Hammer Prototype v0.7" },
                    new() { "Cosmic Cross-pein Hammer Prototype v0.8" },
                    new() { "Cosmic Cross-pein Hammer" },
                    new() { "Cosmic Cross-pein Hammer v1.1" },
                    new() { "Cosmic Cross-pein Hammer v1.2" },
                    new() { "Cosmic Cross-pein Hammer v1.3" },
                    new() { "Cosmic Cross-pein Hammer v1.4" },
                    new() { "Stellar Cross-pein Hammer" },
                    new() { "Stellar Cross-pein Hammer v1.1" },
                    new() { "Stellar Cross-pein Hammer v1.2" },
                    new() { "Cross-pein Hyperhammer" },
                }
            },
            {
                this.ARM, new List<List<string>>
                {
                    new() { "Cosmic Raising Hammer Prototype v0.1" },
                    new() { "Cosmic Raising Hammer Prototype v0.2" },
                    new() { "Cosmic Raising Hammer Prototype v0.3" },
                    new() { "Cosmic Raising Hammer Prototype v0.4" },
                    new() { "Cosmic Raising Hammer Prototype v0.5" },
                    new() { "Cosmic Raising Hammer Prototype v0.6" },
                    new() { "Cosmic Raising Hammer Prototype v0.7" },
                    new() { "Cosmic Raising Hammer Prototype v0.8" },
                    new() { "Cosmic Raising Hammer" },
                    new() { "Cosmic Raising Hammer v1.1" },
                    new() { "Cosmic Raising Hammer v1.2" },
                    new() { "Cosmic Raising Hammer v1.3" },
                    new() { "Cosmic Raising Hammer v1.4" },
                    new() { "Stellar Raising Hammer" },
                    new() { "Stellar Raising Hammer v1.1" },
                    new() { "Stellar Raising Hammer v1.2" },
                    new() { "Raising Hyperhammer" },
                }
            },
            {
                this.GSM, new List<List<string>>
                {
                    new() { "Cosmic Mallet Prototype v0.1" },
                    new() { "Cosmic Mallet Prototype v0.2" },
                    new() { "Cosmic Mallet Prototype v0.3" },
                    new() { "Cosmic Mallet Prototype v0.4" },
                    new() { "Cosmic Mallet Prototype v0.5" },
                    new() { "Cosmic Mallet Prototype v0.6" },
                    new() { "Cosmic Mallet Prototype v0.7" },
                    new() { "Cosmic Mallet Prototype v0.8" },
                    new() { "Cosmic Mallet" },
                    new() { "Cosmic Mallet v1.1" },
                    new() { "Cosmic Mallet v1.2" },
                    new() { "Cosmic Mallet v1.3" },
                    new() { "Cosmic Mallet v1.4" },
                    new() { "Stellar Mallet" },
                    new() { "Stellar Mallet v1.1" },
                    new() { "Stellar Mallet v1.2" },
                    new() { "Hypermallet" },
                }
            },
            {
                this.LTW, new List<List<string>>
                {
                    new() { "Cosmic Round Knife v0.1" },
                    new() { "Cosmic Round Knife v0.2" },
                    new() { "Cosmic Round Knife v0.3" },
                    new() { "Cosmic Round Knife v0.4" },
                    new() { "Cosmic Round Knife v0.5" },
                    new() { "Cosmic Round Knife v0.6" },
                    new() { "Cosmic Round Knife v0.7" },
                    new() { "Cosmic Round Knife v0.8" },
                    new() { "Cosmic Round Knife" },
                    new() { "Cosmic Round Knife v1.1" },
                    new() { "Cosmic Round Knife v1.2" },
                    new() { "Cosmic Round Knife v1.3" },
                    new() { "Cosmic Round Knife v1.4" },
                    new() { "Stellar Round Knife" },
                    new() { "Stellar Round Knife v1.1" },
                    new() { "Stellar Round Knife v1.2" },
                    new() { "Round Hyperknife" },
                }
            },
            {
                this.WVR, new List<List<string>>
                {
                    new() { "Cosmic Needle v0.1" },
                    new() { "Cosmic Needle v0.2" },
                    new() { "Cosmic Needle v0.3" },
                    new() { "Cosmic Needle v0.4" },
                    new() { "Cosmic Needle v0.5" },
                    new() { "Cosmic Needle v0.6" },
                    new() { "Cosmic Needle v0.7" },
                    new() { "Cosmic Needle v0.8" },
                    new() { "Cosmic Needle" },
                    new() { "Cosmic Needle v1.1" },
                    new() { "Cosmic Needle v1.2" },
                    new() { "Cosmic Needle v1.3" },
                    new() { "Cosmic Needle v1.4" },
                    new() { "Stellar Needle" },
                    new() { "Stellar Needle v1.1" },
                    new() { "Stellar Needle v1.2" },
                    new() { "Hyperneedle" },
                }
            },
            {
                this.ALC, new List<List<string>>
                {
                    new() { "Cosmic Alembic Prototype v0.1" },
                    new() { "Cosmic Alembic Prototype v0.2" },
                    new() { "Cosmic Alembic Prototype v0.3" },
                    new() { "Cosmic Alembic Prototype v0.4" },
                    new() { "Cosmic Alembic Prototype v0.5" },
                    new() { "Cosmic Alembic Prototype v0.6" },
                    new() { "Cosmic Alembic Prototype v0.7" },
                    new() { "Cosmic Alembic Prototype v0.8" },
                    new() { "Cosmic Alembic" },
                    new() { "Cosmic Alembic v1.1" },
                    new() { "Cosmic Alembic v1.2" },
                    new() { "Cosmic Alembic v1.3" },
                    new() { "Cosmic Alembic v1.4" },
                    new() { "Stellar Alembic" },
                    new() { "Stellar Alembic v1.1" },
                    new() { "Stellar Alembic v1.2" },
                    new() { "Hyperalembic" },
                }
            },
            {
                this.CUL, new List<List<string>>
                {
                    new() { "Cosmic Frypan Prototype v0.1" },
                    new() { "Cosmic Frypan Prototype v0.2" },
                    new() { "Cosmic Frypan Prototype v0.3" },
                    new() { "Cosmic Frypan Prototype v0.4" },
                    new() { "Cosmic Frypan Prototype v0.5" },
                    new() { "Cosmic Frypan Prototype v0.6" },
                    new() { "Cosmic Frypan Prototype v0.7" },
                    new() { "Cosmic Frypan Prototype v0.8" },
                    new() { "Cosmic Frypan" },
                    new() { "Cosmic Frypan v1.1" },
                    new() { "Cosmic Frypan v1.2" },
                    new() { "Cosmic Frypan v1.3" },
                    new() { "Cosmic Frypan v1.4" },
                    new() { "Stellar Frypan" },
                    new() { "Stellar Frypan v1.1" },
                    new() { "Stellar Frypan v1.2" },
                    new() { "Hyperfrypan" },
                }
            },
            {
                this.MIN, new List<List<string>>
                {
                    new() { "Cosmic Pickaxe Prototype v0.1" },
                    new() { "Cosmic Pickaxe Prototype v0.2" },
                    new() { "Cosmic Pickaxe Prototype v0.3" },
                    new() { "Cosmic Pickaxe Prototype v0.4" },
                    new() { "Cosmic Pickaxe Prototype v0.5" },
                    new() { "Cosmic Pickaxe Prototype v0.6" },
                    new() { "Cosmic Pickaxe Prototype v0.7" },
                    new() { "Cosmic Pickaxe Prototype v0.8" },
                    new() { "Cosmic Pickaxe" },
                    new() { "Cosmic Pickaxe v1.1" },
                    new() { "Cosmic Pickaxe v1.2" },
                    new() { "Cosmic Pickaxe v1.3" },
                    new() { "Cosmic Pickaxe v1.4" },
                    new() { "Stellar Pickaxe" },
                    new() { "Stellar Pickaxe v1.1" },
                    new() { "Stellar Pickaxe v1.2" },
                    new() { "Hyperpickaxe" },
                }
            },
            {
                this.BTN, new List<List<string>>
                {
                    new() { "Cosmic Hatchet Prototype v0.1" },
                    new() { "Cosmic Hatchet Prototype v0.2" },
                    new() { "Cosmic Hatchet Prototype v0.3" },
                    new() { "Cosmic Hatchet Prototype v0.4" },
                    new() { "Cosmic Hatchet Prototype v0.5" },
                    new() { "Cosmic Hatchet Prototype v0.6" },
                    new() { "Cosmic Hatchet Prototype v0.7" },
                    new() { "Cosmic Hatchet Prototype v0.8" },
                    new() { "Cosmic Hatchet" },
                    new() { "Cosmic Hatchet v1.1" },
                    new() { "Cosmic Hatchet v1.2" },
                    new() { "Cosmic Hatchet v1.3" },
                    new() { "Cosmic Hatchet v1.4" },
                    new() { "Stellar Hatchet" },
                    new() { "Stellar Hatchet v1.1" },
                    new() { "Stellar Hatchet v1.2" },
                    new() { "Hyperhatchet" },
                }
            },
            {
                this.FSH, new List<List<string>>
                {
                    new() { "Cosmic Fishing Rod Prototype v0.1" },
                    new() { "Cosmic Fishing Rod Prototype v0.2" },
                    new() { "Cosmic Fishing Rod Prototype v0.3" },
                    new() { "Cosmic Fishing Rod Prototype v0.4" },
                    new() { "Cosmic Fishing Rod Prototype v0.5" },
                    new() { "Cosmic Fishing Rod Prototype v0.6" },
                    new() { "Cosmic Fishing Rod Prototype v0.7" },
                    new() { "Cosmic Fishing Rod Prototype v0.8" },
                    new() { "Cosmic Fishing Rod" },
                    new() { "Cosmic Fishing Rod v1.1" },
                    new() { "Cosmic Fishing Rod v1.2" },
                    new() { "Cosmic Fishing Rod v1.3" },
                    new() { "Cosmic Fishing Rod v1.4" },
                    new() { "Stellar Fishing Rod" },
                    new() { "Stellar Fishing Rod v1.1" },
                    new() { "Stellar Fishing Rod v1.2" },
                    new() { "Fishing Hyperrod" },
                }
            }
        };

        stageQuests = new Dictionary<RelicToolType, uint>()
        {
            { RelicToolType.CosmicPrototype01, 70789 },
        };

        foreach (var item in crafterGathererMap)
        {
            var stage = 1u;
            var originalRelicToolType = relicToolType;
            var previousWeapon = 0u;
            foreach(var items in item.Value)
            {
                uint item1;
                uint item2 = 0;
                if (!this.itemsByName.TryGetValue(items[0].ToParseable(), out item1))
                {
                    throw new Exception($"Unknown item: {items[0]}");
                }

                if (items.Count > 1)
                {
                    if (!this.itemsByName.TryGetValue(items[1].ToParseable(), out item2))
                    {
                        throw new Exception($"Unknown item: {items[1]}");
                    }
                }

                var weaponCategory = (RelicToolCategory)relicToolCategory;
                var weaponType = (RelicToolType)relicToolType;
                relicTools.Add(new RelicTool(rowId, item1, stage, item.Key.RowId, weaponCategory, weaponType, stageQuests.TryGetValue(weaponType, out var quest) ? quest : 0, previousWeapon));
                previousWeapon = rowId;
                relicToolType++;
                stage++;
                rowId++;
            }

            relicToolType = originalRelicToolType;
        }

        return relicTools;
    }

    public override List<ICsv> Run(Dictionary<Type, List<ICsv>> stepData)
    {
        List<RelicTool> items = new List<RelicTool>();
        items.AddRange(this.ProcessTools());

        return [..items.Select(c => c)];
    }
}
