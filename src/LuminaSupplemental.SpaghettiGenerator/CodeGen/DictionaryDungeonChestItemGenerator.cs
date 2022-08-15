using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LuminaSupplemental.Excel.Model;

namespace LuminaSupplemental.SpaghettiGenerator.CodeGen;

public class DictionaryDungeonChestItemGenerator : BaseShitGenerator
{
    protected string FieldName { get; set; }
    protected Dictionary<uint, DungeonChestItem> Dictionary { get; set; }

    public DictionaryDungeonChestItemGenerator( string fieldName, Dictionary<uint, DungeonChestItem> dictionary) 
    {
        FieldName = fieldName;
        Dictionary = dictionary;
    }
    
    public override void WriteFields( StringBuilder sb )
    {
        sb.AppendLine( $"public static Dictionary<uint, DungeonChestItem> {FieldName} {{ get; }} = new(){{" );
        foreach( var item in Dictionary )
        {
            sb.AppendLine( "{" );
            sb.AppendLine( item.Key + ", new() { ItemId = " + item.Value.ItemId + ", Coordinates = new Vector2(" + item.Value.Coordinates.X + "f," + item.Value.Coordinates.Y + "f), ContentFinderConditionId = " + item.Value.ContentFinderConditionId + "}"  );
            sb.AppendLine( "}," );
        }
        sb.AppendLine( "};" );
        
    }
}