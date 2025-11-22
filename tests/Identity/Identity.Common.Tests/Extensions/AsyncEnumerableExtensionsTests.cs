using Identity.Common.Extensions;

namespace Identity.Common.Tests.Extensions;

public class AsyncEnumerableExtensionsTests
{
    [Fact]
    public async Task ToListAsync_ShouldSucceed()
    {
        // Arrange
        var expectedResult = new List<int> { 0, 1, 2, 3, 4 };
        var count = expectedResult.Count;

        // Act
        var result = await DoSomethingAsync(count).ToListAsync();

        // Assert
        Assert.Equal(expectedResult, result);
    }

    private static async IAsyncEnumerable<int> DoSomethingAsync(int count)
    {
        for (var i = 0; i < count; i++)
        {
            await Task.Delay(i);
            yield return i;
        }
    }
}