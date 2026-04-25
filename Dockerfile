# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj files and restore
COPY ["Velora.Web/Velora.Web.csproj", "Velora.Web/"]
COPY ["Velora.Application/Velora.Application.csproj", "Velora.Application/"]
COPY ["Velora.Infrastructure/Velora.Infrastructure.csproj", "Velora.Infrastructure/"]
COPY ["Velora.Core/Velora.Core.csproj", "Velora.Core/"]
RUN dotnet restore "Velora.Web/Velora.Web.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/Velora.Web"
RUN dotnet build "Velora.Web.csproj" -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "Velora.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Velora.Web.dll"]
