using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using CsvHelper;
using CsvHelper.Configuration.Attributes;
using CsvHelper.TypeConversion;
using Lumina;
using Lumina.Data;
using Lumina.Excel;


namespace LuminaSupplemental.Excel.Model
{
    public class ItemPatch : ICsv
    {
        [Name("StartItemId")] public uint StartItemId { get; set; }
        [Name("EndItemId")] public uint EndItemId { get; set; }
        [Name("PatchNo")] public decimal PatchNo { get; set; }

        public ItemPatch(uint startItemId, uint endItemId, decimal patchNo )
        {
            StartItemId = startItemId;
            EndItemId = endItemId;
            PatchNo = patchNo;
        }

        public ItemPatch()
        {

        }

        public void FromCsv(string[] lineData)
        {
            StartItemId = uint.Parse( lineData[ 0 ] );
            EndItemId = uint.Parse( lineData[ 1 ] );
            PatchNo = decimal.Parse( lineData[ 2 ], CultureInfo.InvariantCulture );
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
        }

        public static Dictionary< uint, decimal > ToItemLookup(List<ItemPatch> itemPatches)
        {
            Dictionary< uint, decimal > lookup = new();
            foreach( var itemPatch in itemPatches )
            {
                for( uint i = itemPatch.StartItemId; i <= itemPatch.EndItemId; i++ )
                {
                    lookup.Add( i, itemPatch.PatchNo );
                }
            }

            return lookup;
        }
    }
}
