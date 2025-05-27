using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Numerics;

using Lumina.Excel;
using Lumina.Excel.Sheets;

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
    private readonly ExcelSheet<ContentFinderCondition> contentFinderConditionSheet;
    private readonly ExcelSheet<BNpcName> bNpcNameSheet;
    private readonly ExcelSheet<Item> itemSheet;
    private readonly Dictionary<string, uint> dutiesByString;
    private readonly Dictionary<string, uint> itemsByName;
    private readonly Dictionary<string, uint> bNpcsByName;
    private bool processed = false;

    public GTParser(DataCacher dataCacher, ExcelSheet<ContentFinderCondition> contentFinderConditionSheet, ExcelSheet<BNpcName> bNpcNameSheet, ExcelSheet<Item> itemSheet)
    {
        this.dataCacher = dataCacher;
        this.contentFinderConditionSheet = contentFinderConditionSheet;
        this.bNpcNameSheet = bNpcNameSheet;
        this.itemSheet = itemSheet;
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

        var dutyText = File.ReadAllText(Path.Join("ManualData", "FFXIV Data - Duties.json"));
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
                var actualDuty = this.contentFinderConditionSheet.GetRow(this.dutiesByString[dutyName]);
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
                                var bnpc = this.bNpcNameSheet.GetRow(this.bNpcsByName[bossName]);
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
                                Item? actualItem = this.itemsByName.ContainsKey(itemName) ? this.itemSheet.GetRow(this.itemsByName[itemName]) : null;
                                if (actualItem != null && actualItem.Value.RowId != 0)
                                {
                                    this.DungeonBossDrops.Add(
                                        new DungeonBossDrop((uint)(this.DungeonBossDrops.Count + 1), actualDuty.RowId, (uint)index, actualItem.Value.RowId, 1));
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
                                    Item? actualItem = this.itemsByName.ContainsKey(itemName) ? this.itemSheet.GetRow(this.itemsByName[itemName]) : null;
                                    if (actualItem != null)
                                    {
                                        this.DungeonBossChests.Add(
                                            new DungeonBossChest(dungeonBossChestCount, (uint)index, actualItem.Value.RowId, actualDuty.RowId, 1, chestNo));
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
                            Item? actualItem = this.itemsByName.ContainsKey(itemName) ? this.itemSheet.GetRow(this.itemsByName[itemName]) : null;
                            if (actualItem != null)
                            {
                                var chestItem = new DungeonChestItem((uint)(this.DungeonChestItems.Count + 1), actualItem.Value.RowId, dungeonChest.RowId);
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
