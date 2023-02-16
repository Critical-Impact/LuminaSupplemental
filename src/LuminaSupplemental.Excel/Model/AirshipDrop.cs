using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using CsvHelper;
using CsvHelper.Configuration.Attributes;

namespace LuminaSupplemental.Excel.Model
{
    public struct AirshipDrop : ICsv
    {
        [Name("RowId")] public uint RowId { get; set; }
        [Name("ItemId")] public uint ItemId { get; set; }
        [Name("AirshipExplorationPointId")] public uint AirshipExplorationPointId { get; set; }

        public AirshipDrop(uint rowId, uint itemId, uint airshipExplorationPointId )
        {
            RowId = rowId;
            ItemId = itemId;
            AirshipExplorationPointId = airshipExplorationPointId;
        }

        public void FromCsv(string[] lineData)
        {
            RowId = uint.Parse( lineData[ 0 ] );
            ItemId = uint.Parse( lineData[ 1 ] );
            AirshipExplorationPointId = uint.Parse( lineData[ 2 ] );
        }
    }
}