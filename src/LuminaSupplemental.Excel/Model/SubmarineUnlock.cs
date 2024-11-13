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
        [Name("RowId")] public uint RowId { get; set; }
        
        [Name("SubmarineExplorationId")] public uint SubmarineExplorationId { get; set; }
        [Name("SubmarineExplorationUnlockId")] public uint SubmarineExplorationUnlockId { get; set; }
        [Name("RankRequired")] public uint RankRequired { get; set; }

        public RowRef< SubmarineExploration > SubmarineExploration;
        public RowRef< SubmarineExploration > SubmarineExplorationUnlock;
        
        public SubmarineUnlock(uint rowId, uint submarineExplorationId, uint submarineExplorationUnlockId,uint rankRequired)
        {
            RowId = rowId;
            SubmarineExplorationId = submarineExplorationId;
            SubmarineExplorationUnlockId = submarineExplorationUnlockId;
            RankRequired = rankRequired;
        }

        public SubmarineUnlock()
        {
            
        }

        public void FromCsv(string[] lineData)
        {
            RowId = uint.Parse( lineData[ 0 ] );
            SubmarineExplorationId = uint.Parse( lineData[ 1 ] );
            SubmarineExplorationUnlockId = uint.Parse( lineData[ 2 ] );
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
            SubmarineExploration = new RowRef< SubmarineExploration >( module, SubmarineExplorationId);
            SubmarineExplorationUnlock = new RowRef< SubmarineExploration >( module, SubmarineExplorationUnlockId);
        }
    }
}
