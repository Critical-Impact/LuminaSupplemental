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
    public class SubmarineDrop : ICsv
    {
        [Name("ItemId")] public uint ItemId { get; set; }
        [Name("SubmarineExplorationId")] public uint SubmarineExplorationId { get; set; }
        [Name("LootTier")] public byte LootTier { get; set; }
        [Name("Min")] public uint Min { get; set; }
        [Name("Max")] public uint Max { get; set; }
        [Name("Probability")] public decimal Probability { get; set; }


        public RowRef< SubmarineExploration > SubmarineExploration;

        public RowRef< Item > Item;

        public SubmarineDrop(uint itemId, uint submarineExplorationId, byte lootTier, uint min, uint max, decimal probability )
        {
            ItemId = itemId;
            SubmarineExplorationId = submarineExplorationId;
            LootTier = lootTier;
            Min = min;
            Max = max;
            Probability = probability;
        }

        public SubmarineDrop()
        {

        }

        public void FromCsv(string[] lineData)
        {
            ItemId = uint.Parse( lineData[ 0 ] );
            SubmarineExplorationId = uint.Parse( lineData[ 1 ] );
            LootTier = byte.Parse( lineData[ 2 ], CultureInfo.InvariantCulture );
            Min = uint.Parse( lineData[ 3 ], CultureInfo.InvariantCulture );
            Max = uint.Parse( lineData[ 4 ], CultureInfo.InvariantCulture );
            Probability = decimal.Parse( lineData[ 5 ], CultureInfo.InvariantCulture );
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
            Item = new RowRef< Item >( module, ItemId);
        }
    }
}
