FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/BusinessUnits.API/BusinessUnits.API.csproj", "src/BusinessUnits.API/"]
COPY ["src/BusinessUnits.Application/BusinessUnits.Application.csproj", "src/BusinessUnits.Application/"]
COPY ["src/BusinessUnits.Infrastructure/BusinessUnits.Infrastructure.csproj", "src/BusinessUnits.Infrastructure/"]
COPY ["src/BusinessUnits.Domain/BusinessUnits.Domain.csproj", "src/BusinessUnits.Domain/"]

RUN dotnet restore "src/BusinessUnits.API/BusinessUnits.API.csproj"

COPY . .

WORKDIR "/src/src/BusinessUnits.API"
RUN dotnet build "BusinessUnits.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BusinessUnits.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BusinessUnits.API.dll"]
