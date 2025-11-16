using Microsoft.Extensions.Configuration;

const string defaultSqlDefaultConnectionStringName = "SqlDefault";
const string heathCheckEndpointPath = "/health";

var builder = DistributedApplication.CreateBuilder(args);

var sqlServerPassword = builder.AddParameter("SqlServerPassword", secret: true);

var sqlServerPort = builder.Configuration.GetSection("Parameters:SqlServerPort").Get<int>();

// SQL Server container is configured with an auto-generated password by default
var sqlserver = builder.AddSqlServer("sqlserver",
        password: sqlServerPassword,
        port: sqlServerPort)
    // Configure the container to store data in a volume so that it persists across instances.
    .WithDataVolume()
    // Keep the container running between app host sessions.
    .WithLifetime(ContainerLifetime.Persistent);

// Add the database to the application model so that it can be referenced by other resources.
var todoDb = sqlserver.AddDatabase("Todo");
var identityDb = sqlserver.AddDatabase("Identity");

builder.AddProject<Projects.Todo_Presentation_Api>("todo-api")
    .WithHttpHealthCheck(heathCheckEndpointPath)
    .WithReference(todoDb, defaultSqlDefaultConnectionStringName)
    .WaitFor(todoDb);

builder.AddProject<Projects.Identity_Presentation_Api>("identity-api")
    .WithHttpHealthCheck(heathCheckEndpointPath)
    .WithReference(identityDb, defaultSqlDefaultConnectionStringName)
    .WaitFor(identityDb);

builder.Build().Run();