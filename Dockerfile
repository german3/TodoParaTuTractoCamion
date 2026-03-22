FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["TodoParaTuTractoCamion.API/TodoParaTuTractoCamion.API.csproj", "TodoParaTuTractoCamion.API/"]
COPY ["TodoParaTuTractoCamion.Application/TodoParaTuTractoCamion.Application.csproj", "TodoParaTuTractoCamion.Application/"]
COPY ["TodoParaTuTractoCamion.Domain/TodoParaTuTractoCamion.Domain.csproj", "TodoParaTuTractoCamion.Domain/"]
COPY ["TodoParaTuTractoCamion.Infrastructure/TodoParaTuTractoCamion.Infrastructure.csproj", "TodoParaTuTractoCamion.Infrastructure/"]
RUN dotnet restore "./TodoParaTuTractoCamion.API/TodoParaTuTractoCamion.API.csproj"
COPY . .
WORKDIR "/src/TodoParaTuTractoCamion.API"
RUN dotnet build "./TodoParaTuTractoCamion.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./TodoParaTuTractoCamion.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY ["productos_backup.json", "."]
ENTRYPOINT ["dotnet", "TodoParaTuTractoCamion.API.dll"]
