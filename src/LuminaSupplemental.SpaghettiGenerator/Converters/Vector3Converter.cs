using System;
using System.ComponentModel;
using System.Globalization;
using System.Numerics;

using Newtonsoft.Json;

namespace LuminaSupplemental.SpaghettiGenerator.Converters;


public sealed class Vector3TypeConverter : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
        => sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);

    public override object ConvertFrom(
        ITypeDescriptorContext? context,
        CultureInfo? culture,
        object value)
    {
        if (value is not string s)
            return base.ConvertFrom(context, culture, value);

        s = s.Trim();

        if (s.StartsWith("<") && s.EndsWith(">"))
            s = s.Substring(1, s.Length - 2);

        var parts = s.Split(',');

        if (parts.Length != 3)
            throw new FormatException($"Invalid Vector3 format: '{value}'");

        return new Vector3(
            float.Parse(parts[0], CultureInfo.InvariantCulture),
            float.Parse(parts[1], CultureInfo.InvariantCulture),
            float.Parse(parts[2], CultureInfo.InvariantCulture)
        );
    }

    public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
        => destinationType == typeof(string) || base.CanConvertTo(context, destinationType);

    public override object ConvertTo(
        ITypeDescriptorContext? context,
        CultureInfo? culture,
        object value,
        Type destinationType)
    {
        if (destinationType == typeof(string) && value is Vector3 v)
        {
            return $"<{v.X.ToString(CultureInfo.InvariantCulture)}, " +
                   $"{v.Y.ToString(CultureInfo.InvariantCulture)}, " +
                   $"{v.Z.ToString(CultureInfo.InvariantCulture)}>";
        }

        return base.ConvertTo(context, culture, value, destinationType);
    }
}
