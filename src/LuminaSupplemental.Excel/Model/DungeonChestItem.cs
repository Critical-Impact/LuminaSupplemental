using System.Numerics;
using CsvHelper.Configuration.Attributes;

namespace LuminaSupplemental.Excel.Model
{
    public struct DungeonChestItem : ICsv
    {
        [Name("RowId")] public uint RowId { get; set; }
        [Name("ItemId")] public uint ItemId { get; set; }
        [Name("ChestId")] public uint ChestId { get; set; }

        public DungeonChestItem( uint rowId, uint itemId, uint chestId)
        {
            RowId = rowId;
            ItemId = itemId;
            ChestId = chestId;
        }

        public void FromCsv( string[] lineData )
        {
            RowId = uint.Parse( lineData[ 0 ] );
            ItemId = uint.Parse( lineData[ 1 ] );
            ChestId = uint.Parse( lineData[ 2 ] );
        }
    }
}