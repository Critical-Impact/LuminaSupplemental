# Lumina Supplemental

Lumina Supplemental is an additional library that provides additional data that the game's data sheets do not provide.

| Sheet               | Description                                                                                    | Source                                       |
|---------------------|------------------------------------------------------------------------------------------------|----------------------------------------------|
| AirshipDrop         | Items that drop on specific airship routes                                                     | Manual + Gubal                               |
| AirshipUnlock       | The route required to be run to unlock a route                                                 | Manual                                       |
| BNpcLink            | The association between BNpcName and BNpcBase                                                  | Tracky + manual                              |
| DungeonBoss         | A list of bosses, which duty they show up in and which fight                                   | Manual                                       |
| DungeonBossChest    | Items that drop from duty boss chests                                                          | Manual                                       |
| DungeonBossDrop     | Items that drop from a duty boss                                                               | Manual                                       |
| DungeonChest        | Chests that show up inside a duty                                                              | Tracky                                       |
| DungeonChestItem    | Items which show up inside chests inside a duty                                                | Tracky                                       |
| DungeonDrop         | Items which drop within a duty(generic)                                                        | Manual                                       |
| ENpcPlace           | Extra NPC locations that are not stored within the game's sheets                               | Manual                                       |
| ENpcShop            | Extra shop mapping data that is not stored within the game's sheets                            | Manual                                       |
| FateItem            | Items that can be earned from a fate                                                           | Gubal                                        |
| FauxHollowsPattern  | Community-maintained Faux Hollows board layouts                                                | Manual                                       |
| FestivalName        | The names for festivals are not stored within the game, this associates a festival with a name | Manual                                       |
| FieldOpCoffer       | Items that drop from field op chests(pagos, etc)                                               | Tracky                                       |
| GardeningCrossbreed | Items gained by crossbreeding seeds                                                            | Manual                                       |
| Gearset             | Armor/weapon/accessories that belong to sets                                                   | Lodestone                                    |
| HouseVendor         | A sheet that helps deduplicate housing vendors                                                 | Manual                                       |
| ItemPatch           | The patch in which an item was introduced                                                      | xivapi datamining-patches                    |
| ItemSupplement      | Contains general information about pairs of items(desynth, reduction, coffers, etc)            | Lodestone, Gubal, Tracky, manual & game data |
| MobDrop             | Items that can be sourced from monsters                                                        | Lodestone + Gubal                            |
| MobSpawn            | Where monsters spawn                                                                           | Tracky + manual                              |
| QuestRequiredItem   | The items required for quests are baked into the game's LUA, this is data extracted from that  | Manual (extracted from game LUA)             |
| RetainerVentureItem | Items that can be gained from sending a retainer on a venture(exploration + quick)             | Manual                                       |
| ShopName            | Extra shop names when the shop is blank in the sheets                                          | Game data + manual                           |
| StoreItem           | Items that can be obtained through the SQ shop for $                                           | Square Enix Store                            |
| SubmarineDrop       | Items that drop on specific submarine routes                                                   | Tracky                                       |
| SubmarineUnlock     | The route required to be run to unlock a route                                                 | Tracky                                       |
| UnobtainableItem    | Items that can no longer be obtained by any means                                              | Game data + manual                           |

Data sources referenced above:

| Source                    | Description                                                                                            |
|---------------------------|-------------------------------------------------------------------------------------------------------|
| Manual                    | Hand-maintained data kept in this repository's `ManualData` directory                                  |
| Game data                 | Derived from the game's own Excel sheets                                                               |
| Gubal                     | The Gubal (FFXIV Teamcraft) Allagan Reports database                                                   |
| Tracky                    | Community-collected data from the [FFXIVGachaSpreadsheet](https://github.com/Infiziert90/FFXIVGachaSpreadsheet) project |
| Lodestone                 | Scraped from the Lodestone / Eorzea Database                                                           |
| Square Enix Store         | The official Square Enix online store API                                                              |
| xivapi datamining-patches | xivapi's [ffxiv-datamining-patches](https://github.com/xivapi/ffxiv-datamining-patches) dataset        |

`FauxHollowsPattern` is sourced from u/Ylandah's community spreadsheet via the
[ffxiv-faux-hollows](https://github.com/JoshuaEN/ffxiv-faux-hollows) project.

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
