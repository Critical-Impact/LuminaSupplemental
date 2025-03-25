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
    public class ENpcPlace : ICsv
    {
        [Name("ENpcResidentId")] public uint ENpcResidentId { get; set; }
        [Name("TerritoryTypeId")] public uint TerritoryTypeId { get; set; }
        [Name("MapId")] public uint MapId { get; set; }
        [Name("PlaceNameId")] public uint PlaceNameId { get; set; }
        [Name("Position"), TypeConverter(typeof(Vector2Converter))] public Vector2 Position { get; set; }

        public RowRef<ENpcResident> ENpcResident;
        public RowRef<TerritoryType> TerritoryType;
        public RowRef<Map> Map;
        public RowRef<PlaceName> PlaceName;

        public ENpcPlace(uint eNpcResidentId, uint territoryTypeId, uint mapId, uint placeNameId, Vector2 position )
        {
            ENpcResidentId = eNpcResidentId;
            TerritoryTypeId = territoryTypeId;
            MapId = mapId;
            PlaceNameId = placeNameId;
            Position = position;
        }

        public ENpcPlace()
        {

        }

        public void FromCsv(string[] lineData)
        {
            ENpcResidentId = uint.Parse( lineData[ 0 ] );
            TerritoryTypeId = uint.Parse( lineData[ 1 ] );
            MapId = uint.Parse( lineData[ 2 ] );
            PlaceNameId = uint.Parse( lineData[ 3 ] );
            var positionData = lineData[4].Split(";").Select(c => float.Parse(c, CultureInfo.InvariantCulture)).ToList();
            Position = new Vector2(positionData[0], positionData[1]);
        }

        public string[] ToCsv()
        {
            List<String> data = new List<string>()
            {
                ENpcResidentId.ToString(),
                TerritoryTypeId.ToString(),
                MapId.ToString(),
                PlaceNameId.ToString(),
                Position.X + ";" + Position.Y,
            };
            return data.ToArray();
        }

        public bool IncludeInCsv()
        {
            return false;
        }

        public virtual void PopulateData( ExcelModule module, Language language )
        {
            ENpcResident = new RowRef< ENpcResident >( module, ENpcResidentId);
            TerritoryType = new RowRef< TerritoryType >( module, TerritoryTypeId);
            Map = new RowRef<Map>( module, MapId);
            PlaceName = new RowRef<PlaceName>( module, PlaceNameId);
        }

        public bool EqualRounded(ENpcPlace other)
        {
            if (Map.RowId.Equals(other.Map.RowId) && PlaceName.RowId.Equals(other.PlaceName.RowId))
            {
                var x = (int) Position.X;
                var y = (int) Position.Y;
                var otherX = (int) other.Position.X;
                var otherY = (int) other.Position.Y;
                if (x == otherX && y == otherY)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
