# Use .NET SDK for building the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy only .csproj files and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy the entire project and build
COPY . ./
RUN dotnet publish -c Release -o /out --no-restore

# Use ASP.NET runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /out ./

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080 \
    DOTNET_ENVIRONMENT=Production

# Expose port 8080 for Render
EXPOSE 8080

# Start the application
ENTRYPOINT ["dotnet", "Backend.dll"]
