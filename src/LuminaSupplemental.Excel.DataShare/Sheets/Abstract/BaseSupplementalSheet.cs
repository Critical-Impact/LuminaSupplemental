using Dalamud.Plugin;

using Lumina.Data;
using Lumina.Excel;

namespace LuminaSupplemental.Excel.DataShare.Sheets;

public abstract class BaseSupplementalSheet
{
    internal ExcelModule Module { get; set; } = null!;
    
    internal Language Language { get; set; }
    
    internal IDalamudPluginInterface PluginInterface { get; set; } = null!;
    
    public abstract string SheetName { get; }
    
    public abstract int Version { get; }
    
    public abstract string ResourceName { get; }
}
