using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Text;
using CSVFile;
using CsvHelper;
using Garland.Data;
using Lumina.Excel;
using Lumina.Excel.GeneratedSheets;
using LuminaSupplemental.Excel.Model;
using Newtonsoft.Json;

namespace LuminaSupplemental.SpaghettiGenerator
{
    public class LookupGenerator
    {
        private Dictionary< string, Item > _itemsByName;
        private Dictionary< string, ContentFinderCondition > _dutiesByString;
        private Dictionary< string, BNpcName > _bNpcsByName;
        private Dictionary< string, SubmarineExploration > _submarinesByName;
        private Dictionary< string, AirshipExplorationPoint > _airshipsByName;
        private Dictionary< string, RetainerTaskRandom > _retainerTaskRandomByName;
        private Dictionary< string, FittingShopItemSet > _fittingShopItemSetByName;
        private ExcelSheet<Item> _itemSheet;
        private HashSet<uint> _bannedItems;
        private ExcelSheet<BNpcName> _bnpcNameSheet;
        private AppConfig _appConfig;
        private readonly ExcelSheet<SubmarineExploration> _submarineSheet;
        private readonly ExcelSheet<AirshipExplorationPoint> _airshipSheet;
        private readonly ExcelSheet<RetainerTaskRandom> _retainerTaskRandomSheet;
        private readonly ExcelSheet<FittingShopItemSet> _fittingShopItemSetSheet;
        private readonly ExcelSheet<TerritoryType> _territoryTypeSheet;
        private readonly ExcelSheet<Companion> _companionSheet;
        private readonly ExcelSheet<Pet> _petSheet;
        private readonly ExcelSheet<ENpcResident> _enpcResidentSheet;


