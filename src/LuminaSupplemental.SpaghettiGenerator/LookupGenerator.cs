using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using CSVFile;
using Lumina;
using Lumina.Excel.GeneratedSheets;
using LuminaSupplemental.Excel.Model;
using LuminaSupplemental.SpaghettiGenerator.CodeGen;
using Newtonsoft.Json;

namespace LuminaSupplemental.SpaghettiGenerator
{
    public class LookupGenerator
    {
        public string _sheetTemplate;
        private Dictionary< string, Item > _itemsByString;
        private Dictionary< string, ContentFinderCondition > _dutiesByString;
        

        public LookupGenerator(string tmplPath = null)
        {
            _sheetTemplate = File.ReadAllText( tmplPath ?? "class.tmpl" );
            var itemSheet = Service.GameData.GetExcelSheet<Item>()!;
            var dutySheet = Service.GameData.GetExcelSheet<ContentFinderCondition>()!;

            _itemsByString = new Dictionary<string, Item>();
            foreach (var item in itemSheet)
            {
                _itemsByString.TryAdd(item.Name.ToString().ToParseable(), item);
            }

            _dutiesByString = new Dictionary<string, ContentFinderCondition>();
            foreach (var item in dutySheet)
            {
                _dutiesByString.TryAdd(item.Name.ToString().ToParseable(), item);
            }
        }
        
        public string FixIndent( StringBuilder sb, int level )
        {
            var indent = "";
            for( int i = 0; i < level * 4; i++ )
            {
                indent += " ";
            }

            return indent + sb.ToString().Replace( "\n", $"\n{indent}");
        }

        public string ProcessDutiesJson( string className )
        {
            /*
             * To Process:
             * Dungeon Treasure Coffers
             * Dungeon Boss Treasure Coffers
             * Dungeon Boss Drop(Drop + Currency(amount))
             * 
             */
            Dictionary< uint, HashSet<uint> > dungeonChestItems = new();
            Dictionary< uint, HashSet<uint> > reverseDungeonChestItems = new();
            Dictionary< uint, DungeonChestItem > actualDungeonChestItems = new();
            uint dungeonChestItemCount = 1;
            
            var dutyText = File.ReadAllText( @"FFXIV Data - Duties.json" );
            JsonConverter[] converters = {new CategoryConverter(), new ConditionConverter(), new VersionConverter(), new ChestEnumConverter(), new ChestNameConverter(), new IlvlEnumConverter(), new IlvlUnionConverter(), new TokenNameConverter()};
            var duties = JsonConvert.DeserializeObject<DutyJson[]>(dutyText, converters)!;
            foreach( var duty in duties )
            {
                var dutyName = duty.Name.ToParseable();
                if( _dutiesByString.ContainsKey( dutyName ) )
                {
                    var actualDuty = _dutiesByString[ dutyName ];
                    if( duty.Chests != null )
                    {
                        foreach( var chest in duty.Chests )
                        {
                            foreach( var item in chest.Items )
                            {
                                var itemName = item.ToParseable();
                                var actualItem = _itemsByString.ContainsKey( itemName ) ? _itemsByString[ itemName ] : null;
                                if( actualItem != null )
                                {
                                    var xCoord = float.Parse( chest.Coords.X );
                                    var yCoord = float.Parse( chest.Coords.Y );
                                    DungeonChestItem chestItem = new DungeonChestItem( actualItem.RowId, actualDuty.RowId, new Vector2( xCoord, yCoord ) );
                                    actualDungeonChestItems.Add( dungeonChestItemCount, chestItem );
                                    if( !dungeonChestItems.ContainsKey( actualItem.RowId ) )
                                    {
                                        dungeonChestItems.Add(actualItem.RowId, new HashSet< uint >());
                                    }

                                    if( !dungeonChestItems[ actualItem.RowId ].Contains( dungeonChestItemCount ) )
                                    {
                                        dungeonChestItems[ actualItem.RowId ].Add( dungeonChestItemCount );
                                    }
                                    
                                    if( !reverseDungeonChestItems.ContainsKey( dungeonChestItemCount ) )
                                    {
                                        reverseDungeonChestItems.Add(dungeonChestItemCount, new HashSet< uint >());
                                    }

                                    if( !reverseDungeonChestItems[ dungeonChestItemCount ].Contains(  actualItem.RowId ) )
                                    {
                                        reverseDungeonChestItems[ dungeonChestItemCount ].Add(  actualItem.RowId );
                                    }
                                    dungeonChestItemCount++;
                                }
                            }
                        }
                    }

                    var a = "";
                }
                else
                {
                    Console.WriteLine("Could not find duty " + duty.Name);
                }

            }

            var tmpl = _sheetTemplate;
            tmpl = tmpl.Replace( "%%LOOKUP_NAME%%", className );
            var generators = new List< BaseShitGenerator >();
            generators.Add( new DictionaryHashSetGenerator( "ItemToDungeonChests", dungeonChestItems ) );
            generators.Add( new DictionaryHashSetGenerator( "DungeonChestToItems", reverseDungeonChestItems ) );
            generators.Add( new DictionaryDungeonChestItemGenerator( "DungeonChests", actualDungeonChestItems ) );

            var fieldsSb = new StringBuilder();
            var readsSb = new StringBuilder();
            var structsSb = new StringBuilder();
            var usingsSb = new StringBuilder();
            
            // run the generators
            foreach( var generator in generators )
            {
                generator.WriteFields( fieldsSb );
                // fieldsSb.AppendLine();
                generator.WriteReaders( readsSb );
                // readsSb.AppendLine();
                generator.WriteStructs( structsSb );
            }

            usingsSb.Append( "using System.Numerics;" );
            usingsSb.AppendLine();
            usingsSb.Append( "using LuminaSupplemental.Excel.Model;" );
            usingsSb.AppendLine();

            tmpl = tmpl.Replace( "%%STRUCT_DEFS%%", FixIndent( structsSb, 2 ) );
            tmpl = tmpl.Replace( "%%DATA_MEMBERS%%", FixIndent( fieldsSb, 2 ) );
            tmpl = tmpl.Replace( "%%USING%%", FixIndent( usingsSb, 0 ));

            return tmpl;
        }

