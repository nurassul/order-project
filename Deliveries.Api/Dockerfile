FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Deliveries.Api/Deliveries.Api.csproj", "Deliveries.Api/"]
COPY ["Deliveries.Application/Deliveries.Application.csproj", "Deliveries.Application/"]
COPY ["Deliveries.Domain/Deliveries.Domain.csproj", "Deliveries.Domain/"]
COPY ["Deliveries.Infrastructure/Deliveries.Infrastructure.csproj", "Deliveries.Infrastructure/"]
COPY ["Orders.Contracts/Orders.Contracts.csproj", "Orders.Contracts/"]
COPY ["Common.Abstractions/Common.Abstractions.csproj", "Common.Abstractions/"]
COPY ["Common.Messaging/Common.Messaging.csproj", "Common.Messaging/"]
COPY ["Common.Application/Common.Application.csproj", "Common.Application/"]
COPY ["Directory.Build.props", "./"]
RUN dotnet restore "Deliveries.Api/Deliveries.Api.csproj"
COPY . .
WORKDIR "/src/Deliveries.Api"
RUN dotnet build "./Deliveries.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Deliveries.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Deliveries.Api.dll"]
