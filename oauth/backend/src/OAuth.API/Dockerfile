FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/OAuth.API/OAuth.API.csproj", "src/OAuth.API/"]
COPY ["src/OAuth.Application/OAuth.Application.csproj", "src/OAuth.Application/"]
COPY ["src/OAuth.Infrastructure/OAuth.Infrastructure.csproj", "src/OAuth.Infrastructure/"]
COPY ["src/OAuth.Domain/OAuth.Domain.csproj", "src/OAuth.Domain/"]

RUN dotnet restore "src/OAuth.API/OAuth.API.csproj"
COPY . .

WORKDIR "/src/src/OAuth.API"
RUN dotnet build "OAuth.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OAuth.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OAuth.API.dll"]
