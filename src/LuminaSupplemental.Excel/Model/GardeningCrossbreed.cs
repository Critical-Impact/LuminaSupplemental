using System.Globalization;

using CsvHelper.Configuration.Attributes;

using Lumina.Data;
using Lumina.Excel;
using Lumina.Excel.Sheets;

namespace LuminaSupplemental.Excel.Model;

public class GardeningCrossbreed : ICsv
{
    [Name("ItemResultId")] public uint ItemResultId { get; set; }
    [Name("ItemRequirement1Id")] public uint ItemRequirement1Id { get; set; }
    [Name("ItemRequirement2Id")] public uint ItemRequirement2Id { get; set; }

    public RowRef<Item> ItemResult;

    public RowRef<Item> ItemRequirement1;
    public RowRef<Item> ItemRequirement2;

    public GardeningCrossbreed(uint itemResultId, uint itemRequirement1Id, uint itemRequirement2Id )
    {
        ItemResultId = itemResultId;
        ItemRequirement1Id = itemRequirement1Id;
        ItemRequirement2Id = itemRequirement2Id;
    }

    public GardeningCrossbreed()
    {

    }

    public void FromCsv(string[] lineData)
    {
        ItemResultId = uint.Parse( lineData[ 0 ] );
        ItemRequirement1Id = uint.Parse( lineData[ 1 ] );
        ItemRequirement2Id = uint.Parse( lineData[ 2 ] );
    }

    public string[] ToCsv()
    {
        return [ItemResultId.ToString(CultureInfo.InvariantCulture), ItemRequirement1Id.ToString(CultureInfo.InvariantCulture), ItemRequirement2Id.ToString(CultureInfo.InvariantCulture)
        ];
    }

    public bool IncludeInCsv()
    {
        return true;
    }

    public void PopulateData(ExcelModule gameData, Language language)
    {
        ItemResult = new RowRef<Item>( gameData, ItemResultId, language);
        ItemRequirement1 = new RowRef<Item>( gameData, ItemRequirement1Id, language);
        ItemRequirement2 = new RowRef<Item>( gameData, ItemRequirement2Id, language);
    }
}
