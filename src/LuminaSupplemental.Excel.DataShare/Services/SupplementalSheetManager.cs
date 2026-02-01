using System;
using System.Collections.Generic;
using System.Reflection;

using Dalamud.Plugin;
using Dalamud.Plugin.Services;

using Lumina;
using Lumina.Data;

using LuminaSupplemental.Excel.DataShare.Sheets;

namespace LuminaSupplemental.Excel.DataShare.Services;

public class SupplementalSheetManager(IDalamudPluginInterface pluginInterface, GameData gameData, IPluginLog pluginLog, Language? language = null) : IDisposable
{
    private readonly IPluginLog pluginLog = pluginLog;
    private readonly Dictionary<Type, object> sheetCache = new();

    public delegate SupplementalSheetManager Factory(Language language);

    public TSheet GetSheet<TSheet>()
        where TSheet : BaseSupplementalSheet
    {
        var sheetType = typeof(TSheet);

        if (this.sheetCache.TryGetValue(sheetType, out var cached))
            return (TSheet)cached;

        var sheet = (TSheet)this.CreateSheet(sheetType);

        sheet.Module = gameData.Excel;
        sheet.Language = language ?? gameData.Options.DefaultExcelLanguage;
        sheet.PluginInterface = pluginInterface;

        this.sheetCache[sheetType] = sheet;
        return sheet;
    }

    private object CreateSheet(Type sheetType)
    {
        var baseType = sheetType;
        while (baseType != null &&
               (!baseType.IsGenericType ||
                baseType.GetGenericTypeDefinition() != typeof(SupplementalSheet<,>)))
            baseType = baseType.BaseType;

        if (baseType == null)
        {
            throw new InvalidOperationException(
                $"{sheetType} does not inherit from SupplementalSheet<,>.");
        }

        return Activator.CreateInstance(
                   sheetType,
                   BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                   null,
                   [],
                   null
               ) ?? throw new InvalidOperationException(
                   $"Failed to create instance of {sheetType}.");
    }

    public void Dispose()
    {
        foreach (var sheet in this.sheetCache)
        {
            var supplementalSheet = (BaseSupplementalSheet)sheet.Value;
            var dataName = "Lumina.Supplemental.DataShare." + supplementalSheet.SheetName + ".V" + supplementalSheet.Version;
            pluginInterface.RelinquishData(dataName);
            pluginLog.Verbose("Relinquished " + dataName);
        }
    }
}
