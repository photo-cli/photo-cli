Important note: This application is a command line tool that doesn't have any user interface. To use this application, basic knowledge of how to run and pass arguments to CLI applications is required.

This application can run locally on macOS, Windows & Linux for Arm64, x64, and x86 architectures, and also in a container (Docker, Podman) environment. Executable types vary depending on the OS.

# Installation Types

- [1. Running as Self Contained Executable](#1-running-as-self-contained-executable)
- [2. Homebrew (macOS & Linux)](#2-homebrew-macos--linux)
- [3. Installing as .NET Tool](#3-installing-as-net-tool)
- [4. Running in Container (Docker, Podman)](#4-running-in-container-docker-podman)

## 1. Running as Self Contained Executable

Easiest way to run the application without installing any dependency (contains the .NET runtime also in a single file) [downloadable directly from releases page as assets](https://github.com/photo-cli/photo-cli/releases) differs by OS and architecture.

## 2. Homebrew (macOS & Linux)

Automated way to always use the updated version of this tool on macOS & Linux using [Homebrew](https://brew.sh/) package manager.

Homebrew installation command:
```shell
brew tap photo-cli/homebrew-photo-cli && brew install photo-cli
```

More details on [homebrew tap repository](https://github.com/photo-cli/homebrew-photo-cli) source code.

## 3. Installing as .NET Tool

### Dependency
.NET SDK (10 or later) required for application to run and `dotnet` command should be available on your path variable.

.NET tool installation command:
```shell
dotnet tool install photo-cli -g
```
Published on [Nuget](https://www.nuget.org/packages/photo-cli/)

## 4. Running in Container (Docker, Podman)

Using the published Docker [image on DockerHub](https://hub.docker.com/r/photocli/photocli), you can run the tool in your isolated environment by mounting your photographs and output directory as bind mounts on an ephemeral container (the container can be safely discarded after execution).

Here is the example command to accomplish this. The bind-mounted directories appear as empty directories in the container filesystem and must be given as the input and output directories to the application.

```shell
docker run --rm --volume ./test-photographs:/photos/input --volume ./archive:/photos/output photocli/photocli archive --input /photos/input --output /photos/output --album-type DateRange --album-name My-Album --auto-reverse-geocode-album --expected-day-range 7300 --delete-on-source --reverse-geocode OpenStreetMapFoundation --openstreetmap-properties country city
```

## Accessing Application

Installing the application globally provides access to the `photo-cli` command in your terminal.
```shell
photo-cli [command]

photo-cli help [command]
```

## Uninstallation

## For .NET Tool
```
dotnet tool uninstall -g photo-cli
```

## MCP Server Setup

The `mcp` command is built into `photo-cli` and requires no additional installation. It starts an [MCP](https://modelcontextprotocol.io/) stdio server on top of an existing archive folder (one previously created with `photo-cli archive`).

See the [MCP section in README](README.md#mcp-model-context-protocol-server) for setup instructions for Claude Code, Claude Desktop, VS Code, and MCP Inspector.

## Issues

### Command Not Found Issue Solution for .NET Tool Installations

For macOS and Linux You should add your `.dotnet/tools` (path may change for your installation choices) to your PATH environment variable.

For macOS - Z Shell add the following line to your `~/.zshenv` file.
```shell
export PATH="$PATH:/Users/[your-account-name]/.dotnet/tools"
```

For Linux Bash add the following line to your `~/.profile` file.
```shell
export PATH="$PATH:/home/[your-account-name]/.dotnet/tools"
```
