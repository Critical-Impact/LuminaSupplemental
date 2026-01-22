using System.Linq;

using Dalamud.Utility;

using Lumina.Text.ReadOnly;

namespace LuminaSupplemental.SpaghettiGenerator.Extensions;

public static class StringExtensions
{
    public static string GetNumbers(this string input)
    {
        return new string(input.Where(c => char.IsDigit(c) || char.IsPunctuation(c)).ToArray());
    }

    public static string ToImGuiString(this ReadOnlySeString readOnlySeString)
    {
        return readOnlySeString.ExtractText().StripSoftHyphen();
    }
}
