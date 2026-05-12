# STAGE 1: Build menggunakan SDK .NET 10
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution dan project file (Gunakan wildcard agar lebih ringkas)
COPY ["WareHaus.slnx", "./"]
COPY ["WareHaus.Api/WareHaus.Api.csproj", "WareHaus.Api/"]

# Restore menggunakan cache mount untuk mempercepat build di .NET 10
RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages \
    dotnet restore "WareHaus.Api/WareHaus.Api.csproj"

# Copy sisa file lainnya
COPY . .

# Build project
WORKDIR "/src/WareHaus.Api"
RUN dotnet build "WareHaus.Api.csproj" -c Release -o /app/build

# STAGE 2: Publish
FROM build AS publish
RUN dotnet publish "WareHaus.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# STAGE 3: Runtime .NET 10 (Chiseled/Distroless untuk keamanan ekstra)
# .NET 10 sangat menyarankan image 'chiseled' untuk produksi yang lebih aman dan kecil
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# .NET 10 tetap menggunakan port 8080 secara default untuk non-root user
EXPOSE 8080
ENTRYPOINT ["dotnet", "WareHaus.Api.dll"]