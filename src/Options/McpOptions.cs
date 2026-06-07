using CommandLine;

namespace PhotoCli.Options;

[Verb(OptionNames.McpVerb, HelpText = HelpTexts.McpVerbHelpText)]
public class McpOptions
{
	// Notes: Constructor parameters and properties should be in the same order for Immutable Options Type in CommandLineParser.
	// ref: https://github.com/commandlineparser/commandline/wiki/Immutable-Options-Type
	public McpOptions(string? archivePath = null, string? customDatabasePath = null)
	{
		ArchivePath = archivePath;
		CustomDatabasePath = customDatabasePath;
	}

	[Option(OptionNames.ArchivePathOptionNameShort, OptionNames.ArchivePathOptionNameLong, HelpText = HelpTexts.McpArchivePath)]
	public string? ArchivePath { get; }

	[Option(OptionNames.CustomDatabasePathShort, OptionNames.CustomDatabasePathLong, HelpText = HelpTexts.CustomDatabasePath)]
	public string? CustomDatabasePath { get; }
}
