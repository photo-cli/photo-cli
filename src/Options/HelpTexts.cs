namespace PhotoCli.Options;

public static class HelpTexts
{
	#region Copy

	public const string NamingStyle = "(MUST) Naming strategy of newly copied file name. " +
	                                  "( Numeric: 1, Day: 2, DateTimeWithMinutes: 3, DateTimeWithSeconds: 4, Address: 5, DayAddress: 6, DateTimeWithMinutesAddress: 7, " +
	                                  "DateTimeWithSecondsAddress: 8, " + "AddressDay: 9, AddressDateTimeWithMinutes: 10, AddressDateTimeWithSeconds: 11 )";

	public const string FolderProcessType = "(MUST) Reading photos strategy from input folder. ( Single: 1, SubFoldersPreserveFolderHierarchy: 2, FlattenAllSubFolders: 3 )";

	public const string NumberNamingTextStyle = "(MUST) Number naming strategy when using `NamingStyle` as `Numeric` or using to numbering the possible same names. " +
	                                            "( AllNamesAreSameLength: 1, PaddingZeroCharacter: 2, OnlySequentialNumbers: 3 )";

	public const string CopyNoPhotoTakenDateAction = "(Optional) Action to do when a photo with a no taken date. " +
	                                                 "( Continue: 0 [default], PreventProcess: 1, DontCopyToOutput: 2, InSubFolder: 3, " +
	                                                 "AppendToEndOrderByFileName: 4, InsertToBeginningOrderByFileName: 5 )";

	public const string CopyNoCoordinateAction = "(Optional) Action to do when a photo with a no coordinate. ( Continue: 0 [default], PreventProcess: 1, DontCopyToOutput: 2, InSubFolder: 3 )";

	public const string CopyInvalidFormatAction = "(Optional) Action to do when a photo format is invalid. " +
	                                              "( Continue: 0 [default], PreventProcess: 1, DontCopyToOutput: 2, InSubFolder: 3 )";

	public const string IsDryRun = "(Optional) Simulate the same process without writing to the output folder. (no extra parameter needed)";

	public const string GroupByFolderType = "(Optional) Strategy to group photos into folders. [Can't use with `FolderProcessType` is `SubFoldersPreserveFolderHierarchy`] " +
	                                        "( YearMonthDay: 1, YearMonth: 2, Year: 3, AddressFlat: 4, AddressHierarchy: 5 )";

	public const string FolderAppendType = "(Optional) Appending name strategy to folder names cloned from source folder hierarchy. " +
	                                       "[Can be with `FolderProcessType` is `SubFoldersPreserveFolderHierarchy`] " +
	                                       "( FirstYearMonthDay: 1, FirstYearMonth: 2, FirstYear: 3, DayRange: 4, MatchingMinimumAddress: 5 )";

	public const string FolderAppendLocationType = "(Optional) Append location for `FolderAppendType`. " +
	                                               "[Can be use with `FolderProcessType` is `SubFoldersPreserveFolderHierarchy`] ( Prefix: 1, Suffix: 2 )";

	public const string Verify = "(Optional) Verify that all photo files copied successfully by comparing file hashes. (no extra parameter needed)";

	#endregion

	#region Address

	public const string AddressListType = "(MUST) Response list detail level. ( AllAvailableProperties: 0, SelectedProperties: 1, FullResponse: 2 )";

	#endregion

	#region Info

	public const string AllFolders = "(Optional) Read & list all photos in all subfolders (no extra parameter needed)";

	public const string InfoNoPhotoTakenDateAction = "(Optional) Action to do when a photo with a no taken date. ( Continue: 0 [default], PreventProcess: 1 )";

	public const string InfoNoCoordinateAction = "(Optional) Action to do when a photo with a no coordinate. ( Continue: 0 [default], PreventProcess: 1 )";

	public const string InfoInvalidFormatAction = "(Optional) Action to do when a photo format is invalid. ( Continue: 0 [default], PreventProcess: 1 )";

	#endregion

	#region Archive

	public const string ArchiveNoPhotoTakenDateAction = "(Optional) Action to do when a photo with a no taken date. ( Continue: 0 [default], PreventProcess: 1 )";

	public const string ArchiveNoCoordinateAction = "(Optional) Action to do when a photo with a no coordinate. ( Continue: 0 [default], PreventProcess: 1 )";

	public const string ArchiveInvalidFormatAction = "(Optional) Action to do when a photo format is invalid. ( Continue: 0 [default], PreventProcess: 1 )";

	#endregion

	#region Settings

	public const string Key = "(Optional) Setting property name to change.";
	public const string Value = "(Optional) Setting value to set.";
	public const string Reset = "(Optional) Reset all settings value to default ones. (no extra parameter needed)";

	#endregion

	#region Shared

	public const string InputPath = "(Default current executing folder) File system path to read & copy photos from. ( there will be no modification on the input path )";

	public const string OutputPathCopy = "(MUST) File system path to create new organized folder. " +
	                                     "A new folder hierarchy will be created on that location with new file names. (will create folder if not exist)";

	public const string OutputPathInfo = "(MUST) File system path to write report file.";

	public const string ReverseGeocodeProvider = "(Optional) Third-party provider to resolve photo taken address by photo's coordinates. " +
	                                             "( Disabled: 0 [default], BigDataCloud: 1, OpenStreetMapFoundation: 2, GoogleMaps: 3, LocationIq: 5 )";

	public const string BigDataCloudApiKey = "(Optional) API key needed to use BigDataCloud. https://www.bigdatacloud.com/geocoding-apis/reverse-geocode-to-city-api/ " +
	                                         "(Instead of using this option, environment name: " + ApiKeyStore.BigDataCloudApiKeyEnvironmentKey + " can be used or " +
	                                         "`BigDataCloudApiKey` key can be set via settings command. )";

	public const string BigDataCloudAdminLevels = "(Optional) Admin levels separated with space. " +
	                                              "( To see which level correspond to which address level, you may use `photo-cli address` to see the full response returned from BigDataCloud. )";

	public const string GoogleMapsAddressTypes = "(Optional) GoogleMaps address types separated with space. " +
	                                             "( To see which level correspond to which address level, you may use `photo-cli address` to see the full response returned from GoogleMaps. )";

	public const string GoogleMapsApiKey = "(Optional) API key needed to use GoogleMaps. https://developers.google.com/maps/documentation/geocoding/overview/ (Instead of using this option, environment name: " +
	                                       ApiKeyStore.GoogleMapsApiKeyEnvironmentKey + " can be used or `GoogleMapsApiKey` key can be set via settings command. )";

	public const string OpenStreetMapProperties = "(Optional) OpenStreetMap properties separated with space. " +
	                                              "( To see which level correspond to which address level, you may use `photo-cli address` to see the full response returned from OpenStreetMap provider. )";

	public const string LocationIqApiKey = "(Optional) API key needed to use LocationIq. https://locationiq.com/docs/ (Instead of using this option, environment name: " +
	                                       ApiKeyStore.LocationIqApiKeyEnvironmentKey + " can be used or `LocationIqApiKey` key can be set via settings command. )";

	public const string HasPaidLicense = "(Optional) Bypass the free rate limit if you have paid license. ( For LocationIq. )";

	public const string Language = "(Optional) Language/culture value to get localized address result for BigDataCloud " +
	                               "( https://www.bigdatacloud.com/supported-languages/ ) and GoogleMaps (https://developers.google.com/maps/faq#languagesupport ). ";

	#endregion
}
