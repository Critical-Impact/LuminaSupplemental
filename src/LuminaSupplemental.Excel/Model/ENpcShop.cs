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

using Lumina.Excel.Sheets;

using LuminaSupplemental.Excel.Converters;

namespace LuminaSupplemental.Excel.Model
{
    public class ENpcShop : ICsv
    {
        [Name("RowId")] public uint RowId { get; set; }
        [Name("ENpcResidentId")] public uint ENpcResidentId { get; set; }
        [Name("ShopId")] public uint ShopId { get; set; }
        
        public RowRef< ENpcResident > ENpcResident;

        public ENpcShop(uint rowId, uint eNpcResidentId, uint shopId)
        {
            RowId = rowId;
            ENpcResidentId = eNpcResidentId;
            ShopId = shopId;
        }

        public ENpcShop()
        {
            
        }

        public void FromCsv(string[] lineData)
        {
            RowId = uint.Parse( lineData[ 0 ] );
            ENpcResidentId = uint.Parse( lineData[ 1 ] );
            ShopId = uint.Parse( lineData[ 2 ] );
        }

        public string[] ToCsv()
        {
            List<String> data = new List<string>()
            {
                RowId.ToString(),
                ENpcResidentId.ToString(),
                ShopId.ToString()
            };
            return data.ToArray();
        }

        public bool IncludeInCsv()
        {
            return false;
        }

        public virtual void PopulateData( ExcelModule module, Language language )
        {
            ENpcResident = new RowRef< ENpcResident >( module, ENpcResidentId);
        }
    }
}
