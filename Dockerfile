# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the current directory contents into the image
COPY . ./

# Build the backend app from the copied backend folder
RUN if [ -f /src/Backend.csproj ]; then \
      dotnet restore "Backend.csproj" && dotnet build "Backend.csproj" -c Release -o /app/build; \
    elif [ -f /src/Backend/Backend.csproj ]; then \
      cd /src/Backend && dotnet restore "Backend.csproj" && dotnet build "Backend.csproj" -c Release -o /app/build; \
    else \
      echo "Backend project not found" && exit 1; \
    fi

# Publish stage
FROM build AS publish
RUN if [ -f /src/Backend.csproj ]; then \
      dotnet publish "/src/Backend.csproj" -c Release -o /app/publish; \
    elif [ -f /src/Backend/Backend.csproj ]; then \
      dotnet publish "/src/Backend/Backend.csproj" -c Release -o /app/publish; \
    else \
      echo "Backend project not found" && exit 1; \
    fi

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
