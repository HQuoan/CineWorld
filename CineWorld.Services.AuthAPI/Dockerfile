#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 7000

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["CineWorld.Services.AuthAPI/CineWorld.Services.AuthAPI.csproj", "CineWorld.Services.AuthAPI/"]
COPY ["CineWorld.EmailService/CineWorld.EmailService.csproj", "CineWorld.EmailService/"]
RUN dotnet restore "./CineWorld.Services.AuthAPI/CineWorld.Services.AuthAPI.csproj"
COPY . .
WORKDIR "/src/CineWorld.Services.AuthAPI"
RUN dotnet build "./CineWorld.Services.AuthAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CineWorld.Services.AuthAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CineWorld.Services.AuthAPI.dll"]