namespace Identity.Common.Extensions;

/// <summary>
/// The <see cref="IAsyncEnumerable{T}"/> extensions.
/// </summary>
public static class AsyncEnumerableExtensions
{
    /// <summary>
    /// The <see cref="IAsyncEnumerable{T}"/> extensions.
    /// </summary>
    /// <param name="source">The source enumerable.</param>
    /// <typeparam name="T">The type of the values.</typeparam>
    extension<T>(IAsyncEnumerable<T> source)
    {
        /// <summary>
        /// Converts the <see cref="IAsyncEnumerable{T}"/> into a <see cref="List{T}"/> asynchronously.
        /// </summary>
        /// <returns>The list.</returns>
        public Task<List<T>> ToListAsync()
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
}