using System.Linq;

namespace LuminaSupplemental.SpaghettiGenerator.Extensions;

public static class StringExtensions
{
    public static string GetNumbers(this string input)
    {
        return new string(input.Where(c => char.IsDigit(c) || char.IsPunctuation(c)).ToArray());
    }
}
