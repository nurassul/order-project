FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Delivery.Api/Delivery.Api.csproj", "Delivery.Api/"]
COPY ["Delivery.Application/Delivery.Application.csproj", "Delivery.Application/"]
COPY ["Delivery.Domain/Delivery.Domain.csproj", "Delivery.Domain/"]
COPY ["Delivery.Infrastructure/Delivery.Infrastructure.csproj", "Delivery.Infrastructure/"]
COPY ["Orders.Contracts/Orders.Contracts.csproj", "Orders.Contracts/"]
COPY ["Common.Application/Common.Application.csproj", "Common.Application/"]
COPY ["Directory.Build.props", "./"]
RUN dotnet restore "Delivery.Api/Delivery.Api.csproj"
COPY . .
WORKDIR "/src/Delivery.Api"
RUN dotnet build "./Delivery.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Delivery.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Delivery.Api.dll"]
