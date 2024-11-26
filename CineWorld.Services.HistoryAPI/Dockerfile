#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 7004

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["CineWorld.Services.HistoryAPI/CineWorld.Services.HistoryAPI.csproj", "CineWorld.Services.HistoryAPI/"]
RUN dotnet restore "./CineWorld.Services.HistoryAPI/CineWorld.Services.HistoryAPI.csproj"
COPY . .
WORKDIR "/src/CineWorld.Services.HistoryAPI"
RUN dotnet build "./CineWorld.Services.HistoryAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CineWorld.Services.HistoryAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CineWorld.Services.HistoryAPI.dll"]