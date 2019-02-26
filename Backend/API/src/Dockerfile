#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["Backend/API/src/TransportSystems.Backend.API.csproj", "Backend/API/src/"]
COPY ["Backend/Core/Infrastructure.Http/TransportSystems.Backend.Core.Infrastructure.Http.csproj", "Backend/Core/Infrastructure.Http/"]
COPY ["Backend/Core/Domain.Core/TransportSystems.Backend.Core.Domain.Core.csproj", "Backend/Core/Domain.Core/"]
COPY ["Backend/Identity/API/TransportSystems.Backend.Identity.API.csproj", "Backend/Identity/API/"]
COPY ["Backend/Identity/Core/Domain/Data/TransportSystems.Backend.Identity.Core.Data.csproj", "Backend/Identity/Core/Domain/Data/"]
COPY ["Backend/Core/Domain.Interfaces/TransportSystems.Backend.Core.Domain.Interfaces.csproj", "Backend/Core/Domain.Interfaces/"]
COPY ["Backend/Core/Infrastructure.Business/TransportSystems.Backend.Core.Infrastructure.Business.csproj", "Backend/Core/Infrastructure.Business/"]
COPY ["Backend/Core/Services.Interfaces/TransportSystems.Backend.Core.Services.Interfaces.csproj", "Backend/Core/Services.Interfaces/"]
COPY ["Backend/Application/Interfaces/TransportSystems.Backend.Application.Interfaces.csproj", "Backend/Application/Interfaces/"]
COPY ["Backend/External/Models/TransportSystems.Backend.External.Models.csproj", "Backend/External/Models/"]
COPY ["Common/Common.Models/Common.Models.csproj", "Common/Common.Models/"]
COPY ["Backend/Application/Models/TransportSystems.Backend.Application.Models.csproj", "Backend/Application/Models/"]
COPY ["Backend/Core/Infrastructure.Database/TransportSystems.Backend.Core.Infrastructure.Database.csproj", "Backend/Core/Infrastructure.Database/"]
COPY ["Backend/Application/Business/TransportSystems.Backend.Application.Business.csproj", "Backend/Application/Business/"]
COPY ["Backend/Identity/Core/Interfaces/TransportSystems.Backend.Identity.Core.Interfaces.csproj", "Backend/Identity/Core/Interfaces/"]
COPY ["Backend/External/Interfaces/TransportSystems.Backend.External.Interfaces.csproj", "Backend/External/Interfaces/"]
COPY ["Backend/External/Business/TransportSystems.Backend.External.Business.csproj", "Backend/External/Business/"]
RUN dotnet restore "Backend/API/src/TransportSystems.Backend.API.csproj"
COPY . .
WORKDIR "/src/Backend/API/src"
RUN dotnet build "TransportSystems.Backend.API.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "TransportSystems.Backend.API.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "TransportSystems.Backend.API.dll"]