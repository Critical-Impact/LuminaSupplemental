using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using CsvHelper.Configuration.Attributes;
using CsvHelper.TypeConversion;
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
        [Name("TreasureId")] public uint TreasureId { get; set; }
        [Name("DungeonBossId")] public uint DungeonBossId { get; set; }
        [Name("Position"), TypeConverter(typeof(Vector3Converter))] public Vector3 Position { get; set; }

        public RowRef< ContentFinderCondition > ContentFinderCondition;
        public RowRef< Map > Map;
        public RowRef< TerritoryType > TerritoryType;
        public RowRef< Treasure > Treasure;

        public DungeonChest(uint rowId, byte chestNo, uint contentFinderConditionId, uint mapId, uint territoryTypeId, uint treasureId, uint dungeonBossId = 0, Vector3 position = default )
        {
            RowId = rowId;
            ChestNo = chestNo;
            ContentFinderConditionId = contentFinderConditionId;
            MapId = mapId;
            TerritoryTypeId = territoryTypeId;
            TreasureId = treasureId;
            DungeonBossId = dungeonBossId;
            Position = position;
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
            TreasureId = uint.Parse( lineData[ 5 ] );
            DungeonBossId = uint.Parse( lineData[ 6 ] );
            var positionData = lineData[7].Split(";").Select(c => float.Parse(c, CultureInfo.InvariantCulture)).ToList();
            Position = new Vector3(positionData[0], positionData[1], positionData[2]);
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
                TreasureId.ToString(),
                DungeonBossId.ToString(),
                $"{Position.X.ToString(CultureInfo.InvariantCulture)};{Position.Y.ToString(CultureInfo.InvariantCulture)};{Position.Z.ToString(CultureInfo.InvariantCulture)}",
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
            Treasure = new RowRef< Treasure >( module, TreasureId);
        }
    }
}
