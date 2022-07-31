namespace PhotoCli.Tests.EndToEndTests;

[Collection(XunitSharedCollectionsToDisableParallelExecution.AppSettingsJson)]
public class HelpVerbEndToEndTests : BaseEndToEndTests
{
	private const string DefaultHelpText = "Type `photo-cli help [copy|info|address|settings]` for detailed option list and example usages";

	public HelpVerbEndToEndTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
	{
	}

	[Fact]
	public async Task Running_Without_Arguments_Should_Output_Custom_Help_Text()
	{
		var actualOutput = await RunMain(Array.Empty<string>(), ExitCode.ParseArgsFailed);
		actualOutput.Should().Be(DefaultHelpText);
	}

	[Fact]
	public async Task Running_With_Help_Verb_Should_Output_Custom_Help_Text()
	{
		var actualOutput = await RunMain(new[] { "help" });
		actualOutput.Should().Be(DefaultHelpText);
	}

	[Fact]
	public async Task Running_Help_Verb_With_Copy_Verb_Should_Output_Copy_Usages()
	{
		var copyExampleUsages = @"NOTES:
- Instead of option names (for ex: DateTimeWithMinutes), you may use options values too. (for ex: 3)
- You can use relative folder paths. If you use the input folder as the working directory, you don't need to use the input argument.

EXAMPLE USAGES:
- Preserve same folder hierarchy, copy photos with sequential number ordering by photo taken date.

Example with long argument names;
photo-cli copy --process-type SubFoldersPreserveFolderHierarchy --input [input-folder] --number-style PaddingZeroCharacter --output [output-folder] --naming-style Numeric

Example with short argument names;
photo-cli copy -f SubFoldersPreserveFolderHierarchy -i [input-folder] -n PaddingZeroCharacter -o [output-folder] -s Numeric

- Groups photos by photo taken year, month, day than copy on [year]/[month]/[day] directory with a file name as photo taken date.

Example with long argument names;
photo-cli copy --process-type FlattenAllSubFolders --group-by YearMonthDay --input [input-folder] --number-style OnlySequentialNumbers --output [output-folder] --naming-style DateTimeWithSeconds

Example with short argument names;
photo-cli copy -f FlattenAllSubFolders -g YearMonthDay -i [input-folder] -n OnlySequentialNumbers -o [output-folder] -s DateTimeWithSeconds

- Adding day range as a prefix to existing folder names and photos copied with a file name as address and day.

Example with long argument names;
photo-cli copy --folder-append DayRange --no-coordinate InSubFolder --reverse-geocode GoogleMaps --process-type SubFoldersPreserveFolderHierarchy --input [input-folder] --googlemaps-key google-api-key --googlemaps-types administrative_area_level_1 administrative_area_level_2 administrative_area_level_3 --number-style AllNamesAreSameLength --output [output-folder] --folder-append-location Prefix --naming-style AddressDay --no-taken-date InSubFolder

Example with short argument names;
photo-cli copy -a DayRange -c InSubFolder -e GoogleMaps -f SubFoldersPreserveFolderHierarchy -i [input-folder] -k google-api-key -m administrative_area_level_1 administrative_area_level_2 administrative_area_level_3 -n AllNamesAreSameLength -o [output-folder] -p Prefix -s AddressDay -t InSubFolder

- Preserve same folder hierarchy, copy photos with a file name as photo taken date, time and address. Possible file name will have number suffix. Photos that don't have any coordinate or photo taken date will be copied in a relative subfolder.

Example with long argument names;
photo-cli copy --no-coordinate InSubFolder --reverse-geocode OpenStreetMapFoundation --process-type SubFoldersPreserveFolderHierarchy --input [input-folder] --number-style AllNamesAreSameLength --output [output-folder] --openstreetmap-properties country city town suburb --naming-style AddressDateTimeWithSeconds --no-taken-date InSubFolder

Example with short argument names;
photo-cli copy -c InSubFolder -e OpenStreetMapFoundation -f SubFoldersPreserveFolderHierarchy -i [input-folder] -n AllNamesAreSameLength -o [output-folder] -r country city town suburb -s AddressDateTimeWithSeconds -t InSubFolder

- Groups photos by photo taken year, month, day than copy on [year]/[month]/[day] directory with a file name as photo taken date. Photos that don't have any coordinate will be copied in a relative subfolder.

Example with long argument names;
photo-cli copy --no-coordinate InSubFolder --reverse-geocode BigDataCloud --process-type FlattenAllSubFolders --group-by AddressHierarchy --input [input-folder] --number-style OnlySequentialNumbers --output [output-folder] --naming-style DayAddress --bigdatacloud-levels 2 4 6 8

Example with short argument names;
photo-cli copy -c InSubFolder -e BigDataCloud -f FlattenAllSubFolders -g AddressHierarchy -i [input-folder] -n OnlySequentialNumbers -o [output-folder] -s DayAddress -v 2 4 6 8
";

		await RunHelpAndVerifyOutput("copy", copyExampleUsages);
	}

