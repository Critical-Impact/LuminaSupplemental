using System;
using System.IO;
using Lumina;

namespace LuminaSupplemental.SpaghettiGenerator
{
    public class Program
    {
        static void Main( string[] args )
        {
            Service.GameData = new GameData( args[ 0 ], new LuminaOptions(){ PanicOnSheetChecksumMismatch = false} );

            var sg = new LookupGenerator();
            var libraConnection = new SQLite.SQLiteConnection(args[ 1 ], SQLite.SQLiteOpenFlags.ReadOnly);
            Service.DatabaseBuilder = new DatabaseBuilder(libraConnection);
            Service.DatabaseBuilder.Build();

            Directory.CreateDirectory( "output" );

            var name = "ItemSupplement";
            var code = sg.ProcessItemsTSV( name );
            var path = $"./output/{name}.cs";
            File.WriteAllText( path, code );

            name = "DutySupplement";
            code = sg.ProcessDutiesJson( name );
            path = $"./output/{name}.cs";
            File.WriteAllText( path, code );

            name = "MobSupplement";
            code = sg.ProcessMobData( name );
            path = $"./output/{name}.cs";
            File.WriteAllText( path, code );
        }
    }
}