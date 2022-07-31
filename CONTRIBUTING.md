# Contributing to `photo-cli`

We welcome your contributions! :heart:

## Spread The Word

If you like and use our project, you can share it with those around you.

## Bug Report

If you find any bug, please [create an issue on the this repository](https://github.com/photo-cli/photo-cli/issues/new) to make it better for every user.

## Discuss

Use [repository discussion](https://github.com/photo-cli/photo-cli/discussions) for interaction with the community.

## Code Contribution

We are using git flow and actively develop on `dev` branch. All pull requests should submitted to the dev branch. The `main` branch matches with the current release on nuget.org and releases are also tagged.

### Code Style Rules

- Project is using [Editor Config`](https://editorconfig.org/) . Make sure that your IDE is using our [`.editorconfig`](.editorconfig).
- Code base follows usual .NET code conventions documented in [Framework Design Guidelines](https://docs.microsoft.com/en-us/dotnet/standard/design-guidelines/).
- Should match with with current style of the project.

### Commit Message Formats

- Project is using [Conventional Commits](https://www.conventionalcommits.org/en/v1.0.0/).

#### Commit Structure

```
<type>[optional scope][!, if it is breaking change]: <description>

[BREAKING CHANGE:]

[optional body]

[optional footer(s)]
```

#### Type to be Used

- build
- chore
- ci
- docs
- feat
- fix
- idle
- perf
- refactor
- revert
- style
- test

#### Character Limits

First line only as summary with a 50 chars or less.

Optional body and footers to be wrap about 72 characters.

### Submitting a Pull requests (PR)

- Use feature/hotfix branch.
- Open a issue first.
- Follow our [code style](#code-style-rules).
- For new features; write unit, integration, end-to-end tests.
- For fixing bugs; Should add a tests that highlights current behavior is broken.
- No big PRs. Start a discussion with an issue first, so we can agree on direction.
- Keep your commits atomic and descriptive. It should revolve only one task.
- Not to large with unrelated things in same commit.
- Not so small changes applied on many commits.
- No merge commits in your branch, use rebase personally.

### Local Building

Run these commands on repository root. The local package will be created on the `local-packages` folder.

- Build NuGet package

```
dotnet pack --configuration Release --output local-packages /p:Version=[version]
```

- Installing a local package

```
dotnet tool install photo-cli -g --add-source local-packages --no-cache --version [version]
```

- Tool should be executable with this command on the terminal.

```
photo-cli
```

- Updating a local package

```
dotnet tool update photo-cli -g --add-source local-packages --no-cache --version [version]
```

- Uninstalling a package

```
dotnet tool uninstall -g photo-cli
```

## Code of Conduct

Finally when contributing please keep in mind [Code of Conduct](CODE_OF_CONDUCT.md).
