using CommandLine;
using Spectre.Console;

namespace PhotoCli.Utils;

public static class HelpTextBuilder
{
	public static void ExampleUsages(string helpVerb, IAnsiConsole ansiConsole)
	{
		var ansiConsoleExtended = new AnsiConsoleExtended(ansiConsole);
		ansiConsoleExtended.WriteLine("NOTES:");
		ansiConsoleExtended.WriteLine("- Instead of option names (for ex: DateTimeWithMinutes), you may use options values too. (for ex: 3)");
		ansiConsoleExtended.WriteLine("- You can use relative folder paths. If you use the input folder as the working directory, you don't need to use the input argument.");
		ansiConsoleExtended.EmptyLine();
		ansiConsoleExtended.WriteLine("EXAMPLE USAGES:");

		var inputFolder = "(input-folder)";
		var outputFolder = "(output-folder)";

		switch (helpVerb)
		{
			case OptionNames.SettingsVerb:
				WriteOptionArgumentsToConsole(ansiConsoleExtended, new SettingsOptions(), "List all settings");
				WriteOptionArgumentsToConsole(ansiConsoleExtended, new SettingsOptions("YearFormat"), "Get a setting");
				WriteOptionArgumentsToConsole(ansiConsoleExtended, new SettingsOptions("YearFormat", "yyyy"), "Save a setting");
				WriteOptionArgumentsToConsole(ansiConsoleExtended, new SettingsOptions(reset: true), "Reset all settings");
				break;
			case OptionNames.AddressVerb:
				var photoPathJpg = "(photo-path).jpg";
				WriteOptionArgumentsToConsole(ansiConsoleExtended, new AddressOptions(photoPathJpg, ReverseGeocodeProvider.OpenStreetMapFoundation, AddressListType.AllAvailableProperties), "All properties");

				WriteOptionArgumentsToConsole(ansiConsoleExtended,
					new AddressOptions(photoPathJpg, ReverseGeocodeProvider.OpenStreetMapFoundation, AddressListType.SelectedProperties,
						openStreetMapProperties: ["country", "city", "town", "suburb"]), "Selected properties");

				WriteOptionArgumentsToConsole(ansiConsoleExtended, new AddressOptions(photoPathJpg, ReverseGeocodeProvider.OpenStreetMapFoundation, AddressListType.FullResponse), "Show full response");
				break;
			case OptionNames.InfoVerb:
				var outputFileCsv = "(output-file).csv";
				WriteOptionArgumentsToConsole(ansiConsoleExtended,
					new InfoOptions(outputFileCsv, inputFolder, true, InfoInvalidFormatAction.Continue, InfoNoPhotoTakenDateAction.Continue, InfoNoCoordinateAction.Continue,
						ReverseGeocodeProvider.OpenStreetMapFoundation, openStreetMapProperties: ["country", "city", "town", "suburb"]),
					"Photos located on all subfolders will be processed and their photograph's taken date and address information will be saved on CSV file using BigDataCloud reverse geocode provider.");

				WriteOptionArgumentsToConsole(ansiConsoleExtended,
					new InfoOptions(outputFileCsv, inputFolder, false, InfoInvalidFormatAction.Continue, InfoNoPhotoTakenDateAction.PreventProcess, InfoNoCoordinateAction.PreventProcess,
						ReverseGeocodeProvider.GoogleMaps, googleMapsAddressTypes: ["administrative_area_level_1", "administrative_area_level_2"], googleMapsApiKey: "google-api-key"),
					"Using Google Maps reverse geocode provider (need api key) with an option to prevent processing if there is no coordinate or no photo taken date found on any photo.");
				break;
			case OptionNames.CopyVerb:
				WriteOptionArgumentsToConsole(ansiConsoleExtended,
					new CopyOptions(outputFolder, NamingStyle.Numeric, FolderProcessType.SubFoldersPreserveFolderHierarchy, NumberNamingTextStyle.PaddingZeroCharacter,
						CopyInvalidFormatAction.Continue, CopyNoPhotoTakenDateAction.Continue, CopyNoCoordinateAction.Continue, inputFolder),
					"Preserve same folder hierarchy, copy photos with sequential number ordering by photo taken date.");

				WriteOptionArgumentsToConsole(ansiConsoleExtended,
					new CopyOptions(outputFolder, NamingStyle.DateTimeWithSeconds, FolderProcessType.FlattenAllSubFolders, NumberNamingTextStyle.OnlySequentialNumbers,
						CopyInvalidFormatAction.Continue, CopyNoPhotoTakenDateAction.Continue, CopyNoCoordinateAction.Continue, inputFolder,
						groupByFolderType: GroupByFolderType.YearMonthDay),
					"Groups photos by photo taken year, month, day than copy on (year)/(month)/(day) directory with a file name as photo taken date.");

				WriteOptionArgumentsToConsole(ansiConsoleExtended,
					new CopyOptions(outputFolder, NamingStyle.AddressDay, FolderProcessType.SubFoldersPreserveFolderHierarchy, NumberNamingTextStyle.AllNamesAreSameLength,
						CopyInvalidFormatAction.PreventProcess, CopyNoPhotoTakenDateAction.InSubFolder, CopyNoCoordinateAction.InSubFolder, inputFolder, folderAppendType: FolderAppendType.DayRange,
						folderAppendLocationType: FolderAppendLocationType.Prefix, reverseGeoCodeProvider: ReverseGeocodeProvider.GoogleMaps,
						googleMapsAddressTypes: ["administrative_area_level_1", "administrative_area_level_2", "administrative_area_level_3"], googleMapsApiKey: "google-api-key"),
					"Adding day range as a prefix to existing folder names and photos copied with a file name as address and day.");

				WriteOptionArgumentsToConsole(ansiConsoleExtended,
					new CopyOptions(outputFolder, NamingStyle.AddressDateTimeWithSeconds, FolderProcessType.SubFoldersPreserveFolderHierarchy, NumberNamingTextStyle.AllNamesAreSameLength,
						CopyInvalidFormatAction.PreventProcess, CopyNoPhotoTakenDateAction.InSubFolder, CopyNoCoordinateAction.InSubFolder, inputFolder, reverseGeoCodeProvider: ReverseGeocodeProvider.OpenStreetMapFoundation,
						openStreetMapProperties: ["country", "city", "town", "suburb"]),
					"Preserve the same folder hierarchy while copying photos with filenames consisting of the photo-taken date, time, and address (with possible number suffixes), " +
					"and copy photos without coordinates or photo-taken dates into a relative subfolder.");

				WriteOptionArgumentsToConsole(ansiConsoleExtended,
					new CopyOptions(outputFolder, NamingStyle.DayAddress, FolderProcessType.FlattenAllSubFolders, NumberNamingTextStyle.OnlySequentialNumbers,
						CopyInvalidFormatAction.PreventProcess, CopyNoPhotoTakenDateAction.Continue, CopyNoCoordinateAction.InSubFolder, inputFolder, groupByFolderType: GroupByFolderType.AddressHierarchy,
						reverseGeoCodeProvider: ReverseGeocodeProvider.BigDataCloud, bigDataCloudAdminLevels: [2, 4, 6, 8]),
					"Groups photos by photo-taken year, month, and day, then copies them into a year/month/day directory structure with filenames as the photo-taken date, " +
					"while photos without coordinates are copied into a relative subfolder.");

				break;
			case OptionNames.ArchiveVerb:
				WriteOptionArgumentsToConsole(ansiConsoleExtended, new ArchiveOptions(outputFolder),
					"Archive all photos in current folder (and it's subfolders recursively) into output folder by (year)/(month)/(day) hierarchy with a file name photo taken date with seconds " +
					"prefixed by file hash. Saves all photo taken information into local SQLite database.");

				WriteOptionArgumentsToConsole(ansiConsoleExtended, new ArchiveOptions(outputFolder, inputFolder, false,
						ArchiveInvalidFormatAction.PreventProcess, ArchiveNoPhotoTakenDateAction.PreventProcess, ArchiveNoCoordinateAction.PreventProcess, null,
						ArchiveAlbumType.NoAlbumLinking, null, null, false, false, ReverseGeocodeProvider.OpenStreetMapFoundation, openStreetMapProperties: ["country", "city", "town", "suburb"]),
					"Archive all photos in the input folder and its subfolders recursively by fetching each photo's reverse geocode information, copying them into the output folder organized by " +
					"year/month/day hierarchy with filenames consisting of the photo-taken date with seconds prefixed by file hash, and saving all photo metadata and addresses into a local SQLite database.");

				WriteOptionArgumentsToConsole(ansiConsoleExtended, new ArchiveOptions(outputFolder, inputFolder, false,
						ArchiveInvalidFormatAction.PreventProcess, ArchiveNoPhotoTakenDateAction.PreventProcess, ArchiveNoCoordinateAction.PreventProcess, 30,
						ArchiveAlbumType.DateRange,"album 1", null, true, true, ReverseGeocodeProvider.GoogleMaps, googleMapsAddressTypes: ["administrative_area_level_1", "administrative_area_level_2"],
						missingReverseGeocodeAction: MissingReverseGeocodeAction.PreventProcess),
					"Archive all photos in the input folder (and its subfolders recursively) by creating a date range album named 'album 1', fetching each photo's reverse geocode information " +
					"using Google Maps with properties administrative area level 1 and 2 to create location-based albums, and copying them into the output folder with a (year)/(month)/(day) hierarchy " +
					"using filenames formatted as photo-taken-date-with-seconds prefixed by file hash but the process won't start if any photo has an invalid format, missing photo-taken date or " +
					"coordinates, or if the photo-taken date span exceeds 30 days and upon successful operation, it would delete the input source folder.");

				break;
			case OptionNames.ListVerb:
				WriteOptionArgumentsToConsole(ansiConsoleExtended, new ListOptions(ListType.Summary, inputFolder), "List statistics of the archive folder");
				WriteOptionArgumentsToConsole(ansiConsoleExtended, new ListOptions(ListType.Albums, inputFolder), "List all the album information of the archive folder");

				WriteOptionArgumentsToConsole(ansiConsoleExtended, new ListOptions(ListType.PhotosByAlbumId, inputFolder, 1),
					"List paths (to be send as process arguments to photo viewers) or " +
					"open (only supporting in macOS , Preview app for now) for the given album id");

				WriteOptionArgumentsToConsole(ansiConsoleExtended, new ListOptions(ListType.PhotosByExactDate, inputFolder, year: 2007),
					"List paths (to be send as process arguments to photo viewers) or " +
					"open (only supporting in macOS , Preview app for now) for the given year");

				WriteOptionArgumentsToConsole(ansiConsoleExtended, new ListOptions(ListType.PhotosByExactDate, inputFolder, year: 2007, month: 8),
					"List paths (to be send as process arguments to photo viewers) or " +
					"open (only supporting in macOS , Preview app for now) for the given year & month");

				WriteOptionArgumentsToConsole(ansiConsoleExtended, new ListOptions(ListType.PhotosByExactDate, inputFolder, year: 2007, month: 8, day: 19),
					"List paths (to be send as process arguments to photo viewers) or " +
					"open (only supporting in macOS , Preview app for now) for the given year, month & day");

				WriteOptionArgumentsToConsole(ansiConsoleExtended,
					"list --type PhotosByDateRange --start-date 2025-09-21 --end-date 2026-01-30 --input (input-folder)",
					"list -t PhotosByDateRange -s 2025-09-21 -e 2026-01-30 -input (input-folder)",
					"List paths (to be send as process arguments to photo viewers) or " +
					"open (only supporting in macOS , Preview app for now) for photos taken within the given date range");

				break;
		}
	}

