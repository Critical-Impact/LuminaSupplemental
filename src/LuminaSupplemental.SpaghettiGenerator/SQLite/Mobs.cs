using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

using Lumina.Excel.Sheets;


namespace LuminaSupplemental.SpaghettiGenerator
{
    public class Mobs : Module
    {
        Dictionary< long, BNpcData > _bnpcDataByFullKey = new Dictionary< long, BNpcData >();

        public override string Name => "Mobs";
        public override void Start()
        {
            BuildItems();
            BuildMobs();
        }
        void BuildMobs()
        {
            var bNpcNameSheet = Service.GameData.GetExcelSheet<BNpcName>()!;
            var placeNameSheet = Service.GameData.GetExcelSheet<PlaceName>()!;
            var libraIndex = _builder.Libra.Table< Libra.BNpcName >()
                .ToArray()
                .ToDictionary( i => i.Key );

            foreach( var bnpcName in libraIndex )
            {
                var realMobId = bnpcName.Key.ToString().Substring( bnpcName.Key.ToString().Length - 5, 5 ).Trim( '0' );
                if( UInt32.TryParse( realMobId, out var parsedMobId ) )
                {
                    var actualNpc = bNpcNameSheet.GetRowOrDefault( parsedMobId );
                    if( actualNpc != null )
                    {
                        dynamic data = JsonConvert.DeserializeObject( bnpcName.Value.data );

                        if( data.nonpop != null )
                        {
                            //bnpcData.HasSpecialSpawnRules = true;
                        }

                        if (data.region != null)
                        {
                            var area = Utils.GetPair(data.region);
                            var zone = Utils.GetPair(area.Value);

                            string key = (string)zone.Key;
                            if( UInt32.TryParse( key, out var parsedKey ) )
                            {
                                var placeName = placeNameSheet.GetRowOrDefault( parsedKey );
                                if( placeName != null )
                                {
                                    if( !_builder.PlaceNamesByMobId.TryGetValue( actualNpc.Value.RowId, out var itemIds ) )
                                    {
                                        itemIds = new HashSet< uint >();
                                        _builder.PlaceNamesByMobId[ actualNpc.Value.RowId ] = itemIds;
                                    }

                                    itemIds.Add( placeName.Value.RowId );
                                }
                            }
                        }
                    }
                }
            }


        }

        void BuildItems()
        {
            var itemSheet = Service.GameData.GetExcelSheet<Item>()!;
            var itemsToImport = itemSheet.ToList();
            var libraIndex = _builder.Libra.Table< Libra.Item >()
                .ToArray()
                .ToDictionary( i => i.Key );

            foreach( var sItem in itemsToImport )
            {
                var item = _builder.CreateItem( sItem.RowId );


                libraIndex.TryGetValue( (int)sItem.RowId, out var lItem );

                if( lItem != null && lItem.data != null )
                {
                    dynamic extraLibraData = JsonConvert.DeserializeObject( lItem.data );

                    // Mob drops
                    if( extraLibraData.bnpc != null && extraLibraData.bnpc.Count > 0 )
                    {
                        var mobIds = new JArray();
                        foreach( long mob in extraLibraData.bnpc )
                        {
                            mobIds.Add( mob );
                            var realMobId = mob.ToString().Substring( mob.ToString().Length - 5, 5 ).Trim( '0' );
                            if( UInt32.TryParse( realMobId, out var parsedMobId ) )
                            {
                                var actualNpc = Service.GameData.GetExcelSheet< BNpcName >()!.GetRowOrDefault(  parsedMobId );
                                if( actualNpc != null )
                                {
                                    if( !_builder.ItemDropsByMobId.TryGetValue( actualNpc.Value.RowId, out var itemIds ) )
                                    {
                                        itemIds = new HashSet< uint >();
                                        _builder.ItemDropsByMobId[ actualNpc.Value.RowId ] = itemIds;
                                    }

                                    itemIds.Add( sItem.RowId );
                                }
                            }
                        }

                        // References are added by Mobs module.
                        item.drops = mobIds;
                    }
                }
            }
        }

        class BNpcData
        {
            public int BNpcBaseKey;
            public int BNpcNameKey;
            public long FullKey;
            public string DebugName;
            public bool HasSpecialSpawnRules;
            public List< BNpcLocation > Locations = new List< BNpcLocation >();

            public override string ToString() => DebugName;
        }

        class BNpcLocation
        {
            public int PlaceNameKey;
            public double X;
            public double Y;
            public double Z;
            public double Radius;
            public string LevelRange;

            public override string ToString() => $"({X}, {Y}, {Z}, r{Radius})";
        }
    }
}
