using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LuminaSupplemental.SpaghettiGenerator.CodeGen;

public class DictionaryHashSetGenerator : BaseShitGenerator
{
    protected string FieldName { get; set; }
    protected Dictionary<uint, HashSet<uint>> Dictionary { get; set; }

    public DictionaryHashSetGenerator( string fieldName, Dictionary<uint, HashSet<uint>> dictionary) 
    {
        FieldName = fieldName;
        Dictionary = dictionary;
    }
    
    public override void WriteFields( StringBuilder sb )
    {
        sb.AppendLine( $"public static Dictionary<uint, HashSet<uint>> {FieldName} {{ get; }} = new(){{" );
        foreach( var item in Dictionary )
        {
            sb.AppendLine( "{" );
            sb.AppendLine( item.Key + ", new()" );
            sb.AppendLine( "{" );
            sb.AppendLine(String.Join( ",", item.Value.Select( c => c.ToString() ) ));
            sb.AppendLine( "}" );
            sb.AppendLine( "}," );
        }
        sb.AppendLine( "};" );
        
    }
}