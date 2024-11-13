using System;
using CsvHelper.Configuration.Attributes;
using Lumina.Data;
using Lumina.Excel;
using Lumina.Excel.Sheets;

namespace LuminaSupplemental.Excel.Model
{
    public class DungeonBossChest : ICsv
    {
        [Name("RowId")] public uint RowId { get; set; }
        [Name("ItemId")] public uint ItemId { get; set; }
        [Name("ContentFinderConditionId")] public uint ContentFinderConditionId { get; set; }
        [Name("Quantity")] public uint Quantity { get; set; }
        [Name("FightNo")] public uint FightNo { get; set; }
        [Name("CofferNo")] public uint CofferNo { get; set; }

        public RowRef< Item > Item;

        public RowRef< ContentFinderCondition > ContentFinderCondition;

        public DungeonBossChest(uint rowId, uint fightNo, uint itemId, uint contentFinderConditionId, uint quantity, uint cofferNo )
        {
            RowId = rowId;
            ItemId = itemId;
            ContentFinderConditionId = contentFinderConditionId;
            Quantity = quantity;
            FightNo = fightNo;
            CofferNo = cofferNo;
        }

        public DungeonBossChest()
        {

        }

        public void FromCsv(string[] lineData)
        {
            RowId = uint.Parse( lineData[ 0 ] );
            ItemId = uint.Parse( lineData[ 1 ] );
            ContentFinderConditionId = uint.Parse( lineData[ 2 ] );
            Quantity = uint.Parse( lineData[ 3 ] );
            FightNo = uint.Parse( lineData[ 4 ] );
            CofferNo = uint.Parse( lineData[ 5 ] );
        }

        public string[] ToCsv()
        {
            return Array.Empty<string>();
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
