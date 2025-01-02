Important note: This application is a command line tool which don't have any user interface. To use this application, basic knowledge of how to run and send arguments to CLI applications is a must.

This application can run on macOS, Windows & Linux for Arm64, x64, x86 architectures. Executable types varies depending on OS.

## Dependency
.NET SDK (8 or later) required for application to run and `dotnet` command should be available on your path variable.

You can either download from https://dotnet.microsoft.com/en-us/download or using the following Brew cask for macOS / Linux.
```
brew install dotnet-sdk --cask
```

# macOS

You can install by Homebrew (preferred) or as .NET tool.

#### Homebrew tap configuration & package installation
```shell
brew tap photo-cli/homebrew-photo-cli && brew install photo-cli
```

Ref: https://github.com/photo-cli/homebrew-photo-cli

# Windows && Linux

You can install as .NET tool (preferred) or standalone executable that can be found on assets.

# Install as .NET Tool

.NET tool can be installed as .NET tool (preferred), could be downloaded manually from https://www.nuget.org/packages/photo-cli/ or directly from assets.

#### .NET CLI
```shell
dotnet tool install photo-cli -g
```

# Accessing Application

Installing the application globally provides access to the `photo-cli` command in your terminal.
```
photo-cli [command]

photo-cli help [command]
```

## Command Not Found Issue Solution

For macOS and Linux You should add your `.dotnet/tools` (path may change for your installation choices) to your PATH environment variable.

For macOS - Z Shell add the following line to your `~/.zshenv` file.
```shell
export PATH="$PATH:/Users/[your-account-name]/.dotnet/tools"
```

For Linux Bash add the following line to your `~/.profile` file.
```shell
export PATH="$PATH:/home/[your-account-name]/.dotnet/tools"
```
