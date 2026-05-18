namespace PhotoCli.Options;

internal static class OptionNames
{
	/* Short Option Name Usages
		a - copy, info, archive, list
		b - copy, info, address, archive
		c - copy, info, archive
		d - copy, archive
		e - copy, info, address, archive, list
		f - copy, archive
		g - copy
		h - copy, info, archive
		i - copy, info, address, archive
		j -
		k - copy, info, address, settings, archive
		l - copy, info, address, archive
		m - copy, info, address, archive
		n - copy, list
		o - copy, info, archive
		p - copy, archive
		q - copy, info, address, archive
		r - copy, info, address, settings, archive
		s - copy, archive, list
		t - copy, info, address, archive, list
		u - copy, info, address, archive
		v - copy, settings
		w - copy, info, archive
		x - copy, info, archive
		y - archive
		z - copy, info, archive
	*/

	internal const string ApplicationAlias = "photo-cli";

	internal const string AddressVerb = "address";
	internal const string CopyVerb = "copy";
	internal const string InfoVerb = "info";
	internal const string SettingsVerb = "settings";
	internal const string ArchiveVerb = "archive";
	internal const string ListVerb = "list";
	internal const string McpVerb = "mcp";

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

	internal const char CopyInvalidFormatActionOptionNameShort = InvalidFormatActionOptionNameShort;
	internal const string CopyInvalidFormatActionOptionNameLong = InvalidFormatActionOptionNameLong;

	internal const char CopyExpectedDayRangeShort = ExpectedDayRangeShort;
	internal const string CopyExpectedDayRangeLong = ExpectedDayRangeLong;

	internal const char VerifyOptionNameShort = 'v';
	internal const string VerifyOptionNameLong = "verify";

	#endregion

	#region Info

	internal const char AllFoldersOptionNameShort = 'a';
	internal const string AllFoldersOptionNameLong = "all-folders";

	internal const char InfoNoPhotoDateTimeTakenActionOptionNameShort = NoPhotoDateTimeTakenActionOptionNameShort;
	internal const string InfoNoPhotoDateTimeTakenActionOptionNameLong = NoPhotoDateTimeTakenActionOptionNameLong;

	internal const char InfoNoCoordinateActionOptionNameShort = NoCoordinateActionOptionNameShort;
	internal const string InfoNoCoordinateActionOptionNameLong = NoCoordinateActionOptionNameLong;

	internal const char InfoInvalidFormatActionOptionNameShort = InvalidFormatActionOptionNameShort;
	internal const string InfoInvalidFormatActionOptionNameLong = InvalidFormatActionOptionNameLong;

	internal const char InfoExpectedDayRangeShort = ExpectedDayRangeShort;
	internal const string InfoExpectedDayRangeLong = ExpectedDayRangeLong;

	#endregion

	#region Archive

	internal const char ArchiveNoPhotoDateTimeTakenActionOptionNameShort = NoPhotoDateTimeTakenActionOptionNameShort;
	internal const string ArchiveNoPhotoDateTimeTakenActionOptionNameLong = NoPhotoDateTimeTakenActionOptionNameLong;

	internal const char ArchiveNoCoordinateActionOptionNameShort = NoCoordinateActionOptionNameShort;
	internal const string ArchiveNoCoordinateActionOptionNameLong = NoCoordinateActionOptionNameLong;

	internal const char ArchiveInvalidFormatActionOptionNameShort = InvalidFormatActionOptionNameShort;
	internal const string ArchiveInvalidFormatActionOptionNameLong = InvalidFormatActionOptionNameLong;

	internal const char ArchiveExpectedDayRangeShort = ExpectedDayRangeShort;
	internal const string ArchiveExpectedDayRangeLong = ExpectedDayRangeLong;

	internal const char AlbumTypeShort = 'y';
	internal const string AlbumTypeLong = "album-type";

	internal const char AlbumNameNewShort = 'a';
	internal const string AlbumNameNewLong = "album-name";

	internal const char AlbumIdUpdateShort = 'p';
	internal const string AlbumIdUpdateLong = "update-album";

	internal const char AutoReverseGeocodeAlbumShort = 's';
	internal const string AutoReverseGeocodeAlbumLong = "auto-reverse-geocode-album";

	internal const char AlbumDeleteSourceShort = 'f';
	internal const string AlbumDeleteSourceLong = "delete-on-source";

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

	internal const char ArchivePathOptionNameShort = 'i';
	internal const string ArchivePathOptionNameLong = "input";

	internal const char IsDryRunOptionNameShort = 'd';
	internal const string IsDryRunOptionNameLong = "dry-run";

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

	internal const char MissingReverseGeocodeActionShort = 'z';
	internal const string MissingReverseGeocodeActionLong = "missing-reverse-geocode";

	#endregion

	#region Different but similar purpose shared same options

	private const char NoPhotoDateTimeTakenActionOptionNameShort = 't';
	private const string NoPhotoDateTimeTakenActionOptionNameLong = "no-taken-date";

	private const char NoCoordinateActionOptionNameShort = 'c';
	private const string NoCoordinateActionOptionNameLong = "no-coordinate";

	private const char InvalidFormatActionOptionNameShort = 'x';
	private const string InvalidFormatActionOptionNameLong = "invalid-format";

	private const char ExpectedDayRangeShort = 'w';
	private const string ExpectedDayRangeLong = "expected-day-range";

	#endregion

	#region List

	internal const char ListTypeShort = 't';
	internal const string ListTypeLong = "type";

	internal const char AlbumIdShort = 'n';
	internal const string AlbumIdLong = "id";

	internal const char AlbumNameShort = 'a';
	internal const string AlbumNameLong = "name";

	internal const char YearShort = 'y';
	internal const string YearLong = "year";

	internal const char MonthShort = 'm';
	internal const string MonthLong = "month";

	internal const char DayShort = 'd';
	internal const string DayLong = "day";

	internal const char RawOutputShort = 'r';
	internal const string RawOutputLong = "raw";

	internal const char StartDateShort = 's';
	internal const string StartDateLong = "start-date";

	internal const char EndDateShort = 'e';
	internal const string EndDateLong = "end-date";

	#endregion
}
