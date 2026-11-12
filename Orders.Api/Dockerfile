FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Build.props", "./"]
COPY ["Orders.Api/Orders.Api.csproj", "Orders.Api/"]
COPY ["Orders.Domain/Orders.Domain.csproj", "Orders.Domain/"]
COPY ["Orders.Application/Orders.Application.csproj", "Orders.Application/"]
COPY ["Orders.Infrastructure/Orders.Infrastructure.csproj", "Orders.Infrastructure/"]
COPY ["Orders.Contracts/Orders.Contracts.csproj", "Orders.Contracts/"]
COPY ["Common.Abstractions/Common.Abstractions.csproj", "Common.Abstractions/"]
COPY ["Common.Messaging/Common.Messaging.csproj", "Common.Messaging/"]
RUN dotnet restore "Orders.Api/Orders.Api.csproj"
COPY . .
WORKDIR "/src/Orders.Api"
RUN dotnet build "./Orders.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Orders.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Orders.Api.dll"]

