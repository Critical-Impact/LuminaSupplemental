using System;
using CsvHelper.Configuration.Attributes;
using Lumina;
using Lumina.Data;
using Lumina.Excel;
using Lumina.Excel.Sheets;

namespace LuminaSupplemental.Excel.Model
{
    public class AirshipUnlock : ICsv
    {
        [Name("RowId")] public uint RowId { get; set; }

        [Name("AirshipExplorationPointId")] public uint AirshipExplorationPointId { get; set; }
        [Name("AirshipExplorationPointUnlockId")] public uint AirshipExplorationPointUnlockId { get; set; }
        [Name("SurveillanceRequired")] public uint SurveillanceRequired { get; set; }
        [Name("RankRequired")] public uint RankRequired { get; set; }

        public RowRef< AirshipExplorationPoint > AirshipExplorationPoint;
        public RowRef< AirshipExplorationPoint > AirshipExplorationPointUnlock;

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

        public virtual void PopulateData( ExcelModule module, Language language )
        {
            AirshipExplorationPoint = new RowRef< AirshipExplorationPoint >( module, AirshipExplorationPointId);
            AirshipExplorationPointUnlock = new RowRef< AirshipExplorationPoint >( module, AirshipExplorationPointUnlockId);
        }
    }
}
