namespace PhotoCli.Tests.EndToEndTests;

public class HelpVerbEndToEndTests : BaseEndToEndTests
{
	private const string DefaultHelpText = "Type `photo-cli help (copy|info|address|settings|mcp)` for detailed option list and example usages";

	[Fact]
	public async Task Running_Without_Arguments_Should_Output_Custom_Help_Text()
	{
		var actualOutput = await RunMainRaw([], ExitCode.ParseArgsFailed);
		actualOutput.Should().Be(DefaultHelpText);
	}

	[Fact]
	public async Task Running_With_Help_Verb_Should_Output_Custom_Help_Text()
	{
		var actualOutput = await RunMainRaw(["help"]);
		actualOutput.Should().Be(DefaultHelpText);
	}

	[Fact]
	public async Task Running_Help_Verb_With_Copy_Verb_Should_Output_Custom_Help_Text()
	{
		var copyExampleUsages = @"NOTES:
- Instead of option names (for ex: DateTimeWithMinutes), you may use options values too. (for ex: 3)
- You can use relative folder paths. If you use the input folder as the working directory, you don't need to use the input argument.

EXAMPLE USAGES:
- Preserve same folder hierarchy, copy photos with sequential number ordering by photo taken date.

Example with long argument names;
photo-cli copy --process-type SubFoldersPreserveFolderHierarchy --input (input-folder) --number-style PaddingZeroCharacter --output (output-folder) --naming-style Numeric

Example with short argument names;
photo-cli copy -f SubFoldersPreserveFolderHierarchy -i (input-folder) -n PaddingZeroCharacter -o (output-folder) -s Numeric

- Groups photos by photo taken year, month, day than copy on (year)/(month)/(day) directory with a file name as photo taken date.

Example with long argument names;
photo-cli copy --process-type FlattenAllSubFolders --group-by YearMonthDay --input (input-folder) --number-style OnlySequentialNumbers --output (output-folder) --naming-style DateTimeWithSeconds

Example with short argument names;
photo-cli copy -f FlattenAllSubFolders -g YearMonthDay -i (input-folder) -n OnlySequentialNumbers -o (output-folder) -s DateTimeWithSeconds

- Adding day range as a prefix to existing folder names and photos copied with a file name as address and day.

Example with long argument names;
photo-cli copy --folder-append DayRange --no-coordinate InSubFolder --reverse-geocode GoogleMaps --process-type SubFoldersPreserveFolderHierarchy --input (input-folder) --googlemaps-key google-api-key --googlemaps-types administrative_area_level_1 administrative_area_level_2 administrative_area_level_3 --number-style AllNamesAreSameLength --output (output-folder) --folder-append-location Prefix --naming-style AddressDay --no-taken-date InSubFolder --invalid-format PreventProcess

Example with short argument names;
photo-cli copy -a DayRange -c InSubFolder -e GoogleMaps -f SubFoldersPreserveFolderHierarchy -i (input-folder) -k google-api-key -m administrative_area_level_1 administrative_area_level_2 administrative_area_level_3 -n AllNamesAreSameLength -o (output-folder) -p Prefix -s AddressDay -t InSubFolder -x PreventProcess

- Preserve the same folder hierarchy while copying photos with filenames consisting of the photo-taken date, time, and address (with possible number suffixes), and copy photos without coordinates or photo-taken dates into a relative subfolder.

Example with long argument names;
photo-cli copy --no-coordinate InSubFolder --reverse-geocode OpenStreetMapFoundation --process-type SubFoldersPreserveFolderHierarchy --input (input-folder) --number-style AllNamesAreSameLength --output (output-folder) --openstreetmap-properties country city town suburb --naming-style AddressDateTimeWithSeconds --no-taken-date InSubFolder --invalid-format PreventProcess

Example with short argument names;
photo-cli copy -c InSubFolder -e OpenStreetMapFoundation -f SubFoldersPreserveFolderHierarchy -i (input-folder) -n AllNamesAreSameLength -o (output-folder) -r country city town suburb -s AddressDateTimeWithSeconds -t InSubFolder -x PreventProcess

- Groups photos by photo-taken year, month, and day, then copies them into a year/month/day directory structure with filenames as the photo-taken date, while photos without coordinates are copied into a relative subfolder.

Example with long argument names;
photo-cli copy --no-coordinate InSubFolder --reverse-geocode BigDataCloud --process-type FlattenAllSubFolders --group-by AddressHierarchy --input (input-folder) --number-style OnlySequentialNumbers --output (output-folder) --naming-style DayAddress --bigdatacloud-levels 2 4 6 8 --invalid-format PreventProcess

Example with short argument names;
photo-cli copy -c InSubFolder -e BigDataCloud -f FlattenAllSubFolders -g AddressHierarchy -i (input-folder) -n OnlySequentialNumbers -o (output-folder) -s DayAddress -u 2 4 6 8 -x PreventProcess";

		await RunHelpAndVerifyOutput("copy", copyExampleUsages);
	}

