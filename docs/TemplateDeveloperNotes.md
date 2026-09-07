# Template Developer Notes

This solution is for a .NET solution template.
It includes the source files (the files that will be in the output solution) and the template files (files that belongs to the template, not to its output).

# Template Files

- `Modular.Api.Template.csproj`: To build a template package (NuGet `.nupkg` file that bundles one or more templates together), we need a C# project file (`.csproj`) configured to act as a packaging project rather than a compilation project. The file has specific properties for templating.
- `.template.config/template.json`: file with the template configuration.
- `README.md`: the template's README that explains how to create a solution with the template and all the information about the projects, architecture decisions and technologies.
- `README-template.md`: The README that will exist in the template output. Basically, the template engine will copy it and then rename it to `README.md`. The template's `README.md` is not copied.
- `docs/**`: files used to document the template (e.g. diagrams used in the template's README).
- `icon.png`: the icon for the NuGet package.
- `src/ModuleTemplate/**`: source files to create the project for a new module (with `dotnet new mod-api --add module --module-name YourNewModuleName`).
- `tests/ModuleTemplate/**`: source files to create the project for testing a module (with `dotnet new mod-api --add tests --module-name YourNewModuleName`).

# Template Configuration

The template is configured in the file `.template.config/template.json`, namely the symbols, parameters, what to include or exclude, post actions, etc.

Some of the configurations used:
- `sources`: include/exclude the files that will be created. For instance, it excludes template specific files, that we don't want to include in the template output.
- `guids`: list of GUIDs which appear in the template source and should be replaced in the template output. For each GUID listed, a replacement GUID is generated, and replaces all occurrences of the source GUID in the output. This is useful to have different GUIDs for each time the template is used.
- `primaryOutputs`: a list of files that will be used in `postActions`.
- `postActions`: actions to be performed after the projects are created. In this case we are using the action with ID [`D396686C-DE0E-4DE6-906D-291CD29FC5DE`](https://github.com/dotnet/templating/wiki/Post-Action-Registry#add-projects-to-a-solution-file), which adds new projects to the solution file.

# Parameters

- `-n` or `--name`: This is an optional parameter from `dotnet new <TEMPLATE>` command, not a parameter from this template. It is the name for the created output. If no name is specified, the name of the current directory is used. This name will be used to replace `Modular.Api.Template` string everywhere.
- `add`: To create the whole solution, or just to add a new module to an existent solution, or to add new test projects for an existent module. Possible values: `solution` (default), `module` or `tests`. If not provided, is the same as using `--add solution`.
    - For `module`, it will use as source only the files in `src/ModuleTemplate` and replace `ModuleTemplate` with the value of `--module-name`.
    - For `tests`,  it will use as source only the files in `tests/ModuleTemplate` and replace `ModuleTemplate` with the value of `--module-name`.
- `module-name`: the name of the module, when creating a new module (with `--add module --module-name YourNewModuleName`) or tests for a module (`--add tests --module-name YourNewModuleName`). If `--add module` or `--add tests` is used without `--module-name`, the default is `ModuleTemplate`.
- `with-docker`: adds support for Docker in the new solution or new module. The template output will include Docker-related files (like `Dockerfile`and `docker-compose.yml`). Without this option, everything inside `#if (IsToAddDocker)` directives will be excluded from the output.

# Test Template Locally

Before shipping the NuGet package, we need to test the template.
The best way to check the output is installing the template directly from its folder and then test it:

- Uninstall the template, if installed normally (with package): `dotnet new uninstall PMart.Modular.Api.Template`
- Install/reinstall from local folder (in the root of the solution): `dotnet new install .\ --force`
- Test it: `dotnet new mod-api -n YourSolutionName`
- In the end, uninstall it:
  - Check the command to uninstall it: `dotnet new uninstall`
  - Run the uninstallation command (instead of the name of the template, it uses the template local full path)

# Re-create Initial Migrations

To re-do the initial migrations:

- Remove the folders 'Migrations' from projects `Todo.Pesistence`, `Identity.Persistence` and `SharedCore.Persistence.IntegrationTests`
- Temporally remove `Modular.Api.Template.csproj` (its existence along with the solution file will cause an error in the migration command)
- Create new migrations, using the commands:
  ```
  dotnet ef migrations add InitialMigration --startup-project ./src/Todo/Todo.Presentation.Api/ --project ./src/Todo/Todo.Persistence/ -- --environment Migration
  dotnet ef migrations add InitialMigration --startup-project ./src/Identity/Identity.Presentation.Api/ --project ./src/Identity/Identity.Persistence/ -- --environment Migration
  dotnet ef migrations add InitialMigration --startup-project ./tests/SharedCore/SharedCore.Persistence.IntegrationTests/ --project ./tests/SharedCore/SharedCore.Persistence.IntegrationTests/
  ```
- Revert the removal of `Modular.Api.Template.csproj`

# References

- [.NET templates for authors](https://learn.microsoft.com/en-us/dotnet/core/tools/templates)
- [.NET Templating Wiki](https://github.com/dotnet/templating/wiki)
- [Tutorial: Create a project template](https://learn.microsoft.com/en-us/dotnet/core/tutorials/cli-templates-create-project-template)