# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**photo-cli** is a .NET 10 console application for organizing photos based on EXIF metadata (date/time, GPS coordinates) and reverse geocoding. It extracts photo metadata, organizes files with intelligent naming strategies, and provides CSV export capabilities for mapping/analysis.

## Development Commands

### Core Commands
```bash
# Build the application
dotnet build

# Run tests
dotnet test

# Build and run locally
dotnet run --project src/PhotoCli.csproj

# Package as global tool
dotnet pack
dotnet tool install --global --add-source ./local-packages photo-cli

# Remove global tool
dotnet tool uninstall photo-cli -g
```

### Prerequisites
- .NET SDK 8.0+ required
- Project targets `net8.0` framework

## Architecture Overview

### CLI Commands (Verbs)
- `copy` - Organize photos into new folder structure with intelligent naming
- `archive` - Index photos into SQLite database incrementally
- `info` - Export photo metadata to CSV reports
- `address` - Query reverse geocoding for coordinates
- `settings` - Manage application configuration

### Core Architecture Patterns
- **Dependency Injection** - Microsoft.Extensions.DependencyInjection throughout
- **Command Pattern** - Each CLI verb has dedicated Runner classes in `/src/Runners/`
- **Service Layer** - Business logic in `/src/Services/` with Contracts/Implementations
- **Entity Framework Core** - SQLite database with code-first migrations
- **Options Pattern** - Strongly-typed CLI options with FluentValidation
- **HTTP Resilience** - Retry and timeout policies for reverse geocoding APIs

### Key Services
- `IExifParserService` - Extract EXIF metadata from photo files using MetadataExtractor
- `IReverseGeocodeService` - Coordinate-to-address conversion (BigDataCloud, OpenStreetMap, Google Maps, LocationIQ)
- `IFileService` - File operations with System.IO.Abstractions for testability
- `IPhotoCollectorService` - Discover and collect photo files from directories
- `IDbService` - Database operations for archive functionality
- `IReverseGeocodeCache` - Memory and database caching for API responses

### Database Schema
- `PhotoEntity` - Core photo metadata with EXIF data and reverse geocoding
- `AlbumEntity` - Photo organization/grouping with configuration JSON
- `AlbumHistoryEntity` - Album change tracking
- `ReverseGeocodeCacheEntity` - API response caching with provider/precision keys

### Project Structure
```
src/
├── Models/           # Domain models, EF entities, migrations
├── Options/          # CLI options, validators, configuration classes
├── Runners/          # Command execution classes (CopyRunner, ArchiveRunner, etc.)
├── Services/         # Business logic with interface/implementation separation
└── Utils/            # Extensions, constants, helpers

tests/
├── UnitTests/        # Fast isolated tests with mocked dependencies
├── IntegrationTests/ # Real database/HTTP testing
├── EndToEndTests/    # Full CLI scenario testing
└── Fakes/            # Test data builders and mock objects
```

## Testing Strategy

### Test Categories
- **Unit Tests** - Isolated with System.IO.Abstractions.TestingHelpers for file mocking
- **Integration Tests** - Real SQLite databases and HTTP clients
- **End-to-End Tests** - Full CLI execution with test photo collections

### Test Data
- `/tests/TestImages/` - EXIF test photos with known metadata
- `/docs/test-photographs/` - Real-world photo samples
- Fakes pattern for complex object creation in `/tests/Fakes/`

### Launch Configurations
`/src/Properties/launchSettings.json` contains extensive development scenarios for rapid testing of CLI commands.

## Key Dependencies

### Core Framework
- **CommandLineParser** - CLI argument parsing
- **Entity Framework Core** - SQLite ORM with migrations
- **FluentValidation** - Input validation pipeline
- **MetadataExtractor** - EXIF metadata extraction

### HTTP & Resilience
- **Microsoft.Extensions.Http.Resilience** - Retry/timeout policies
- **System.IO.Abstractions** - Testable file system operations

### UI & Output
- **Spectre.Console** - Rich console output and progress bars
- **CsvHelper** - CSV export functionality

## Reverse Geocoding Architecture

Supports multiple providers with intelligent caching:
- Provider-specific response models in `/src/Models/ReverseGeocode/`
- Two-tier caching: memory + SQLite database
- Precision-based coordinate rounding for cache efficiency
- HTTP resilience with retry/timeout for external APIs

## Configuration Management

- **appsettings.json** - Default application settings
- **Command Line Options** - Strongly-typed with validation
- **Environment Variables** - API key overrides (e.g., `GOOGLE_MAPS_API_KEY`)
- **Global Tool Packaging** - Distributed via NuGet as dotnet tool

## Development Notes

### Current Known Issues
⚠️ Build may have compilation errors in unit tests related to `StatusContext` parameter mismatches in Runner.Execute() method signatures.

### Testing Workflow
1. Use launch profiles in launchSettings.json for rapid iteration
2. Test with real photos in `/docs/test-photographs/`
3. Verify reverse geocoding with multiple providers
4. Run a full test suite before commits
5. Test names should follow the AAA (Arrange-Act-Assert) naming pattern specifically using the format: MethodUnderTest_Scenario_ExpectedResult .

### Extension Points
- New reverse geocoding providers: implement service contracts in `/src/Services/Contracts/ReverseGeocodes/`
- Custom naming strategies: extend `/src/Services/Implementations/FileNamerService.cs`
- Additional CLI verbs: create new Runner classes following existing patterns
