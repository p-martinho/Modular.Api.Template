using System.Diagnostics.CodeAnalysis;
using Identity.Application.Commands.OpenId.Seed;
using Identity.Application.Commands.Roles.Seed;
using SharedCore.Application.Commands.Models;

namespace Identity.Presentation.Api.Extensions;

/// <summary>
/// The Web application extensions.
/// </summary>
[ExcludeFromCodeCoverage]
internal static class WebApplicationExtensions
{
    /// <summary>
    /// Seeds the required resources asynchronous.
    /// </summary>
    /// <param name="app">The Web application.</param>
    /// <returns>The task.</returns>
    public static async Task SeedResourcesAsync(this WebApplication app)
    {
        await using var scope = app.Services.CreateAsyncScope();

        if (app.Environment.IsDevelopment())
        {
            // Seed the OpenId resources in Development.
            await SeedOpenIdResourcesAsync(scope);
        }

        await SeedUserRolesAsync(scope);
    }

    private static Task<CommandOut<bool>> SeedOpenIdResourcesAsync(AsyncServiceScope scope)
    {
        var seedHandler = scope.ServiceProvider.GetRequiredService<ISeedOpenIdTestingResourcesCommandHandler>();

        return seedHandler.HandleAsync();
    }

    private static Task<CommandOut<bool>> SeedUserRolesAsync(AsyncServiceScope scope)
    {
        var seedHandler = scope.ServiceProvider.GetRequiredService<ISeedRolesCommandHandler>();

        return seedHandler.HandleAsync();
    }
}