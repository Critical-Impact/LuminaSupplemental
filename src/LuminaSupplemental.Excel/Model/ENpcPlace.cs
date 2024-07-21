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
using Lumina.Excel.GeneratedSheets;
using LuminaSupplemental.Excel.Converters;

namespace LuminaSupplemental.Excel.Model
{
    public class ENpcPlace : ICsv
    {
        [Name("RowId")] public uint RowId { get; set; }
        [Name("ENpcResidentId")] public uint ENpcResidentId { get; set; }
        [Name("TerritoryTypeId")] public uint TerritoryTypeId { get; set; }
        [Name("MapId")] public uint MapId { get; set; }
        [Name("PlaceNameId")] public uint PlaceNameId { get; set; }
        [Name("Position"), TypeConverter(typeof(Vector2Converter))] public Vector2 Position { get; set; }
        
        public LazyRow<ENpcResident> ENpcResident;
        public LazyRow<TerritoryType> TerritoryType;
        public LazyRow<Map> Map;
        public LazyRow<PlaceName> PlaceName;

        public ENpcPlace(uint rowId, uint eNpcResidentId, uint territoryTypeId, uint mapId, uint placeNameId, Vector2 position )
        {
            RowId = rowId;
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
            RowId = uint.Parse( lineData[ 0 ] );
            ENpcResidentId = uint.Parse( lineData[ 1 ] );
            TerritoryTypeId = uint.Parse( lineData[ 2 ] );
            MapId = uint.Parse( lineData[ 3 ] );
            PlaceNameId = uint.Parse( lineData[ 4 ] );
            var positionData = lineData[5].Split(";").Select(c => float.Parse(c, CultureInfo.InvariantCulture)).ToList();
            Position = new Vector2(positionData[0], positionData[1]);
        }

        public string[] ToCsv()
        {
            List<String> data = new List<string>()
            {
                RowId.ToString(),
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

        public virtual void PopulateData( GameData gameData, Language language )
        {
            ENpcResident = new LazyRow< ENpcResident >( gameData, ENpcResidentId, language );
            TerritoryType = new LazyRow< TerritoryType >( gameData, TerritoryTypeId, language );
            Map = new LazyRow<Map>( gameData, MapId, language );
            PlaceName = new LazyRow<PlaceName>( gameData, PlaceNameId, language );
        }
        
        public bool EqualRounded(ENpcPlace other)
        {
            if (Map.Row.Equals(other.Map.Row) && PlaceName.Row.Equals(other.PlaceName.Row))
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