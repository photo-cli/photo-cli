using CommandLine;

namespace PhotoCli.Options;

[Verb(OptionNames.ListVerb, HelpText = "List & open photos from archive folders.")]
public class ListOptions
{
	// Notes: Constructor parameters and properties should be in the same order for Immutable Options Type in CommandLineParser.
	// ref: https://github.com/commandlineparser/commandline/wiki/Immutable-Options-Type
	public ListOptions(
		// Required
		ListType listType,
		// Optional
		string? archivePath, int? albumId = null, int? year = null, byte? month = null, byte? day = null, bool rawOutput = false)
	{
		// Required
		ListType = listType;

		// Optional
		ArchivePath = archivePath ?? Environment.CurrentDirectory;
		AlbumId = albumId;
		Year = year;
		Month = month;
		Day = day;
		RawOutput = rawOutput;
	}

	#region Required

	[Option(OptionNames.ListTypeShort, OptionNames.ListTypeLong, HelpText = HelpTexts.ListType)]
	public ListType ListType { get; }

	#endregion

	#region Optional

	[Option(OptionNames.ArchivePathOptionNameShort, OptionNames.ArchivePathOptionNameLong, HelpText = HelpTexts.ArchivePath)]
	public string ArchivePath { get; }

	[Option(OptionNames.AlbumIdShort, OptionNames.AlbumIdLong, HelpText = HelpTexts.AlbumId)]
	public int? AlbumId { get; }

	[Option(OptionNames.YearShort, OptionNames.YearLong, HelpText = HelpTexts.Year)]
	public int? Year { get; }

	[Option(OptionNames.MonthShort, OptionNames.MonthLong, HelpText = HelpTexts.Month)]
	public byte? Month { get; }

	[Option(OptionNames.DayShort, OptionNames.DayLong, HelpText = HelpTexts.Day)]
	public byte? Day { get; }

	[Option(OptionNames.RawOutputShort, OptionNames.RawOutputLong, HelpText = HelpTexts.RawOutput)]
	public bool RawOutput { get; }

	#endregion
}
