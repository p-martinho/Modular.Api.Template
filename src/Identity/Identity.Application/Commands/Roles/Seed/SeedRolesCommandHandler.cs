using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SharedCore.Application.Commands;
using SharedCore.Application.Commands.Models;
using SharedCore.Common.Authorization;

namespace Identity.Application.Commands.Roles.Seed;

/// <summary>
/// The seed roles command handler.
/// </summary>
/// <seealso cref="CommandHandler{TOutData}"/>
/// <seealso cref="ISeedRolesCommandHandler"/>
[ExcludeFromCodeCoverage]
internal class SeedRolesCommandHandler : CommandHandler<bool>, ISeedRolesCommandHandler
{
    private readonly IEnumerable<string> _rolesToSeed = [UserRoles.Admin];

    private readonly RoleManager<IdentityRole> _roleManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="SeedRolesCommandHandler"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="roleManager">The role manager.</param>
    public SeedRolesCommandHandler(ILogger<SeedRolesCommandHandler> logger,
        RoleManager<IdentityRole> roleManager)
        : base(logger)
    {
        _roleManager = roleManager;
    }

    /// <inheritdoc />
    protected override async Task<CommandOut<bool>> ExecuteAsync(CancellationToken cancellationToken)
    {
        foreach (var roleName in _rolesToSeed)
        {
            await CreateRoleIfNotExistsAsync(roleName);
        }

        return CommandOut<bool>.Success(true);
    }

    private async Task CreateRoleIfNotExistsAsync(string roleName)
    {
        if (!await _roleManager.RoleExistsAsync(roleName))
        {
            await _roleManager.CreateAsync(new IdentityRole(roleName));

            Logger.LogInformation("Created role {RoleName}", roleName);
        }
        else
        {
            Logger.LogInformation("Role {RoleName} already exists, not added", roleName);
        }
    }
}