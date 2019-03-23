#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["Backend/Identity/Manage/TransportSystems.Backend.Identity.Manage.csproj", "Backend/Identity/Manage/"]
COPY ["Backend/Identity/Core/Business/TransportSystems.Backend.Identity.Core.Business.csproj", "Backend/Identity/Core/Business/"]
COPY ["Backend/Identity/Core/Interfaces/TransportSystems.Backend.Identity.Core.Interfaces.csproj", "Backend/Identity/Core/Interfaces/"]
COPY ["Backend/Identity/Core/Domain/Data/TransportSystems.Backend.Identity.Core.Data.csproj", "Backend/Identity/Core/Domain/Data/"]
COPY ["Backend/Identity/Core/Database/TransportSystems.Backend.Identity.Core.Database.csproj", "Backend/Identity/Core/Database/"]
COPY ["Backend/Identity/Core/Domain/Interfaces/TransportSystems.Backend.Identity.Core.Domain.Interfaces.csproj", "Backend/Identity/Core/Domain/Interfaces/"]
COPY ["Backend/Identity/API/TransportSystems.Backend.Identity.API.csproj", "Backend/Identity/API/"]
RUN dotnet restore "Backend/Identity/Manage/TransportSystems.Backend.Identity.Manage.csproj"
COPY . .
WORKDIR "/src/Backend/Identity/Manage"
RUN dotnet build "TransportSystems.Backend.Identity.Manage.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "TransportSystems.Backend.Identity.Manage.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "TransportSystems.Backend.Identity.Manage.dll"]