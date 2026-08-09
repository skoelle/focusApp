# Stage 1: React Frontend bauen
FROM node:20-alpine AS frontend
WORKDIR /app/client
COPY client/package.json client/package-lock.json ./
RUN npm ci
COPY client/ ./
RUN npm run build

# Stage 2: .NET Publish
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
RUN apt-get update && apt-get install -y curl && \
    curl -fsSL https://deb.nodesource.com/setup_20.x | bash - && \
    apt-get install -y nodejs && \
    rm -rf /var/lib/apt/lists/*
WORKDIR /src
COPY *.csproj *.sln ./
RUN dotnet restore
COPY . .
COPY --from=frontend /app/client/build ./client/build
RUN dotnet publish FocusApp.csproj -c Release -o /app/publish --no-restore

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 5000
ENV ASPNETCORE_URLS=http://0.0.0.0:5000
ENV ASPNETCORE_ENVIRONMENT=Production
ENTRYPOINT ["dotnet", "FocusApp.dll"]
