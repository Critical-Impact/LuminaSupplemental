#nullable enable
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;

using CsvHelper;

using Lumina;
using Lumina.Data;

using LuminaSupplemental.Excel.Model;
using Sylvan.Data.Csv;

using CsvDataReader = Sylvan.Data.Csv.CsvDataReader;

namespace LuminaSupplemental.Excel.Services;

public static class CsvLoader
{
    public static List< T > LoadCsv<T>(string filePath, bool includesHeaders, out List<string> failedLines, out List<Exception> exceptions, GameData? gameData = null, Language? language = null) where T : ICsv, new()
    {
        failedLines = new List< string >();
        exceptions = new List< Exception >();
        var items = new List< T >();

        using CsvDataReader dr = CsvDataReader.Create(filePath, new CsvDataReaderOptions(){HasHeaders = includesHeaders});
        while (dr.Read())
        {
            string[] fields = new string[dr.FieldCount];
            
            for(int i = 0; i < dr.FieldCount; i++)
            {
                fields[i] = dr.GetString(i);
            }
            T item = new T();
            try
            {
                item.FromCsv( fields );
                if( gameData != null && language != null )
                {
                    item.PopulateData( gameData.Excel, language.Value );
                }
                items.Add( item );
            }
            catch( Exception e )
            {
                exceptions.Add(e);
                failedLines.Add( String.Join( ",",fields ) );
            }
        }
        return items;
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
    public const string ItemPatchResourceName = "LuminaSupplemental.Excel.Generated.ItemPatch.csv";
    public const string RetainerVentureItemResourceName = "LuminaSupplemental.Excel.Generated.RetainerVentureItem.csv";
    public const string StoreItemResourceName = "LuminaSupplemental.Excel.Generated.StoreItem.csv";
    public const string HouseVendorResourceName = "LuminaSupplemental.Excel.Generated.HouseVendor.csv";
    public const string FateItemResourceName = "LuminaSupplemental.Excel.Generated.FateItem.csv";
    public const string GardeningCrossbreedResourceName = "LuminaSupplemental.Excel.Generated.GardeningCrossbreed.csv";

    public static List< T > LoadResource<T>(string resourceName, bool includesHeaders, out List<string> failedLines, out List<Exception> exceptions, GameData? gameData = null, Language? language = null) where T : ICsv, new()
    {
        var assembly = Assembly.GetExecutingAssembly();
        using( Stream? stream = assembly.GetManifestResourceStream( resourceName ) )
        {
            exceptions = new List< Exception >();
            failedLines = new List< string >();
            if( stream == null )
            {
                return new List< T >();
            }
            using( StreamReader reader = new StreamReader( stream ) )
            {
                var items = new List< T >();

                using CsvDataReader dr = CsvDataReader.Create(reader, new CsvDataReaderOptions(){HasHeaders = includesHeaders});
                while (dr.Read())
                {
                    string[] fields = new string[dr.FieldCount];
                
                    for(int i = 0; i < dr.FieldCount; i++)
                    {
                        fields[i] = dr.GetString(i);
                    }
                    T item = new T();
                    try
                    {
                        item.FromCsv( fields );

                        if( gameData != null && language != null )
                        {
                            item.PopulateData( gameData.Excel, language.Value );
                        }

                        items.Add( item );
                    }
                    catch( Exception e )
                    {
                        exceptions.Add(e);
                        failedLines.Add( String.Join( ",",fields ) );
                    }
                }

                return items;
            }
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
