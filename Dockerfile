# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy central package management and build props
COPY Directory.Build.props .
COPY Directory.Packages.props .

# Copy all production project files for restore (no tests)
COPY src/Bootstrapper/ArchitecturePlayground.API/*.csproj src/Bootstrapper/ArchitecturePlayground.API/
COPY src/Shared/ArchitecturePlayground.Common.Abstractions/*.csproj src/Shared/ArchitecturePlayground.Common.Abstractions/
COPY src/Shared/ArchitecturePlayground.Common.Infrastructure/*.csproj src/Shared/ArchitecturePlayground.Common.Infrastructure/
COPY src/Shared/ArchitecturePlayground.Common.Contracts/*.csproj src/Shared/ArchitecturePlayground.Common.Contracts/
COPY src/Modules/Identity/Identity.Core/*.csproj src/Modules/Identity/Identity.Core/
COPY src/Modules/Identity/Identity.Infrastructure/*.csproj src/Modules/Identity/Identity.Infrastructure/
COPY src/Modules/Identity/Identity.Contracts/*.csproj src/Modules/Identity/Identity.Contracts/
COPY src/Modules/Catalog/Catalog.Core/*.csproj src/Modules/Catalog/Catalog.Core/
COPY src/Modules/Catalog/Catalog.Infrastructure/*.csproj src/Modules/Catalog/Catalog.Infrastructure/
COPY src/Modules/Catalog/Catalog.Contracts/*.csproj src/Modules/Catalog/Catalog.Contracts/
COPY src/Modules/Ordering/Ordering.Core/*.csproj src/Modules/Ordering/Ordering.Core/
COPY src/Modules/Ordering/Ordering.Infrastructure/*.csproj src/Modules/Ordering/Ordering.Infrastructure/
COPY src/Modules/Ordering/Ordering.Contracts/*.csproj src/Modules/Ordering/Ordering.Contracts/
COPY src/Modules/Basket/Basket.Core/*.csproj src/Modules/Basket/Basket.Core/
COPY src/Modules/Basket/Basket.Infrastructure/*.csproj src/Modules/Basket/Basket.Infrastructure/
COPY src/Modules/Basket/Basket.Contracts/*.csproj src/Modules/Basket/Basket.Contracts/
COPY src/Modules/Payment/Payment.Core/*.csproj src/Modules/Payment/Payment.Core/
COPY src/Modules/Payment/Payment.Infrastructure/*.csproj src/Modules/Payment/Payment.Infrastructure/
COPY src/Modules/Payment/Payment.Contracts/*.csproj src/Modules/Payment/Payment.Contracts/
COPY src/Modules/Notification/Notification.Core/*.csproj src/Modules/Notification/Notification.Core/
COPY src/Modules/Notification/Notification.Infrastructure/*.csproj src/Modules/Notification/Notification.Infrastructure/
COPY src/Modules/Notification/Notification.Contracts/*.csproj src/Modules/Notification/Notification.Contracts/

# Restore only the API project (will restore all dependencies transitively)
RUN dotnet restore src/Bootstrapper/ArchitecturePlayground.API/ArchitecturePlayground.API.csproj

# Copy source code (no tests - excluded by .dockerignore)
COPY src/ src/

# Build and publish
RUN dotnet publish src/Bootstrapper/ArchitecturePlayground.API/ArchitecturePlayground.API.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS runtime
WORKDIR /app

# Install curl for health checks
RUN apk add --no-cache curl

# Copy published app
COPY --from=build /app/publish .

# Render uses PORT env variable, default to 10000
ENV ASPNETCORE_URLS=http://+:${PORT:-10000}
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 10000

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:${PORT:-10000}/health || exit 1

ENTRYPOINT ["dotnet", "ArchitecturePlayground.API.dll"]
