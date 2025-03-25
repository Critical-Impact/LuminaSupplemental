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
    public class AirshipDrop : ICsv
    {
        [Name("ItemId")] public uint ItemId { get; set; }
        [Name("AirshipExplorationPointId")] public uint AirshipExplorationPointId { get; set; }

        public RowRef< AirshipExplorationPoint > AirshipExplorationPoint;

        public RowRef< Item > Item;

        public AirshipDrop(uint itemId, uint airshipExplorationPointId )
        {
            ItemId = itemId;
            AirshipExplorationPointId = airshipExplorationPointId;
        }

        public AirshipDrop()
        {

        }

        public void FromCsv(string[] lineData)
        {
            ItemId = uint.Parse( lineData[ 0 ] );
            AirshipExplorationPointId = uint.Parse( lineData[ 1 ] );
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
            AirshipExplorationPoint = new RowRef< AirshipExplorationPoint >( module, AirshipExplorationPointId );
            Item = new RowRef< Item >( module, ItemId);
        }
    }
}