	private static void WriteOptionArgumentsToConsole<TOptions>(AnsiConsoleExtended ansiConsoleExtended, TOptions options, string description)
	{
		WriteOptionArgumentsToConsole(ansiConsoleExtended,
			Parser.Default.FormatCommandLine(options),
			Parser.Default.FormatCommandLine(options, settings => settings.PreferShortName = true),
			description);
	}

	private static void WriteOptionArgumentsToConsole(AnsiConsoleExtended ansiConsoleExtended, string commandWithLongArguments, string commandWithShortArguments, string description)
	{
		ansiConsoleExtended.WriteLine($"- {description}");
		ansiConsoleExtended.EmptyLine();
		ansiConsoleExtended.WriteLine("Example with long argument names;");
		ansiConsoleExtended.WriteLine($"{OptionNames.ApplicationAlias} {commandWithLongArguments}");
		ansiConsoleExtended.EmptyLine();
		ansiConsoleExtended.WriteLine("Example with short argument names;");
		ansiConsoleExtended.WriteLine($"{OptionNames.ApplicationAlias} {commandWithShortArguments}");
		ansiConsoleExtended.EmptyLine();
	}

	public static void ExtendedHelpWritingToConsole(IAnsiConsole ansiConsole)
	{
		var ansiConsoleExtended = new AnsiConsoleExtended(ansiConsole);
		var verbs = new[] { OptionNames.CopyVerb, OptionNames.InfoVerb, OptionNames.AddressVerb, OptionNames.SettingsVerb, OptionNames.McpVerb };
		ansiConsoleExtended.WriteLine($"Type `{OptionNames.ApplicationAlias} help ({string.Join('|', verbs)})` for detailed option list and example usages");
	}
}
