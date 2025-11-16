FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Directory.Build.props", "src/"]
COPY ["Directory.Build.props", "."]
COPY ["src/Todo/Todo.Presentation.Api/Todo.Presentation.Api.csproj", "src/Todo/Todo.Presentation.Api/"]
COPY ["src/Aspire/Aspire.ServiceDefaults/Aspire.ServiceDefaults.csproj", "src/Aspire/Aspire.ServiceDefaults/"]
COPY ["src/SharedCore/SharedCore.Common/SharedCore.Common.csproj", "src/SharedCore/SharedCore.Common/"]
COPY ["src/SharedCore/SharedCore.Presentation/SharedCore.Presentation.csproj", "src/SharedCore/SharedCore.Presentation/"]
COPY ["src/SharedCore/SharedCore.Application/SharedCore.Application.csproj", "src/SharedCore/SharedCore.Application/"]
COPY ["src/SharedCore/SharedCore.Persistence/SharedCore.Persistence.csproj", "src/SharedCore/SharedCore.Persistence/"]
COPY ["src/SharedCore/SharedCore.Domain/SharedCore.Domain.csproj", "src/SharedCore/SharedCore.Domain/"]
COPY ["src/Todo/Todo.Application/Todo.Application.csproj", "src/Todo/Todo.Application/"]
COPY ["src/Todo/Todo.Persistence/Todo.Persistence.csproj", "src/Todo/Todo.Persistence/"]
COPY ["src/Todo/Todo.Domain/Todo.Domain.csproj", "src/Todo/Todo.Domain/"]
COPY ["src/Todo/Todo.Common/Todo.Common.csproj", "src/Todo/Todo.Common/"]
RUN dotnet restore "./src/Todo/Todo.Presentation.Api/Todo.Presentation.Api.csproj"
COPY . .
WORKDIR "/src/src/Todo/Todo.Presentation.Api"
RUN dotnet build "./Todo.Presentation.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Todo.Presentation.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Todo.Presentation.Api.dll"]