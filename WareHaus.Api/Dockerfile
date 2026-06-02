# Gunakan SDK .NET 10 untuk proses build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build-env
WORKDIR /app

# Copy project file secara eksplisit dan restore dependencies
# Menyebutkan nama proyek membuat Docker build lebih robust terhadap konteks build
COPY WareHaus.Api.csproj ./
RUN dotnet restore WareHaus.Api.csproj

# Copy sisa file dan publish aplikasi (tanpa restore lagi)
COPY . ./
RUN dotnet publish WareHaus.Api.csproj -c Release -o out --no-restore

# Gunakan runtime .NET 10 agar ukuran image akhir jauh lebih ringan
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build-env /app/out .

RUN apt-get update && apt-get install -y \
    libfontconfig1 \
    && rm -rf /var/lib/apt/lists/*

# Di .NET 10, port default internal container tetap menggunakan port 8080
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "WareHaus.Api.dll"]