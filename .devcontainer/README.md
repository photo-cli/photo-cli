# PhotoCli DevContainer

This devcontainer provides a complete development environment for PhotoCli with .NET 10.0 SDK.

## Features

- .NET 10.0 SDK pre-installed
- C# and .NET development extensions
- Git integration
- Non-root user (vscode) for better security
- Auto-restore of NuGet packages on container creation

## Getting Started

### Prerequisites

- [Visual Studio Code](https://code.visualstudio.com/)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- [Dev Containers extension](https://marketplace.visualstudio.com/items?itemName=ms-vscode-remote.remote-containers)

### Opening in DevContainer

1. Open this project in VS Code
2. Press `F1` or `Cmd/Ctrl+Shift+P`
3. Select "Dev Containers: Reopen in Container"
4. Wait for the container to build (first time will take a few minutes)

Alternatively, VS Code should prompt you to "Reopen in Container" when you open the project.

## Working in the Container

Once the container is running, you can:

- Build the project: `dotnet build`
- Run tests: `dotnet test`
- Run the CLI: `dotnet run --project src/PhotoCli.csproj -- [arguments]`
- Debug using F5 (uses the provided launch.json configuration)

## Customization

You can customize the devcontainer by editing:

- `.devcontainer/devcontainer.json` - VS Code settings and extensions
- `.devcontainer/Dockerfile` - Additional tools and packages

## Troubleshooting

If you encounter issues:

1. Rebuild the container: `Dev Containers: Rebuild Container`
2. Check Docker Desktop is running
3. Ensure you have enough disk space for the .NET SDK image (~1GB+)
