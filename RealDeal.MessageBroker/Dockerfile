#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["RealDeal.MessageBroker/RealDeal.MessageBroker.csproj", "RealDeal.MessageBroker/"]

RUN dotnet restore "RealDeal.MessageBroker/RealDeal.MessageBroker.csproj"

COPY RealDeal.MessageBroker/ RealDeal.MessageBroker/

WORKDIR /src/RealDeal.MessageBroker
RUN dotnet build "RealDeal.MessageBroker.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "RealDeal.MessageBroker.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RealDeal.MessageBroker.dll"]
