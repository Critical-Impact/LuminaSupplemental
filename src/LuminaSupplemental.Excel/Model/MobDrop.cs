using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using CsvHelper;
using CsvHelper.Configuration.Attributes;

namespace LuminaSupplemental.Excel.Model
{
    public struct MobDrop : ICsv
    {
        [Name("RowId")] public uint RowId { get; set; }
        [Name("ItemId")] public uint ItemId { get; set; }
        [Name("BNpcNameId")] public uint BNpcNameId { get; set; }

        public MobDrop(uint rowId, uint itemId, uint bNpcNameId )
        {
            RowId = rowId;
            ItemId = itemId;
            BNpcNameId = bNpcNameId;
        }

        public void FromCsv(string[] lineData)
        {
            RowId = uint.Parse( lineData[ 0 ] );
            ItemId = uint.Parse( lineData[ 1 ] );
            BNpcNameId = uint.Parse( lineData[ 2 ] );
        }
    }
}