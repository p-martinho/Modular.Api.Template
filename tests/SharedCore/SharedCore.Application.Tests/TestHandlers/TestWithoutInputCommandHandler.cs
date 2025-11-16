using Microsoft.Extensions.Logging;
using SharedCore.Application.Commands;
using SharedCore.Application.Commands.Models;

namespace SharedCore.Application.Tests.TestHandlers;

internal class TestWithoutInputCommandHandler : CommandHandler<string>
{
    private readonly ITestService _testService;

    public TestWithoutInputCommandHandler(ILogger logger,
        ITestService testService)
        : base(logger)
    {
        _testService = testService;
    }

    protected override Task<CommandOut<string>> ExecuteAsync(CancellationToken cancellationToken)
    {
        _testService.DoSomething();

        return Task.FromResult(CommandOut<string>.Success("CommandOut"));
    }
}