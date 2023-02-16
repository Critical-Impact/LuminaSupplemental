using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using CSVFile;
using CsvHelper;
using Lumina.Excel;
using Lumina.Excel.GeneratedSheets;
using LuminaSupplemental.Excel.Model;
using Newtonsoft.Json;

namespace LuminaSupplemental.SpaghettiGenerator
{
    public class LookupGenerator
    {
        private Dictionary< string, Item > _itemsByString;
        private Dictionary< string, ContentFinderCondition > _dutiesByString;
        private Dictionary< string, BNpcName > _bNpcsByName;
        private Dictionary< string, SubmarineExploration > _submarinesByName;
        private Dictionary< string, AirshipExplorationPoint > _airshipsByName;
        private ExcelSheet<Item> _itemSheet;
        private ExcelSheet<BNpcName> _bnpcNameSheet;
        private readonly ExcelSheet<SubmarineExploration> _submarineSheet;
        private readonly ExcelSheet<AirshipExplorationPoint> _airshipSheet;


        public LookupGenerator(string tmplPath = null)
        {
            _itemSheet = Service.GameData.GetExcelSheet<Item>()!;
            _bnpcNameSheet = Service.GameData.GetExcelSheet<BNpcName>()!;
            _submarineSheet = Service.GameData.GetExcelSheet<SubmarineExploration>()!;
            _airshipSheet = Service.GameData.GetExcelSheet<AirshipExplorationPoint>()!;
            var dutySheet = Service.GameData.GetExcelSheet<ContentFinderCondition>()!;

            _itemsByString = new Dictionary<string, Item>();
            foreach (var item in _itemSheet)
            {
                _itemsByString.TryAdd(item.Name.ToString().ToParseable(), item);
            }

            _dutiesByString = new Dictionary<string, ContentFinderCondition>();
            foreach (var item in dutySheet)
            {
                _dutiesByString.TryAdd(item.Name.ToString().ToParseable(), item);
            }

            _bNpcsByName = new Dictionary<string, BNpcName>();
            foreach (var bNpcName in _bnpcNameSheet)
            {
                _bNpcsByName.TryAdd(bNpcName.Singular.ToString().ToParseable(), bNpcName);
            }

            _airshipsByName = new Dictionary<string, AirshipExplorationPoint>();
            foreach (var airship in _airshipSheet)
            {
                _airshipsByName.TryAdd(airship.NameShort.ToString().ToParseable(), airship);
            }

            _submarinesByName = new Dictionary<string, SubmarineExploration>();
            foreach (var submarine in _submarineSheet)
            {
                _submarinesByName.TryAdd(submarine.Destination.ToString().ToParseable(), submarine);
            }
        }
        
        public string FixIndent( StringBuilder sb, int level )
        {
            var indent = "";
            for( var i = 0; i < level * 4; i++ )
            {
                indent += " ";
            }

            return indent + sb.ToString().Replace( "\n", $"\n{indent}");
        }