        public string ProcessItemsTSV(string className)
        {
            var reader = CSVFile.CSVReader.FromFile(@"FFXIV Data - Items.tsv", CSVSettings.TSV);


            var desynthItems = new Dictionary<uint, HashSet<uint>>();
            var reduceItems = new Dictionary<uint, HashSet<uint>>();
            var lootItems = new Dictionary<uint, HashSet<uint>>();
            var gardeningItems = new Dictionary<uint, HashSet<uint>>();

            var reverseDesynthItems = new Dictionary<uint, HashSet<uint>>();
            var reverseReduceItems = new Dictionary<uint, HashSet<uint>>();
            var reverseLootItems = new Dictionary<uint, HashSet<uint>>();
            var reverseGardeningItems = new Dictionary<uint, HashSet<uint>>();

            foreach (var line in reader.Lines())
            {
                var outputItemId = line[0];
                var method = line[1];
                if (method == "")
                {
                    continue;
                }
                var sources = new List<string>();
                for (int i = 2; i < 13; i++)
                {
                    if (line[i] != "")
                    {
                        sources.Add(line[i]);
                    }
                }

                void MapItems(string s, List<string> list, Dictionary<uint, HashSet<uint>> dictionary, Dictionary<uint, HashSet<uint>> reverseDictionary)
                {
                    s = s.ToParseable();
                    var outputItem = _itemsByString.ContainsKey(s) ? _itemsByString[s] : null;
                    if (outputItem != null)
                    {
                        foreach (var source in list)
                        {
                            var sourceName = source.ToParseable();
                            var sourceItem = _itemsByString.ContainsKey(sourceName) ? _itemsByString[sourceName] : null;
                            if (sourceItem != null)
                            {
                                if (!dictionary.ContainsKey(outputItem.RowId))
                                {
                                    dictionary.Add(outputItem.RowId, new HashSet<uint>());
                                }

                                if (!dictionary[outputItem.RowId].Contains(sourceItem.RowId))
                                {
                                    dictionary[outputItem.RowId].Add(sourceItem.RowId);
                                }
                                
                                if (!reverseDictionary.ContainsKey(sourceItem.RowId))
                                {
                                    reverseDictionary.Add(sourceItem.RowId, new HashSet<uint>());
                                }

                                if (!reverseDictionary[sourceItem.RowId].Contains(outputItem.RowId))
                                {
                                    reverseDictionary[sourceItem.RowId].Add(outputItem.RowId);
                                }
                            }
                            else
                            {
                                Console.WriteLine("Could not find a match for input item: " + s);
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Could not find a match for output item: " + s);
                    }
                }

                switch (method)
                {
                    case "Desynth":
                        MapItems(outputItemId, sources, desynthItems, reverseDesynthItems);
                        break;
                    case "Reduce":
                        MapItems(outputItemId, sources, reduceItems, reverseReduceItems);
                        break;
                    case "Loot":
                        MapItems(outputItemId, sources, lootItems, reverseLootItems);
                        break;
                    case "Gardening":
                        MapItems(outputItemId, sources, gardeningItems, reverseGardeningItems);
                        break;
                }
            }
            var tmpl = _sheetTemplate;
            tmpl = tmpl.Replace( "%%LOOKUP_NAME%%", className );
            var generators = new List< BaseShitGenerator >();
            generators.Add( new DictionaryHashSetGenerator( "DesynthItems", desynthItems ) );
            generators.Add( new DictionaryHashSetGenerator( "ReduceItems", reduceItems ) );
            generators.Add( new DictionaryHashSetGenerator( "LootItems", lootItems ) );
            generators.Add( new DictionaryHashSetGenerator( "GardeningItems", gardeningItems ) );
            generators.Add( new DictionaryHashSetGenerator( "ReverseDesynthItems", reverseDesynthItems ) );
            generators.Add( new DictionaryHashSetGenerator( "ReverseReduceItems", reverseReduceItems ) );
            generators.Add( new DictionaryHashSetGenerator( "ReverseLootItems", reverseLootItems ) );
            generators.Add( new DictionaryHashSetGenerator( "ReverseGardeningItems", reverseGardeningItems ) );

            var fieldsSb = new StringBuilder();
            var readsSb = new StringBuilder();
            var structsSb = new StringBuilder();
            
            // run the generators
            foreach( var generator in generators )
            {
                generator.WriteFields( fieldsSb );
                // fieldsSb.AppendLine();
                generator.WriteReaders( readsSb );
                // readsSb.AppendLine();
                generator.WriteStructs( structsSb );
            }

            tmpl = tmpl.Replace( "%%STRUCT_DEFS%%", FixIndent( structsSb, 2 ) );
            tmpl = tmpl.Replace( "%%DATA_MEMBERS%%", FixIndent( fieldsSb, 2 ) );
            tmpl = tmpl.Replace( "%%USING%%", "" );

            return tmpl;
        }

        public string ProcessMobData(string className)
        {
            var itemDrops = Service.DatabaseBuilder.ItemDropsByMobId;
            var placeNamesByMobId = Service.DatabaseBuilder.PlaceNamesByMobId;
            var mobDrops = new Dictionary< uint, HashSet< uint > >();
            foreach( var itemDrop in itemDrops )
            {
                foreach( var itemId in itemDrop.Value )
                {
                    if( !mobDrops.TryGetValue( itemId, out var mobIds ) )
                    {
                        mobIds = new HashSet< uint >();
                        mobDrops[ itemId ] = mobIds;
                    }

                    mobIds.Add( itemDrop.Key );
                }
            }

            var tmpl = _sheetTemplate;
            tmpl = tmpl.Replace( "%%LOOKUP_NAME%%", className );
            var generators = new List< BaseShitGenerator >();
            generators.Add( new DictionaryHashSetGenerator( "ItemDropsByBNpcNameId", itemDrops ) );
            generators.Add( new DictionaryHashSetGenerator( "MobDropsByItemId", mobDrops ) );
            generators.Add( new DictionaryHashSetGenerator( "PlaceNamesByMobId", placeNamesByMobId ) );

            var fieldsSb = new StringBuilder();
            var readsSb = new StringBuilder();
            var structsSb = new StringBuilder();
            
            // run the generators
            foreach( var generator in generators )
            {
                generator.WriteFields( fieldsSb );
                // fieldsSb.AppendLine();
                generator.WriteReaders( readsSb );
                // readsSb.AppendLine();
                generator.WriteStructs( structsSb );
            }

            tmpl = tmpl.Replace( "%%STRUCT_DEFS%%", FixIndent( structsSb, 2 ) );
            tmpl = tmpl.Replace( "%%DATA_MEMBERS%%", FixIndent( fieldsSb, 2 ) );
            tmpl = tmpl.Replace( "%%USING%%", "" );

            return tmpl;
        }
    }
}