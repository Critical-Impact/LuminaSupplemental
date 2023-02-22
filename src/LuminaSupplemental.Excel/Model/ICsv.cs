namespace LuminaSupplemental.Excel.Model;

public interface ICsv
{
    public void FromCsv( string[] lineData );
    public string[] ToCsv();
    public bool IncludeInCsv();
}