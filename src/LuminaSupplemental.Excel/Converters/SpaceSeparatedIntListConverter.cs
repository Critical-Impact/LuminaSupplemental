using System;
using System.Collections.Generic;
using System.Globalization;

using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace LuminaSupplemental.Excel.Converters;

public sealed class SpaceSeparatedIntListConverter : DefaultTypeConverter
{
    public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        => Parse(text);

    public override string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
        => Format((IReadOnlyList<int>)value);

    internal static IReadOnlyList<int> Parse(string value)
    {
        var values = value.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var result = new int[values.Length];
        for (var i = 0; i < values.Length; i++)
            result[i] = int.Parse(values[i], CultureInfo.InvariantCulture);

        return result;
    }

    internal static string Format(IReadOnlyList<int> values)
        => string.Join(' ', values);
}
