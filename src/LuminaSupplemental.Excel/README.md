# Lumina Supplemental

Lumina Supplemental is an additional library that provides additional data that the game's data sheets do not provide.

| Sheet               | Description                                                                                    |
|---------------------|------------------------------------------------------------------------------------------------|
| AirshipDrop         | Item's that drop on specific airship routes                                                    |
| AirshipUnlock       | The route required to be run to unlock a route                                                 |
| BNpcLink            | The association between BNpcName and BNpcBase, sourced from gubal, mappy and manual data       |
| BNpcLinkNoGubal     | The association between BNpcName and BNpcBase, sourced from mappy and manual data              |
| DungeonBoss         | A list of bosses, which duty they show up in and which fight                                   |
| DungeonBossChest    | Items that drop from duty boss chests                                                          |
| DungeonBossDrop     | Items that drop from a duty boss                                                               |
| DungeonChest        | Chests that show up inside a duty                                                              |
| DungeonChestItem    | Items which show up inside chests inside a duty                                                |
| DungeonDrop         | Items which drop within a duty(generic)                                                        |
| ENpcPlace           | Extra NPC locations that are not stored within the game's sheets                               |
| ENpcShop            | Extra shop mapping data that is not stored within the game's sheets                            |
| FateItem            | Item's that can be earned from a fate                                                          |
| FestivalName        | The names for festivals are not stored within the game, this associates a festival with a name |
| FieldOpCoffer       | Items that drop from field op chests(pagos, etc)                                               |
| GardeningCrossbreed | Items gained by crossbreeding seeds                                                            |
| Gearset             | Armor/weapon/accessories that belong to sets                                                   |
| HouseVendor         | A sheet that helps deduplicate housing vendors                                                 |
| ItemPatch           | The patch in which an item was introduced                                                      |
| ItemSupplement      | Contains general information about pairs of items(desynth, reduction, coffers, etc)            |
| MobDrop             | Items that can be sourced from monsters                                                        |
| MobSpawn            | Where monsters spawn                                                                           |
| QuestRequiredItem   | The items required for quests are baked into the game's LUA, this is data extracted from that  |
| RetainerVentureItem | Items that can be gained from sending a retainer on a venture(exploration + quick)             |
| ShopName            | Extra shop names when the shop is blank in the sheets                                          |
| StoreItem           | Items that can be obtained through the SQ shop for $                                           |
| SubmarineDrop       | Items that drop on specific submarine routes                                                   |
| SubmarineUnlock     | The route required to be run to unlock a route                                                 |


To load a specific CSV (Dungeon Bosses as an example):

```
try
{
    var lines = CsvLoader.LoadResource<DungeonBoss>(CsvLoader.DungeonBossResourceName, out var failedLines, GameData, GameData.Options.DefaultExcelLanguage);
    if (failedLines.Count != 0)
    {
        foreach (var failedLine in failedLines)
        {
            PluginLog.Error("Failed to load line from " + title + ": " + failedLine);
        }
    }
    return lines;
}
catch (Exception e)
{
    PluginLog.Error("Failed to load " + title);
    PluginLog.Error(e.Message);
}
return new List<T>();
```

Feel free to submit a PR if any of the data is wrong or you have additional data that could be added.
