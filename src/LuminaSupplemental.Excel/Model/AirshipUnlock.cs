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
    public class AirshipUnlock : ICsv
    {
        [Name("RowId")] public uint RowId { get; set; }
        
        [Name("AirshipExplorationPointId")] public uint AirshipExplorationPointId { get; set; }
        [Name("AirshipExplorationPointUnlockId")] public uint AirshipExplorationPointUnlockId { get; set; }
        [Name("SurveillanceRequired")] public uint SurveillanceRequired { get; set; }
        [Name("RankRequired")] public uint RankRequired { get; set; }

        public LazyRow< AirshipExplorationPoint > AirshipExplorationPoint;
        public LazyRow< AirshipExplorationPoint > AirshipExplorationPointUnlock;
        
        public AirshipUnlock(uint rowId, uint airshipExplorationPointId, uint airshipExplorationPointUnlockId,uint surveillanceRequired, uint rankRequired)
        {
            RowId = rowId;
            AirshipExplorationPointId = airshipExplorationPointId;
            AirshipExplorationPointUnlockId = airshipExplorationPointUnlockId;
            SurveillanceRequired = surveillanceRequired;
            RankRequired = rankRequired;
        }

        public AirshipUnlock()
        {
            
        }

        public void FromCsv(string[] lineData)
        {
            RowId = uint.Parse( lineData[ 0 ] );
            AirshipExplorationPointId = uint.Parse( lineData[ 0 ] );
            AirshipExplorationPointUnlockId = uint.Parse( lineData[ 1 ] );
            SurveillanceRequired = uint.Parse( lineData[ 2 ] );
            RankRequired = uint.Parse( lineData[ 3 ] );
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
            AirshipExplorationPointUnlock = new LazyRow< AirshipExplorationPoint >( gameData, AirshipExplorationPointUnlockId, language );
        }
    }
}