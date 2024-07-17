using System;
using System.IO;
using System.Net;
using System.Net.Http;
using Lumina;
using Microsoft.Extensions.Configuration;
using SocksSharp;
using SocksSharp.Proxy;

namespace LuminaSupplemental.SpaghettiGenerator
{
    public class Program
    {
        public static HttpClient client;
        static void Main()
        {
            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddIniFile("config.ini");

            var configuration = configurationBuilder.Build();

            // Create an instance of the configuration class and bind the values from the INI file
            var appConfig = new AppConfig();
            configuration.GetSection("Basic").Bind(appConfig.Basic);
            configuration.GetSection("Parsing").Bind(appConfig.Parsing);
            configuration.GetSection("Proxy").Bind(appConfig.Proxy);

            Console.WriteLine("Configuration:");
            Console.WriteLine();

            // Validate and use the configuration values
            if (appConfig.Basic.IsValid())
            {
                Console.WriteLine("Basic Section:");
                Console.WriteLine($"FFXIV Game Directory: {appConfig.Basic.FFXIVGameDirectory}");
                Console.WriteLine($"Libra SQL File Path: {appConfig.Basic.LibraSQLFilePath}");
            }
            else
            {
                Console.WriteLine("Basic Section: Invalid");
                return;
            }
            
            Console.WriteLine();

            if( appConfig.Basic.IsValid() )
            {
                Console.WriteLine( "Parsing Section:" );
                Console.WriteLine( $"Parse Online Sources: {appConfig.Parsing.ParseOnlineSources}" );
                Console.WriteLine( $"Online Cache Directory: {appConfig.Parsing.OnlineCacheDirectory}" );
                Console.WriteLine( $"Process Mob Spawn HTML: {appConfig.Parsing.ProcessMobSpawnHTML}" );
            }
            else
            {
                Console.WriteLine("Parsing Section: Invalid");
                return;
            }

            Console.WriteLine();

            if (appConfig.Proxy.IsValid())
            {
                Console.WriteLine("Proxy Section:");
                Console.WriteLine($"Proxy Enable: {appConfig.Proxy.ProxyEnable}");
                Console.WriteLine($"Proxy Host: {appConfig.Proxy.ProxyHost}");
                Console.WriteLine($"Proxy Port: {appConfig.Proxy.ProxyPort}");
                Console.WriteLine($"Proxy Username: {appConfig.Proxy.ProxyUsername}");
                Console.WriteLine($"Proxy Password: {appConfig.Proxy.ProxyPassword}");
            }
            else
            {
                Console.WriteLine("Proxy Section: Invalid");
                return;
            }



            if( appConfig.Proxy.ProxyEnable )
            {
                var settings = new ProxySettings()
                {
                    Host = appConfig.Proxy.ProxyHost,
                    Port = appConfig.Proxy.ProxyPort,
                    Credentials = new NetworkCredential(  appConfig.Proxy.ProxyUsername, appConfig.Proxy.ProxyPassword )
                };
                var proxyClientHandler = new ProxyClientHandler< Socks5 >( settings );
                client = new HttpClient( proxyClientHandler );
            }
            else
            {
                client = new HttpClient();
            }

            
            Service.GameData = new GameData( appConfig.Basic.FFXIVGameDirectory, new LuminaOptions(){ PanicOnSheetChecksumMismatch = false} );
            var sg = new LookupGenerator(appConfig);
            //var libraConnection = new SQLite.SQLiteConnection(appConfig.Basic.LibraSQLFilePath, SQLite.SQLiteOpenFlags.ReadOnly);
            //Service.DatabaseBuilder = new DatabaseBuilder(libraConnection);
            //Service.DatabaseBuilder.Build();
            Directory.CreateDirectory( "output" );
            sg.Generate();
        }
    }
}