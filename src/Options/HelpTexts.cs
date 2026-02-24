namespace PhotoCli.Options;

public static class HelpTexts
{
	#region Copy

	public const string NamingStyle = """
	                                  (Required) Naming strategy of newly copied file name.

	                                  For the options other than the `Numeric` could end with a same file name. For these cases, it's appending a number at the end by the `NumberNamingTextStyle` option.

	                                  Numeric: 1
	                                  Names files using sequential numbers only, without any date, time, or location information.

	                                  Day: 2
	                                  Names files using the date (day) when the photo was taken.

	                                  DateTimeWithMinutes: 3
	                                  Names files using the date and time (including hours and minutes) when the photo was taken.

	                                  DateTimeWithSeconds: 4
	                                  Names files using the date and time (including hours, minutes, and seconds) when the photo was taken.

	                                  Address: 5
	                                  Names files using reverse geocoded location address information only, grouping photos by the same location.

	                                  DayAddress: 6
	                                  Names files with the date first followed by the address, combining temporal and location information in that order.

	                                  DateTimeWithMinutesAddress: 7
	                                  Names files with the date-time (with minutes) first followed by the address, providing precise temporal and location information.

	                                  DateTimeWithSecondsAddress: 8
	                                  Names files with the date-time (with seconds) first followed by the address, providing the most precise temporal and location information.

	                                  AddressDay: 9
	                                  Names files with the address first followed by the date, prioritizing location over temporal information.

	                                  AddressDateTimeWithMinutes: 10
	                                  Names files with the address first followed by the date-time (with minutes), prioritizing location with moderate temporal precision.

	                                  AddressDateTimeWithSeconds: 11
	                                  Names files with the address first followed by the date-time (with seconds), prioritizing location with maximum temporal precision.
	                                  """;

	public const string FolderProcessType = """
	                                        (Required) Reading photos strategy from input folder.

	                                        Single: 1
	                                        Processes only photos directly within the specified folder without scanning any subfolders; all files must be located in the source path root, and folder append/grouping options cannot be used.

	                                        SubFoldersPreserveFolderHierarchy: 2
	                                        Processes the folder and all its subfolders while maintaining the original directory structure in the output; supports folder append operations but cannot be combined with GroupByFolderType options.,

	                                        FlattenAllSubFolders: 3
	                                        Processes the folder and all its subfolders but outputs all photos into a single flat structure without preserving the directory hierarchy; folder append options cannot be used with this mode
	                                        """;

	public const string NumberNamingTextStyle = """
	                                            (Required) Number naming strategy when using `NamingStyle` as `Numeric` or using to numbering the possible same names.

	                                            AllNamesAreSameLength: 1
	                                            Generates sequential numbers starting from the minimum value with a consistent digit length (e.g., 100, 101, 102 for three-digit numbers), ensuring all numbers have the same length without padding characters.

	                                            PaddingZeroCharacter: 2
	                                            Generates sequential numbers starting from 1 with leading zeros padded to match the maximum digit length needed (e.g., 001, 002, 003 for a total count requiring three digits).

	                                            OnlySequentialNumbers: 3
	                                            Generates plain sequential numbers starting from 1 without any padding or length constraints (e.g., 1, 2, 3, 10, 100).
	                                            """;

	public const string CopyNoPhotoTakenDateAction = """
	                                                 (Optional) Action to do when a photo with a no taken date.

	                                                 Continue: 0 [default],
	                                                 Processes and copies all photos including those without a taken date without any special handling or filtering.

	                                                 PreventProcess: 1
	                                                 Stops the entire copy operation if any photos without a taken date are found, returning an error exit code.

	                                                 DontCopyToOutput: 2
	                                                 Excludes photos without a taken date from the output, only copying photos that have valid taken date information.

	                                                 InSubFolder: 3
	                                                 Groups photos without a taken date into a separate subfolder while copying photos with taken dates to their normal destinations.

	                                                 AppendToEndOrderByFileName: 4
	                                                 Places photos without a taken date at the end of the sequence, ordered by filename, after all photos with taken dates.

	                                                 InsertToBeginningOrderByFileName: 5
	                                                 Places photos without a taken date at the beginning of the sequence, ordered by filename, before all photos with taken dates.
	                                                 """;

	public const string CopyNoCoordinateAction = """
	                                             (Optional) Action to do when a photo with a no coordinate.

	                                             Continue: 0 [default]
	                                             Processes and copies all photos including those without GPS coordinates without any special handling or filtering.

	                                             PreventProcess: 1
	                                             Stops the entire copy operation if any photos without GPS coordinates are found, returning an error exit code.

	                                             DontCopyToOutput: 2
	                                             Excludes photos without GPS coordinates from the output, only copying photos that have valid coordinate information.

	                                             InSubFolder: 3
	                                             Groups photos without GPS coordinates into a separate subfolder while copying photos with coordinates to their normal destinations.
	                                             """;

