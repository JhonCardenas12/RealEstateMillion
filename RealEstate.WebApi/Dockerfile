# Imagen base de ASP.NET Core Runtime (.NET 8)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Imagen para compilar con SDK (.NET 8)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar el archivo de solución y los csproj primero (mejora cache de restore)
COPY ["RealEstate.sln", "."]
COPY ["RealEstate.WebApi/RealEstate.WebApi.csproj", "RealEstate.WebApi/"]
COPY ["RealEstate.Application/RealEstate.Application.csproj", "RealEstate.Application/"]
COPY ["RealEstate.Domain/RealEstate.Domain.csproj", "RealEstate.Domain/"]
COPY ["RealEstate.Infrastructure/RealEstate.Infrastructure.csproj", "RealEstate.Infrastructure/"]
COPY ["RealEstate.DataBase/RealEstate.DataBase.csproj", "RealEstate.DataBase/"]

# Restaurar dependencias
RUN dotnet restore "RealEstate.WebApi/RealEstate.WebApi.csproj"

# Copiar el resto del código
COPY . .

# Publicar en modo Release (sin apphost para reducir tamaño)
WORKDIR "/src/RealEstate.WebApi"
RUN dotnet publish "RealEstate.WebApi.csproj" -c Release -o /app/publish --no-restore /p:UseAppHost=false

# Imagen final más ligera
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "RealEstate.WebApi.dll"]
