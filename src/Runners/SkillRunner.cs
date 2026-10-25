using System.IO.Abstractions;

namespace PhotoCli.Runners;

public class SkillRunner : IConsoleRunner
{
	private readonly SkillOptions _options;
	private readonly IConsoleWriter _consoleWriter;
	private readonly IFileSystem _fileSystem;

	public SkillRunner(SkillOptions options, IFileSystem fileSystem, IConsoleWriter consoleWriter)
	{
		_options = options;
		_fileSystem = fileSystem;
		_consoleWriter = consoleWriter;
	}

	public async Task<ExitCode> Execute()
	{
		var assembly = typeof(SkillRunner).Assembly;
		var resourceNames = assembly.GetManifestResourceNames().Where(w => w.StartsWith(Constants.SkillsResourcePrefix)).ToList();
		if (resourceNames.Count == 0)
			throw new PhotoCliException("No bundled skill files found in the assembly");

		var claudeDirectory = ClaudeDirectory();
		if (claudeDirectory == null)
		{
			_consoleWriter.WriteError($"Can't find the user home directory. Set the {Constants.ClaudeConfigDirectoryEnvironmentKey} environment variable to the Claude folder to install into.");
			return ExitCode.UserHomeDirectoryNotFound;
		}

		var skillsDirectory = _fileSystem.Path.Combine(claudeDirectory, "skills");
		foreach (var resourceName in resourceNames)
		{
			// Resource names keep the folder structure under `Skills/`, e.g. `Skills/photocli/references/options.md`.
			var relativePathParts = resourceName[Constants.SkillsResourcePrefix.Length..].Split('/', '\\');
			var targetPath = _fileSystem.Path.Combine([skillsDirectory, ..relativePathParts]);
			_fileSystem.Directory.CreateDirectory(_fileSystem.Path.GetDirectoryName(targetPath)!);

			await using var resourceStream = assembly.GetManifestResourceStream(resourceName)!;
			await using var fileStream = _fileSystem.FileStream.New(targetPath, FileMode.Create);
			await resourceStream.CopyToAsync(fileStream);
		}

		var skillDirectory = _fileSystem.Path.Combine(skillsDirectory, Constants.SkillName);
		_consoleWriter.WriteSuccess($"Installed {Constants.SkillName} skill ({resourceNames.Count} files) into {skillDirectory}");
		_consoleWriter.WriteSuccess($"Ask your agent, for example: \"use {Constants.SkillName} to organize my photos in ~/Pictures/Trip by date\"");
		return ExitCode.Success;
	}

	private string? ClaudeDirectory()
	{
		if (!_options.Global)
			return _fileSystem.Path.Combine(_fileSystem.Directory.GetCurrentDirectory(), Constants.ClaudeDirectoryName);

		// Same resolution as Claude Code: CLAUDE_CONFIG_DIR overrides the default `~/.claude` on all platforms.
		var claudeConfigDirectory = Environment.GetEnvironmentVariable(Constants.ClaudeConfigDirectoryEnvironmentKey);
		if (claudeConfigDirectory.IsPresent())
			return claudeConfigDirectory;

		// UserProfile resolves to $HOME on macOS & Linux and %USERPROFILE% on Windows. Without DoNotVerify, a missing home folder returns an empty string
		// and the skill would silently land in a relative `.claude` folder.
		var userHomeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile, Environment.SpecialFolderOption.DoNotVerify);
		return userHomeDirectory.IsPresent() ? _fileSystem.Path.Combine(userHomeDirectory, Constants.ClaudeDirectoryName) : null;
	}
}
