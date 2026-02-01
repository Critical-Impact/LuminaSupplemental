namespace LuminaSupplemental.Excel.DataShare.Sheets;

public interface ISupplementalRow<T>
{
    static string ResourcePath;
    
    int RowId { get; }
    
    void FromCsv(string[] lineData);
}
