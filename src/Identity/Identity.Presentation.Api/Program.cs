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

builder.Services.AddApiDependencies(builder.Configuration, builder.Environment);

builder.Services.AddOpenApiDocuments();

// Build app.

var app = builder.Build();

await app.SeedResourcesAsync();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseSerilogRequestLogging();

app.UseExceptionHandler();

app.UseAuthentication();
app.UseAuthorization();

app.UseInternalErrorMiddleware();

app.MapDefaultEndpoints();

app.MapEndpoints<Program>();

if (app.Environment.IsDevelopment())
{
    // This needs to be after mapping the API versions (in MapEndpoints())
    app.MapOpenApi()
        .WithDocumentPerVersion()
        .CacheOutput();

    app.MapScalar();
}

// Run app.

app.Run();