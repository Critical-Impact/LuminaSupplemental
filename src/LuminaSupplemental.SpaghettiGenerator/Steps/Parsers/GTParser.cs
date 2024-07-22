using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Numerics;

using Lumina.Excel.GeneratedSheets;

using LuminaSupplemental.Excel.Model;
using LuminaSupplemental.SpaghettiGenerator.Generator;
using LuminaSupplemental.SpaghettiGenerator.Model;

using Newtonsoft.Json;

namespace LuminaSupplemental.SpaghettiGenerator.Steps.Parsers;

public class GTParser
{
    public List<DungeonChest> DungeonChests = [];
    public List<DungeonChestItem> DungeonChestItems = [];
    public List<DungeonBoss> DungeonBosses = [];
    public List<DungeonBossChest> DungeonBossChests = [];
    public List<DungeonBossDrop> DungeonBossDrops = [];
    
    private readonly DataCacher dataCacher;
    private readonly Dictionary<string, ContentFinderCondition> dutiesByString;
    private readonly Dictionary<string, Item> itemsByName;
    private readonly Dictionary<string, BNpcName> bNpcsByName;
    private bool processed = false;

    public GTParser(DataCacher dataCacher)
    {
        this.dataCacher = dataCacher;
        var bannedItems = new HashSet<uint>() { 0, 24225 };
        this.dutiesByString = this.dataCacher.ByName<ContentFinderCondition>(item => item.Name.ToString().ToParseable());
        this.bNpcsByName = this.dataCacher.ByName<BNpcName>(item => item.Singular.ToString().ToParseable());
        this.itemsByName = this.dataCacher.ByName<Item>(item => item.Name.ToString().ToParseable(), item => !bannedItems.Contains(item.RowId));
    }

    public void ProcessDutiesJson()
    {
        if (this.processed)
        {
            return;
        }

        this.processed = true;
        var dungeonBossCount = 1u;
        var dungeonBossChestCount = 1u;

        var dutyText = File.ReadAllText(@"FFXIV Data - Duties.json");
        JsonConverter[] converters =
        {
            new CategoryConverter(), new ConditionConverter(), new VersionConverter(), new ChestEnumConverter(), new ChestNameConverter(),
            new IlvlEnumConverter(), new IlvlUnionConverter(), new TokenNameConverter()
        };
        var duties = JsonConvert.DeserializeObject<DutyJson[]>(dutyText, converters)!;
        foreach (var duty in duties)
        {
            var dutyName = duty.Name.ToParseable();
            if (this.dutiesByString.ContainsKey(dutyName))
            {
                var actualDuty = this.dutiesByString[dutyName];
                if (duty.Fights != null)
                {
                    for (var index = 0; index < duty.Fights.Length; index++)
                    {
                        var fight = duty.Fights[index];
                        foreach (var boss in fight.Boss)
                        {
                            var bossName = boss.Name.ToParseable();
                            if (this.bNpcsByName.ContainsKey(bossName))
                            {
                                var bnpc = this.bNpcsByName[bossName];
                                this.DungeonBosses.Add(new DungeonBoss(dungeonBossCount, bnpc.RowId, (uint)index, actualDuty.RowId));
                                dungeonBossCount++;
                            }
                            else
                            {
                                Console.WriteLine("Could not parse boss with name: " + boss.Name);
                            }
                        }

                        if (fight.Drops != null)
                        {
                            foreach (var drop in fight.Drops)
                            {
                                var itemName = drop.Name.ToParseable();
                                var actualItem = this.itemsByName.ContainsKey(itemName) ? this.itemsByName[itemName] : null;
                                if (actualItem != null && actualItem.RowId != 0)
                                {
                                    this.DungeonBossDrops.Add(
                                        new DungeonBossDrop((uint)(this.DungeonBossDrops.Count + 1), actualDuty.RowId, (uint)index, actualItem.RowId, 1));
                                }
                                else
                                {
                                    Console.WriteLine("Could not find item with name " + itemName + " in duty " + duty.Name);
                                }
                            }
                        }

                        if (fight.Treasures != null)
                        {
                            foreach (var treasure in fight.Treasures)
                            {
                                var chestNo = (uint)treasure.Name;
                                foreach (var item in treasure.Items)
                                {
                                    var itemName = item.ToParseable();
                                    var actualItem = this.itemsByName.ContainsKey(itemName) ? this.itemsByName[itemName] : null;
                                    if (actualItem != null)
                                    {
                                        this.DungeonBossChests.Add(
                                            new DungeonBossChest(dungeonBossChestCount, (uint)index, actualItem.RowId, actualDuty.RowId, 1, chestNo));
                                        dungeonBossChestCount++;
                                    }
                                    else
                                    {
                                        Console.WriteLine("Could not find item with name " + item + " in duty " + duty.Name);
                                    }
                                }
                            }
                        }
                    }
                }

                if (duty.Chests != null)
                {
                    for (var index = 0; index < duty.Chests.Length; index++)
                    {
                        var chest = duty.Chests[index];
                        var xCoord = float.Parse(chest.Coords.X, CultureInfo.InvariantCulture);
                        var yCoord = float.Parse(chest.Coords.Y, CultureInfo.InvariantCulture);
                        var dungeonChest = new DungeonChest((uint)(this.DungeonChests.Count + 1), (byte)(index + 1), actualDuty.RowId, new Vector2(xCoord, yCoord));
                        this.DungeonChests.Add(dungeonChest);
                        foreach (var item in chest.Items)
                        {
                            var itemName = item.ToParseable();
                            var actualItem = this.itemsByName.ContainsKey(itemName) ? this.itemsByName[itemName] : null;
                            if (actualItem != null)
                            {
                                var chestItem = new DungeonChestItem((uint)(this.DungeonChestItems.Count + 1), actualItem.RowId, dungeonChest.RowId);
                                this.DungeonChestItems.Add(chestItem);
                            }
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("Could not find duty " + duty.Name);
            }
        }
    }
}