	[Fact]
	public async Task Running_Help_Verb_With_Info_Verb_Should_Output_Custom_Help_Text()
	{
		var infoExampleUsages = @"NOTES:
- Instead of option names (for ex: DateTimeWithMinutes), you may use options values too. (for ex: 3)
- You can use relative folder paths. If you use the input folder as the working directory, you don't need to use the input argument.

EXAMPLE USAGES:
- Photos located on all subfolders will be processed and their photograph's taken date and address information will be saved on CSV file using BigDataCloud reverse geocode provider.

Example with long argument names;
photo-cli info --all-folders --reverse-geocode OpenStreetMapFoundation --input (input-folder) --output (output-file).csv --openstreetmap-properties country city town suburb

Example with short argument names;
photo-cli info -a -e OpenStreetMapFoundation -i (input-folder) -o (output-file).csv -r country city town suburb

- Using Google Maps reverse geocode provider (need api key) with an option to prevent processing if there is no coordinate or no photo taken date found on any photo.

Example with long argument names;
photo-cli info --no-coordinate PreventProcess --reverse-geocode GoogleMaps --input (input-folder) --googlemaps-key google-api-key --googlemaps-types administrative_area_level_1 administrative_area_level_2 --output (output-file).csv --no-taken-date PreventProcess

Example with short argument names;
photo-cli info -c PreventProcess -e GoogleMaps -i (input-folder) -k google-api-key -m administrative_area_level_1 administrative_area_level_2 -o (output-file).csv -t PreventProcess";
		await RunHelpAndVerifyOutput("info", infoExampleUsages);
	}

	[Fact]
	public async Task Running_Help_Verb_With_Address_Verb_Should_Output_Custom_Help_Text()
	{
		var addressExampleUsages = @"NOTES:
- Instead of option names (for ex: DateTimeWithMinutes), you may use options values too. (for ex: 3)
- You can use relative folder paths. If you use the input folder as the working directory, you don't need to use the input argument.

EXAMPLE USAGES:
- All properties

Example with long argument names;
photo-cli address --reverse-geocode OpenStreetMapFoundation --input (photo-path).jpg

Example with short argument names;
photo-cli address -e OpenStreetMapFoundation -i (photo-path).jpg

- Selected properties

Example with long argument names;
photo-cli address --reverse-geocode OpenStreetMapFoundation --input (photo-path).jpg --openstreetmap-properties country city town suburb --type SelectedProperties

Example with short argument names;
photo-cli address -e OpenStreetMapFoundation -i (photo-path).jpg -r country city town suburb -t SelectedProperties

- Show full response

Example with long argument names;
photo-cli address --reverse-geocode OpenStreetMapFoundation --input (photo-path).jpg --type FullResponse

Example with short argument names;
photo-cli address -e OpenStreetMapFoundation -i (photo-path).jpg -t FullResponse";
		await RunHelpAndVerifyOutput("address", addressExampleUsages);
	}

