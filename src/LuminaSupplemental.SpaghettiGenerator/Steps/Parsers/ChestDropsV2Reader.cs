using System.Collections.Generic;
using System.IO;
using System.Linq;

using Newtonsoft.Json;

using SupabaseExporter.Structures.Exports;

namespace LuminaSupplemental.SpaghettiGenerator.Steps.Parsers;

public static class ChestDropsV2Reader
{
    private const string FilePath = "../../../../FFXIVGachaSpreadsheet/website/static/data/ChestDropsV2.json";

    public static List<(uint DutyId, List<ChestDrop.Chest> Chests)> Load()
    {
        var json = File.ReadAllText(FilePath);
        var chestList = JsonConvert.DeserializeObject<List<ChestDropWithPatch>>(json)!;

        var duties = new List<(uint DutyId, List<ChestDrop.Chest> Chests)>();
        foreach (var chestDrop in chestList)
        {
            foreach (var expansion in chestDrop.Expansions)
            {
                foreach (var header in expansion.Headers)
                {
                    foreach (var duty in header.Duties)
                    {
                        duties.Add((duty.Id, MergePatches(duty)));
                    }
                }
            }
        }

        return duties;
    }

    private static List<ChestDrop.Chest> MergePatches(ChestDropWithPatch.Duty duty)
    {
        var chests = new Dictionary<uint, ChestDrop.Chest>();
        var rewards = new Dictionary<uint, Dictionary<uint, Reward>>();
        foreach (var patchChests in duty.Chests.Values)
        {
            foreach (var chest in patchChests)
            {
                if (!chests.TryGetValue(chest.Id, out var merged))
                {
                    merged = new ChestDrop.Chest()
                    {
                        Id = chest.Id,
                        Name = chest.Name,
                        MapId = chest.MapId,
                        TerritoryId = chest.TerritoryId,
                        PlaceNameSub = chest.PlaceNameSub,
                        Position = chest.Position
                    };
                    chests[chest.Id] = merged;
                    rewards[chest.Id] = new Dictionary<uint, Reward>();
                }

                merged.Records += chest.Records;
                var chestRewards = rewards[chest.Id];
                foreach (var reward in chest.Rewards)
                {
                    chestRewards[reward.Id] = chestRewards.TryGetValue(reward.Id, out var existing)
                        ? existing with
                        {
                            Amount = existing.Amount + reward.Amount,
                            Total = existing.Total + reward.Total,
                            Min = System.Math.Min(existing.Min, reward.Min),
                            Max = System.Math.Max(existing.Max, reward.Max)
                        }
                        : reward;
                }
            }
        }

        foreach (var chest in chests.Values)
        {
            chest.Rewards = rewards[chest.Id].Values
                .Select(r => r with { Pct = r.Amount / (double)chest.Records })
                .ToList();
        }

        return chests.Values.OrderBy(c => c.MapId).ThenBy(c => c.Id).ToList();
    }
}
