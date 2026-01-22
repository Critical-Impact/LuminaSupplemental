using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using CsvHelper;
using CsvHelper.Configuration.Attributes;
using CsvHelper.TypeConversion;
using Lumina;
using Lumina.Data;
using Lumina.Excel;

using Lumina.Excel.Sheets;

namespace LuminaSupplemental.Excel.Model
{
    public class Gearset : ICsv
    {
        [Name("Key")] public string Key { get; set; }
        [Name("Name")] public string Name { get; set; }
        [Name("ItemId1")] public uint ItemId1 { get; set; }
        [Name("ItemId2")] public uint ItemId2 { get; set; }
        [Name("ItemId3")] public uint ItemId3 { get; set; }
        [Name("ItemId4")] public uint ItemId4 { get; set; }
        [Name("ItemId5")] public uint ItemId5 { get; set; }
        [Name("ItemId6")] public uint ItemId6 { get; set; }
        [Name("ItemId7")] public uint ItemId7 { get; set; }
        [Name("ItemId8")] public uint ItemId8 { get; set; }
        [Name("ItemId9")] public uint ItemId9 { get; set; }
        [Name("ItemId10")] public uint ItemId10 { get; set; }
        [Name("ItemId11")] public uint ItemId11 { get; set; }
        [Name("ItemId12")] public uint ItemId12 { get; set; }
        [Name("ItemId13")] public uint ItemId13 { get; set; }
        [Name("ItemId14")] public uint ItemId14 { get; set; }

        public RowRef< Item > Item1;
        public RowRef< Item > Item2;
        public RowRef< Item > Item3;
        public RowRef< Item > Item4;
        public RowRef< Item > Item5;
        public RowRef< Item > Item6;
        public RowRef< Item > Item7;
        public RowRef< Item > Item8;
        public RowRef< Item > Item9;
        public RowRef< Item > Item10;
        public RowRef<Item> Item11;
        public RowRef< Item > Item12;
        public RowRef< Item > Item13;
        public RowRef< Item > Item14;


        public Gearset(string key, string name, uint itemId1, uint itemId2,  uint itemId3, uint itemId4, uint itemId5, uint itemId6, uint itemId7, uint itemId8, uint itemId9, uint itemId10, uint itemId11, uint itemId12, uint itemId13, uint itemId14 )
        {
            Key = key;
            Name = name;
            ItemId1 = itemId1;
            ItemId2 = itemId2;
            ItemId3 = itemId3;
            ItemId4 = itemId4;
            ItemId5 = itemId5;
            ItemId6 = itemId6;
            ItemId7 = itemId7;
            ItemId8 = itemId8;
            ItemId9 = itemId9;
            ItemId10 = itemId10;
            ItemId11 = itemId11;
            ItemId12 = itemId12;
            ItemId13 = itemId13;
            ItemId14 = itemId14;
        }

        public Gearset()
        {

        }

        public void FromCsv(string[] lineData)
        {
            Key =  lineData[ 0 ];
            Name = lineData[ 1 ];
            ItemId1 = uint.Parse( lineData[ 2 ] );
            ItemId2 = uint.Parse( lineData[ 3 ] );
            ItemId3 = uint.Parse( lineData[ 4 ] );
            ItemId4 = uint.Parse( lineData[ 5 ] );
            ItemId5 = uint.Parse( lineData[ 6 ] );
            ItemId6 = uint.Parse( lineData[ 7 ] );
            ItemId7 = uint.Parse( lineData[ 8 ] );
            ItemId8 = uint.Parse( lineData[ 9 ] );
            ItemId9 = uint.Parse( lineData[ 10 ] );
            ItemId10 = uint.Parse( lineData[ 11 ] );
            ItemId11 = uint.Parse( lineData[ 12 ] );
            ItemId12 = uint.Parse( lineData[ 13 ] );
            ItemId13 = uint.Parse( lineData[ 14 ] );
            ItemId14 = uint.Parse( lineData[ 15 ] );
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
            Item1 = new RowRef< Item >( module, ItemId1);
            Item2 = new RowRef< Item>( module, ItemId2);
            Item3 = new RowRef< Item>( module, ItemId3);
            Item4 = new RowRef< Item>( module, ItemId4);
            Item5 = new RowRef< Item>( module, ItemId5);
            Item6 = new RowRef< Item>( module, ItemId6);
            Item7 = new RowRef< Item>( module, ItemId7);
            Item8 = new RowRef< Item>( module, ItemId8);
            Item9 = new RowRef< Item>( module, ItemId9);
            Item10 = new RowRef< Item>( module, ItemId10);
            Item11 = new RowRef< Item>( module, ItemId11);
            Item12 = new RowRef< Item>( module, ItemId12);
            Item13 = new RowRef< Item>( module, ItemId13);
            Item14 = new RowRef< Item>( module, ItemId14);
        }
    }
}
