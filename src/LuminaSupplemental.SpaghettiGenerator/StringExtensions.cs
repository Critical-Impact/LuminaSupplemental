using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace LuminaSupplemental.SpaghettiGenerator;

public static class StringExtensions
{
    public static string RemoveWhitespace(this string input)
    {
        return new string(input.ToCharArray()
            .Where(c => !Char.IsWhiteSpace(c))
            .ToArray());
    }

    public static string RemoveSpecialCharacters( this string input )
    {
        Regex rgx = new Regex("[^a-zA-Z0-9]");
        return rgx.Replace(input, "");
    }

    public static string ToParseable( this string input )
    {
        if( input.Contains( "\u00a0" ) )
        {
            var startIndex = input.IndexOf( "\u00a0" );
            input = input.Remove( startIndex );
        }
        if( input.Contains( "(IL" ) )
        {
            var startIndex = input.IndexOf( "(IL" );
            input = input.Remove( startIndex );
        }

        var replacements = new Dictionary< string, string >()
        {
            { "High Mythril", "Dwarven Mythril"},
            {"Armor", "Attire"},
            {"Garb","Attire"},
            {"pair of ", ""},
            {"suit of ", ""},
            {"Level 110 Weapon Coffer", "Level 50 Weapon Coffer"},
            {"Level 90 Weapon Coffer", "Level 50 Weapon Coffer"},
            {"Level 70 Weapon Coffer", "Level 50 Weapon Coffer"},
        };
        foreach( var replacement in replacements )
        {
            input = input.Replace( replacement.Key, replacement.Value );
        }
        return input.ToLower().RemoveWhitespace().RemoveSpecialCharacters();
    }
}