	[Fact]
	public async Task Running_Help_Verb_With_Info_Verb_Should_Output_Copy_Usages()
	{
		var infoExampleUsages = @"NOTES:
- Instead of option names (for ex: DateTimeWithMinutes), you may use options values too. (for ex: 3)
- You can use relative folder paths. If you use the input folder as the working directory, you don't need to use the input argument.

EXAMPLE USAGES:
- Photos located on all subfolders will be processed and their photograph's taken date and address information will be saved on CSV file using BigDataCloud reverse geocode provider.

Example with long argument names;
photo-cli info --all-folders --reverse-geocode OpenStreetMapFoundation --input [input-folder] --output [output-file].csv --openstreetmap-properties country city town suburb

Example with short argument names;
photo-cli info -a -e OpenStreetMapFoundation -i [input-folder] -o [output-file].csv -r country city town suburb

- Using Google Maps reverse geocode provider (need api key) with an option to prevent processing if there is no coordinate or no photo taken date found on any photo.

Example with long argument names;
photo-cli info --no-coordinate PreventProcess --reverse-geocode GoogleMaps --input [input-folder] --googlemaps-key google-api-key --googlemaps-types administrative_area_level_1 administrative_area_level_2 --output [output-file].csv --no-taken-date PreventProcess

Example with short argument names;
photo-cli info -c PreventProcess -e GoogleMaps -i [input-folder] -k google-api-key -m administrative_area_level_1 administrative_area_level_2 -o [output-file].csv -t PreventProcess
";
		await RunHelpAndVerifyOutput("info", infoExampleUsages);
	}

	[Fact]
	public async Task Running_Help_Verb_With_Address_Verb_Should_Output_Copy_Usages()
	{
		var addressExampleUsages = @"NOTES:
- Instead of option names (for ex: DateTimeWithMinutes), you may use options values too. (for ex: 3)
- You can use relative folder paths. If you use the input folder as the working directory, you don't need to use the input argument.

EXAMPLE USAGES:
- All properties

Example with long argument names;
photo-cli address --reverse-geocode OpenStreetMapFoundation --input [photo-path].jpg

Example with short argument names;
photo-cli address -e OpenStreetMapFoundation -i [photo-path].jpg

- Selected properties

Example with long argument names;
photo-cli address --reverse-geocode OpenStreetMapFoundation --input [photo-path].jpg --openstreetmap-properties country city town suburb --type SelectedProperties

Example with short argument names;
photo-cli address -e OpenStreetMapFoundation -i [photo-path].jpg -r country city town suburb -t SelectedProperties

- Show full response

Example with long argument names;
photo-cli address --reverse-geocode OpenStreetMapFoundation --input [photo-path].jpg --type FullResponse

Example with short argument names;
photo-cli address -e OpenStreetMapFoundation -i [photo-path].jpg -t FullResponse
";
		await RunHelpAndVerifyOutput("address", addressExampleUsages);
	}

	[Fact]
	public async Task Running_Help_Verb_With_Settings_Verb_Should_Output_Copy_Usages()
	{
		var settingsExampleUsages = @"NOTES:
- Instead of option names (for ex: DateTimeWithMinutes), you may use options values too. (for ex: 3)
- You can use relative folder paths. If you use the input folder as the working directory, you don't need to use the input argument.

EXAMPLE USAGES:
- List all settings

Example with long argument names;
photo-cli settings

Example with short argument names;
photo-cli settings

- Get a setting

Example with long argument names;
photo-cli settings --key YearFormat

Example with short argument names;
photo-cli settings -k YearFormat

- Save a setting

Example with long argument names;
photo-cli settings --key YearFormat --value yyyy

Example with short argument names;
photo-cli settings -k YearFormat -v yyyy

- Reset all settings

Example with long argument names;
photo-cli settings --reset

Example with short argument names;
photo-cli settings -r
";
		await RunHelpAndVerifyOutput("settings", settingsExampleUsages);
	}

	private async Task RunHelpAndVerifyOutput(string verb, string expectedOutput)
	{
		var actualOutput = await RunMain(new[] { "help", verb });
		StringsShouldMatchDiscardingLineEndings(actualOutput, expectedOutput);
	}
}
