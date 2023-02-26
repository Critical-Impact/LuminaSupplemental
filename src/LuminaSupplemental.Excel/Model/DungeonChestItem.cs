using System;
using System.Numerics;
using CsvHelper.Configuration.Attributes;
using Lumina;
using Lumina.Data;
using Lumina.Excel;
using Lumina.Excel.GeneratedSheets;

namespace LuminaSupplemental.Excel.Model
{
    public class DungeonChestItem : ICsv
    {
        [Name("RowId")] public uint RowId { get; set; }
        [Name("ItemId")] public uint ItemId { get; set; }
        [Name("ChestId")] public uint ChestId { get; set; }
        
        public LazyRow< Item > Item;

        public DungeonChestItem( uint rowId, uint itemId, uint chestId)
        {
            RowId = rowId;
            ItemId = itemId;
            ChestId = chestId;
        }

        public DungeonChestItem()
        {
            
        }

        public void FromCsv( string[] lineData )
        {
            RowId = uint.Parse( lineData[ 0 ] );
            ItemId = uint.Parse( lineData[ 1 ] );
            ChestId = uint.Parse( lineData[ 2 ] );
        }

        public string[] ToCsv()
        {
            return Array.Empty<string>();
        }

        public bool IncludeInCsv()
        {
            return false;
        }

        public virtual void PopulateData( GameData gameData, Language language )
        {
            Item = new LazyRow< Item >( gameData, ItemId, language );
        }
    }
}