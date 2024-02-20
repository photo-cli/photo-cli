using CommandLine;

namespace PhotoCli.Utils;

public static class HelpTextBuilder
{
	public static void ExampleUsages(string helpVerb, TextWriter textWriter)
	{
		textWriter.WriteLine("NOTES:");
		textWriter.WriteLine("- Instead of option names (for ex: DateTimeWithMinutes), you may use options values too. (for ex: 3)");
		textWriter.WriteLine("- You can use relative folder paths. If you use the input folder as the working directory, you don't need to use the input argument.");
		textWriter.WriteLine();
		textWriter.WriteLine("EXAMPLE USAGES:");

		var inputFolder = "[input-folder]";
		var outputFolder = "[output-folder]";

		switch (helpVerb)
		{
			case OptionNames.SettingsVerb:
				WriteOptionArgumentsToConsole(new SettingsOptions(), "List all settings", textWriter);
				WriteOptionArgumentsToConsole(new SettingsOptions("YearFormat"), "Get a setting", textWriter);
				WriteOptionArgumentsToConsole(new SettingsOptions("YearFormat", "yyyy"), "Save a setting", textWriter);
				WriteOptionArgumentsToConsole(new SettingsOptions(reset: true), "Reset all settings", textWriter);
				break;
			case OptionNames.AddressVerb:
				var photoPathJpg = "[photo-path].jpg";
				WriteOptionArgumentsToConsole(new AddressOptions(photoPathJpg, ReverseGeocodeProvider.OpenStreetMapFoundation, AddressListType.AllAvailableProperties), "All properties",
					textWriter);
				WriteOptionArgumentsToConsole(
					new AddressOptions(photoPathJpg, ReverseGeocodeProvider.OpenStreetMapFoundation, AddressListType.SelectedProperties,
						openStreetMapProperties: new[] { "country", "city", "town", "suburb" }),
					"Selected properties", textWriter);
				WriteOptionArgumentsToConsole(new AddressOptions(photoPathJpg, ReverseGeocodeProvider.OpenStreetMapFoundation, AddressListType.FullResponse), "Show full response", textWriter);
				break;
			case OptionNames.InfoVerb:
				var outputFileCsv = "[output-file].csv";
				WriteOptionArgumentsToConsole(
					new InfoOptions(outputFileCsv, inputFolder, true, InfoInvalidFormatAction.Continue, InfoNoPhotoTakenDateAction.Continue, InfoNoCoordinateAction.Continue, ReverseGeocodeProvider.OpenStreetMapFoundation,
						openStreetMapProperties: new[] { "country", "city", "town", "suburb" }),
					"Photos located on all subfolders will be processed and their photograph's taken date and address information will be saved on CSV file using BigDataCloud reverse geocode provider.",
					textWriter);

				WriteOptionArgumentsToConsole(
					new InfoOptions(outputFileCsv, inputFolder, false, InfoInvalidFormatAction.Continue, InfoNoPhotoTakenDateAction.PreventProcess, InfoNoCoordinateAction.PreventProcess, ReverseGeocodeProvider.GoogleMaps, googleMapsAddressTypes: new[] { "administrative_area_level_1", "administrative_area_level_2" }, googleMapsApiKey: "google-api-key"),
					"Using Google Maps reverse geocode provider (need api key) with an option to prevent processing if there is no coordinate or no photo taken date found on any photo.", textWriter);
				break;
			case OptionNames.CopyVerb:
				WriteOptionArgumentsToConsole(
					new CopyOptions(outputFolder, NamingStyle.Numeric, FolderProcessType.SubFoldersPreserveFolderHierarchy, NumberNamingTextStyle.PaddingZeroCharacter,
						CopyInvalidFormatAction.Continue, CopyNoPhotoTakenDateAction.Continue, CopyNoCoordinateAction.Continue, inputFolder),
					"Preserve same folder hierarchy, copy photos with sequential number ordering by photo taken date.",
					textWriter);

				WriteOptionArgumentsToConsole(
					new CopyOptions(outputFolder, NamingStyle.DateTimeWithSeconds, FolderProcessType.FlattenAllSubFolders, NumberNamingTextStyle.OnlySequentialNumbers,
						CopyInvalidFormatAction.Continue, CopyNoPhotoTakenDateAction.Continue, CopyNoCoordinateAction.Continue, inputFolder,
						groupByFolderType: GroupByFolderType.YearMonthDay),
					"Groups photos by photo taken year, month, day than copy on [year]/[month]/[day] directory with a file name as photo taken date.",
					textWriter);

				WriteOptionArgumentsToConsole(
					new CopyOptions(outputFolder, NamingStyle.AddressDay, FolderProcessType.SubFoldersPreserveFolderHierarchy, NumberNamingTextStyle.AllNamesAreSameLength,
						CopyInvalidFormatAction.PreventProcess, CopyNoPhotoTakenDateAction.InSubFolder, CopyNoCoordinateAction.InSubFolder, inputFolder, folderAppendType: FolderAppendType.DayRange,
						folderAppendLocationType: FolderAppendLocationType.Prefix, reverseGeoCodeProvider: ReverseGeocodeProvider.GoogleMaps,
						googleMapsAddressTypes: new[] { "administrative_area_level_1", "administrative_area_level_2", "administrative_area_level_3" }, googleMapsApiKey: "google-api-key"),
					"Adding day range as a prefix to existing folder names and photos copied with a file name as address and day.",
					textWriter);

				WriteOptionArgumentsToConsole(
					new CopyOptions(outputFolder, NamingStyle.AddressDateTimeWithSeconds, FolderProcessType.SubFoldersPreserveFolderHierarchy, NumberNamingTextStyle.AllNamesAreSameLength,
						CopyInvalidFormatAction.PreventProcess, CopyNoPhotoTakenDateAction.InSubFolder, CopyNoCoordinateAction.InSubFolder, inputFolder, reverseGeoCodeProvider: ReverseGeocodeProvider.OpenStreetMapFoundation,
						openStreetMapProperties: new[] { "country", "city", "town", "suburb" }),
					"Preserve same folder hierarchy, copy photos with a file name as photo taken date, time and address. Possible file name will have number suffix. Photos that don't have any coordinate or photo taken date will be copied in a relative subfolder.",
					textWriter);

				WriteOptionArgumentsToConsole(
					new CopyOptions(outputFolder, NamingStyle.DayAddress, FolderProcessType.FlattenAllSubFolders, NumberNamingTextStyle.OnlySequentialNumbers,
						CopyInvalidFormatAction.PreventProcess, CopyNoPhotoTakenDateAction.Continue, CopyNoCoordinateAction.InSubFolder, inputFolder, groupByFolderType: GroupByFolderType.AddressHierarchy,
						reverseGeoCodeProvider: ReverseGeocodeProvider.BigDataCloud, bigDataCloudAdminLevels: new[] { 2, 4, 6, 8 }),
					"Groups photos by photo taken year, month, day than copy on [year]/[month]/[day] directory with a file name as photo taken date. Photos that don't have any coordinate will be copied in a relative subfolder.",
					textWriter);

				break;
			case OptionNames.ArchiveVerb:
				WriteOptionArgumentsToConsole(new ArchiveOptions(outputFolder), "Archive all photos in current folder (and it's subfolders recursively) into output folder by [year]/[month]/[day] hierarchy with a file name photo taken date with seconds prefixed by file hash. Saves all photo taken information into local SQLite database.",
					textWriter);

				WriteOptionArgumentsToConsole(new ArchiveOptions(outputFolder, inputFolder, false,
						ArchiveInvalidFormatAction.PreventProcess, ArchiveNoPhotoTakenDateAction.PreventProcess, ArchiveNoCoordinateAction.PreventProcess,
						ReverseGeocodeProvider.OpenStreetMapFoundation, openStreetMapProperties: new[] { "country", "city", "town", "suburb" }),
					"Archive all photos in input folder (and it's subfolders recursively), fetches all photo's reverse geocode information, copies into output folder by [year]/[month]/[day] hierarchy with a file name photo taken date with seconds prefixed by file hash. Saves all photo taken information and it's address (reverse geocode) into local SQLite database.",
					textWriter);
				break;
		}
	}

	private static void WriteOptionArgumentsToConsole<TOptions>(TOptions options, string description, TextWriter textWriter)
	{
		textWriter.WriteLine($"- {description}");
		textWriter.WriteLine();
		textWriter.WriteLine("Example with long argument names;");
		textWriter.WriteLine($"{OptionNames.ApplicationAlias} {Parser.Default.FormatCommandLine(options)}");
		textWriter.WriteLine();
		textWriter.WriteLine("Example with short argument names;");
		textWriter.WriteLine($"{OptionNames.ApplicationAlias} {Parser.Default.FormatCommandLine(options, settings => settings.PreferShortName = true)}");
		textWriter.WriteLine();
	}

	public static void ExtendedHelpWritingToConsole(TextWriter textWriter)
	{
		var verbs = new[] { OptionNames.CopyVerb, OptionNames.InfoVerb, OptionNames.AddressVerb, OptionNames.SettingsVerb };
		textWriter.WriteLine($"Type `{OptionNames.ApplicationAlias} help [{string.Join('|', verbs)}]` for detailed option list and example usages");
	}
}
