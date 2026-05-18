# AGENTS.md

This file provides guidance to AI coding assistants working with code in this repository.

## Overview

photo-cli is a .NET 10 CLI tool that reads EXIF metadata from photos, reverse-geocodes GPS coordinates via Google Maps / OpenStreetMap / BigDataCloud, and organizes photos into structured folders with SQLite storage and CSV export. It also exposes an MCP server so LLMs can query archived photo metadata directly.

## Repository Layout

```
src/                  — main executable (PhotoCli.csproj)
  McpTools/           — MCP server surface wrapping runners for LLM access
  Models/             — EF Core entities, enums, CSV DTOs, ArchiveDbContext
  Options/            — strongly-typed CLI argument classes per subcommand
    Validators/       — FluentValidation rules for each Options class
  Runners/            — one IConsoleRunner per subcommand
  Services/
    Contracts/        — service interfaces
    Implementations/  — service implementations
  Utils/              — extensions, constants, path helpers, logging
  Program.cs          — entry point: CommandLine parser + DI setup
tests/
  UnitTests/          — fast, fully mocked tests
  IntegrationTests/   — tests against real SQLite / file system
  EndToEndTests/      — full CLI invocation tests
  Fakes/              — fake implementations shared across test projects
  Utils/              — MockFileSystemHelper, MockHttpClient, etc.
```

## Commands

```bash
# Build
dotnet build --configuration Release

# Run all tests
dotnet test --configuration Release --no-build

# Run a single test class
dotnet test --configuration Release --no-build --filter "FullyQualifiedName~ArchiveRunnerUnitTests"

# Run a single test method
dotnet test --configuration Release --no-build --filter "FullyQualifiedName~ArchiveRunnerUnitTests.MethodName"

# Run with coverage
dotnet test --configuration Release --no-build /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

## Architecture

**Request flow:** CLI arguments → `Program.cs` (CommandLine parser + DI setup) → Runner → Services → file system / SQLite / HTTP

**Key layers:**

- **Runners** (`src/Runners/`) — `ArchiveRunner`, `CopyRunner`, `InfoRunner`, `AddressRunner`, `ListRunner`, `SettingsRunner`. Each implements `IConsoleRunner`.
- **Options** (`src/Options/`) — strongly-typed argument classes per subcommand; FluentValidation validators in `Options/Validators/`.
- **Services** (`src/Services/`) — business logic behind interfaces. Key services: `IExifParserService` (MetadataExtractor library), `IReverseGeocodeFetcherService`, `IFileNamerService`, `IDbService` (EF Core + SQLite), `ICsvService`.
- **Models** (`src/Models/`) — EF Core entities (`PhotoEntity`, `AlbumEntity`), `ArchiveDbContext`, `ExifData`, CSV DTOs, and enums controlling naming strategy, address format, photo grouping, etc.
- **McpTools** (`src/McpTools/`) — thin wrappers around runners; each public method maps to one MCP tool exposed to LLMs.

## Code Style

Rules are enforced via `.editorconfig` at the repo root:

- **Indentation:** tabs (displayed as 4 spaces)
- **Line endings:** LF (Unix)
- **Max line length:** 200 characters
- **Naming:** PascalCase for types and public members; camelCase for parameters and locals
- **Nullability:** nullable reference types enabled — annotate and handle `null` explicitly
- **Braces:** always use braces for control-flow blocks, even single-line

## Testing

Three test categories — keep them separate:

| Category | What it tests | Mocking strategy |
|---|---|---|
| `UnitTests` | Single class in isolation | Moq for all dependencies |
| `IntegrationTests` | Multiple layers together | Real SQLite (in-memory or temp file); `MockFileSystem` for I/O |
| `EndToEndTests` | Full CLI invocation | Real file system, real SQLite |

**Utilities available in `tests/Utils/`:**
- `MockFileSystemHelper` — builds `MockFileSystem` instances with pre-populated photo files
- `MockHttpClient` — intercepts HTTP calls to reverse-geocode APIs
- `MockLoggerExtensions` — asserts on `ILogger` invocations
- `CsvFileHelper` — parses CSV output for assertions

Use **FluentAssertions** for all assertions. Use **Moq** for mocks. Test class names follow `<Subject>UnitTests` / `<Subject>IntegrationTests` / `<Subject>EndToEndTests`.

## Key Patterns

### Adding a new subcommand

1. Create `src/Options/FooOptions.cs` (inherit from base options if applicable)
2. Create `src/Options/Validators/FooOptionsValidator.cs` (FluentValidation `AbstractValidator<FooOptions>`)
3. Create `src/Runners/FooRunner.cs` implementing `IConsoleRunner`
4. Register options, validator, and runner in `Program.cs`
5. Add `tests/UnitTests/FooRunnerUnitTests.cs`

### Adding a new service

1. Define the interface in `src/Services/Contracts/IFooService.cs`
2. Implement in `src/Services/Implementations/FooService.cs`
3. Register in `Program.cs` DI container
4. Inject via constructor — never instantiate services directly

### Adding an MCP tool

1. Add a public method to `src/McpTools/ArchiveMcpTools.cs` (or a new `*McpTools.cs` class)
2. Decorate with the appropriate MCP tool attribute
3. Delegate to an existing runner or service — MCP tools are thin facades only
4. Add tests in `tests/UnitTests/ArchiveMcpToolsUnitTests.cs`

## Do Not

- **Do not use `System.IO` directly.** Always use `System.IO.Abstractions` (`IFileSystem`) so tests can use `MockFileSystem`.
- **Do not skip FluentValidation.** Every `Options` class must have a corresponding validator; validate before the runner executes.
- **Do not inject concrete types.** Register and inject via interfaces so tests can substitute fakes or mocks.
- **Do not add logic to MCP tool methods.** They are facades; put business logic in services or runners.
- **Do not hardcode paths or API keys.** Use `IOptions<T>`, `appsettings.json`, or environment variables.
