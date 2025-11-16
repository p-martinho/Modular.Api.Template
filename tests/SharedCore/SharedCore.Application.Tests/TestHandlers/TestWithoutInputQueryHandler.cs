using Microsoft.Extensions.Logging;
using SharedCore.Application.Queries;
using SharedCore.Application.Queries.Models;

namespace SharedCore.Application.Tests.TestHandlers;

internal class TestWithoutInputQueryHandler : QueryHandler<string>
{
    private readonly ITestService _testService;

    public TestWithoutInputQueryHandler(ILogger logger,
        ITestService testService)
        : base(logger)
    {
        _testService = testService;
    }

    protected override Task<QueryOut<string>> ExecuteAsync(CancellationToken cancellationToken)
    {
        _testService.DoSomething();

        return Task.FromResult(QueryOut<string>.Success("QueryOut"));
    }
}