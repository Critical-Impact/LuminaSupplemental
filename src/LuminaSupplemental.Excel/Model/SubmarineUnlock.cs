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
    public class SubmarineUnlock : ICsv
    {
        [Name("SubmarineExplorationId")] public uint SubmarineExplorationId { get; set; }
        [Name("SubmarineExplorationUnlockId")] public uint SubmarineExplorationUnlockId { get; set; }
        [Name("RankRequired")] public uint RankRequired { get; set; }

        public RowRef< SubmarineExploration > SubmarineExploration;
        public RowRef< SubmarineExploration > SubmarineExplorationUnlock;

        public SubmarineUnlock(uint submarineExplorationId, uint submarineExplorationUnlockId,uint rankRequired)
        {
            SubmarineExplorationId = submarineExplorationId;
            SubmarineExplorationUnlockId = submarineExplorationUnlockId;
            RankRequired = rankRequired;
        }

        public SubmarineUnlock()
        {

        }

        public void FromCsv(string[] lineData)
        {
            SubmarineExplorationId = uint.Parse( lineData[ 0 ] );
            SubmarineExplorationUnlockId = uint.Parse( lineData[ 1 ] );
            RankRequired = uint.Parse( lineData[ 2 ] );
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
            SubmarineExploration = new RowRef< SubmarineExploration >( module, SubmarineExplorationId);
            SubmarineExplorationUnlock = new RowRef< SubmarineExploration >( module, SubmarineExplorationUnlockId);
        }
    }
}
