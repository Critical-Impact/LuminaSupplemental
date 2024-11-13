using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System.Linq;
using System.Numerics;

using Lumina.Excel.Sheets;

using LuminaSupplemental.Excel.Extensions;


namespace LuminaSupplemental.SpaghettiGenerator
{
    public class ENpcPlaces : Module
    {
        public override string Name => "ENpcPlaces";
        public override void Start()
        {
            BuildENpcPlaces();
        }
        void BuildENpcPlaces()
        {
            var territoryTypeSheet = Service.GameData.GetExcelSheet<TerritoryType>()!;

            var placeIndex = _builder.Libra.Table< Libra.ENpcResident_PlaceName >()
                .ToArray();

            var npcIndex = _builder.Libra.Table< Libra.ENpcResident >()
                .ToArray()
                .ToDictionary( i => i.Key );

            foreach( var bnpcName in placeIndex )
            {
                var territoryType = territoryTypeSheet.FirstOrNull( c =>
                                                                        c.PlaceNameRegion.RowId == (uint)bnpcName.region && c.PlaceName.RowId == (uint)bnpcName.PlaceName_Key );

                if( territoryType != null )
                {
                    if( npcIndex.ContainsKey( bnpcName.ENpcResident_Key ) )
                    {
                        var bNpc = npcIndex[ bnpcName.ENpcResident_Key ];
                        dynamic lData = JsonConvert.DeserializeObject((string)bNpc.data);
                        var lZone = Utils.GetPair(lData.coordinate);
                        var position = Utils.GetFirst(lZone.Value);
                        var coords = (JArray)position;
                        var actualCoords = coords.Values< float >().ToArray();
                         DatabaseBuilder.Instance.ENpcPlaces.Add( new ENpcData()
                         {
                             ENpcResidentId = (uint)bNpc.Key,
                             TerritoryTypeId = territoryType.Value.RowId,
                             Position = new Vector2( actualCoords[0], actualCoords[1])
                         } );
                    }
                }
            }
        }

        public class ENpcData
        {
            public uint ENpcResidentId;
            public uint TerritoryTypeId;
            public Vector2 Position;
        }
    }
}
