using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using CsvHelper;
using CsvHelper.Configuration.Attributes;
using Lumina;
using Lumina.Data;
using Lumina.Excel;

using Lumina.Excel.Sheets;

namespace LuminaSupplemental.Excel.Model
{
    public class MobDrop : ICsv
    {
        [Name("RowId")] public uint RowId { get; set; }
        [Name("ItemId")] public uint ItemId { get; set; }
        [Name("BNpcNameId")] public uint BNpcNameId { get; set; }

        public RowRef< BNpcName > BNpcName;
        
        public RowRef< Item > Item;

        public MobDrop(uint rowId, uint itemId, uint bNpcNameId )
        {
            RowId = rowId;
            ItemId = itemId;
            BNpcNameId = bNpcNameId;
        }

        public MobDrop()
        {
            
        }

        public void FromCsv(string[] lineData)
        {
            RowId = uint.Parse( lineData[ 0 ] );
            ItemId = uint.Parse( lineData[ 1 ] );
            BNpcNameId = uint.Parse( lineData[ 2 ] );
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
            BNpcName = new RowRef< BNpcName >( module, BNpcNameId);
            Item = new RowRef< Item >( module, ItemId);
        }
    }
}
