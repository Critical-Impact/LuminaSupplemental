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
    public class BNpcLink : ICsv
    {
        [Name("BNpcNameId")] public uint BNpcNameId { get; set; }
        [Name("BNpcBaseId")] public uint BNpcBaseId { get; set; }

        public RowRef<BNpcName> BNpcName;
        public RowRef<BNpcBase> BNpcBase;

        public BNpcLink(uint bNpcNameId, uint bNpcBaseId)
        {
            BNpcNameId = bNpcNameId;
            BNpcBaseId = bNpcBaseId;
        }

        public BNpcLink()
        {

        }

        public void FromCsv(string[] lineData)
        {
            BNpcNameId = uint.Parse( lineData[ 0 ] );
            BNpcBaseId = uint.Parse( lineData[ 1 ] );
        }

        public string[] ToCsv()
        {
            List<String> data = new List<string>()
            {
                BNpcNameId.ToString(),
                BNpcBaseId.ToString(),
            };
            return data.ToArray();
        }

        public bool IncludeInCsv()
        {
            return false;
        }

        public virtual void PopulateData( ExcelModule module, Language language )
        {
            BNpcName = new RowRef< BNpcName >( module, BNpcNameId);
            BNpcBase = new RowRef< BNpcBase >( module, BNpcBaseId);
        }
    }
}
