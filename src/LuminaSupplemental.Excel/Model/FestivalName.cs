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
    public class FestivalName : ICsv
    {
        [Name("FestivalId")] public uint FestivalId { get; set; }

        [Name("Name")] public string Name { get; set; }

        public RowRef<Festival> Festival;

        public FestivalName(uint festivalId, string name )
        {
            FestivalId = festivalId;
            Name = name;
        }

        public FestivalName()
        {

        }

        public void FromCsv(string[] lineData)
        {
            this.FestivalId = uint.Parse( lineData[ 0 ] );
            Name = lineData[ 1 ];
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
            Festival = new RowRef<Festival>( module, this.FestivalId);
        }
    }
}
