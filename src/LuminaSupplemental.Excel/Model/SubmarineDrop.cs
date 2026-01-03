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
        [Name("NormalMin")] public uint NormalMin { get; set; }
        [Name("NormalMax")] public uint NormalMax { get; set; }
        [Name("PoorMin")] public uint PoorMin { get; set; }
        [Name("PoorMax")] public uint PoorMax { get; set; }
        [Name("OptimalMin")] public uint OptimalMin { get; set; }
        [Name("OptimalMax")] public uint OptimalMax { get; set; }


        public RowRef< SubmarineExploration > SubmarineExploration;

        public RowRef< Item > Item;

        public SubmarineDrop(uint itemId, uint submarineExplorationId, byte lootTier, uint normalMin, uint normalMax,  uint poorMin, uint poorMax,  uint optimalMin, uint optimalMax)
        {
            ItemId = itemId;
            SubmarineExplorationId = submarineExplorationId;
            LootTier = lootTier;
            NormalMin = normalMin;
            NormalMax = normalMax;
            PoorMin = poorMin;
            PoorMax = poorMax;
            OptimalMin = optimalMin;
            OptimalMax = optimalMax;
        }

        public SubmarineDrop()
        {

        }

        public void FromCsv(string[] lineData)
        {
            ItemId = uint.Parse( lineData[ 0 ] );
            SubmarineExplorationId = uint.Parse( lineData[ 1 ] );
            LootTier = byte.Parse( lineData[ 2 ], CultureInfo.InvariantCulture );
            NormalMin = uint.Parse( lineData[ 3 ], CultureInfo.InvariantCulture );
            NormalMax = uint.Parse( lineData[ 4 ], CultureInfo.InvariantCulture );
            PoorMin = uint.Parse( lineData[ 5 ], CultureInfo.InvariantCulture );
            PoorMax = uint.Parse( lineData[ 6 ], CultureInfo.InvariantCulture );
            OptimalMin = uint.Parse( lineData[ 7 ], CultureInfo.InvariantCulture );
            OptimalMax = uint.Parse( lineData[ 8 ], CultureInfo.InvariantCulture );
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
