using System;
using System.Collections;
using System.Collections.Generic;

using LuminaSupplemental.Excel.Model;
using LuminaSupplemental.Excel.Services;

namespace LuminaSupplemental.Excel.DataShare.Sheets;

public abstract class SupplementalSheet<TRow, TBackedData>
    : BaseSupplementalSheet,
      ICollection<TRow>,
      IReadOnlyCollection<TRow>
    where TRow : struct, ISupplementalRow<TBackedData>, ICsv
{
    private List<TBackedData>? backingData;

    public TRow this[uint rowId] => this.GetRow(rowId);

    public List<TBackedData> BackingData => this.backingData ??= this.GetBackingData();

    public int Count => this.BackingData.Count;

    bool ICollection<TRow>.IsReadOnly => true;

    public bool Contains(TRow item)
    {
        for (var i = 0; i < this.Count; i++)
        {
            var row = this.GetRowOrDefault(i);
            if (row.HasValue && EqualityComparer<TRow>.Default.Equals(row.Value, item))
                return true;
        }

        return false;
    }

    public void CopyTo(TRow[] array, int arrayIndex)
    {
        if (array == null)
            throw new ArgumentNullException(nameof(array));
        if (arrayIndex < 0)
            throw new ArgumentOutOfRangeException(nameof(arrayIndex));
        if (array.Length - arrayIndex < this.Count)
            throw new ArgumentException("Destination array is not large enough.");

        for (var i = 0; i < this.Count; i++)
            array[arrayIndex + i] = this.GetRow((uint)i);
    }

    void ICollection<TRow>.Add(TRow item)
    {
        throw new NotSupportedException();
    }

    void ICollection<TRow>.Clear()
    {
        throw new NotSupportedException();
    }

    bool ICollection<TRow>.Remove(TRow item)
    {
        throw new NotSupportedException();
    }

    public IEnumerator<TRow> GetEnumerator()
    {
        for (var i = 0; i < this.Count; i++)
        {
            var row = this.GetRowOrDefault(i);
            if (row.HasValue)
                yield return row.Value;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }

    public abstract TBackedData ToBackedData(TRow row);
    public abstract TRow FromBackedData(TBackedData backedData, int rowIndex);

    public TRow? GetRowOrDefault(int rowIndex)
    {
        var backedData = this.BackingData;

        if (rowIndex >= 0 && rowIndex < backedData.Count)
        {
            var rowData = backedData[rowIndex];
            var fromBackedData = this.FromBackedData(rowData, rowIndex);
            fromBackedData.PopulateData(this.Module, this.Language);
            return fromBackedData;
        }

        return null;
    }

    public bool TryGetRow(uint rowIndex, out TRow row)
    {
        var result = this.GetRowOrDefault((int)rowIndex);
        if (result.HasValue)
        {
            row = result.Value;
            return true;
        }

        row = default;
        return false;
    }

    public TRow GetRow(uint rowIndex)
    {
        var result = this.GetRowOrDefault((int)rowIndex);
        if (!result.HasValue)
        {
            throw new KeyNotFoundException(
                $"Row index {rowIndex} does not exist in sheet '{this.SheetName}'.");
        }

        return result.Value;
    }

    private List<TBackedData> GetBackingData()
    {
        return this.PluginInterface.GetOrCreateData(
            "Lumina.Supplemental.DataShare." + this.SheetName + ".V" + this.Version,
            () =>
            {
                var loadedLines = CsvLoader.LoadResource<TRow>(
                    this.ResourceName,
                    true,
                    out _,
                    out _,
                    this.Module,
                    this.Language);

                var list = new List<TBackedData>(loadedLines.Count);
                foreach (var line in loadedLines)
                    list.Add(this.ToBackedData(line));

                return list;
            });
    }
}
