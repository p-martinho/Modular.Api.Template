using FluentValidation;
using Microsoft.Extensions.Logging;
using SharedCore.Application.Commands;
using SharedCore.Application.Commands.Models;

namespace SharedCore.Application.Tests.TestHandlers;

internal class TestCommandHandler : CommandHandler<string, string>
{
    public TestCommandHandler(ILogger logger, IValidator<string> validator) : base(logger, validator)
    {
    }

    protected override Task<CommandOut<string>> HandleCommandInAsync(string commandIn,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(CommandOut<string>.Success("CommandOut"));
    }
}