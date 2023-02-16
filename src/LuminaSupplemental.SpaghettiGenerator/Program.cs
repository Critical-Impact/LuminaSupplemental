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
            sg.Generate();
        }
    }
}