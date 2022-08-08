using System;
using System.IO;

namespace LuminaSupplemental.SpaghettiGenerator
{
    public class Program
    {
        static void Main( string[] args )
        {
            var sg = new LookupGenerator( args[ 0 ] );

            Directory.CreateDirectory( "output" );

            var name = "ItemSupplement";
            var code = sg.ProcessItemsTSV( name );
            var path = $"./output/{name}.cs";
            File.WriteAllText( path, code );

            name = "DutySupplement";
            code = sg.ProcessDutiesJson( name );
            path = $"./output/{name}.cs";
            File.WriteAllText( path, code );
        }
    }
}