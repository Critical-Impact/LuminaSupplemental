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
using Lumina.Excel.GeneratedSheets;

namespace LuminaSupplemental.Excel.Model
{
    public class AirshipDrop : ICsv
    {
        [Name("RowId")] public uint RowId { get; set; }
        [Name("ItemId")] public uint ItemId { get; set; }
        [Name("AirshipExplorationPointId")] public uint AirshipExplorationPointId { get; set; }

        public LazyRow< AirshipExplorationPoint > AirshipExplorationPoint;
        
        public LazyRow< Item > Item;
        
        public AirshipDrop(uint rowId, uint itemId, uint airshipExplorationPointId )
        {
            RowId = rowId;
            ItemId = itemId;
            AirshipExplorationPointId = airshipExplorationPointId;
        }

        public AirshipDrop()
        {
            
        }

        public void FromCsv(string[] lineData)
        {
            RowId = uint.Parse( lineData[ 0 ] );
            ItemId = uint.Parse( lineData[ 1 ] );
            AirshipExplorationPointId = uint.Parse( lineData[ 2 ] );
        }

        public string[] ToCsv()
        {
            return Array.Empty<string>();
        }

        public bool IncludeInCsv()
        {
            return false;
        }

        public virtual void PopulateData( GameData gameData, Language language )
        {
            AirshipExplorationPoint = new LazyRow< AirshipExplorationPoint >( gameData, AirshipExplorationPointId, language );
            Item = new LazyRow< Item >( gameData, ItemId, language );
        }
    }
}