	public const string CopyInvalidFormatAction = """
	                                              (Optional) Action to do when a photo format is invalid.

	                                              Continue: 0 [default]
	                                              Processes and copies all files including those with invalid or unrecognized formats without any special handling or filtering.

	                                              PreventProcess: 1
	                                              Stops the entire copy operation if any files with invalid photo format are found, returning an error exit code.

	                                              DontCopyToOutput: 2
	                                              Excludes files with invalid photo format from the output, only copying files that have valid photo format.

	                                              InSubFolder: 3
	                                              Groups files with invalid photo format into a separate subfolder.
	                                              """;

	public const string IsDryRun = "(Optional) Simulate the same process without writing to the output folder. (no extra parameter needed)";

	public const string GroupByFolderType = """
	                                        (Optional) Grouping photos by hierarchical directories in file system by EXIF data.

	                                        Can't use with `FolderProcessType` is `SubFoldersPreserveFolderHierarchy`.

	                                        YearMonthDay: 1
	                                        Creating a file system hiearchy by year, month and day like /[year]/[month]/[day]/[sequential-number-on-that-day].jpg. For example /2017/03/23/1.jpg

	                                        YearMonth: 2
	                                        Creating a file system hiearchy by year and month like /[year]/[month]/[sequential-number-on-that-month].jpg. For example /2017/03/1.jpg

	                                        Year: 3
	                                        Creating a file system hiearchy by year like [year]/[sequential-number-on-that-year].jpg . For example /2017/1.jpg

	                                        AddressFlat: 4
	                                        Create a single folder on built with the reverse geocode properties formatted by joining the properties by the `AddressSeparator` which can be set on the `appsetting.json`, which is default by `-`. It could differs by your reverser geocode request /[country]-[[region]-[city]-[neighbourhood]-[street]/[sequential-number-on-that-street].jpg . For example /Italy-Toscana-Firenze-Santa Maria Novella-Via Claudio Monteverdi/1.jpg

	                                        AddressHierarchy: 5
	                                        Creating a file system hiearchy by reverse geocode properties requested (differs by your reverse geocode request) like /[country]/[region]/[city]/[neighbourhood]/[street]/[photos-on-street].jpg . For example /Italy/Toscana/Firenze/Santa Maria Novella/Via Claudio Monteverdi/7.jpg

	                                        """;

	public const string FolderAppendType = """
	                                       (Optional) Appending name strategy to folder names cloned from source folder hierarchy.

	                                       [Only use with `FolderProcessType` as `SubFoldersPreserveFolderHierarchy`]

	                                       FirstYearMonthDay: 1
	                                       Appends the year, month, and day of the first photo in the folder to the folder name.

	                                       FirstYearMonth: 2
	                                       Appends the year and month of the first photo in the folder to the folder name.

	                                       FirstYear: 3
	                                       Appends only the year of the first photo in the folder to the folder name.

	                                       DayRange: 4
	                                       Appends a date range spanning from the first to the last photo's date in the folder to the folder name.

	                                       MatchingMinimumAddress: 5
	                                       Appends the common address prefix shared by all photos in the folder, based on matching reverse geocode properties.

	                                       """;

	public const string FolderAppendLocationType = """
	                                               (Optional) Append location for `FolderAppendType`.

	                                               [Can use with `FolderProcessType` as `SubFoldersPreserveFolderHierarchy`]

	                                               Prefix: 1
	                                               Prepends the appended name before the original folder name.

	                                               Suffix: 2
	                                               Appends the appended name after the original folder name.

	                                               """;

	public const string Verify = "(Optional) Verify that all photo files copied successfully by comparing file hashes. (no extra parameter needed)";

	#endregion

	#region Address

	public const string AddressListType = """
	                                      (Required) Response list detail level.

	                                      AllAvailableProperties: 0
	                                      Lists all structured address properties available from the reverse geocode provider response (e.g., country, region, city, neighbourhood, street), without raw response data.

	                                      SelectedProperties: 1
	                                      Lists only the specific address properties you have configured for use (e.g., via BigDataCloudAdminLevels, OpenStreetMapProperties, or GoogleMapsAddressTypes options).

	                                      FullResponse: 2
	                                      Displays the complete raw response returned by the reverse geocode provider, useful for exploring available data before configuring selected properties.

	                                      """;

	#endregion

	#region Info

	public const string AllFolders = "(Optional) Read & list all photos in all subfolders (no extra parameter needed)";

