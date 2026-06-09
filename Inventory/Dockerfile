# Etapa base
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Etapa build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["FleetManager.Api/FleetManager.Api.csproj", "FleetManager.Api/"]
RUN dotnet restore "FleetManager.Api/FleetManager.Api.csproj"

COPY . .
WORKDIR "/src/FleetManager.Api"
RUN dotnet build "FleetManager.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Etapa publish
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "FleetManager.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Etapa final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FleetManager.Api.dll"]
