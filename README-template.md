# Modular.Api.Template

This solution was generated using the template [PMart.Modular.Api.Template](https://github.com/p-martinho/Modular.Api.Template).
Check the documentation [here](https://github.com/p-martinho/Modular.Api.Template).

This is a Modular Monolith, that includes several ASP.NET Core APIs.

# Requirements

* [.NET 10.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/10.0) (or later)
* [Docker Desktop](https://www.docker.com/)
* [EF Core CLI Tool](https://learn.microsoft.com/en-us/ef/core/cli/dotnet)

# Run

Run the `Aspire.AppHost` project. It requires **Docker Desktop** running, for the database.

The **Aspire Dashboard** will be launched automatically.

Navigate to [https://localhost:7107/scalar](), to see the **Identity API** documentation
and navigate to [https://localhost:7217/scalar]() to see the **Todo API** documentation.

You can test the APIs, using the provided examples in the `.http` files (in the `Presentation.Api` folders of each module).

# Test

Run the tests using the IDE or with the [command line](https://xunit.net/docs/getting-started/v3/cmdline):

```
dotnet run
```

The integration tests use a real database, using the [TestContainers](https://dotnet.testcontainers.org/) library (requires **Docker Desktop** running).

To assess the code coverage, and if your IDE does not include a tool for it, follow the instructions [here](https://xunit.net/docs/getting-started/v3/code-coverage-with-mtp).