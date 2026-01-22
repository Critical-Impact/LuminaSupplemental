using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;

using Dalamud.Utility;

using Lumina.Excel;
using Lumina.Excel.Sheets;

using LuminaSupplemental.Excel.Model;
using LuminaSupplemental.SpaghettiGenerator.Converters;
using LuminaSupplemental.SpaghettiGenerator.Generator;
using LuminaSupplemental.SpaghettiGenerator.Steps.Parsers;

using Newtonsoft.Json;

using SupabaseExporter.Structures.Exports;

namespace LuminaSupplemental.SpaghettiGenerator.Steps;

public partial class MobSpawnStep : GeneratorStep
{
    private readonly ExcelSheet<BNpcName> bnpcNameSheet;
    private readonly ExcelSheet<BNpcName> bnpcBaseSheet;
    private readonly ExcelSheet<Pet> petSheet;
    private readonly ExcelSheet<Companion> companionSheet;
    private readonly ExcelSheet<TerritoryType> territoryTypeSheet;
    private readonly ExcelSheet<Map> mapSheet;
    private readonly MappyParser mappyParser;

    public override Type OutputType => typeof(MobSpawnPosition);

    public override string FileName => "MobSpawn.csv";

    public override string Name => "Mob Spawns";


    public MobSpawnStep(ExcelSheet<BNpcName> bnpcNameSheet, ExcelSheet<BNpcName> bnpcBaseSheet, ExcelSheet<Pet> petSheet, ExcelSheet<Companion> companionSheet, ExcelSheet<TerritoryType> territoryTypeSheet, ExcelSheet<Map> mapSheet, MappyParser mappyParser)
    {
        this.bnpcNameSheet = bnpcNameSheet;
        this.bnpcBaseSheet = bnpcBaseSheet;
        this.petSheet = petSheet;
        this.companionSheet = companionSheet;
        this.territoryTypeSheet = territoryTypeSheet;
        this.mapSheet = mapSheet;
        this.mappyParser = mappyParser;
    }


    public override List<ICsv> Run(Dictionary<Type, List<ICsv>> stepData)
    {
        List<MobSpawnPosition> items = new ();
        items.AddRange(this.Process());

        return [..items.Where(c => this.bnpcNameSheet.HasRow(c.BNpcNameId) && this.bnpcBaseSheet.HasRow(c.BNpcBaseId) && c.BNpcBaseId != 0 && c.BNpcNameId != 0).Select(c => c).OrderBy(c => c.BNpcBaseId).ThenBy(c => c.BNpcNameId)];
    }

    private List<MobSpawnPosition> Process()
    {
        List<MobSpawnPosition> mobSpawns = new();

        Dictionary< uint, Dictionary< uint, List< MobSpawnPosition > > > positions = new();
        var importFiles = Directory.GetFiles( Path.Join(  AppDomain.CurrentDomain.BaseDirectory, "ManualData", "MobImports"), "*.csv" );
        foreach( var importFile in importFiles )
        {
            var reader = CSVFile.CSVReader.FromFile( importFile );
            var lines = reader.Lines();
            foreach( var line in lines )
            {
                MobSpawnPosition mobSpawnPosition = new MobSpawnPosition();
                mobSpawnPosition.FromCsv(line);
                AddEntry( mobSpawnPosition, positions );
            }
        }

        try
        {
           var mappyEntries = mappyParser.RetrieveMappyCache();

           foreach( var mappyEntry in mappyEntries )
           {
               if( mappyEntry.Type == "BNPC" )
               {
                   MobSpawnPosition mobSpawnPosition = new MobSpawnPosition();
                   mobSpawnPosition.Position = new Vector3( (float)mappyEntry.PosX, (float)mappyEntry.PosY, (float)mappyEntry.PosZ );
                   mobSpawnPosition.BNpcNameId = mappyEntry.BNpcNameID;
                   mobSpawnPosition.BNpcBaseId = mappyEntry.BNpcBaseID;
                   mobSpawnPosition.TerritoryTypeId = (uint)mappyEntry.MapTerritoryID;
                   mobSpawnPosition.Subtype = 0;
                   if( bnpcNameSheet.GetRowOrDefault( mappyEntry.BNpcNameID ) != null )
                   {
                       AddEntry( mobSpawnPosition, positions );
                   }
                   else
                   {
                       Console.WriteLine( $"Failed to find bnpcname with ID {mappyEntry.BNpcNameID}");
                   }
               }
           }
        }
        catch( Exception e )
        {
            Console.WriteLine( "Failed to parse mappy data because " + e.Message );
            throw;
        }

        var filePath = "../../../../FFXIVGachaSpreadsheet/website/static/data/BnpcPairs.json";

        var json = File.ReadAllText(filePath);


        var pairingData = JsonConvert.DeserializeObject<BnpcPairing>(json, new JsonSerializerSettings()
        {
            Converters = {new Vector3JsonConverter() }
        })!;
        foreach (var pairing in pairingData.BnpcPairings)
        {
            var baseId = pairing.Value.Base;
            var nameId = pairing.Value.Base;
            foreach (var location in pairing.Value.Locations)
            {
                var territory = location.Value.Territory;
                var map = this.mapSheet.GetRowOrDefault(location.Value.Map);
                if (map.HasValue)
                {
                    foreach (var position in location.Value.Positions)
                    {
                        var mapPosition = new Vector3(
                            MapUtil.ConvertWorldCoordXZToMapCoord(position.X, map.Value.SizeFactor, map.Value.OffsetX),
                            MapUtil.ConvertWorldCoordXZToMapCoord(position.Z, map.Value.SizeFactor, map.Value.OffsetY),
                            MapUtil.ConvertWorldCoordYToMapCoord(position.Y, map.Value.SizeFactor));
                        AddEntry(new MobSpawnPosition(baseId, nameId, territory, mapPosition, 0), positions);
                    }
                }
            }
        }

        var newPositions = positions.SelectMany(c => c.Value.SelectMany(d => d.Value.Select(e => e))).ToList();
        mobSpawns.AddRange( newPositions );

        return mobSpawns;
    }

