# syntax=docker/dockerfile:1

# ---------- Build stage ----------
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# mejor caché
COPY inegi.csproj .
RUN dotnet restore inegi.csproj

# copia resto del código
COPY . .
RUN dotnet publish inegi.csproj -c Release -o /app /p:UseAppHost=false

# ---------- Runtime stage ----------
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app ./

EXPOSE 8080
# 👇 Usa el shell para expandir $PORT en tiempo de ejecución.
#    Si no existe PORT (local), usa 8080 por defecto.
ENTRYPOINT ["sh","-c","dotnet inegi.dll --urls http://0.0.0.0:${PORT:-8080}"]