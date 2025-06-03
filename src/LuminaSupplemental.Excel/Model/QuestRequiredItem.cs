using System.Globalization;

using CsvHelper.Configuration.Attributes;

using Lumina.Data;
using Lumina.Excel;
using Lumina.Excel.Sheets;

namespace LuminaSupplemental.Excel.Model;

public class QuestRequiredItem : ICsv
{
    [Name("ItemId")] public uint ItemId { get; set; }
    [Name("QuestId")] public uint QuestId { get; set; }
    [Name("Quantity")] public uint Quantity { get; set; }
    [Name("IsHq")] public bool IsHq { get; set; }

    public RowRef< Item > Item;
    public RowRef< Quest > Quest;

    public void FromCsv(string[] lineData)
    {
        ItemId = uint.Parse( lineData[ 0 ], CultureInfo.InvariantCulture );
        QuestId = uint.Parse( lineData[ 1 ], CultureInfo.InvariantCulture );
        Quantity = uint.Parse( lineData[ 2 ], CultureInfo.InvariantCulture );
        IsHq = bool.Parse( lineData[ 3 ] );
    }

    public string[] ToCsv()
    {
        return new[]
        {
            ItemId.ToString(),
            QuestId.ToString(),
            Quantity.ToString(),
            IsHq.ToString()
        };
    }

    public bool IncludeInCsv()
    {
        return true;
    }

    public void PopulateData(ExcelModule gameData, Language language)
    {
        Item = new RowRef< Item >( gameData, ItemId);
        Quest = new RowRef< Quest >( gameData, QuestId);
    }
}