        public void ProcessDutiesJson( List< DungeonChest > dungeonChests, List< DungeonChestItem > dungeonChestItems, List< DungeonBoss > dungeonBosses, List< DungeonBossChest > dungeonBossChests )
        {
            var dungeonBossCount = 1u;
            var dungeonBossChestCount = 1u;
            
            var dutyText = File.ReadAllText( @"FFXIV Data - Duties.json" );
            JsonConverter[] converters = {new CategoryConverter(), new ConditionConverter(), new VersionConverter(), new ChestEnumConverter(), new ChestNameConverter(), new IlvlEnumConverter(), new IlvlUnionConverter(), new TokenNameConverter()};
            var duties = JsonConvert.DeserializeObject<DutyJson[]>(dutyText, converters)!;
            foreach( var duty in duties )
            {
                var dutyName = duty.Name.ToParseable();
                if( _dutiesByString.ContainsKey( dutyName ) )
                {
                    var actualDuty = _dutiesByString[ dutyName ];
                    if( duty.Fights != null )
                    {
                        for( var index = 0; index < duty.Fights.Length; index++ )
                        {
                            var fight = duty.Fights[ index ];
                            foreach( var boss in fight.Boss )
                            {
                                var bossName = boss.Name.ToParseable();
                                if( _bNpcsByName.ContainsKey( bossName ) )
                                {
                                    var bnpc = _bNpcsByName[ bossName ];
                                    dungeonBosses.Add( new DungeonBoss(dungeonBossCount, bnpc.RowId, (uint)index, actualDuty.RowId ) );
                                    dungeonBossCount++;
                                }
                                else
                                {
                                    Console.WriteLine( "Could not parse boss with name: " + boss.Name );
                                }
                            }

                            if( fight.Treasures != null )
                            {
                                foreach( var treasure in fight.Treasures )
                                {
                                    var chestNo = (uint)treasure.Name;
                                    foreach( var item in treasure.Items )
                                    {
                                        var itemName = item.ToParseable();
                                        var actualItem = _itemsByString.ContainsKey( itemName ) ? _itemsByString[ itemName ] : null;
                                        if( actualItem != null )
                                        {
                                            dungeonBossChests.Add( new DungeonBossChest(dungeonBossChestCount, (uint)index, actualItem.RowId, actualDuty.RowId, 1, chestNo ) );
                                            dungeonBossChestCount++;
                                        }
                                        else
                                        {
                                            Console.WriteLine( "Could not find item with name " + item + " in duty " + duty.Name );
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if( duty.Chests != null )
                    {
                        for( var index = 0; index < duty.Chests.Length; index++ )
                        {
                            var chest = duty.Chests[ index ];
                            var xCoord = float.Parse( chest.Coords.X );
                            var yCoord = float.Parse( chest.Coords.Y );
                            var dungeonChest = new DungeonChest( (uint)(dungeonChests.Count + 1), (byte)(index + 1), actualDuty.RowId, new Vector2( xCoord, yCoord ) );
                            dungeonChests.Add( dungeonChest );
                            foreach( var item in chest.Items )
                            {
                                var itemName = item.ToParseable();
                                var actualItem = _itemsByString.ContainsKey( itemName ) ? _itemsByString[ itemName ] : null;
                                if( actualItem != null )
                                {

                                    var chestItem = new DungeonChestItem( (uint)(dungeonChestItems.Count + 1), actualItem.RowId, dungeonChest.RowId );
                                    dungeonChestItems.Add( chestItem );
                                }
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Could not find duty " + duty.Name);
                }
            }
        }

        public void ParseExtraItemSets(List<ItemSupplement> itemSupplements)
        {
            var cofferNames = new Dictionary<string[], string>
            {
                {new []{"Crystarium", "Coffer"}, "Augmented Crystarium"},
                {new []{"Abyssos", "Coffer"}, "Abyssos"},
                {new []{"Alexandrian", "Coffer"}, "Alexandrian"},
                {new []{"Allagan", "Coffer"}, "Allagan"},
                {new []{"High Allagan", "Coffer"}, "High Allagan"},
                {new []{"Asphodelos", "Coffer"}, "Asphodelos"},
                {new []{"Bluefeather", "Coffer"}, "Bluefeather"},
                {new []{"Byakko", "Coffer"}, "Byakko"},
            };
            foreach( var cofferName in cofferNames )
            {
                var coffers = _itemSheet.Where( c =>
                {
                    return c.Name.ToString().StartsWith( cofferName.Key[ 0 ] ) && cofferName.Key.Skip( 1 ).All( d => c.Name.ToString().Contains( d ) );
                } ).ToList();
                foreach( var coffer in coffers )
                {
                    //Weapon, Gear, Accessories
                    if( coffer.Name.ToString().Contains( "Weapon" ) )
                    {
                        var items = _itemSheet.Where( c =>
                            c.Name.ToString().Contains( cofferName.Value ) &&
                            ( ( c.EquipSlotCategory.Value?.MainHand ?? 0 ) == 1 || ( c.EquipSlotCategory.Value?.OffHand ?? 0 ) == 1 ) );
                        foreach( var item in items )
                        {
                            
                            itemSupplements.Add( new ItemSupplement((uint)itemSupplements.Count + 1, item.RowId, coffer.RowId, ItemSupplementSource.Loot) );
                        }
                    }

                    if( coffer.Name.ToString().Contains( "Gear" ) )
                    {
                        var armourTypes = new string[]
                        {
                            "Striking",
                            "Maiming",
                            "Fending",
                            "Aiming",
                            "Scouting",
                            "Healing",
                            "Casting",
                            "Slaying"
                        };
                        foreach( var armourType in armourTypes )
                        {
                            if( coffer.Name.ToString().Contains( armourType ) )
                            {
                                var items = _itemSheet.Where( c =>
                                    c.Name.ToString().Contains( cofferName.Value ) && c.Name.ToString().Contains( armourType ) && 
                                    ( 
                                        ( c.EquipSlotCategory.Value?.Body ?? 0 ) == 1 || 
                                        ( c.EquipSlotCategory.Value?.Feet ?? 0 ) == 1  || 
                                        ( c.EquipSlotCategory.Value?.Head ?? 0 ) == 1  || 
                                        ( c.EquipSlotCategory.Value?.Gloves ?? 0 ) == 1  || 
                                        ( c.EquipSlotCategory.Value?.Legs ?? 0 ) == 1 
                                    ));
                                foreach( var item in items )
                                {
                                    itemSupplements.Add( new ItemSupplement((uint)itemSupplements.Count + 1, item.RowId, coffer.RowId, ItemSupplementSource.Loot) );
                                }
                            }
                        }
                    }

                    if( coffer.Name.ToString().Contains( "Head Gear" ) )
                    {
                        var items = _itemSheet.Where( c =>
                            c.Name.ToString().StartsWith( cofferName.Value ) && c.Name.ToString().Contains( " of " ) && 
                            ( c.EquipSlotCategory.Value?.Head ?? 0 ) == 1);
                        foreach( var item in items )
                        {
                            itemSupplements.Add( new ItemSupplement((uint)itemSupplements.Count + 1, item.RowId, coffer.RowId, ItemSupplementSource.Loot) );
                        }
                    }

                    if( coffer.Name.ToString().Contains( "Chest Gear" ) )
                    {
                        var items = _itemSheet.Where( c =>
                            c.Name.ToString().StartsWith( cofferName.Value ) && c.Name.ToString().Contains( " of " ) && 
                            ( c.EquipSlotCategory.Value?.Body ?? 0 ) == 1);
                        foreach( var item in items )
                        {
                            itemSupplements.Add( new ItemSupplement((uint)itemSupplements.Count + 1, item.RowId, coffer.RowId, ItemSupplementSource.Loot) );
                        }
                    }

                    if( coffer.Name.ToString().Contains( "Hand Gear" ) )
                    {
                        var items = _itemSheet.Where( c =>
                            c.Name.ToString().StartsWith( cofferName.Value ) && c.Name.ToString().Contains( " of " ) && 
                            ( c.EquipSlotCategory.Value?.Gloves ?? 0 ) == 1);
                        foreach( var item in items )
                        {
                            itemSupplements.Add( new ItemSupplement((uint)itemSupplements.Count + 1, item.RowId, coffer.RowId, ItemSupplementSource.Loot) );
                        }
                    }

                    if( coffer.Name.ToString().Contains( "Leg Gear" ) )
                    {
                        var items = _itemSheet.Where( c =>
                            c.Name.ToString().StartsWith( cofferName.Value ) && c.Name.ToString().Contains( " of " ) && 
                            ( c.EquipSlotCategory.Value?.Legs ?? 0 ) == 1);
                        foreach( var item in items )
                        {
                            itemSupplements.Add( new ItemSupplement((uint)itemSupplements.Count + 1, item.RowId, coffer.RowId, ItemSupplementSource.Loot) );
                        }
                    }

                    if( coffer.Name.ToString().Contains( "Foot Gear" ) )
                    {
                        var items = _itemSheet.Where( c =>
                            c.Name.ToString().StartsWith( cofferName.Value ) && c.Name.ToString().Contains( " of " ) && 
                            ( c.EquipSlotCategory.Value?.Feet ?? 0 ) == 1);
                        foreach( var item in items )
                        {
                            itemSupplements.Add( new ItemSupplement((uint)itemSupplements.Count + 1, item.RowId, coffer.RowId, ItemSupplementSource.Loot) );
                        }
                    }

                    if( coffer.Name.ToString().Contains( "Earring Coffer" ) )
                    {
                        var items = _itemSheet.Where( c =>
                            c.Name.ToString().StartsWith( cofferName.Value ) && c.Name.ToString().Contains( " of " ) && 
                            ( c.EquipSlotCategory.Value?.Ears ?? 0 ) == 1);
                        foreach( var item in items )
                        {
                            itemSupplements.Add( new ItemSupplement((uint)itemSupplements.Count + 1, item.RowId, coffer.RowId, ItemSupplementSource.Loot) );
                        }
                    }

                    if( coffer.Name.ToString().Contains( "Necklace Coffer" ) )
                    {
                        var items = _itemSheet.Where( c =>
                            c.Name.ToString().StartsWith( cofferName.Value ) && c.Name.ToString().Contains( " of " ) && 
                            ( c.EquipSlotCategory.Value?.Neck ?? 0 ) == 1);
                        foreach( var item in items )
                        {
                            itemSupplements.Add( new ItemSupplement((uint)itemSupplements.Count + 1, item.RowId, coffer.RowId, ItemSupplementSource.Loot) );
                        }
                    }

                    if( coffer.Name.ToString().Contains( "Bracelet Coffer" ) )
                    {
                        var items = _itemSheet.Where( c =>
                            c.Name.ToString().StartsWith( cofferName.Value ) && c.Name.ToString().Contains( " of " ) && 
                            ( c.EquipSlotCategory.Value?.Wrists ?? 0 ) == 1);
                        foreach( var item in items )
                        {
                            itemSupplements.Add( new ItemSupplement((uint)itemSupplements.Count + 1, item.RowId, coffer.RowId, ItemSupplementSource.Loot) );
                        }
                    }

                    if( coffer.Name.ToString().Contains( "Ring Coffer" ) )
                    {
                        var items = _itemSheet.Where( c =>
                            c.Name.ToString().StartsWith( cofferName.Value ) && c.Name.ToString().Contains( " of " ) && 
                            ( c.EquipSlotCategory.Value?.FingerL ?? 0 ) == 1);
                        foreach( var item in items )
                        {
                            itemSupplements.Add( new ItemSupplement((uint)itemSupplements.Count + 1, item.RowId, coffer.RowId, ItemSupplementSource.Loot) );
                        }
                    }

                    if( coffer.Name.ToString().Contains( "Accessories" ) )
                    {
                        var armourTypes = new string[]
                        {
                            "Striking",
                            "Maiming",
                            "Fending",
                            "Aiming",
                            "Scouting",
                            "Healing",
                            "Casting",
                            "Slaying"
                        };
                        foreach( var armourType in armourTypes )
                        {
                            if( coffer.Name.ToString().Contains( armourType ) )
                            {
                                var items = _itemSheet.Where( c => c.Name.ToString().Contains( cofferName.Value ) && c.Name.ToString().Contains( armourType ) && 
                               ( 
                                   ( c.EquipSlotCategory.Value?.Ears ?? 0 ) == 1 || 
                                   ( c.EquipSlotCategory.Value?.Neck ?? 0 ) == 1  || 
                                   ( c.EquipSlotCategory.Value?.FingerL ?? 0 ) == 1  || 
                                   ( c.EquipSlotCategory.Value?.FingerR ?? 0 ) == 1  || 
                                   ( c.EquipSlotCategory.Value?.Wrists ?? 0 ) == 1 
                               ));
                                foreach( var item in items )
                                {
                                    itemSupplements.Add( new ItemSupplement((uint)itemSupplements.Count + 1, item.RowId, coffer.RowId, ItemSupplementSource.Loot) );
                                }
                            }
                        }
                    }
                }

            }
        }

        public void Generate()
        {
            var itemSupplements = new List<ItemSupplement>();
            var submarineDrops = new List<SubmarineDrop>();
            var airshipDrops = new List<AirshipDrop>();
            var dungeonDrops = new List<DungeonDrop>();
            var mobDrops = new List< MobDrop >();
            var dungeonChests = new List< DungeonChest >();
            var dungeonChestItems = new List< DungeonChestItem >();
            var dungeonBosses = new List< DungeonBoss >();
            var dungeonBossChests = new List< DungeonBossChest >();
            
            ProcessItemsTSV(itemSupplements, submarineDrops, airshipDrops, dungeonDrops);
            ParseExtraItemSets(itemSupplements);
            ProcessMobDrops( mobDrops );
            ProcessDutiesJson( dungeonChests, dungeonChestItems, dungeonBosses, dungeonBossChests );

            WriteFile( itemSupplements, $"./output/ItemSupplement.csv" );
            WriteFile( airshipDrops, $"./output/AirshipDrop.csv" );
            WriteFile( submarineDrops, $"./output/SubmarineDrop.csv" );
            WriteFile( dungeonDrops, $"./output/DungeonDrop.csv" );
            WriteFile( mobDrops, $"./output/MobDrop.csv" );
            WriteFile( dungeonChests, $"./output/DungeonChest.csv" );
            WriteFile( dungeonChestItems, $"./output/DungeonChestItem.csv" );
            WriteFile( dungeonBosses, $"./output/DungeonBoss.csv" );
            WriteFile( dungeonBossChests, $"./output/DungeonBossChest.csv" );
        }

        public void WriteFile< T >( List< T > items, string outputPath )
        {
            using var fileStream5 = new FileStream( outputPath, FileMode.Create );
            var writer5 = new CsvWriter( new StreamWriter( fileStream5 ), CultureInfo.CurrentCulture );
            writer5.WriteHeader<T>();
            writer5.NextRecord();
            foreach( var item in items )
            {
                writer5.WriteRecord<T>( item );
                writer5.NextRecord();
            }
            writer5.Flush();
            fileStream5.Close();
        }

        private void ProcessItemsTSV( List< ItemSupplement > itemSupplements, List< SubmarineDrop > submarineDrops, List< AirshipDrop > airshipDrops, List< DungeonDrop > dungeonDrops )
        {
            var reader = CSVFile.CSVReader.FromFile(@"FFXIV Data - Items.tsv", CSVSettings.TSV);

            foreach (var line in reader.Lines())
            {
                var outputItemId = line[0];
                var method = line[1];
                if (method == "")
                {
                    continue;
                }
                var sources = new List<string>();
                for (var i = 2; i < 13; i++)
                {
                    if (line[i] != "")
                    {
                        sources.Add(line[i]);
                    }
                }

                ItemSupplementSource? source; 
                switch (method)
                {
                    case "Desynth":
                        source = ItemSupplementSource.Desynth;
                        GenerateItemSupplement( source, outputItemId, sources, itemSupplements );
                        break;
                    case "Reduce":
                        source = ItemSupplementSource.Reduction;
                        GenerateItemSupplement( source, outputItemId, sources, itemSupplements );
                        break;
                    case "Loot":
                        source = ItemSupplementSource.Loot;
                        GenerateItemSupplement( source, outputItemId, sources, itemSupplements );
                        break;
                    case "Gardening":
                        source = ItemSupplementSource.Gardening;
                        GenerateItemSupplement( source, outputItemId, sources, itemSupplements );
                        break;
                    case "Voyage":
                        GenerateSubmarineDrops( outputItemId, sources, submarineDrops );
                        GenerateAirshipDrops( outputItemId, sources, airshipDrops );
                        break;
                    case "Instance":
                        GenerateDungeonDrops( outputItemId, sources, dungeonDrops );
                        break;
                }

                
            }
        }
        
        private void GenerateSubmarineDrops( string outputItemId, List< string > sources, List< SubmarineDrop > submarineDrops )
        {
            outputItemId = outputItemId.ToParseable();
            var outputItem = _itemsByString.ContainsKey( outputItemId ) ? _itemsByString[ outputItemId ] : null;
            if( outputItem != null )
            {
                foreach( var sourceItem in sources )
                {
                    var sourceName = sourceItem.ToParseable();
                    var submarineExploration = _submarinesByName.ContainsKey( sourceName ) ? _submarinesByName[ sourceName ] : null;
                    if( submarineExploration != null )
                    {
                        submarineDrops.Add( new SubmarineDrop( (uint)submarineDrops.Count + 1, outputItem.RowId, submarineExploration.RowId ) );
                    }
                }
            }
            else
            {
                Console.WriteLine( "Could not find a match for output item: " + outputItemId );
            }
        }
        
        private void GenerateAirshipDrops( string outputItemId, List< string > sources, List< AirshipDrop > airshipDrops )
        {
            outputItemId = outputItemId.ToParseable();
            var outputItem = _itemsByString.ContainsKey( outputItemId ) ? _itemsByString[ outputItemId ] : null;
            if( outputItem != null )
            {
                foreach( var sourceItem in sources )
                {
                    var sourceName = sourceItem.ToParseable();
                    var airshipExploration = _airshipsByName.ContainsKey( sourceName ) ? _airshipsByName[ sourceName ] : null;
                    if( airshipExploration != null )
                    {
                        airshipDrops.Add( new AirshipDrop( (uint)airshipDrops.Count + 1, outputItem.RowId, airshipExploration.RowId ) );
                    }
                    else
                    {
                        Console.WriteLine( "Could not find a match for input item: " + outputItemId + " and airship zone " + sourceName );
                    }
                }
            }
            else
            {
                Console.WriteLine( "Could not find a match for output item: " + outputItemId );
            }
        }
        
        private void GenerateDungeonDrops( string outputItemId, List< string > sources, List< DungeonDrop > dungeonDrops )
        {
            outputItemId = outputItemId.ToParseable();
            var outputItem = _itemsByString.ContainsKey( outputItemId ) ? _itemsByString[ outputItemId ] : null;
            if( outputItem != null )
            {
                foreach( var sourceItem in sources )
                {
                    var sourceName = sourceItem.ToParseable();
                    var duty = _dutiesByString.ContainsKey( sourceName ) ? _dutiesByString[ sourceName ] : null;
                    if( duty != null )
                    {
                        dungeonDrops.Add( new DungeonDrop( (uint)dungeonDrops.Count + 1, outputItem.RowId, duty.RowId ) );
                    }
                    else
                    {
                        Console.WriteLine( "Could not find a match for input item: " + outputItemId + " and duty " + sourceName );
                    }
                }
            }
            else
            {
                Console.WriteLine( "Could not find a match for output item: " + outputItemId );
            }
        }

        private void GenerateItemSupplement( ItemSupplementSource? source, string outputItemId, List< string > sources, List< ItemSupplement > itemSupplements )
        {
            if( source == null )
            {
                return;
            }

            outputItemId = outputItemId.ToParseable();
            var outputItem = _itemsByString.ContainsKey( outputItemId ) ? _itemsByString[ outputItemId ] : null;
            if( outputItem != null )
            {
                foreach( var sourceItem in sources )
                {
                    var sourceName = sourceItem.ToParseable();
                    var actualItem = _itemsByString.ContainsKey( sourceName ) ? _itemsByString[ sourceName ] : null;
                    if( actualItem != null )
                    {
                        itemSupplements.Add( new ItemSupplement( (uint)itemSupplements.Count + 1, outputItem.RowId, actualItem.RowId, source.Value ) );
                    }
                    else
                    {
                        Console.WriteLine( "Could not find a match for input item: " + outputItemId + " and source " + sourceName );
                    }
                }
            }
            else
            {
                Console.WriteLine( "Could not find a match for output item: " + outputItemId );
            }
        }

        public void ProcessMobDrops(List<MobDrop> mobDrops)
        {
            var itemDrops = Service.DatabaseBuilder.ItemDropsByMobId;
            foreach( var itemDrop in itemDrops )
            {
                foreach( var itemId in itemDrop.Value )
                {
                    mobDrops.Add( new MobDrop((uint)(mobDrops.Count + 1), itemId, itemDrop.Key ) );
                }
            }
        }
    }
}