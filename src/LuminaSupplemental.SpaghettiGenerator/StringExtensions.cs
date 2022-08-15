using System;
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
        return input.Replace( "pair of ", "" ).Replace( "suit of ", "" ).ToLower().RemoveWhitespace().RemoveSpecialCharacters();
    }
}