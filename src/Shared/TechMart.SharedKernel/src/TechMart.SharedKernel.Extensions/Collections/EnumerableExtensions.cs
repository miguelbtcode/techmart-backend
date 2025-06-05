namespace TechMart.SharedKernel.Extensions.Collections;

/// <summary>
/// Extension methods for IEnumerable<T>.
/// </summary>
public static class EnumerableExtensions
{
    /// <summary>
    /// Checks if the enumerable is null or empty.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="source">The enumerable.</param>
    /// <returns>True if null or empty; otherwise, false.</returns>
    public static bool IsNullOrEmpty<T>(this IEnumerable<T>? source) => source == null || !source.Any();

    /// <summary>
    /// Checks if the enumerable has any items.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="source">The enumerable.</param>
    /// <returns>True if has items; otherwise, false.</returns>
    public static bool HasAny<T>(this IEnumerable<T>? source) => source?.Any() == true;

    /// <summary>
    /// Returns the enumerable or empty if null.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="source">The enumerable.</param>
    /// <returns>The enumerable or empty.</returns>
    public static IEnumerable<T> OrEmpty<T>(this IEnumerable<T>? source) => source ?? Enumerable.Empty<T>();

    /// <summary>
    /// Splits the enumerable into batches of the specified size.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="source">The enumerable.</param>
    /// <param name="batchSize">The batch size.</param>
    /// <returns>Batches of the specified size.</returns>
    public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> source, int batchSize)
    {
        using var enumerator = source.GetEnumerator();
        while (enumerator.MoveNext())
        {
            yield return GetBatch(enumerator, batchSize);
        }
    }

    private static IEnumerable<T> GetBatch<T>(IEnumerator<T> enumerator, int batchSize)
    {
        do
        {
            yield return enumerator.Current;
        } while (--batchSize > 0 && enumerator.MoveNext());
    }

    /// <summary>
    /// Performs an action on each element.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="source">The enumerable.</param>
    /// <param name="action">The action to perform.</param>
    public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
    {
        foreach (var item in source)
        {
            action(item);
        }
    }

    /// <summary>
    /// Performs an action on each element with index.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="source">The enumerable.</param>
    /// <param name="action">The action to perform.</param>
    public static void ForEach<T>(this IEnumerable<T> source, Action<T, int> action)
    {
        var index = 0;
        foreach (var item in source)
        {
            action(item, index++);
        }
    }

    /// <summary>
    /// Returns distinct elements based on a key selector.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <typeparam name="TKey">The key type.</typeparam>
    /// <param name="source">The enumerable.</param>
    /// <param name="keySelector">The key selector.</param>
    /// <returns>Distinct elements.</returns>
    public static IEnumerable<T> DistinctBy<T, TKey>(this IEnumerable<T> source, Func<T, TKey> keySelector)
    {
        var seenKeys = new HashSet<TKey>();
        foreach (var element in source)
        {
            if (seenKeys.Add(keySelector(element)))
            {
                yield return element;
            }
        }
    }

    /// <summary>
    /// Converts the enumerable to a delimited string.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="source">The enumerable.</param>
    /// <param name="delimiter">The delimiter.</param>
    /// <returns>The delimited string.</returns>
    public static string ToDelimitedString<T>(this IEnumerable<T> source, string delimiter = ",")
    {
        return string.Join(delimiter, source);
    }

    /// <summary>
    /// Safely gets an element at the specified index.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="source">The enumerable.</param>
    /// <param name="index">The index.</param>
    /// <returns>The element at index or default.</returns>
    public static T? SafeElementAt<T>(this IEnumerable<T> source, int index)
    {
        if (index < 0) return default;
        
        if (source is IList<T> list)
        {
            return index < list.Count ? list[index] : default;
        }

        return source.Skip(index).FirstOrDefault();
    }
}