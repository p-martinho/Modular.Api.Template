namespace Identity.Common.Extensions;

/// <summary>
/// The <see cref="IAsyncEnumerable{T}"/> extensions.
/// </summary>
public static class AsyncEnumerableExtensions
{
    /// <summary>
    /// Converts the <see cref="IAsyncEnumerable{T}"/> into a <see cref="List{T}"/> asynchronously.
    /// </summary>
    /// <param name="source">The source enumerable.</param>
    /// <typeparam name="T">The type of the values.</typeparam>
    /// <returns>The list.</returns>
    public static Task<List<T>> ToListAsync<T>(this IAsyncEnumerable<T> source)
    {
        ArgumentNullException.ThrowIfNull(source);

        return ExecuteAsync();

        async Task<List<T>> ExecuteAsync()
        {
            var list = new List<T>();

            await foreach (var element in source)
            {
                list.Add(element);
            }

            return list;
        }
    }
}