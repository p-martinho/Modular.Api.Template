FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Directory.Build.props", "src/"]
COPY ["Directory.Build.props", "."]
COPY ["src/ModuleTemplate/ModuleTemplate.Presentation.Api/ModuleTemplate.Presentation.Api.csproj", "src/ModuleTemplate/ModuleTemplate.Presentation.Api/"]
COPY ["src/Aspire/Aspire.ServiceDefaults/Aspire.ServiceDefaults.csproj", "src/Aspire/Aspire.ServiceDefaults/"]
COPY ["src/SharedCore/SharedCore.Common/SharedCore.Common.csproj", "src/SharedCore/SharedCore.Common/"]
COPY ["src/SharedCore/SharedCore.Presentation/SharedCore.Presentation.csproj", "src/SharedCore/SharedCore.Presentation/"]
COPY ["src/SharedCore/SharedCore.Application/SharedCore.Application.csproj", "src/SharedCore/SharedCore.Application/"]
COPY ["src/SharedCore/SharedCore.Persistence/SharedCore.Persistence.csproj", "src/SharedCore/SharedCore.Persistence/"]
COPY ["src/SharedCore/SharedCore.Domain/SharedCore.Domain.csproj", "src/SharedCore/SharedCore.Domain/"]
COPY ["src/ModuleTemplate/ModuleTemplate.Application/ModuleTemplate.Application.csproj", "src/ModuleTemplate/ModuleTemplate.Application/"]
COPY ["src/ModuleTemplate/ModuleTemplate.Persistence/ModuleTemplate.Persistence.csproj", "src/ModuleTemplate/ModuleTemplate.Persistence/"]
COPY ["src/ModuleTemplate/ModuleTemplate.Domain/ModuleTemplate.Domain.csproj", "src/ModuleTemplate/ModuleTemplate.Domain/"]
COPY ["src/ModuleTemplate/ModuleTemplate.Common/ModuleTemplate.Common.csproj", "src/ModuleTemplate/ModuleTemplate.Common/"]
RUN dotnet restore "./src/ModuleTemplate/ModuleTemplate.Presentation.Api/ModuleTemplate.Presentation.Api.csproj"
COPY . .
WORKDIR "/src/src/ModuleTemplate/ModuleTemplate.Presentation.Api"
RUN dotnet build "./ModuleTemplate.Presentation.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ModuleTemplate.Presentation.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ModuleTemplate.Presentation.Api.dll"]