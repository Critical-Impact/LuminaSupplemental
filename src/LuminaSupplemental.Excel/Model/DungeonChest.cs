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
    public class DungeonChest : ICsv
    {
        [Name("RowId")] public uint RowId { get; set; }
        [Name("ChestNo")] public byte ChestNo { get; set; }
        [Name("ContentFinderConditionId")] public uint ContentFinderConditionId { get; set; }
        [Name("Position"), TypeConverter(typeof(Vector2Converter))] public Vector2 Position { get; set; }
        
        public LazyRow< ContentFinderCondition > ContentFinderCondition;

        public DungeonChest(uint rowId, byte chestNo,uint contentFinderConditionId, Vector2 position )
        {
            RowId = rowId;
            ChestNo = chestNo;
            ContentFinderConditionId = contentFinderConditionId;
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
            var positionData = lineData[ 3 ].Split( ";" ).Select( c => float.Parse(c, CultureInfo.InvariantCulture)).ToList();
            Position = new Vector2( positionData[ 0 ], positionData[ 1 ] );
        }

        public string[] ToCsv()
        {
            List<String> data = new List<string>()
            {
                RowId.ToString(),
                ChestNo.ToString(),
                ContentFinderConditionId.ToString(),
                Position.X + ";" + Position.Y
            };
            return data.ToArray();
        }

        public bool IncludeInCsv()
        {
            return false;
        }

        public virtual void PopulateData( GameData gameData, Language language )
        {
            ContentFinderCondition = new LazyRow< ContentFinderCondition >( gameData, ContentFinderConditionId, language );
        }
    }
}