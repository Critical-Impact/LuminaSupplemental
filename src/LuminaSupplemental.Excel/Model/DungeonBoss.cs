using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;

namespace LuminaSupplemental.Excel.Model
{
    public struct DungeonBoss : ICsv
    {
        [Name("RowId")] public uint RowId { get; set; }
        [Name("BNpcNameId")] public uint BNpcNameId { get; set; }
        [Name("ContentFinderConditionId")] public uint ContentFinderConditionId { get; set; }
        [Name("FightNo")] public uint FightNo { get; set; }

        public DungeonBoss()
        {
            
        }

        public DungeonBoss(uint rowId, uint bNpcNameId, uint fightNo,  uint contentFinderConditionId )
        {
            RowId = rowId;
            BNpcNameId = bNpcNameId;
            ContentFinderConditionId = contentFinderConditionId;
            FightNo = fightNo;
        }

        public void FromCsv(string[] lineData)
        {
            RowId = uint.Parse( lineData[ 0 ] );
            BNpcNameId = uint.Parse( lineData[ 1 ] );
            ContentFinderConditionId = uint.Parse( lineData[ 2 ] );
            FightNo = uint.Parse( lineData[ 3 ] );
        }

        public string[] ToCsv()
        {
            return Array.Empty<string>();
        }

        public bool IncludeInCsv()
        {
            throw new NotImplementedException();
        }
    }
}