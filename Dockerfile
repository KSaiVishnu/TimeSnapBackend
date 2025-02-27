# Use .NET SDK for building the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy and restore dependencies
COPY . ./
RUN dotnet restore

# Build the application
RUN dotnet publish -c Release -o /out

# Use ASP.NET runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /out ./

# Expose port 8080 for Render
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

# Start the application
CMD ["dotnet", "Backend.dll"]
