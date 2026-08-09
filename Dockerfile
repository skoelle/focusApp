# Stage 1: React Frontend bauen
FROM node:22-alpine AS frontend
WORKDIR /app/client
COPY client/package.json client/package-lock.json ./
RUN npm ci
COPY client/ ./
RUN npm run build

# Stage 2: .NET Publish
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY *.csproj *.sln ./
RUN dotnet restore
COPY . .
COPY --from=frontend /app/client/build ./client/build
RUN dotnet publish FocusApp.csproj -c Release -o /app/publish --no-restore -p:SkipFrontendBuild=true

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 5000
ENV ASPNETCORE_URLS=http://0.0.0.0:5000
ENV ASPNETCORE_ENVIRONMENT=Production
ENTRYPOINT ["dotnet", "FocusApp.dll"]