	[Fact]
	public async Task Running_Help_Verb_With_Settings_Verb_Should_Output_Custom_Help_Text()
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
photo-cli settings -r";
		await RunHelpAndVerifyOutput("settings", settingsExampleUsages);
	}

	[Fact]
	public async Task Running_Help_Verb_With_Archive_Verb_Should_Output_Custom_Help_Text()
	{
		var archiveExampleUsages = @"NOTES:
- Instead of option names (for ex: DateTimeWithMinutes), you may use options values too. (for ex: 3)
- You can use relative folder paths. If you use the input folder as the working directory, you don't need to use the input argument.

EXAMPLE USAGES:
- Archive all photos in current folder (and it's subfolders recursively) into output folder by (year)/(month)/(day) hierarchy with a file name photo taken date with seconds prefixed by file hash. Saves all photo taken information into local SQLite database.

Example with long argument names;
photo-cli archive --output (output-folder)

Example with short argument names;
photo-cli archive -o (output-folder)

- Archive all photos in the input folder and its subfolders recursively by fetching each photo's reverse geocode information, copying them into the output folder organized by year/month/day hierarchy with filenames consisting of the photo-taken date with seconds prefixed by file hash, and saving all photo metadata and addresses into a local SQLite database.

Example with long argument names;
photo-cli archive --no-coordinate PreventProcess --reverse-geocode OpenStreetMapFoundation --input (input-folder) --output (output-folder) --openstreetmap-properties country city town suburb --no-taken-date PreventProcess --invalid-format PreventProcess --album-type NoAlbumLinking

Example with short argument names;
photo-cli archive -c PreventProcess -e OpenStreetMapFoundation -i (input-folder) -o (output-folder) -r country city town suburb -t PreventProcess -x PreventProcess -y NoAlbumLinking

- Archive all photos in the input folder (and its subfolders recursively) by creating a date range album named 'album 1', fetching each photo's reverse geocode information using Google Maps with properties administrative area level 1 and 2 to create location-based albums, and copying them into the output folder with a (year)/(month)/(day) hierarchy using filenames formatted as photo-taken-date-with-seconds prefixed by file hash but the process won't start if any photo has an invalid format, missing photo-taken date or coordinates, or if the photo-taken date span exceeds 30 days and upon successful operation, it would delete the input source folder.

Example with long argument names;
photo-cli archive --album-name ""album 1"" --no-coordinate PreventProcess --reverse-geocode GoogleMaps --delete-on-source --input (input-folder) --googlemaps-types administrative_area_level_1 administrative_area_level_2 --output (output-folder) --auto-reverse-geocode-album --no-taken-date PreventProcess --expected-day-range 30 --invalid-format PreventProcess --album-type DateRange --missing-reverse-geocode PreventProcess

Example with short argument names;
photo-cli archive -a ""album 1"" -c PreventProcess -e GoogleMaps -f -i (input-folder) -m administrative_area_level_1 administrative_area_level_2 -o (output-folder) -s -t PreventProcess -w 30 -x PreventProcess -y DateRange -z PreventProcess";
		await RunHelpAndVerifyOutput("archive", archiveExampleUsages);
	}

	[Fact]
	public async Task Running_Help_Verb_With_List_Verb_Should_Output_Custom_Help_Text()
	{
		var archiveExampleUsages = @"NOTES:
- Instead of option names (for ex: DateTimeWithMinutes), you may use options values too. (for ex: 3)
- You can use relative folder paths. If you use the input folder as the working directory, you don't need to use the input argument.

EXAMPLE USAGES:
- List statistics of the archive folder

Example with long argument names;
photo-cli list --input (input-folder)

Example with short argument names;
photo-cli list -i (input-folder)

- List all the album information of the archive folder

Example with long argument names;
photo-cli list --input (input-folder) --type Albums

Example with short argument names;
photo-cli list -i (input-folder) -t Albums

- List paths (to be send as process arguments to photo viewers) or open (only supporting in macOS , Preview app for now) for the given album id

Example with long argument names;
photo-cli list --input (input-folder) --id 1 --type PhotosByAlbum

Example with short argument names;
photo-cli list -i (input-folder) -n 1 -t PhotosByAlbum

- List paths (to be send as process arguments to photo viewers) or open (only supporting in macOS , Preview app for now) for the given year

Example with long argument names;
photo-cli list --input (input-folder) --type PhotosByDate --year 2007

Example with short argument names;
photo-cli list -i (input-folder) -t PhotosByDate -y 2007

- List paths (to be send as process arguments to photo viewers) or open (only supporting in macOS , Preview app for now) for the given year & month

Example with long argument names;
photo-cli list --input (input-folder) --month 8 --type PhotosByDate --year 2007

Example with short argument names;
photo-cli list -i (input-folder) -m 8 -t PhotosByDate -y 2007

- List paths (to be send as process arguments to photo viewers) or open (only supporting in macOS , Preview app for now) for the given year, month & day

Example with long argument names;
photo-cli list --day 19 --input (input-folder) --month 8 --type PhotosByDate --year 2007

Example with short argument names;
photo-cli list -d 19 -i (input-folder) -m 8 -t PhotosByDate -y 2007";
		await RunHelpAndVerifyOutput("list", archiveExampleUsages);
	}

	private async Task RunHelpAndVerifyOutput(string verb, string expectedOutput)
	{
		var actualOutput = await RunMainRaw(["help", verb]);
		StringsShouldMatchDiscardingLineEndings(actualOutput, expectedOutput);
	}
}
