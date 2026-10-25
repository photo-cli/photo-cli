using CommandLine;

namespace PhotoCli.Options;

[Verb(OptionNames.SkillVerb, HelpText = HelpTexts.SkillVerbHelpText)]
public class SkillOptions
{
	// Notes: Constructor parameters and properties should be in the same order for Immutable Options Type in CommandLineParser.
	// ref: https://github.com/commandlineparser/commandline/wiki/Immutable-Options-Type
	public SkillOptions(bool global = false)
	{
		Global = global;
	}

	[Option(OptionNames.GlobalOptionNameShort, OptionNames.GlobalOptionNameLong, HelpText = HelpTexts.Global)]
	public bool Global { get; }
}
