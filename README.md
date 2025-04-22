# Lumina Supplemental

[![NuGet](https://img.shields.io/nuget/v/LuminaSupplemental.Excel.svg)](https://www.nuget.org/packages/LuminaSupplemental.Excel)

Lumina Supplemental is an additional library that provides additional data that the game's data sheets do not provide.

While this is not an exhaustive list(as more gets added all the time), it provides

* Airship/Submarine Unlocks
* Airship/Submarine Voyage Items
* Dungeon Drops
* Dungeon Boss Drops
* Dungeon Chest Items
* Extra Event NPC Locations
* Extra Event NPC Shops
* Item Patch Data
* Supplemental Item Data(Desynth,Reduction,Loot,Gardening,Coffer Contents)
* Mob Drop Data(This is user sourced, if you want to help out please check out Allagan Tool's mob tracker)
* Mob Spawn Data(As above)
* Retainer Venture Items
* Extra Shop Names
* SQ Paid Shop Items

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