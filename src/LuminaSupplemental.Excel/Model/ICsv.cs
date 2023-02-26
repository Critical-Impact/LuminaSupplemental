using Lumina;
using Lumina.Data;

namespace LuminaSupplemental.Excel.Model;

public interface ICsv
{
    public void FromCsv( string[] lineData );
    public string[] ToCsv();
    public bool IncludeInCsv();

    public  void PopulateData( GameData gameData, Language language );
}