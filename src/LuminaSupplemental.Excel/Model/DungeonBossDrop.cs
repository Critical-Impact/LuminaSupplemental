using System;
using CsvHelper.Configuration.Attributes;
using Lumina;
using Lumina.Data;
using Lumina.Excel;

using Lumina.Excel.Sheets;

namespace LuminaSupplemental.Excel.Model
{
    public class DungeonBossDrop : ICsv
    {
        [Name("RowId")]  public uint RowId { get; set; }
        [Name("ContentFinderConditionId")] public uint ContentFinderConditionId { get; set; }
        [Name("FightNo")] public uint FightNo { get; set; }
        [Name("ItemId")] public uint ItemId { get; set; }
        [Name("Quantity")] public uint Quantity { get; set; }
        
        public RowRef< Item > Item;
        
        public RowRef< ContentFinderCondition > ContentFinderCondition;

        public DungeonBossDrop( uint rowId, uint contentFinderConditionId, uint fightNo, uint itemId, uint quantity )
        {
            RowId = rowId;
            ContentFinderConditionId = contentFinderConditionId;
            FightNo = fightNo;
            ItemId = itemId;
            Quantity = quantity;
        }

        public DungeonBossDrop()
        {
            
        }

        public void FromCsv( string[] lineData )
        {
            RowId = uint.Parse( lineData[ 0 ] );
            ContentFinderConditionId = uint.Parse( lineData[ 1 ] );
            FightNo = uint.Parse( lineData[ 2 ] );
            ItemId = uint.Parse( lineData[ 3 ] );
            Quantity = uint.Parse( lineData[ 4 ] );
        }

        public string[] ToCsv()
        {
            return Array.Empty< string >();
        }

        public bool IncludeInCsv()
        {
            return false;
        }

        public virtual void PopulateData( ExcelModule module, Language language )
        {
            Item = new RowRef< Item >( module, ItemId);
            ContentFinderCondition = new RowRef< ContentFinderCondition >( module, ContentFinderConditionId);
        }
    }
}
