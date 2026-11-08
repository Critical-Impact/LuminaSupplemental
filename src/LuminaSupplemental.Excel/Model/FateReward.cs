using System;
using System.Globalization;

using CsvHelper.Configuration.Attributes;

using Lumina.Data;
using Lumina.Excel;
using Lumina.Excel.Sheets;

namespace LuminaSupplemental.Excel.Model
{
    public class FateReward : ICsv
    {
        [Name("FateId")] public uint FateId { get; set; }
        [Name("GilMin")] public uint? GilMin { get; set; }
        [Name("GilMax")] public uint? GilMax { get; set; }
        [Name("GrandCompanySealsMin")] public uint? GrandCompanySealsMin { get; set; }
        [Name("GrandCompanySealsMax")] public uint? GrandCompanySealsMax { get; set; }

        public RowRef<Fate> Fate;

        public FateReward(uint fateId, uint? gilMin = null, uint? gilMax = null, uint? grandCompanySealsMin = null, uint? grandCompanySealsMax = null)
        {
            FateId = fateId;
            GilMin = gilMin;
            GilMax = gilMax;
            GrandCompanySealsMin = grandCompanySealsMin;
            GrandCompanySealsMax = grandCompanySealsMax;
        }

        public FateReward()
        {

        }

        public void FromCsv(string[] lineData)
        {
            FateId = uint.Parse( lineData[ 0 ] );

            if (lineData[1] != string.Empty)
            {
                GilMin = uint.Parse( lineData[ 1 ], CultureInfo.InvariantCulture );
            }

            if (lineData[2] != string.Empty)
            {
                GilMax = uint.Parse( lineData[ 2 ], CultureInfo.InvariantCulture );
            }

            if (lineData[3] != string.Empty)
            {
                GrandCompanySealsMin = uint.Parse( lineData[ 3 ], CultureInfo.InvariantCulture );
            }

            if (lineData[4] != string.Empty)
            {
                GrandCompanySealsMax = uint.Parse( lineData[ 4 ], CultureInfo.InvariantCulture );
            }
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
            Fate = new RowRef<Fate>( module, FateId);
        }
    }
}
