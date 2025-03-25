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
        [Name("ShopId")] public uint ShopId { get; set; }
        [Name("Name")] public string Name { get; set; }

        public ShopName(uint shopId, string name )
        {
            ShopId = shopId;
            Name = name;
        }

        public ShopName()
        {

        }

        public void FromCsv(string[] lineData)
        {
            ShopId = uint.Parse( lineData[ 0 ] );
            Name = lineData[ 1 ];
        }

        public string[] ToCsv()
        {
            List<String> data = new List<string>()
            {
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
