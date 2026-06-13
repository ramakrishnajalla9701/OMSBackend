# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the build context into the image
COPY . ./

# Support both repo-root and backend-folder build contexts
RUN if [ -f /src/Backend.csproj ]; then \
        echo "Using backend project from build context root"; \
    elif [ -f /src/Backend/Backend.csproj ]; then \
        cp -R /src/Backend/. /src/; \
    else \
        echo "Backend.csproj not found in build context"; exit 1; \
    fi

# Restore dependencies
RUN dotnet restore "Backend.csproj"

# Build the application
RUN dotnet build "Backend.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "Backend.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copy published application
COPY --from=publish /app/publish .

# Install sqlite3 for database support
RUN apt-get update && apt-get install -y sqlite3 && rm -rf /var/lib/apt/lists/*

# Create data directory for SQLite database
RUN mkdir -p /app/data

# Expose port used by Render and other container platforms
EXPOSE 10000

# Set environment variables
ENV ASPNETCORE_URLS=http://0.0.0.0:10000
ENV ASPNETCORE_ENVIRONMENT=Production
ENV PORT=10000

# Run the application
ENTRYPOINT ["dotnet", "Backend.dll"]
