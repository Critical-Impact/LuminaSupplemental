using System;
using System.Collections.Generic;
using System.Linq;

using Lumina;
using Lumina.Excel;

namespace LuminaSupplemental.SpaghettiGenerator.Generator;

public class DataCacher
{
    private readonly GameData gameData;

    public DataCacher(GameData gameData)
    {
        this.gameData = gameData;
    }

    private Dictionary<Type, Dictionary<string, ExcelRow>> dataCache = new();

    public Dictionary<string, T> ByName<T>(Func<T, string> nameFunc, Func<T, bool>? acceptFunc = null) where T : ExcelRow
    {
        var rowType = typeof(T);
        if (!this.dataCache.ContainsKey(rowType))
        {
            Dictionary<string, ExcelRow> results = new();
            var byName = this.gameData.GetExcelSheet<T>()!.Where(c => acceptFunc?.Invoke(c) ?? true);
            foreach (var item in byName)
            {
                results.TryAdd(nameFunc.Invoke(item), item);
            }
            this.dataCache[rowType] = results;
        }
        return this.dataCache[rowType].ToDictionary(c => c.Key, c => c.Value as T)!;
    }
}
