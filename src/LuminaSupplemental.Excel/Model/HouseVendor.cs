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
        [Name("RowId")] public uint RowId { get; set; }
        [Name("ENpcResidentId")] public uint ENpcResidentId { get; set; }
        [Name("Parent")] public uint ParentId { get; set; }

        public HouseVendor(uint rowId, uint eNpcResidentId, uint parentId )
        {
            RowId = rowId;
            ENpcResidentId = eNpcResidentId;
            ParentId = parentId;
        }

        public HouseVendor()
        {
            
        }

        public void FromCsv(string[] lineData)
        {
            RowId = uint.Parse( lineData[ 0 ] );
            ENpcResidentId = uint.Parse( lineData[ 1 ] );
            ParentId = uint.Parse( lineData[ 2 ] );
        }

        public string[] ToCsv()
        {
            List<String> data = new List<string>()
            {
                RowId.ToString(),
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
