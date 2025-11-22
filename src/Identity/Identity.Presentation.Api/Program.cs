using Asp.Versioning;
using Aspire.ServiceDefaults;
using Identity.Presentation.Api.DependencyInjection;
using Identity.Presentation.Api.Extensions;
using Identity.Presentation.Api.OpenApi;
using Serilog;
using SharedCore.Presentation.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.AddServiceDefaults();

builder.Services.AddCustomHealthChecks(builder.Configuration);

ApiVersion[] activeApiVersions = [new(1, 0)];
ApiVersion[] deprecatedApiVersions = [];
var apiVersions = activeApiVersions.Concat(deprecatedApiVersions).ToArray();

builder.Services.AddOpenApiDocuments(apiVersions);

builder.Services.AddApiDependencies(builder.Configuration, builder.Environment);

// Build app.

var app = builder.Build();

await app.SeedResourcesAsync();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseSerilogRequestLogging();

app.UseExceptionHandler();

app.UseInternalErrorMiddleware();

app.MapDefaultEndpoints();

app.MapEndpoints<Program>(activeApiVersions, deprecatedApiVersions);

if (app.Environment.IsDevelopment())
{
    // This needs to be after mapping the API versions (in MapEndpoints())
    app.MapOpenApi().CacheOutput();
    app.MapScalar(apiVersions);
}

// Run app.

app.Run();