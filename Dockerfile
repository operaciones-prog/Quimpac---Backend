# Usar imagen base de .NET 6.0
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 10000

# Instalar curl para healthchecks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Imagen para build
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["PROYEC_QUIMPAC.csproj", "."]
RUN dotnet restore "PROYEC_QUIMPAC.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "PROYEC_QUIMPAC.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PROYEC_QUIMPAC.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Crear usuario no-root para seguridad
RUN addgroup --system --gid 1001 quimpac && \
    adduser --system --uid 1001 --gid 1001 --shell /bin/false quimpac

# Cambiar ownership y permisos
RUN chown -R quimpac:quimpac /app
USER quimpac

ENTRYPOINT ["dotnet", "PROYEC_QUIMPAC.dll"]