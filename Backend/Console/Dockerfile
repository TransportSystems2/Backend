FROM microsoft/dotnet:2.1-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY Backend/Console/TransportSystems.Backend.Console.csproj Backend/Console/
COPY Backend/Identity/API/TransportSystems.Backend.Identity.API.csproj Backend/Identity/API/
COPY Backend/Identity/Core/Data/TransportSystems.Backend.Identity.Core.Data.csproj Backend/Identity/Core/Data/
COPY Backend/Core/Domain.Core/TransportSystems.Backend.Core.Domain.Core.csproj Backend/Core/Domain.Core/
COPY Backend/Core/Infrastructure.Database/TransportSystems.Backend.Core.Infrastructure.Database.csproj Backend/Core/Infrastructure.Database/
COPY Backend/Core/Domain.Interfaces/TransportSystems.Backend.Core.Domain.Interfaces.csproj Backend/Core/Domain.Interfaces/
COPY Backend/Core/Infrastructure.Business/TransportSystems.Backend.Core.Infrastructure.Business.csproj Backend/Core/Infrastructure.Business/
COPY Backend/Core/Services.Interfaces/TransportSystems.Backend.Core.Services.Interfaces.csproj Backend/Core/Services.Interfaces/
COPY Backend/Core/Infrastructure.Http/TransportSystems.Backend.Core.Infrastructure.Http.csproj Backend/Core/Infrastructure.Http/
RUN dotnet restore Backend/Console/TransportSystems.Backend.Console.csproj
COPY . .
WORKDIR /src/Backend/Console
RUN dotnet build TransportSystems.Backend.Console.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish TransportSystems.Backend.Console.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "TransportSystems.Backend.Console.dll"]
