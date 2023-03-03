#nullable enable
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using CsvHelper;
using Lumina;
using Lumina.Data;

namespace LuminaSupplemental.Excel.Model;

public static class CsvLoader
{
    public static List< T > LoadCsv<T>(string filePath, out bool success, GameData? gameData = null, Language? language = null) where T : ICsv, new()
    {
        using var fileStream = new FileStream( filePath, FileMode.Open );
        using( StreamReader reader = new StreamReader( fileStream ) )
        {
            try
            {
                var csvReader = CSVFile.CSVReader.FromString( reader.ReadToEnd() );
                var items = new List< T >();
                foreach( var line in csvReader.Lines() )
                {
                    T item = new T();
                    item.FromCsv( line );
                    if( gameData != null && language != null )
                    {
                        item.PopulateData( gameData, language.Value );
                    }
                    items.Add( item );
                }
                success = true;
                return items;
            }
            catch( Exception )
            {
                success = false;
                return new List< T >();
            }
        }
    }
    public const string DungeonBossResourceName = "LuminaSupplemental.Excel.Generated.DungeonBoss.csv";
    public const string DungeonBossChestResourceName = "LuminaSupplemental.Excel.Generated.DungeonBossChest.csv";
    public const string DungeonBossDropResourceName = "LuminaSupplemental.Excel.Generated.DungeonBossDrop.csv";
    public const string DungeonChestItemResourceName = "LuminaSupplemental.Excel.Generated.DungeonChestItem.csv";
    public const string DungeonChestResourceName = "LuminaSupplemental.Excel.Generated.DungeonChest.csv";
    public const string DungeonDropItemResourceName = "LuminaSupplemental.Excel.Generated.DungeonDrop.csv";
    public const string ItemSupplementResourceName = "LuminaSupplemental.Excel.Generated.ItemSupplement.csv";
    public const string MobDropResourceName = "LuminaSupplemental.Excel.Generated.MobDrop.csv";
    public const string SubmarineDropResourceName = "LuminaSupplemental.Excel.Generated.SubmarineDrop.csv";
    public const string AirshipDropResourceName = "LuminaSupplemental.Excel.Generated.AirshipDrop.csv";
    public const string AirshipUnlockResourceName = "LuminaSupplemental.Excel.Generated.AirshipUnlock.csv";
    public const string SubmarineUnlockResourceName = "LuminaSupplemental.Excel.Generated.SubmarineUnlock.csv";
    public const string MobSpawnResourceName = "LuminaSupplemental.Excel.Generated.MobSpawn.csv";
    public const string ENpcPlaceResourceName = "LuminaSupplemental.Excel.Generated.ENpcPlace.csv";
    public const string ShopNameResourceName = "LuminaSupplemental.Excel.Generated.ShopName.csv";
    public const string ENpcShopResourceName = "LuminaSupplemental.Excel.Generated.ENpcShop.csv";

    public static List< T > LoadResource<T>(string resourceName, out bool success, GameData? gameData = null, Language? language = null) where T : ICsv, new()
    {
        try
        {
            var assembly = Assembly.GetExecutingAssembly();
            using( Stream? stream = assembly.GetManifestResourceStream( resourceName ) )
            {
                if( stream == null )
                {
                    success = false;
                    return new List< T >();
                }
                using( StreamReader reader = new StreamReader( stream ) )
                {


                    var csvReader = CSVFile.CSVReader.FromString( reader.ReadToEnd() );
                    var items = new List< T >();
                    foreach( var line in csvReader.Lines() )
                    {
                        T item = new T();
                        item.FromCsv( line );
                        if( gameData != null && language != null )
                        {
                            item.PopulateData( gameData, language.Value );
                        }

                        items.Add( item );
                    }

                    success = true;
                    return items;
                }
            }
        }
        catch( Exception )
        {
            success = false;
            return new List< T >();
        }
    }
    
    public static bool ToCsv<T>( List<T> items, string filePath ) where T : ICsv, new()
    {
        try
        {
            using var fileStream = new FileStream( filePath, FileMode.Create );
            CsvWriter writer = new CsvWriter( new StreamWriter( fileStream ), CultureInfo.CurrentCulture );
            writer.WriteHeader<T>();
            writer.NextRecord();
            foreach( var item in items )
            {
                writer.WriteRecord( item );
                writer.NextRecord();
            }
            writer.Flush();
            fileStream.Close();
            return true;
        }
        catch( Exception )
        {
            return false;
        }
    }
    public static bool ToCsvRaw<T>( List<T> items, string filePath ) where T : ICsv, new()
    {
        try
        {
            using( StreamWriter writer = new StreamWriter( filePath ) )
            {
                var csvWriter = new CSVFile.CSVWriter( writer );
                foreach( var line in items )
                {
                    if( line.IncludeInCsv() )
                    {
                        csvWriter.WriteLine( line.ToCsv() );
                    }
                }
                return true;
            }
        }
        catch( Exception )
        {
            return false;
        }
    }
}