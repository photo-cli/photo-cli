# Contributing to `photo-cli`

We welcome your contributions! :heart:

## Spread The Word

If you like and use our project, you can share it with those around you.

## Bug Report

If you find any bug, please [create an issue on this repository](https://github.com/photo-cli/photo-cli/issues/new) to make it better for every user.

## Discuss

Use [repository discussion](https://github.com/photo-cli/photo-cli/discussions) for interaction with the community.

## Code Contribution

We are actively looking for contributors, if you like the tool and using it, let's improve it together.

## Development Flow

Development workflow is like the following;

### 0. Check Issues

Check existing [issues](https://github.com/photo-cli/photo-cli/issues) to look if there is a related issue exists. If not, create issue first.

If you are not certain about your request or bug, create a discussion first and let's discuss together on [discussions](https://github.com/photo-cli/photo-cli/discussions).

No big pull/merge requests. If your changes requires a lot of change, start a discussion or issue first, so we can agree on direction.

### 1. Deciding Version

We are using release branch flow and actively develop on related release branch depending its' semantic version like `r/0.0.1`, `r/0.2.7`.

Target version should be:
  - If it is a major change: `r/{{current-major+1}}.0.0`
  - If it is a minor change: `r/{{current-major}}.{{current-minor+1}}.0`
  - If it is a patch change: `r/{{current-major}}.{{current-minor}}.{{current-patch+1}}`

### 2. Is Version Still Developing?

Check whether your related change version still accepts issues on [our boards.](https://github.com/photo-cli/photo-cli/projects?type=classic)

Boards:
- [Active Versions](https://github.com/photo-cli/photo-cli/projects?type=classic&query=is%3Aopen)
- [Closed, Published Stable Version on Nuget](https://github.com/photo-cli/photo-cli/projects?type=classic&query=is%3Aclosed)

### 3. Fork & Develop on Release Branch

Create a fork on your local repository and work on your related version branch.

While developing consider the following items;

- Follow our [code style](#code-style-rules).
- For new features; write unit, integration, end-to-end tests.
- For fixing bugs; Should add a tests that highlights current behavior is broken.
- Keep your commits atomic and descriptive. It should revolve only one task.
- For commit messages, use our [commit message formats](#commit-message-formats).
- Not to large with unrelated things in same commit.
- Not so small changes applied on many commits.
- No merge commits in your branch, use rebase on personal branches.

### 4. Create a Pull/Merge Request

Your changes should be targeted only on your related version branch. As a community we iteratively discuss and give feedbacks about these changes in a positive manner.

After agreement about these changes and CI is not broken, we will merge your changes.

### 4. Publishing Preview Version

Tool deployment to Nuget, done via GitHub actions on tag push on release branch `r/{major}{minor}{patch}`.

After completing development on release branch, first we push preview version with this git tag. `v[0-9]+.[0-9]+.[0-9]+-preview[0-9]`

This preview version is for contributors and early adapters(who installs and love to use preview versions) to test.

### 5. Publishing Stable Version

If preview version is stable and passes our tests, we will push stable version via this tag. `v[0-9]+.[0-9]+.[0-9]+`

After pushing stable version on release branch `r/{major}{minor}{patch}` , we will merge this changes to `main` branch.

The `main` branch should matches with the current stable release on nuget.org.

### 6. Merging Concurrent Changes on Different Branches

If there is a changed on lower version emerged, these changes should be merged firstly into `main` and then active upper release branches.

### Code Style Rules

- Project is using [Editor Config](https://editorconfig.org/). Make sure that your IDE is using our [`.editorconfig`](.editorconfig).
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