	public const string InfoNoPhotoTakenDateAction = """
	                                                 (Optional) Action to do when a photo with a no taken date.

	                                                 Continue: 0 [default],
	                                                 Processes and creates output including those without a taken date without any special handling or filtering.

	                                                 PreventProcess: 1
	                                                 Stops the entire info operation if any photos without a taken date are found, returning an error exit code.
	                                                 """;

	public const string InfoNoCoordinateAction = """
	                                             (Optional) Action to do when a photo with a no coordinate.

	                                             Continue: 0 [default],
	                                             Processes and creates output including those without a coordinate date without any special handling or filtering.

	                                             PreventProcess: 1
	                                             Stops the entire info operation if any photos without a coordinate are found, returning an error exit code.
	                                             """;

	public const string InfoInvalidFormatAction = """
	                                              (Optional) Action to do when a photo format is invalid.

	                                              Continue: 0 [default]
	                                              Processes and creates output all files including those with invalid or unrecognized formats without any special handling or filtering.

	                                              PreventProcess: 1
	                                              Stops the entire info operation if any files with invalid photo format are found, returning an error exit code.
	                                              """;

	#endregion

	#region Archive

	public const string ArchiveNoPhotoTakenDateAction = """
	                                                    (Optional) Action to do when a photo with a no taken date.

	                                                    Continue: 0 [default],
	                                                    Processes and archives all photos including those without a taken date without any special handling or filtering.

	                                                    PreventProcess: 1
	                                                    Stops the entire archive operation if any photos without a taken date are found, returning an error exit code.
	                                                    """;

	public const string ArchiveNoCoordinateAction = """
	                                                (Optional) Action to do when a photo with a no coordinate.

	                                                Continue: 0 [default],
	                                                Processes and archives all photos including those without a coordinate date without any special handling or filtering.

	                                                PreventProcess: 1
	                                                Stops the entire archive operation if any photos without a coordinate are found, returning an error exit code.
	                                                """;

	public const string ArchiveInvalidFormatAction = """
	                                                 (Optional) Action to do when a photo format is invalid.

	                                                 Continue: 0 [default]
	                                                 Processes and creates output all files including those with invalid or unrecognized formats without any special handling or filtering.

	                                                 PreventProcess: 1
	                                                 Stops the entire info operation if any files with invalid photo format are found, returning an error exit code.
	                                                 """;

	public const string AlbumType = """
	                                (Optional) Whether you want to link photos as album by picking the album type.

	                                NoAlbumLinking: 0 [default]
	                                Photos are archived without being linked to any album.

	                                Individual: 1
	                                Links each photo individually to an album. Each photo is associated with the album regardless of when it was taken.

	                                DateRange: 2
	                                Links photos to an album using the date range derived from the photos' taken dates. Requires all photos to have valid date information.

	                                """;

	public const string AlbumNameNew = """
	                                   (Optional) Album name to create a new one for currently archiving photos.

	                                   [Can use with `AlbumType` by values of `Individual` or `DateRange`]
	                                   """;

	public const string AlbumIdUpdate = """
	                                    (Optional) Existing Album ID number value to link currently archiving photos.

	                                    Can use with `AlbumType` by values of `Individual` or `DateRange`.

	                                    Album IDs can be listed by `photo-cli list --type Albums`.
	                                    """;

	public const string AutoReverseGeocodeAlbum = """
	                                              (Optional) Automatically linking photos to an album (creating or using existing) for each reverse geocode property individually.

	                                              For example if you use reverse geocode properties are country, city each archive operation, you could have albums for each country and city variants.
	                                              """;

	public const string AlbumDeleteSource = "(Optional) [Dangerous parameter] Deleting the source folder on successful archive operation.";

	#endregion

	#region Settings

	public const string Key = "(Optional) Setting property name to change.";
	public const string Value = "(Optional) Setting value to set.";
	public const string Reset = "(Optional) Reset all settings value to default ones. (no extra parameter needed)";

	#endregion

	#region Shared

	public const string InputPath = """
	                                File system path to read & copy photos from.

	                                There will be no modification on the input path.

	                                If not given, default value would be the current executing folder.
	                                """;

	public const string OutputPathCopy = """
	                                     (Required) File system path to create new organized folder.

	                                     A new folder hierarchy will be created on that location with new file names.

	                                     Will create folder if not exist.
	                                     """;

	public const string OutputPathInfo = "(Required) File system path to write report file.";

