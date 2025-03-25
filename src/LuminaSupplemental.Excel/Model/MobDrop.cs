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
        [Name("ItemId")] public uint ItemId { get; set; }
        [Name("BNpcNameId")] public uint BNpcNameId { get; set; }

        public RowRef< BNpcName > BNpcName;

        public RowRef< Item > Item;

        public MobDrop(uint itemId, uint bNpcNameId )
        {
            ItemId = itemId;
            BNpcNameId = bNpcNameId;
        }

        public MobDrop()
        {

        }

        public void FromCsv(string[] lineData)
        {
            ItemId = uint.Parse( lineData[ 0 ] );
            BNpcNameId = uint.Parse( lineData[ 1 ] );
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
