using System;
using System.Numerics;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace LuminaSupplemental.Excel.Converters;

public class Vector3Converter : DefaultTypeConverter
{
    public override object ConvertFromString( string text, IReaderRow row, MemberMapData memberMapData )
    {
        throw new Exception( "Not implemented" );
    }

    public override string ConvertToString( object value, IWriterRow row, MemberMapData memberMapData )
    {
        var vector = (Vector3)value;
        return vector.X + ";" + vector.Y + ";" + vector.Z;
    }
}