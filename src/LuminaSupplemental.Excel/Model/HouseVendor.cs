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
    public class HouseVendor : ICsv
    {
        [Name("ENpcResidentId")] public uint ENpcResidentId { get; set; }
        [Name("Parent")] public uint ParentId { get; set; }

        public HouseVendor(uint eNpcResidentId, uint parentId )
        {
            ENpcResidentId = eNpcResidentId;
            ParentId = parentId;
        }

        public HouseVendor()
        {

        }

        public void FromCsv(string[] lineData)
        {
            ENpcResidentId = uint.Parse( lineData[ 0 ] );
            ParentId = uint.Parse( lineData[ 1 ] );
        }

        public string[] ToCsv()
        {
            List<String> data = new List<string>()
            {
                ENpcResidentId.ToString(),
                ParentId.ToString()
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