	public const string ReverseGeocodeProvider = """
	                                             (Optional) Third-party provider to resolve photo taken address by photo's coordinates.
	                                             Disabled: 0 [default]
	                                             Not using any reverse geocode provider.

	                                             BigDataCloud: 1
	                                             Provides reverse geocoding with administrative level data including country, region, city, and neighbourhood information.
	                                             https://www.bigdatacloud.com/

	                                             OpenStreetMapFoundation: 2
	                                             Free and open-source reverse geocoding API powered by community-contributed OpenStreetMap data via the Nominatim service.
	                                             https://nominatim.openstreetmap.org/

	                                             GoogleMaps: 3
	                                             Google's reverse geocoding API offering accurate global address resolution with support for multiple address component types.
	                                             https://developers.google.com/maps/documentation/geocoding/

	                                             LocationIq: 5
	                                             A location data platform providing reverse geocoding based on OpenStreetMap data with both free and paid tiers.
	                                             https://locationiq.com/
	                                             """;

	public const string BigDataCloudApiKey = $"""
	                                          (Optional) API key needed to use BigDataCloud.
	                                          https://www.bigdatacloud.com/geocoding-apis/reverse-geocode-to-city-api/
	                                          Instead of using this option, environment name: {ApiKeyStore.BigDataCloudApiKeyEnvironmentKey} can be used or `BigDataCloudApiKey` key can be set via settings command.
	                                          """;

	public const string BigDataCloudAdminLevels = """
	                                              (Optional) Admin levels separated with space.

	                                              To see which level correspond to which address level, you may use `photo-cli address` to see the full response returned from BigDataCloud.
	                                              """;

	public const string GoogleMapsAddressTypes = """
	                                             (Optional) GoogleMaps address types separated with space.

	                                             To see which level correspond to which address level, you may use `photo-cli address` to see the full response returned from GoogleMaps.
	                                             """;

	public const string GoogleMapsApiKey = $"""
	                                        (Optional) API key needed to use GoogleMaps.

	                                        https://developers.google.com/maps/documentation/geocoding/overview/

	                                        Instead of using this option, environment name: {ApiKeyStore.GoogleMapsApiKeyEnvironmentKey} can be used or `GoogleMapsApiKey` key can be set via settings command.
	                                        """;

	public const string OpenStreetMapProperties = """
	                                              (Optional) OpenStreetMap properties separated with space.

	                                              To see which level correspond to which address level, you may use `photo-cli address` to see the full response returned from OpenStreetMap provider.
	                                              """;

	public const string LocationIqApiKey = $"""
	                                        (Optional) API key needed to use LocationIq.

	                                        https://locationiq.com/docs/

	                                        Instead of using this option, environment name: {ApiKeyStore.LocationIqApiKeyEnvironmentKey} can be used or `LocationIqApiKey` key can be set via settings command.
	                                        """;

	public const string HasPaidLicense = "(Optional) Bypass the free rate limit if you have paid license. ( For LocationIq )";

	public const string Language = """
	                               (Optional) Language/culture value to get localized address result
	                               For;

	                               BigDataCloud: https://www.bigdatacloud.com/supported-languages/

	                               GoogleMaps: https://developers.google.com/maps/faq#languagesupport.

	                               """;

	public const string MissingReverseGeocodeAction = """
	                                                  (Optional) Action to do when any of the photo has missing reverse geocode information.

	                                                  Continue: 0 [default]
	                                                  Ignores missing reverse geocode data and continues processing.

	                                                  PreventProcess: 1
	                                                  Stops the process if any photo is missing reverse geocode data.

	                                                  """;

	public const string ExpectedDayRange = "(Optional) Provide a maximum expected day difference as number for your photos to prevent processing if it's exceeding the range.";

	#endregion

	#region List

	public const string ArchivePath = """
	                                  Archive path to list & open photos from.
	                                  Default current executing folder)
	                                  """;

	public const string ListType = """
	                               (Optional) Listing type for archive folder

	                               Summary: 0 [default] - Shows total counts of albums, photos, and reverse geocode cache entries

	                               Albums: 1 - Lists all albums with their id, name, type, creation date, and configuration

	                               PhotosByAlbum: 2 - Lists or opens photos belonging to a specific album (requires `--album-id`)

	                               PhotosByDate: 3 - Lists or opens photos filtered by date (optionally filtered by `--year`, `--month`, `--day`)
	                               """;

	public const string AlbumId = "(Optional) Album ID to be used while using the list type of `PhotosByAlbum`";
	public const string Year = "(Optional) Year as number to be used while using the list type of `PhotosByDate`";
	public const string Month = "(Optional) Month as number to be used while using the list type of `PhotosByDate`";
	public const string Day = "(Optional) Day as number to be used while using the list type of `PhotosByDate`";
	public const string RawOutput = "(Optional) Listing photo paths each on new line instead of trying to open the default OS app while using the list type of `PhotosByAlbum` or `PhotosByDate`.";

	#endregion
}
