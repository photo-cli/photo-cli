using CommandLine;

namespace PhotoCli.Options;

[Verb(OptionNames.SettingsVerb, HelpText = "Lists, saves and get settings.")]
public class SettingsOptions
{
	// Notes: Constructor parameters and properties should be in same order for Immutable Options Type in CommandLineParser.
	// ref: https://github.com/commandlineparser/commandline/wiki/Immutable-Options-Type
	public SettingsOptions(string? key = null, string? value = null, bool reset = false)
	{
		Key = key;
		Value = value;
		Reset = reset;
	}

	[Option(OptionNames.KeyOptionNameShort, OptionNames.KeyOptionNameLong, HelpText = HelpTexts.Key)]
	public string? Key { get; }

	[Option(OptionNames.ValueOptionNameShort, OptionNames.ValueOptionNameLong, HelpText = HelpTexts.Value)]
	public string? Value { get; }

	[Option(OptionNames.ResetOptionNameShort, OptionNames.ResetOptionNameLong, HelpText = HelpTexts.Reset)]
	public bool Reset { get; }
}
