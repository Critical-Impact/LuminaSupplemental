using Lumina;
using Lumina.Data;
using Lumina.Excel;

namespace LuminaSupplemental.Excel.Model;

public interface ICsv
{
    public void FromCsv( string[] lineData );
    public string[] ToCsv();
    public bool IncludeInCsv();

    public  void PopulateData( ExcelModule gameData, Language language );
}
