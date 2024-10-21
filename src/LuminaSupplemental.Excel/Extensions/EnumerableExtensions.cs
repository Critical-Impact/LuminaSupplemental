using System;
using System.Collections.Generic;
using System.Linq;

namespace LuminaSupplemental.Excel.Extensions;

public static class EnumerableExtensions
{
    public static T? FirstOrNull<T>(this IEnumerable<T> source) where T : struct
    {
        return source.Cast<T?>().FirstOrDefault();
    }

    public static T? FirstOrNull<T>(this IEnumerable<T> source, Func<T, bool> predicate) where T : struct
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(predicate);
        return source.Where(predicate).Cast<T?>().FirstOrDefault();
    }
}
