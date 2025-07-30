# Stage 1: Build & publish .NET + bundle JS
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy everything (including JS)
COPY . ./

# Install Node.js & npm (required for esbuild)
RUN apt-get update && apt-get install -y curl gnupg && \
    curl -fsSL https://deb.nodesource.com/setup_20.x | bash - && \
    apt-get install -y nodejs && \
    npm install && \
    npx esbuild wwwroot/js/main.js --bundle --format=esm --outfile=wwwroot/js/app-bridge-bundle.js

# Restore and publish .NET
RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "StockNotificationWarning.dll"]