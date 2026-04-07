using Asp.Versioning;
using Aspire.ServiceDefaults;
using Serilog;
using SharedCore.Presentation.Extensions;
using Todo.Presentation.Api.DependencyInjection;
using Todo.Presentation.Api.OpenApi;

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

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseSerilogRequestLogging();

app.UseExceptionHandler();

app.UseAuthentication();
app.UseAuthorization();

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