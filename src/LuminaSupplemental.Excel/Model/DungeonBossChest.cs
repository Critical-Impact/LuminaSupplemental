using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using CsvHelper;
using CsvHelper.Configuration.Attributes;

namespace LuminaSupplemental.Excel.Model
{
    public struct DungeonBossChest : ICsv
    {
        [Name("RowId")] public uint RowId { get; set; }
        [Name("ItemId")] public uint ItemId { get; set; }
        [Name("ContentFinderConditionId")] public uint ContentFinderConditionId { get; set; }
        [Name("Quantity")] public uint Quantity { get; set; }
        [Name("FightNo")] public uint FightNo { get; set; }
        [Name("CofferNo")] public uint CofferNo { get; set; }

        public DungeonBossChest(uint rowId, uint fightNo, uint itemId, uint contentFinderConditionId, uint quantity, uint cofferNo )
        {
            RowId = rowId;
            ItemId = itemId;
            ContentFinderConditionId = contentFinderConditionId;
            Quantity = quantity;
            FightNo = fightNo;
            CofferNo = cofferNo;
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
            throw new NotImplementedException();
        }
    }
}