#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 7003

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["CineWorld.Services.ReactionAPI/CineWorld.Services.ReactionAPI.csproj", "CineWorld.Services.ReactionAPI/"]
RUN dotnet restore "./CineWorld.Services.ReactionAPI/CineWorld.Services.ReactionAPI.csproj"
COPY . .
WORKDIR "/src/CineWorld.Services.ReactionAPI"
RUN dotnet build "./CineWorld.Services.ReactionAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CineWorld.Services.ReactionAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CineWorld.Services.ReactionAPI.dll"]