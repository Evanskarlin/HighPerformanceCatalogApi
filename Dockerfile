FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY HighPerformanceCatalog.slnx ./

COPY src/Catalog.Api/Catalog.Api.csproj \
    src/Catalog.Api/

COPY src/Catalog.Application/Catalog.Application.csproj \
    src/Catalog.Application/

COPY src/Catalog.Domain/Catalog.Domain.csproj \
    src/Catalog.Domain/

COPY src/Catalog.Infrastructure/Catalog.Infrastructure.csproj \
    src/Catalog.Infrastructure/

RUN dotnet restore \
    src/Catalog.Api/Catalog.Api.csproj

COPY src/ src/

RUN dotnet publish \
    src/Catalog.Api/Catalog.Api.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "Catalog.Api.dll"]
