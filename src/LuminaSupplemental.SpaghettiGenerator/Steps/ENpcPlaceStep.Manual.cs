using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;

using CSVFile;

using Lumina.Data;

using LuminaSupplemental.Excel.Model;

namespace LuminaSupplemental.SpaghettiGenerator.Steps;

public partial class ENpcPlaceStep
{
    private List<ENpcPlace> ProcessManualData()
    {
        var reader = CSVReader.FromFile(Path.Combine( "ManualData","ManualENpcPlaceData.csv"), CSVSettings.CSV);
        List< ENpcPlace > manualData = new();
        foreach( var lineData in reader.Lines() )
        {
            var eNpcResidentId = uint.Parse( lineData[ 1 ] );
            var territoryTypeId = uint.Parse( lineData[ 2 ] );
            var positionData = lineData[3].Split(";").Select(c => float.Parse(c, CultureInfo.InvariantCulture)).ToList();
            var position = new Vector2(positionData[0], positionData[1]);
            var territoryType = this.territoryTypeSheet.GetRow( territoryTypeId );
            if(territoryType == null) continue;
            var map = territoryType.Map.Value;
            if(map == null) continue;
            var placeName = territoryType.PlaceName.Value;
            if(placeName == null) continue;

            var eNpcPlace = new ENpcPlace( 0, eNpcResidentId, territoryTypeId,map.RowId,
                                           placeName.RowId, position );
            eNpcPlace.PopulateData( this.gameData, Language.English );
            manualData.Add( eNpcPlace );

        }
        
        var rowId = 0u;
        Dictionary<uint, HashSet<ENpcPlace>> npcLevelLookup = new Dictionary<uint, HashSet<ENpcPlace>>();
        
        foreach (var npc in manualData)
        {
            if (!npcLevelLookup.ContainsKey(npc.ENpcResidentId))
            {
                npcLevelLookup.Add(npc.ENpcResidentId, new());
            }

            if (npc.TerritoryType.Value != null)
            {
                if (!npcLevelLookup[npc.ENpcResidentId].Any(c => c.EqualRounded(npc)))
                {
                    npc.RowId = rowId;
                    npcLevelLookup[npc.ENpcResidentId].Add(npc);
                    rowId++;
                }
            }
        }

        return npcLevelLookup.SelectMany(c => c.Value.ToList()).ToList();
    }
}
