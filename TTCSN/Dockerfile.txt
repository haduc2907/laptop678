# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# Copy csproj và restore dependencies
COPY *.csproj .
RUN dotnet restore

# Copy toàn bộ code và build
COPY . .
RUN dotnet publish -c Release -o /app

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .

# Expose port
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

# Start app
ENTRYPOINT ["dotnet", "TTCSN.dll"]