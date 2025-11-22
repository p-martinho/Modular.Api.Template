using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace SharedCore.Persistence.IntegrationTests.TestServices;

internal class TestHostEnvironment : IHostEnvironment
{
    public string EnvironmentName { get; set; } = Environments.Development;

    public string ApplicationName { get; set; } = string.Empty;

    public string ContentRootPath { get; set; } = string.Empty;

    public IFileProvider ContentRootFileProvider { get; set; } = null!;
}