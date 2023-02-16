using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using CsvHelper;

namespace LuminaSupplemental.Excel.Model;

public static class CsvLoader
{
    public static List< T > LoadCsv<T>(string filePath, out bool success) where T : ICsv, new()
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

    public static List< T > LoadResource<T>(string resourceName, out bool success) where T : ICsv, new()
    {
        try
        {
            var assembly = Assembly.GetExecutingAssembly();
            using( Stream stream = assembly.GetManifestResourceStream( resourceName ) )
            using( StreamReader reader = new StreamReader( stream ) )
            {


                var csvReader = CSVFile.CSVReader.FromString( reader.ReadToEnd() );
                var items = new List< T >();
                foreach( var line in csvReader.Lines() )
                {
                    T item = new T();
                    item.FromCsv( line );
                    items.Add( item );
                }

                success = true;
                return items;
            }
        }
        catch( Exception )
        {
            success = false;
            return new List< T >();
        }
    }
    
    public static bool ToCsv<T>( List<DungeonBossChest> bosses, string filePath ) where T : ICsv, new()
    {
        try
        {
            using var fileStream = new FileStream( filePath, FileMode.Create );
            CsvWriter writer = new CsvWriter( new StreamWriter( fileStream ), CultureInfo.CurrentCulture );
            writer.WriteHeader<T>();
            writer.NextRecord();
            foreach( var item in bosses )
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
}