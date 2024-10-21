using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using CsvHelper;
using CsvHelper.Configuration.Attributes;
using Lumina;
using Lumina.Data;
using Lumina.Excel;

using LuminaSupplemental.Excel.Converters;

namespace LuminaSupplemental.Excel.Model
{
    public class ShopName : ICsv
    {
        [Name("RowId")] public uint RowId { get; set; }
        [Name("ShopId")] public uint ShopId { get; set; }
        [Name("Name")] public string Name { get; set; }

        public ShopName(uint rowId, uint shopId, string name )
        {
            RowId = rowId;
            ShopId = shopId;
            Name = name;
        }

        public ShopName()
        {
            
        }

        public void FromCsv(string[] lineData)
        {
            RowId = uint.Parse( lineData[ 0 ] );
            ShopId = uint.Parse( lineData[ 1 ] );
            Name = lineData[ 2 ];
        }

        public string[] ToCsv()
        {
            List<String> data = new List<string>()
            {
                RowId.ToString(),
                ShopId.ToString(),
                Name
            };
            return data.ToArray();
        }

        public bool IncludeInCsv()
        {
            return false;
        }

        public virtual void PopulateData( ExcelModule module, Language language )
        {

        }
    }
}
