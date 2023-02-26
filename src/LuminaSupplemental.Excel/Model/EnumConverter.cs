using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace LuminaSupplemental.Excel.Model;

public class EnumConverter : UInt32Converter
{
    public override string ConvertToString( object value, IWriterRow row, MemberMapData memberMapData )
    {
        return ( (int)value ).ToString();
    }
}