        public LookupGenerator(AppConfig appConfig)
        {
            _itemSheet = Service.GameData.GetExcelSheet<Item>()!;
            _bnpcNameSheet = Service.GameData.GetExcelSheet<BNpcName>()!;
            _submarineSheet = Service.GameData.GetExcelSheet<SubmarineExploration>()!;
            _airshipSheet = Service.GameData.GetExcelSheet<AirshipExplorationPoint>()!;
            _territoryTypeSheet = Service.GameData.GetExcelSheet<TerritoryType>()!;
            _retainerTaskRandomSheet = Service.GameData.GetExcelSheet<RetainerTaskRandom>()!;
            _fittingShopItemSetSheet = Service.GameData.GetExcelSheet<FittingShopItemSet>()!;
            _companionSheet = Service.GameData.GetExcelSheet<Companion>()!;
            _petSheet = Service.GameData.GetExcelSheet<Pet>()!;
            _enpcResidentSheet = Service.GameData.GetExcelSheet<ENpcResident>()!;
            _appConfig = appConfig;
            _bannedItems = new HashSet< uint >()
            {
                0,
                24225
            };
            var dutySheet = Service.GameData.GetExcelSheet<ContentFinderCondition>()!;

            _itemsByName = new Dictionary<string, Item>();
            foreach (var item in _itemSheet.Where(c => !_bannedItems.Contains(c.RowId) && c.Name.ToString() != ""))
            {
                _itemsByName.TryAdd(item.Name.ToString().ToParseable(), item);
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

            _retainerTaskRandomByName = new Dictionary<string, RetainerTaskRandom>();
            foreach (var retainerTaskRandom in _retainerTaskRandomSheet)
            {
                _retainerTaskRandomByName.TryAdd(retainerTaskRandom.Name.ToString().ToParseable(), retainerTaskRandom);
            }

            _submarinesByName = new Dictionary<string, SubmarineExploration>();
            foreach (var submarine in _submarineSheet)
            {
                _submarinesByName.TryAdd(submarine.Destination.ToString().ToParseable(), submarine);
            }

            _fittingShopItemSetByName = new Dictionary<string, FittingShopItemSet>();
            foreach (var fittingShopItemSet in _fittingShopItemSetSheet)
            {
                _fittingShopItemSetByName.TryAdd(fittingShopItemSet.Unknown6.ToString().ToParseable(), fittingShopItemSet);
            }
        }
        
        public void Generate()
        {
            var itemSupplements = new List<ItemSupplement>();
            var submarineDrops = new List<SubmarineDrop>();
            var airshipDrops = new List<AirshipDrop>();
            var dungeonDrops = new List<DungeonDrop>();
            var airshipUnlocks = new List< AirshipUnlock >();
            var submarineUnlocks = new List< SubmarineUnlock >();
            var mobDrops = new List< MobDrop >();
            var dungeonChests = new List< DungeonChest >();
            var dungeonChestItems = new List< DungeonChestItem >();
            var dungeonBosses = new List< DungeonBoss >();
            var dungeonBossChests = new List< DungeonBossChest >();
            var dungeonBossDrops = new List< DungeonBossDrop >();
            var eNpcPlaces = new List< ENpcPlace >();
            var shopNames = new List< ShopName >();
            var eNpcShops = new List< ENpcShop >();
            var mobSpawns = new List< MobSpawnPosition >();
            var itemPatches = new List< ItemPatch >();
            var retainerVentureItems = new List< RetainerVentureItem >();
            var storeItems = new List< StoreItem >();
            var houseVendors = new List< HouseVendor >();

            if( _appConfig.Parsing.ParseOnlineSources )
            {
                Console.WriteLine("Parsing Lodestone");
                LodestoneParser.ParsePages(_appConfig.Parsing.OnlineCacheDirectory);
                Console.WriteLine("Parsing SQ Store");
                StoreParser.UpdateItems(_appConfig.Parsing.OnlineCacheDirectory);
            }

            Console.WriteLine("Processing supplement TSVs");
            ProcessItemsTSV(itemSupplements, dungeonDrops);
            Console.WriteLine("Calculating extra item sets");
            ParseExtraItemSets(itemSupplements);
            Console.WriteLine("Processing manually supplied supplements");
            ProcessManualItems( itemSupplements );
            if( _appConfig.Parsing.ProcessMobSpawnHTML )
            {
                Console.WriteLine("Processing mob drop data from lodestone HTML, this may take a while");
                ProcessMobDrops( mobDrops );
            }
            
            Console.WriteLine("Processing duty information");
            ProcessDutiesJson( dungeonChests, dungeonChestItems, dungeonBosses, dungeonBossChests, dungeonBossDrops );
            Console.WriteLine("Processing additional event NPC locations");
            ProcessEventNpcs( eNpcPlaces );
            Console.WriteLine("Processing additional shop names");
            ProcessShopNames( shopNames );
            Console.WriteLine("Processing additional event shops");
            ProcessEventShops( eNpcShops );
            Console.WriteLine("Processing airship unlock/drop data");
            ProcessAirshipUnlocks( airshipUnlocks, airshipDrops );
            Console.WriteLine("Processing submarine unlock/drop data");
            ProcessSubmarineUnlocks( submarineUnlocks, submarineDrops );
            Console.WriteLine("Processing mob spawn data");
            ProcessMobSpawnData( mobSpawns );
            Console.WriteLine("Processing patch information about items");
            ProcessPatchData( itemPatches );
            Console.WriteLine("Calculating venture items");
            ProcessRetainerVentures( retainerVentureItems );
            Console.WriteLine("Processing SQ store items");
            ProcessStoreItems( storeItems );
            Console.WriteLine("Processing Skybuilder Items");
            ProcessSkybuilderItems(itemSupplements);
            Console.WriteLine("Processing house vendors");
            ProcessHouseVendors( houseVendors );

            Console.WriteLine("Writing to output directory");
            WriteFile( itemSupplements, $"./output/ItemSupplement.csv" );
            WriteFile( airshipDrops, $"./output/AirshipDrop.csv" );
            WriteFile( submarineDrops, $"./output/SubmarineDrop.csv" );
            WriteFile( dungeonDrops, $"./output/DungeonDrop.csv" );
            if( _appConfig.Parsing.ProcessMobSpawnHTML )
            {
                WriteFile( mobDrops, $"./output/MobDrop.csv" );
            }

            WriteFile( dungeonChests, $"./output/DungeonChest.csv" );
            WriteFile( dungeonChestItems, $"./output/DungeonChestItem.csv" );
            WriteFile( dungeonBosses, $"./output/DungeonBoss.csv" );
            WriteFile( dungeonBossChests, $"./output/DungeonBossChest.csv" );
            WriteFile( dungeonBossDrops, $"./output/DungeonBossDrop.csv" );
            WriteFile( eNpcPlaces, $"./output/ENpcPlace.csv" );
            WriteFile( shopNames, $"./output/ShopName.csv" );
            WriteFile( eNpcShops, $"./output/ENpcShop.csv" );
            WriteFile( airshipUnlocks, $"./output/AirshipUnlock.csv" );
            WriteFile( submarineUnlocks, $"./output/SubmarineUnlock.csv" );
            WriteFile( mobSpawns, $"./output/MobSpawn.csv" );
            WriteFile( itemPatches, $"./output/ItemPatch.csv" );
            WriteFile( retainerVentureItems, $"./output/RetainerVentureItem.csv" );
            WriteFile( storeItems, $"./output/StoreItem.csv" );
            WriteFile( houseVendors, $"./output/HouseVendor.csv" );
        }

        public void ProcessShopNames(List<ShopName> shopNames)
        {
            var customTalkSheet = Service.GameData.GetExcelSheet< CustomTalk >();

            foreach( var customTalk in customTalkSheet )
            {
                var instructions = new List<(uint, string)>();
                var count = customTalk.ScriptInstruction.Length;
                for( uint i = 0; i < count; i++ )
                {
                    instructions.Add( (i, customTalk.ScriptInstruction[i].ToString()) );
                }
                var shopInstructions = instructions.Where(i => i.Item2.Contains("SHOP") && !i.Item2.Contains("LOGMSG")).ToArray();
                if (shopInstructions.Length == 0)
                    continue;
                
                foreach (var shopInstruction in shopInstructions)
                {
                    var label = customTalk.ScriptInstruction[ shopInstruction.Item1 ].ToString();
                    var argument = customTalk.ScriptArg[ shopInstruction.Item1 ];
                    var shopName = Utils.GetShopName(argument, label);
                    if( shopName != null )
                    {
                        shopNames.Add( new ShopName()
                        {
                            RowId = (uint)(shopNames.Count +1),
                            ShopId = argument,
                            Name = shopName
                        });
                    }
                }
            }

            var shops = new Dictionary< uint, string >()
            {
                {1769869, "Request to keep your aetherpool gear"},
                {1769743, "Exchange Wolf Marks (Melee)"},
                {1769744, "Exchange Wolf Marks (Ranged)"},
                {1769820, "Create or augment Eureka gear. (Paladin)"},
                {1769821, "Create or augment Eureka gear. (Warrior)"},
                {1769822,"Create or augment Eureka gear. (Dark Knight)"},
                {1769823,"Create or augment Eureka gear. (Dragoon)"},
                {1769824,"Create or augment Eureka gear. (Monk)"},
                {1769825,"Create or augment Eureka gear. (Ninja)"},
                {1769826,"Create or augment Eureka gear. (Samurai)"},
                {1769827,"Create or augment Eureka gear. (Bard)"},
                {1769828,"Create or augment Eureka gear. (Machinist)"},
                {1769829,"Create or augment Eureka gear. (Black Mage)"},
                {1769830,"Create or augment Eureka gear. (Summoner)"},
                {1769831,"Create or augment Eureka gear. (Red Mage)"},
                {1769832,"Create or augment Eureka gear. (White Mage)"},
                {1769833,"Create or augment Eureka gear. (Scholar)"},
                {1769834,"Create or augment Eureka gear. (Astrologian)"},
                {1769871,"Exchange artifacts"},
                {1769870,"Request to keep your empyrean aetherpool gear"},
                {1770282,"Exchange Faux Leaves"},
                {1770286,"Exchange Faire Voucher"},
                {1770087,"Exchange Bozjan clusters for items."},
                {1769957,"Gemstone Trader"},
                {1769958,"Gemstone Trader"},
                {1769959,"Gemstone Trader"},
                {1769960,"Gemstone Trader"},
                {1769961,"Gemstone Trader"},
                {1769962,"Gemstone Trader"},
                {1769963,"Gemstone Trader"},
                {1769964,"Gemstone Trader"},
                {262919,"Doman Junkmonger"},
            };
            foreach( var shopName in shops )
            {
                shopNames.Add( new ShopName()
                {
                    RowId = (uint)( shopNames.Count + 1 ),
                    ShopId = shopName.Key,
                    Name = shopName.Value
                } );
            }

        }

        public void ProcessPatchData( List< ItemPatch > itemPatches )
        {
            var patchText = File.ReadAllText( @"patches.json" );
            var patches = JsonConvert.DeserializeObject<PatchJson[]>(patchText)!.ToList();
            
            //Process extra data not in GT's jsons(no idea why)
            
            var reader = CSVFile.CSVReader.FromFile(@"ManualData\ExtraPatchData.csv", CSVSettings.CSV);
            
            foreach( var line in reader.Lines() )
            {
                var sourceItemId = uint.Parse(line[ 0 ]);
                var patchId = decimal.Parse(line[ 1 ], CultureInfo.InvariantCulture);
                patches.Add( new PatchJson() {Patch = patchId, Id = sourceItemId, Type = "item"} );
            }
            var orderedPatches = patches.Where( c => c.Type == "item" ).OrderBy( c => c.Id );
            decimal? currentPatch = null;
            uint? firstItem = null;
            uint? lastItem = null;
            var patchData = new List< (decimal, uint, uint) >();
            foreach( var orderedPatch in orderedPatches )
            {
                if( currentPatch == null )
                {
                    currentPatch = orderedPatch.Patch;
                }

                if( firstItem == null )
                {
                    firstItem = orderedPatch.Id;
                }
                

                if( currentPatch != orderedPatch.Patch && lastItem != null)
                {
                    patchData.Add( (currentPatch.Value, firstItem.Value, lastItem.Value) );
                    firstItem = orderedPatch.Id;
                    currentPatch = orderedPatch.Patch;
                }

                lastItem = orderedPatch.Id;

            }
            if( currentPatch != null && firstItem != null && lastItem != null)
            {
                patchData.Add( (currentPatch.Value, firstItem.Value, lastItem.Value) );
            }
            foreach( var patch in patchData )
            {
                itemPatches.Add( new ItemPatch((uint)(itemPatches.Count + 1), patch.Item2, patch.Item3, patch.Item1) );
            }
        }

        public void ProcessDutiesJson( List< DungeonChest > dungeonChests, List< DungeonChestItem > dungeonChestItems, List< DungeonBoss > dungeonBosses, List< DungeonBossChest > dungeonBossChests, List<DungeonBossDrop> dungeonBossDrops )
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

                            if( fight.Drops != null )
                            {
                                foreach( var drop in fight.Drops )
                                {
                                    var itemName = drop.Name.ToParseable();
                                    var actualItem = _itemsByName.ContainsKey( itemName ) ? _itemsByName[ itemName ] : null;
                                    if( actualItem != null && actualItem.RowId != 0 )
                                    {
                                        dungeonBossDrops.Add( new DungeonBossDrop((uint)(dungeonBossDrops.Count + 1), actualDuty.RowId, (uint)index, actualItem.RowId, 1 ) );
                                    }
                                    else
                                    {
                                        Console.WriteLine( "Could not find item with name " + itemName + " in duty " + duty.Name );
                                    }
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
                                        var actualItem = _itemsByName.ContainsKey( itemName ) ? _itemsByName[ itemName ] : null;
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
                            var xCoord = float.Parse( chest.Coords.X, CultureInfo.InvariantCulture );
                            var yCoord = float.Parse( chest.Coords.Y, CultureInfo.InvariantCulture );
                            var dungeonChest = new DungeonChest( (uint)(dungeonChests.Count + 1), (byte)(index + 1), actualDuty.RowId, new Vector2( xCoord, yCoord ) );
                            dungeonChests.Add( dungeonChest );
                            foreach( var item in chest.Items )
                            {
                                var itemName = item.ToParseable();
                                var actualItem = _itemsByName.ContainsKey( itemName ) ? _itemsByName[ itemName ] : null;
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
                {new []{"Archfiend Attire Coffer"}, "Archfiend"},
                {new []{"Bookwyrm's Attire Coffer"}, "Bookwyrm's"},
                {new []{"False Monarchy Attire Coffer"}, "False Monarchy"},
                {new []{"Muzhik Attire Coffer"}, "Muzhik"},
                {new []{"Spotted Attire Coffer"}, "Spotted"},
                {new []{"Street Attire Coffer"}, "Street"},
                {new []{"Chondrite", "Coffer"}, "Star Quartz"},
                {new []{"Chondrite", "Coffer"}, "Chondrite"},
                {new []{"Chondrite", "Coffer"}, "AR-Caean Velvet"},
                {new []{"Chondrite", "Coffer"}, "Ophiotauroskin"},
                {new []{"Crag", "Coffer"}, "Crags"},
                {new []{"Crag", "Coffer"}, "Key of Titan"},
                {new []{"Cryptlurker", "Coffer"}, "Cryptlurker's"},
                {new []{"Diamond", "Coffer"}, "Diamond"},
                {new []{"Divine Light", "Coffer"}, "Divine Light"},
                {new []{"Dreadwyrm", "Coffer"}, "Dreadwyrm"},
                {new []{"Emerald", "Coffer"}, "Emerald"},
                {new []{"Expanse", "Coffer"}, "Expanse"},
                {new []{"Flamecloaked", "Coffer"}, "Flamecloaked"},
                {new []{"Genji Armo", "Coffer"}, "Genji"},
                {new []{"Zurvanite", "Coffer"}, "Zurvanite"},
                {new []{"Windswept", "Coffer"}, "Windswept"},
                {new []{"Vortex", "Coffer"}, "Garuda's"},
                {new []{"Tsukuyomi", "Coffer"}, "Tsukuyomi"},
                {new []{"Titania", "Coffer"}, "The King's"},
                {new []{"Tidal", "Coffer"}, "Wave "},
                {new []{"Suzaku", "Coffer"}, "Suzaku's"},
                {new []{"Susano", "Coffer"}, "Susano's"},
                {new []{"Splendorous", "Coffer"}, "Splendorous "},
                {new []{"Sophic", "Coffer"}, "Sophic "},
                {new []{"Shinryu", "Coffer"}, "Shinryu's"},
                {new []{"Sephirotic", "Coffer"}, "of the Sephirot"},
                {new []{"Seiryu ", "Coffer"}, "Seiryu's"},
                {new []{"Ruby ", "Coffer"}, "Ruby"},
                {new []{"Pewter ", "Coffer"}, "Palm"},
                {new []{"Omega ", "Coffer"}, "Omega"},
                {new []{"Voidcast ", "Coffer"}, "Voidcast"},
                {new []{"Ascension ", "Coffer"}, "Ascension"},
                {new []{"Ascension ", "Coffer"}, "of Ascension"},
            };
            foreach( var cofferName in cofferNames )
            {
                var coffers = _itemSheet.Where( c =>
                {
                    return  !_bannedItems.Contains(c.RowId) && c.Name.ToString().StartsWith( cofferName.Key[ 0 ] ) && cofferName.Key.Skip( 1 ).All( d => c.Name.ToString().Contains( d ) );
                } ).ToList();
                foreach( var coffer in coffers )
                {
                    var fullName = String.Join( " ", cofferName.Key );
                    if( coffer.Name == fullName )
                    {
                        var items = _itemSheet.Where( c => !_bannedItems.Contains(c.RowId) &&
                                                           c.Name.ToString().StartsWith( cofferName.Value ) &&
                                                           ( ( c.EquipSlotCategory.Value?.MainHand ?? 0 ) == 1 || ( c.EquipSlotCategory.Value?.OffHand ?? 0 ) == 1 ) );
                        foreach( var item in items )
                        {
                            
                            itemSupplements.Add( new ItemSupplement((uint)itemSupplements.Count + 1, item.RowId, coffer.RowId, ItemSupplementSource.Loot) );
                        }
                    }

                    if( coffer.Name.ToString().Contains( "Attire Coffer" ) )
                    {
                        var potentialItems = new string[]
                        {
                            "Armor",
                            "Breeches",
                            "Gauntlets",
                            "Helm",
                            "Sabatons",
                            "Cap",
                            "Jacket",
                            "Gloves",
                            "Slacks",
                            "Shoes",
                            "Mask",
                            "Culottes",
                            "Hat",
                            "Dress",
                            "Blinder",
                            "Boots",
                            "Coat",
                            "Field Dressing",
                            "Trousers",
                            "Fedora",
                            "Spencer",
                            "Cargo Trousers",
                            "Handwear",
                            "High-top Shoes",
                            "Top",
                            "Spectacles",
                            "Chasuble",
                            "Waistwrap"
                        };
                        foreach( var potentialItem in potentialItems )
                        {
                            var items = _itemSheet.Where( c => !_bannedItems.Contains(c.RowId) && c.Name.ToString().Contains( cofferName.Value ) && c.Name.ToString().Contains( potentialItem ) && 
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
                    
                    //Weapon, Gear, Accessories
                    if( coffer.Name.ToString().Contains( "Weapon" ) )
                    {
                        var items = _itemSheet.Where( c => !_bannedItems.Contains(c.RowId) &&
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
                                var items = _itemSheet.Where( c => !_bannedItems.Contains(c.RowId) &&
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
                        var items = _itemSheet.Where( c => !_bannedItems.Contains(c.RowId) &&
                                                           c.Name.ToString().StartsWith( cofferName.Value ) && c.Name.ToString().Contains( " of " ) && 
                                                           ( c.EquipSlotCategory.Value?.Head ?? 0 ) == 1);
                        foreach( var item in items )
                        {
                            itemSupplements.Add( new ItemSupplement((uint)itemSupplements.Count + 1, item.RowId, coffer.RowId, ItemSupplementSource.Loot) );
                        }
                    }

                    if( coffer.Name.ToString().Contains( "Chest Gear" ) )
                    {
                        var items = _itemSheet.Where( c => !_bannedItems.Contains(c.RowId) &&
                                                           c.Name.ToString().StartsWith( cofferName.Value ) && c.Name.ToString().Contains( " of " ) && 
                                                           ( c.EquipSlotCategory.Value?.Body ?? 0 ) == 1);
                        foreach( var item in items )
                        {
                            itemSupplements.Add( new ItemSupplement((uint)itemSupplements.Count + 1, item.RowId, coffer.RowId, ItemSupplementSource.Loot) );
                        }
                    }

                    if( coffer.Name.ToString().Contains( "Hand Gear" ) )
                    {
                        var items = _itemSheet.Where( c => !_bannedItems.Contains(c.RowId) &&
                                                           c.Name.ToString().StartsWith( cofferName.Value ) && c.Name.ToString().Contains( " of " ) && 
                                                           ( c.EquipSlotCategory.Value?.Gloves ?? 0 ) == 1);
                        foreach( var item in items )
                        {
                            itemSupplements.Add( new ItemSupplement((uint)itemSupplements.Count + 1, item.RowId, coffer.RowId, ItemSupplementSource.Loot) );
                        }
                    }

                    if( coffer.Name.ToString().Contains( "Leg Gear" ) )
                    {
                        var items = _itemSheet.Where( c => !_bannedItems.Contains(c.RowId) &&
                                                           c.Name.ToString().StartsWith( cofferName.Value ) && c.Name.ToString().Contains( " of " ) && 
                                                           ( c.EquipSlotCategory.Value?.Legs ?? 0 ) == 1);
                        foreach( var item in items )
                        {
                            itemSupplements.Add( new ItemSupplement((uint)itemSupplements.Count + 1, item.RowId, coffer.RowId, ItemSupplementSource.Loot) );
                        }
                    }

                    if( coffer.Name.ToString().Contains( "Foot Gear" ) )
                    {
                        var items = _itemSheet.Where( c => !_bannedItems.Contains(c.RowId) &&
                                                           c.Name.ToString().StartsWith( cofferName.Value ) && c.Name.ToString().Contains( " of " ) && 
                                                           ( c.EquipSlotCategory.Value?.Feet ?? 0 ) == 1);
                        foreach( var item in items )
                        {
                            itemSupplements.Add( new ItemSupplement((uint)itemSupplements.Count + 1, item.RowId, coffer.RowId, ItemSupplementSource.Loot) );
                        }
                    }

                    if( coffer.Name.ToString().Contains( "Earring Coffer" ) )
                    {
                        var items = _itemSheet.Where( c => !_bannedItems.Contains(c.RowId) &&
                                                           c.Name.ToString().StartsWith( cofferName.Value ) && c.Name.ToString().Contains( " of " ) && 
                                                           ( c.EquipSlotCategory.Value?.Ears ?? 0 ) == 1);
                        foreach( var item in items )
                        {
                            itemSupplements.Add( new ItemSupplement((uint)itemSupplements.Count + 1, item.RowId, coffer.RowId, ItemSupplementSource.Loot) );
                        }
                    }

                    if( coffer.Name.ToString().Contains( "Necklace Coffer" ) )
                    {
                        var items = _itemSheet.Where( c => !_bannedItems.Contains(c.RowId) &&
                                                           c.Name.ToString().StartsWith( cofferName.Value ) && c.Name.ToString().Contains( " of " ) && 
                                                           ( c.EquipSlotCategory.Value?.Neck ?? 0 ) == 1);
                        foreach( var item in items )
                        {
                            itemSupplements.Add( new ItemSupplement((uint)itemSupplements.Count + 1, item.RowId, coffer.RowId, ItemSupplementSource.Loot) );
                        }
                    }

                    if( coffer.Name.ToString().Contains( "Bracelet Coffer" ) )
                    {
                        var items = _itemSheet.Where( c => !_bannedItems.Contains(c.RowId) &&
                                                           c.Name.ToString().StartsWith( cofferName.Value ) && c.Name.ToString().Contains( " of " ) && 
                                                           ( c.EquipSlotCategory.Value?.Wrists ?? 0 ) == 1);
                        foreach( var item in items )
                        {
                            itemSupplements.Add( new ItemSupplement((uint)itemSupplements.Count + 1, item.RowId, coffer.RowId, ItemSupplementSource.Loot) );
                        }
                    }

                    if( coffer.Name.ToString().Contains( "Ring Coffer" ) )
                    {
                        var items = _itemSheet.Where( c => !_bannedItems.Contains(c.RowId) &&
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
                                var items = _itemSheet.Where( c => !_bannedItems.Contains(c.RowId) && c.Name.ToString().Contains( cofferName.Value ) && c.Name.ToString().Contains( armourType ) && 
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

        public void ProcessManualItems( List< ItemSupplement > itemSupplements )
        {
            var reader = CSVFile.CSVReader.FromFile(@"ManualData\Items.csv", CSVSettings.CSV);

            foreach( var line in reader.Lines() )
            {
                var sourceItemId = uint.Parse(line[ 0 ]);
                var outputItemId = uint.Parse(line[ 1 ]);
                var source = Enum.Parse<ItemSupplementSource>(line[ 2 ]);
                itemSupplements.Add( new ItemSupplement((uint)(itemSupplements.Count + 1), outputItemId, sourceItemId, source) );
            }
            
            reader = CSVFile.CSVReader.FromFile(@"ManualData\ItemsScraped.csv", CSVSettings.CSV);

            foreach( var line in reader.Lines() )
            {
                var s1 = line[ 0 ];
                var s2 = line[ 1 ];

                var sourceItemName = s1.ToParseable();
                var rewardItemName = s2.ToParseable();

                if( _itemsByName.ContainsKey( sourceItemName ) && _itemsByName.ContainsKey( rewardItemName ) )
                {
                    var sourceItem = _itemsByName[ sourceItemName ];
                    var rewardItem = _itemsByName[ rewardItemName ];
                    itemSupplements.Add( new ItemSupplement( (uint)( itemSupplements.Count + 1 ), rewardItem.RowId, sourceItem.RowId, ItemSupplementSource.Loot ) );
                }
                else if( _itemsByName.ContainsKey( sourceItemName ) )
                {
                    Console.WriteLine("Could not find item with matching name: " + s1);
                }
                else if( _itemsByName.ContainsKey( rewardItemName ) )
                {
                    Console.WriteLine("Could not find item with matching name: " + s2);
                }
            }
        }

        private void ProcessSkybuilderItems( List< ItemSupplement > itemSupplements )
        {
            var items = _itemSheet.Where( c => !_bannedItems.Contains(c.RowId) && c.Name.ToString().Contains( "Approved" ) ).ToList();
            foreach( var item in items )
            {
                var regularVersion = item.Name.ToString().Replace( "Approved ", "" ).ToParseable();
                if(_itemsByName.ContainsKey(regularVersion))
                {
                    var sourceItem = _itemsByName[regularVersion];
                    itemSupplements.Add( new ItemSupplement()
                    {
                        ItemSupplementSource = ItemSupplementSource.SkybuilderHandIn,
                        SourceItemId = sourceItem.RowId,
                        ItemId = item.RowId,
                        RowId = (uint)itemSupplements.Count + 1
                    } );
                }
                else
                {
                    Console.WriteLine("Could not find a non-approved version of " + regularVersion + " for matching skybuilder items.");
                }
            }
        }

        private void ProcessHouseVendors( List< HouseVendor > houseVendors )
        {
            var reader = CSVFile.CSVReader.FromFile(@"ManualData\HouseVendors.csv");

            Dictionary< string, List< uint > > groupedResidents = new();

            foreach( var line in reader.Lines() )
            {
                var residentId = uint.Parse(line[ 0 ]);
                var resident = _enpcResidentSheet.GetRow( residentId );
                if( resident != null )
                {
                     var residentName = resident.Singular.ToString().ToParseable();
                     groupedResidents.TryAdd( residentName, new List< uint >() );
                     groupedResidents[residentName].Add( residentId );
                }
            }

            foreach( var item in groupedResidents )
            {
                var parentId = item.Value.First();
                var children = item.Value.Skip( 1 );
                var houseVendor = new HouseVendor((uint)(houseVendors.Count + 1), parentId, 0);
                houseVendors.Add( houseVendor );
                foreach( var child in children )
                {
                    var childVendor = new HouseVendor((uint)(houseVendors.Count + 1), child, parentId);
                    houseVendors.Add( childVendor );
                }
            }

        }

        private void ProcessStoreItems( List< StoreItem > storeItems )
        {
            foreach( var product in StoreParser.StoreProducts )
            {
                var fittingShopItemName = product.Value.Name;
                var parsedShopItemName = fittingShopItemName.Trim().ToParseable();
                FittingShopItemSet? fittingShopItemSet = null;
                if( _fittingShopItemSetByName.ContainsKey( parsedShopItemName ) )
                {
                    fittingShopItemSet = _fittingShopItemSetByName[ parsedShopItemName ];
                }
                else if(product.Value.Items.Count != 1)
                {
                    Console.WriteLine("Could not find a fitting shop item set with the name " + fittingShopItemName + ", it has more than 1 item so the assumption is it's a set.");
                }
                foreach( var item in product.Value.Items )
                {
                    var itemName = item.Name;
                    var parsedItemName = itemName.Trim().ToParseable();
                    if( _itemsByName.ContainsKey( parsedItemName ) )
                    {
                        var outputItem = _itemsByName[ parsedItemName ];
                        storeItems.Add( new StoreItem()
                        {
                            RowId = (uint)(storeItems.Count + 1),
                            FittingShopItemSetId = fittingShopItemSet?.RowId ?? 0,
                            ItemId = outputItem.RowId
                        });
                    }
                    else
                    {
                        Console.WriteLine("Could not find an item with the name " + itemName + " while parsing store data.");
                    }
                }
            }
            
        }

        private void ProcessRetainerVentures( List<RetainerVentureItem> retainerVentureItems )
        {
            var reader = CSVFile.CSVReader.FromFile(@"ManualData\RetainerVentures.csv");

            foreach( var line in reader.Lines() )
            {
                var ventureName = line[ 0 ];
                var items = line[1].Split( "," );
                
                var parsedVentureName = ventureName.ToParseable();
                if( _retainerTaskRandomByName.ContainsKey( parsedVentureName ) )
                {
                    var retainerTaskRandom = _retainerTaskRandomByName[ parsedVentureName ];

                    foreach( var itemName in items )
                    {
                        var parseableItemName = itemName.Trim().ToParseable();
                        var outputItem = _itemsByName.ContainsKey( parseableItemName ) ? _itemsByName[ parseableItemName ] : null;
                        if( outputItem != null )
                        {
                            retainerVentureItems.Add( new RetainerVentureItem()
                            {
                                RowId = (uint)(retainerVentureItems.Count + 1),
                                RetainerTaskRandomId = retainerTaskRandom.RowId,
                                ItemId = outputItem.RowId
                            });
                        }
                        else
                        {
                            Console.WriteLine("Could not find item with name " + itemName.Trim() + " in the retainer task random with name " + ventureName);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Could not find the retainer task random with name " + ventureName);
                }
            }
        }

        private void ProcessSubmarineUnlocks( List<SubmarineUnlock> submarineUnlocks, List<SubmarineDrop> submarineDrops )
        {
            var reader = CSVFile.CSVReader.FromFile(@"ManualData\SubmarineUnlocks.csv");

            foreach( var line in reader.Lines() )
            {
                var sector = line[ 0 ];
                var unlockSector = line[ 1 ];
                var rankRequired = uint.Parse(line[ 2 ]);
                var items = line[ 3 ] + "," + line[ 4 ];
                
                sector = sector.ToParseable();
                unlockSector = unlockSector.ToParseable();
                if( _submarinesByName.ContainsKey( sector ) )
                {
                    var actualSector = _submarinesByName[ sector ];
                    SubmarineExploration? actualUnlockSector = null;
                    if( _submarinesByName.ContainsKey( unlockSector ) )
                    {
                        actualUnlockSector = _submarinesByName[ unlockSector ];
                    }

                    submarineUnlocks.Add(new SubmarineUnlock()
                    {
                        RowId = (uint)( submarineUnlocks.Count + 1 ),
                        SubmarineExplorationId = actualSector.RowId,
                        SubmarineExplorationUnlockId = actualUnlockSector?.RowId ?? 0,
                        RankRequired = rankRequired
                    });

                    var items1List = items.Split( "," );
                    foreach( var itemName in items1List )
                    {
                        var parseableItemName = itemName.Trim().ToParseable();
                        var outputItem = _itemsByName.ContainsKey( parseableItemName ) ? _itemsByName[ parseableItemName ] : null;
                        if( outputItem != null )
                        {
                            submarineDrops.Add( new SubmarineDrop()
                            {
                                RowId = (uint)(submarineDrops.Count + 1),
                                SubmarineExplorationId = actualSector.RowId,
                                ItemId = outputItem.RowId
                            });
                        }
                        else
                        {
                            Console.WriteLine("Could not find item with name " + itemName.Trim() + " in the sector " + actualSector.Destination.ToString());
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Could not find the submarine point with name " + sector);
                }
            }
        }

        private void ProcessAirshipUnlocks( List<AirshipUnlock> airshipUnlocks, List<AirshipDrop> airshipDrops )
        {
            var reader = CSVFile.CSVReader.FromFile(@"ManualData\AirshipUnlocks.csv");

            foreach( var line in reader.Lines() )
            {
                var sector = line[ 0 ];
                var unlockSector = line[ 1 ];
                var rankRequired = uint.Parse(line[ 2 ]);
                var surveillanceRequired = line[ 3 ];
                var items = line[ 4 ] + "," + line[ 5 ];
                
                sector = "Sea of Clouds " + $"{int.Parse( sector ):D2}";
                unlockSector = unlockSector != "" ? "Sea of Clouds " + $"{int.Parse( unlockSector ):D2}" : "";
                sector = sector.ToParseable();
                unlockSector = unlockSector.ToParseable();
                //Sectors are stored as numbers
                if( _airshipsByName.ContainsKey( sector ) )
                {
                    var actualSector = _airshipsByName[ sector ];
                    AirshipExplorationPoint? actualUnlockSector = null;
                    if( _airshipsByName.ContainsKey( unlockSector ) )
                    {
                        actualUnlockSector = _airshipsByName[ unlockSector ];
                    }

                    var actualSurveillanceRequired = uint.Parse( surveillanceRequired );
                    airshipUnlocks.Add(new AirshipUnlock()
                    {
                        RowId = (uint)( airshipUnlocks.Count + 1 ),
                        AirshipExplorationPointId = actualSector.RowId,
                        AirshipExplorationPointUnlockId = actualUnlockSector?.RowId ?? 0,
                        SurveillanceRequired = actualSurveillanceRequired,
                        RankRequired = rankRequired
                    });

                    var items1List = items.Split( "," );
                    foreach( var itemName in items1List )
                    {
                        var parseableItemName = itemName.Trim().ToParseable();
                        var outputItem = _itemsByName.ContainsKey( parseableItemName ) ? _itemsByName[ parseableItemName ] : null;
                        if( outputItem != null )
                        {
                            airshipDrops.Add( new AirshipDrop()
                            {
                                RowId = (uint)(airshipDrops.Count + 1),
                                AirshipExplorationPointId = actualSector.RowId,
                                ItemId = outputItem.RowId
                            });
                        }
                        else
                        {
                            Console.WriteLine("Could not find item with name " + itemName.Trim() + " in the sector " + actualSector.NameShort);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Could not find the airship point with name " + sector);
                }
            }
        }

        private void ProcessEventShops( List<ENpcShop> eNpcShops )
        {
            var shops = new Dictionary< uint, uint >()
            {
                {1769635, 1016289},
                {1769675, 1017338},
                {1769869, 1017338},
                {1769743, 1018655},
                {1769744, 1018655},
                
                {1769820, 1025047},
                {1769821, 1025047},
                {1769822, 1025047},
                {1769823, 1025047},
                {1769824, 1025047},
                {1769825, 1025047},
                {1769826, 1025047},
                {1769827, 1025047},
                {1769828, 1025047},
                {1769829, 1025047},
                {1769830, 1025047},
                {1769831, 1025047},
                {1769832, 1025047},
                {1769833, 1025047},
                {1769834, 1025047},
                
                {1769871, 1025848},
                {1769870, 1025848},
                
                {262919, 1025763},
                
                {1769957, 1027998},
                {1769958, 1027538},
                {1769959, 1027385},
                {1769960, 1027497},
                {1769961, 1027892},
                {1769962, 1027665},
                {1769963, 1027709},
                {1769964, 1027766},
                
                {1770282, 1033921},
                
                {1770087, 1034007},
                //Support this later{1770087, 1036895},
            };
            
            foreach( var shopName in shops )
            {
                eNpcShops.Add( new ENpcShop()
                {
                    RowId = (uint)( eNpcShops.Count + 1 ),
                    ShopId = shopName.Key,
                    ENpcResidentId = shopName.Value
                } );
            }
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

        private void ProcessItemsTSV( List< ItemSupplement > itemSupplements, List< DungeonDrop > dungeonDrops )
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
                        //No longer a need to parse from GT's data as it's out of date.
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
            var outputItem = _itemsByName.ContainsKey( outputItemId ) ? _itemsByName[ outputItemId ] : null;
            if( outputItem != null )
            {
                foreach( var sourceItem in sources )
                {
                    if( sourceItem.Contains( "Sea of Clouds" ) )
                    {
                        continue;
                    }
                    var sourceName = sourceItem.ToParseable();
                    //Hacks because of misspelling
                    sourceName = sourceName.Replace( "valut", "vault" );
                    sourceName = sourceName.Replace( "southernrimilalatrench", "thesouthernrimilalatrench" );
                    sourceName = sourceName.Replace( "rimialashelf", "rimilalashelf" );
                    var submarineExploration = _submarinesByName.ContainsKey( sourceName ) ? _submarinesByName[ sourceName ] : null;
                    if( submarineExploration != null )
                    {
                        submarineDrops.Add( new SubmarineDrop( (uint)submarineDrops.Count + 1, outputItem.RowId, submarineExploration.RowId ) );
                    }
                    else
                    {
                        Console.WriteLine( "Could not find a match for input item: " + outputItemId + " and submarine zone " + sourceName );
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
            var outputItem = _itemsByName.ContainsKey( outputItemId ) ? _itemsByName[ outputItemId ] : null;
            if( outputItem != null )
            {
                foreach( var sourceItem in sources )
                {
                    if( !sourceItem.Contains( "Sea of Clouds" ) )
                    {
                        continue;
                    }
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
            var outputItem = _itemsByName.ContainsKey( outputItemId ) ? _itemsByName[ outputItemId ] : null;
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
            var outputItem = _itemsByName.ContainsKey( outputItemId ) ? _itemsByName[ outputItemId ] : null;
            if( outputItem != null )
            {
                foreach( var sourceItem in sources )
                {
                    var sourceName = sourceItem.ToParseable();
                    var actualItem = _itemsByName.ContainsKey( sourceName ) ? _itemsByName[ sourceName ] : null;
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
            /*var itemDrops = Service.DatabaseBuilder.ItemDropsByMobId;
            foreach( var itemDrop in itemDrops )
            {
                foreach( var itemId in itemDrop.Value )
                {
                    mobDrops.Add( new MobDrop((uint)(mobDrops.Count + 1), itemId, itemDrop.Key ) );
                }
            }*/

            var storeMobDrops = LodestoneParser.ParseMobDrops();
            foreach( var storeMobDrop in storeMobDrops )
            {
                storeMobDrop.RowId = (uint)(mobDrops.Count + 1);
                mobDrops.Add( storeMobDrop );
            }
        }
        public void ProcessEventNpcs(List<ENpcPlace> npcPlaces)
        {
            var eNpcPlaces = Service.DatabaseBuilder.ENpcPlaces;
            foreach( var eNpcPlace in eNpcPlaces )
            {
                npcPlaces.Add( new ENpcPlace()
                {
                    RowId = (uint)(npcPlaces.Count + 1),
                    TerritoryTypeId = eNpcPlace.TerritoryTypeId,
                    ENpcResidentId = eNpcPlace.ENpcResidentId,
                    Position = eNpcPlace.Position
                });
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
            var pets = _petSheet.Where( c => c.Name.ToString().ToParseable() != "" );
            foreach( var pet in pets )
            {
                _disallowedBNpcNames.Add( pet.Name.ToString().ToParseable() );
            }
            var companions = _companionSheet.Where( c => c.Singular.ToString().ToParseable() != "" );
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
            var bnpcName = _bnpcNameSheet.GetRow( bnpcNameId );
            if( bnpcName == null )
            {
                return false;
            }

            if( _disallowedBNpcNames == null )
            {
                GenerateDisallowedBNpcNameList();
            }

            var hashSet = _disallowedBNpcNames.ToHashSet();
            return !hashSet.Contains( bnpcName.Singular.ToString().ToParseable() );
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



            foreach( var territory in _territoryTypeSheet )
            {
                var placeName = territory.PlaceName.Value.Name.ToString().ToParseable();
                if( hashSet.Contains( placeName ) || territory.PlaceName.Row == 0 )
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
        
        public void ProcessMobSpawnData(List<MobSpawnPosition> npcPlaces)
        {
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
            var newPositions = positions.SelectMany(c => c.Value.SelectMany(d => d.Value.Select(e => e))).ToList();
            npcPlaces.AddRange( newPositions );

            try
            {
                MappyParser.UpdateMappyData();
            }
            catch( Exception e )
            {
                Console.WriteLine( "Failed to parse mappy data because " + e.Message );
                throw;
            }

            var mappyEntries = MappyParser.MappyEntries;

            foreach( var mappyEntry in mappyEntries )
            {
                if( mappyEntry.Type == "BNPC" )
                {
                    MobSpawnPosition mobSpawnPosition = new MobSpawnPosition();
                    mobSpawnPosition.Position = new Vector3( (float)mappyEntry.CoordinateX, (float)mappyEntry.CoordinateY, (float)mappyEntry.CoordinateZ );
                    mobSpawnPosition.BNpcNameId = mappyEntry.BNpcNameID;
                    mobSpawnPosition.BNpcBaseId = mappyEntry.BNpcBaseID;
                    mobSpawnPosition.TerritoryTypeId = (uint)mappyEntry.MapTerritoryID;
                    mobSpawnPosition.Subtype = 0;
                    if( _bnpcNameSheet.GetRow( mappyEntry.BNpcNameID ) != null )
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

        private const float maxRange = 1.0f;
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
    }
}