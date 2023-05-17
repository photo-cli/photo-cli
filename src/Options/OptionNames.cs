namespace PhotoCli.Options;

internal static class OptionNames
{
	internal const string ApplicationAlias = "photo-cli";

	internal const string AddressVerb = "address";
	internal const string CopyVerb = "copy";
	internal const string InfoVerb = "info";
	internal const string SettingsVerb = "settings";

	#region Copy

	internal const char NamingStyleOptionNameShort = 's';
	internal const string NamingStyleOptionNameLong = "naming-style";

	internal const char FolderProcessTypeOptionNameShort = 'f';
	internal const string FolderProcessTypeOptionNameLong = "process-type";

	internal const char NumberNamingTextStyleOptionNameShort = 'n';
	internal const string NumberNamingTextStyleOptionNameLong = "number-style";

	internal const char CopyNoPhotoDateTimeTakenActionOptionNameShort = NoPhotoDateTimeTakenActionOptionNameShort;
	internal const string CopyNoPhotoDateTimeTakenActionOptionNameLong = NoPhotoDateTimeTakenActionOptionNameLong;

	internal const char GroupByFolderTypeOptionNameShort = 'g';
	internal const string GroupByFolderTypeOptionNameLong = "group-by";

	internal const char FolderAppendTypeOptionNameShort = 'a';
	internal const string FolderAppendTypeOptionNameLong = "folder-append";

	internal const char FolderAppendLocationTypeOptionNameShort = 'p';
	internal const string FolderAppendLocationTypeOptionNameLong = "folder-append-location";

	internal const char CopyNoCoordinateActionOptionNameShort = NoCoordinateActionOptionNameShort;
	internal const string CopyNoCoordinateActionOptionNameLong = NoCoordinateActionOptionNameLong;

	internal const char IsDryRunOptionNameShort = 'd';
	internal const string IsDryRunOptionNameLong = "dry-run";

	#endregion

	#region Info

	internal const char AllFoldersOptionNameShort = 'a';
	internal const string AllFoldersOptionNameLong = "all-folders";

	internal const char InfoNoPhotoDateTimeTakenActionOptionNameShort = NoPhotoDateTimeTakenActionOptionNameShort;
	internal const string InfoNoPhotoDateTimeTakenActionOptionNameLong = NoPhotoDateTimeTakenActionOptionNameLong;

	internal const char InfoNoCoordinateActionOptionNameShort = NoCoordinateActionOptionNameShort;
	internal const string InfoNoCoordinateActionOptionNameLong = NoCoordinateActionOptionNameLong;

	#endregion

	#region Address Options

	internal const char AddressListTypeTypeOptionNameShort = 't';
	internal const string AddressListTypeOptionNameLong = "type";

	#endregion

	#region Settings Options

	internal const char KeyOptionNameShort = 'k';
	internal const string KeyOptionNameLong = "key";

	internal const char ValueOptionNameShort = 'v';
	internal const string ValueOptionNameLong = "value";

	internal const char ResetOptionNameShort = 'r';
	internal const string ResetOptionNameLong = "reset";

	#endregion

	#region Shared

	internal const char OutputPathOptionNameShort = 'o';
	internal const string OutputPathOptionNameLong = "output";

	internal const char InputPathOptionNameShort = 'i';
	internal const string InputPathOptionNameLong = "input";

	internal const char ReverseGeocodeProvidersOptionNameShort = 'e';
	internal const string ReverseGeocodeProvidersOptionNameLong = "reverse-geocode";

	internal const char BigDataCloudApiKeyOptionNameShort = 'b';
	internal const string BigDataCloudApiKeyOptionNameLong = "bigdatacloud-key";

	internal const char BigDataCloudAdminLevelsOptionNameShort = 'u';
	internal const string BigDataCloudAdminLevelsOptionNameLong = "bigdatacloud-levels";

	internal const char GoogleMapsAddressTypesOptionNameShort = 'm';
	internal const string GoogleMapsAddressTypesOptionNameLong = "googlemaps-types";

	internal const char GoogleMapsApiKeyOptionNameShort = 'k';
	internal const string GoogleMapsApiKeyOptionNameLong = "googlemaps-key";

	internal const char OpenStreetMapPropertiesOptionNameShort = 'r';
	internal const string OpenStreetMapPropertiesOptionNameLong = "openstreetmap-properties";

	internal const char LocationIqApiKeyOptionNameShort = 'q';
	internal const string LocationIqApiKeyOptionNameLong = "locationiq-key";

	internal const char HasPaidLicenseOptionNameShort = 'h';
	internal const string HasPaidLicenseOptionNameLong = "has-paid-license";

	internal const char LanguageOptionNameShort = 'l';
	internal const string LanguageOptionNameLong = "language";

	#endregion

	#region Different but similar purpose shared same options

	private const char NoPhotoDateTimeTakenActionOptionNameShort = 't';
	private const string NoPhotoDateTimeTakenActionOptionNameLong = "no-taken-date";

	private const char NoCoordinateActionOptionNameShort = 'c';
	private const string NoCoordinateActionOptionNameLong = "no-coordinate";

	#endregion
}