    private const float maxRange = 4.0f;
    public void AddEntry(MobSpawnPosition spawnPosition, Dictionary< uint, Dictionary< uint, List< MobSpawnPosition > > > positions)
    {
        positions.TryAdd(spawnPosition.TerritoryTypeId, new Dictionary<uint, List<MobSpawnPosition>>());
        positions[spawnPosition.TerritoryTypeId].TryAdd(spawnPosition.BNpcNameId, new List<MobSpawnPosition>());
        //Store
        var existingPositions = positions[spawnPosition.TerritoryTypeId][spawnPosition.BNpcNameId];
        if (!existingPositions.Any(c => WithinRange(spawnPosition.Position, c.Position, maxRange)))
        {
            if( MobAllowed( spawnPosition.BNpcNameId ) )
            {
                if( TerritoryTypeAllowed( spawnPosition.TerritoryTypeId ) )
                {
                    existingPositions.Add( spawnPosition );
                }
                else
                {
                    //Console.WriteLine("Mob position ignored due to territory type restrictions");
                }
            }
            else
            {
                //Console.WriteLine("Mob position ignored due to mob type restrictions");
            }
        }
        else
        {
            //Console.WriteLine("Mob position ignored due to range restrictions");
        }
    }

    private bool WithinRange(Vector3 pointA, Vector3 pointB, float maxRange)
        {
            RectangleF recA = new RectangleF( new PointF(pointA.X - maxRange, pointA.Y - maxRange), new SizeF(maxRange,maxRange));
            RectangleF recB = new RectangleF( new PointF(pointB.X - maxRange, pointB.Y - maxRange), new SizeF(maxRange,maxRange));
            return recA.IntersectsWith(recB);
        }

        private List<string> _disallowedBNpcNames;

        private void GenerateDisallowedBNpcNameList()
        {
            _disallowedBNpcNames = new List< string >();
            var pets = petSheet.Where( c => c.Name.ToString().ToParseable() != "" );
            foreach( var pet in pets )
            {
                _disallowedBNpcNames.Add( pet.Name.ToString().ToParseable() );
            }
            var companions = companionSheet.Where( c => c.Singular.ToString().ToParseable() != "" );
            foreach( var companion in companions )
            {
                _disallowedBNpcNames.Add( companion.Singular.ToString().ToParseable() );
            }
        }

        private bool MobAllowed( uint bnpcNameId )
        {
            if( bnpcNameId == 0 )
            {
                return false;
            }
            var bnpcName = bnpcNameSheet.GetRowOrDefault( bnpcNameId );
            if( bnpcName == null )
            {
                return false;
            }

            if( _disallowedBNpcNames == null )
            {
                GenerateDisallowedBNpcNameList();
            }

            var hashSet = _disallowedBNpcNames.ToHashSet();
            return !hashSet.Contains( bnpcName.Value.Singular.ToString().ToParseable() );
        }

        private Dictionary< uint, bool > _territoriesAllowed = new Dictionary< uint, bool >();
        private bool TerritoryTypeAllowed( uint territoryRowId )
        {
            if( _territoriesAllowed.ContainsKey( territoryRowId ) )
            {
                return _territoriesAllowed[ territoryRowId ];
            }

            var bannedPlaceNames = new List< string >()
            {
                "The Lavender Beds",
                "The Lavender Beds Subdivision",
                "Mist",
                "Mist Subdivision",
                "The Goblet",
                "The Goblet Subdivision",
                "Shirogane",
                "Shirogane Subdivision",
                "Empyreum",
                "Empyreum Subdivision",

            };
            var hashSet = bannedPlaceNames.Select( c => c.ToParseable() ).ToHashSet();



            foreach( var territory in territoryTypeSheet )
            {
                var placeName = territory.PlaceName.Value!.Name.ToString().ToParseable();
                if( hashSet.Contains( placeName ) || territory.PlaceName.RowId == 0 )
                {
                    _territoriesAllowed.Add( territory.RowId, false );
                }
                else
                {
                    _territoriesAllowed.Add( territory.RowId, true );
                }
            }

            return true;
        }
}
