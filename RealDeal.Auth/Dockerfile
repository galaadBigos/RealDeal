#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["RealDeal.Shared/RealDeal.Shared.csproj", "RealDeal.Shared/"]
COPY ["RealDeal.Auth/RealDeal.Auth.csproj", "RealDeal.Auth/"]

RUN dotnet restore "RealDeal.Auth/RealDeal.Auth.csproj"

COPY RealDeal.Shared/ RealDeal.Shared/
COPY RealDeal.Auth/ RealDeal.Auth/

WORKDIR "/src/RealDeal.Auth"
RUN dotnet build "./RealDeal.Auth.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./RealDeal.Auth.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RealDeal.Auth.dll"]