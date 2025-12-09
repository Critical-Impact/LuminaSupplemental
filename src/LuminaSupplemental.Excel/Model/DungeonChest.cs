using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using CsvHelper;
using CsvHelper.Configuration.Attributes;
using Lumina;
using Lumina.Data;
using Lumina.Excel;

using Lumina.Excel.Sheets;

using LuminaSupplemental.Excel.Converters;

namespace LuminaSupplemental.Excel.Model
{
    public class DungeonChest : ICsv
    {
        [Name("RowId")] public uint RowId { get; set; }
        [Name("ChestNo")] public byte ChestNo { get; set; }
        [Name("ContentFinderConditionId")] public uint ContentFinderConditionId { get; set; }
        [Name("MapId")] public uint MapId { get; set; }
        [Name("TerritoryTypeId")] public uint TerritoryTypeId { get; set; }
        [Name("ChestId")] public uint ChestId { get; set; }

        public RowRef< ContentFinderCondition > ContentFinderCondition;
        public RowRef< Map > Map;
        public RowRef< TerritoryType > TerritoryType;

        public DungeonChest(uint rowId, byte chestNo,uint contentFinderConditionId, uint mapId, uint territoryTypeId, uint chestId )
        {
            RowId = rowId;
            ChestNo = chestNo;
            ContentFinderConditionId = contentFinderConditionId;
            MapId = mapId;
            TerritoryTypeId = territoryTypeId;
            ChestId = chestId;
        }

        public DungeonChest()
        {

        }

        public void FromCsv(string[] lineData)
        {
            RowId = uint.Parse( lineData[ 0 ] );
            ChestNo = byte.Parse( lineData[ 1 ] );
            ContentFinderConditionId = uint.Parse( lineData[ 2 ] );
            MapId = uint.Parse( lineData[ 3 ] );
            TerritoryTypeId = uint.Parse( lineData[ 4 ] );
            this.ChestId = uint.Parse( lineData[ 5 ] );
        }

        public string[] ToCsv()
        {
            List<String> data = new List<string>()
            {
                RowId.ToString(),
                ChestNo.ToString(),
                ContentFinderConditionId.ToString(),
                MapId.ToString(),
                TerritoryTypeId.ToString(),
                this.ChestId.ToString(),
            };
            return data.ToArray();
        }

        public bool IncludeInCsv()
        {
            return false;
        }

        public virtual void PopulateData( ExcelModule module, Language language )
        {
            ContentFinderCondition = new RowRef< ContentFinderCondition >( module, ContentFinderConditionId);
            Map = new RowRef< Map >( module, MapId);
            TerritoryType = new RowRef< TerritoryType >( module, TerritoryTypeId);
        }
    }
}
