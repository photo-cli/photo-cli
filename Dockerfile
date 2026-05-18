# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project files and restore
COPY src/PhotoCli.csproj src/
RUN dotnet restore src/PhotoCli.csproj

# Copy everything and build
COPY . .
RUN dotnet publish src/PhotoCli.csproj -c Release -o /app/publish
RUN dotnet publish src/PhotoCli.csproj -c Debug -o /app/debug

# Runtime stage
FROM mcr.microsoft.com/dotnet/runtime:10.0 AS runtime
WORKDIR /app

# Copy published app
COPY --from=build /app/publish .

# Create directories for mounting photos
RUN mkdir -p /photos/input /photos/output

ENTRYPOINT ["dotnet", "PhotoCli.dll"]

# Debug stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS debug
WORKDIR /app

# Install vsdbg (Visual Studio debugger)
RUN apt-get update && apt-get install -y wget unzip \
    && wget https://aka.ms/getvsdbgsh -O /tmp/getvsdbg.sh \
    && chmod +x /tmp/getvsdbg.sh \
    && /tmp/getvsdbg.sh -v latest -l /vsdbg \
    && rm /tmp/getvsdbg.sh \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/*

# Copy debug build
COPY --from=build /app/debug .

# Create directories for mounting photos
RUN mkdir -p /photos/input /photos/output

# Expose debugger port
EXPOSE 5000

# Keep container running for debugger attachment (don't start the app)
ENTRYPOINT ["tail", "-f", "/dev/null"]