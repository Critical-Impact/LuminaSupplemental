using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CSVFile;
using Lumina;
using Lumina.Excel.GeneratedSheets;
using LuminaSupplemental.SpaghettiGenerator.CodeGen;
using Newtonsoft.Json;

namespace LuminaSupplemental.SpaghettiGenerator
{
    public class LookupGenerator
    {
        public GameData GameData;
        public string _sheetTemplate;
        private Dictionary< string, Item > _itemsByString;


        public LookupGenerator(string path, string tmplPath = null)
        {
            GameData = new GameData( path );
            _sheetTemplate = File.ReadAllText( tmplPath ?? "class.tmpl" );
            var itemSheet = GameData.GetExcelSheet<Item>()!;

            _itemsByString = new Dictionary<string, Item>();
            foreach (var item in itemSheet)
            {
                _itemsByString.TryAdd(item.Name.ToString(), item);
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
            var dutyText = File.ReadAllText( @"FFXIV Data - Duties.json" );
            JsonConverter[] converters = {new CategoryConverter(), new ConditionConverter(), new VersionConverter(), new ChestEnumConverter(), new ChestNameConverter(), new IlvlEnumConverter(), new IlvlUnionConverter(), new TokenNameConverter()};
            var duties = JsonConvert.DeserializeObject<DutyJson[]>(dutyText, converters)!;
            foreach( var duty in duties )
            {
                foreach( var chest in duty.Chests )
                {
                    foreach( var item in chest.Items )
                    {
                        var actualItem = _itemsByString.ContainsKey(item) ? _itemsByString[item] : null;

                    }
                }
                var a = "";
            }

            return "";
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
                    var outputItem = _itemsByString.ContainsKey(s) ? _itemsByString[s] : null;
                    if (outputItem != null)
                    {
                        foreach (var source in list)
                        {
                            var sourceItem = _itemsByString.ContainsKey(source) ? _itemsByString[source] : null;
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

            return tmpl;
        }
    }
}