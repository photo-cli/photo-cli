using CommandLine;

namespace PhotoCli.Options;

[Verb(OptionNames.ArchiveVerb, HelpText = "Archives photos into same specific folder, optionally groups them by albums (date range, reverse geocode or individual), and indexes photo taken date, address (reverse geocode) information into SQLite database.")]
public class ArchiveOptions : IActionableReverseGeocodeOptions
{
	// Notes: Constructor parameters and properties should be in the same order for Immutable Options Type in CommandLineParser.
	// ref: https://github.com/commandlineparser/commandline/wiki/Immutable-Options-Type
	public ArchiveOptions(
		// Required
		// Optional
		string? outputPath = null,
		string? inputPath = null,
		bool isDryRun = false, ArchiveInvalidFormatAction invalidFileFormatAction = ArchiveInvalidFormatAction.Continue,
		ArchiveNoPhotoTakenDateAction noPhotoTakenDateAction = ArchiveNoPhotoTakenDateAction.Continue, ArchiveNoCoordinateAction noCoordinateAction = ArchiveNoCoordinateAction.Continue,
		short? expectedDayRange = null, ArchiveAlbumType? albumType = null, string? albumNameNew = null, int? albumIdUpdate = null, bool autoReverseGeocodeAlbum = false, bool deleteSource = false,
		// ReverseGeocode - Shared
		ReverseGeocodeProvider reverseGeoCodeProvider = ReverseGeocodeProvider.Disabled, string? bigDataCloudApiKey = null, IEnumerable<int>? bigDataCloudAdminLevels = null,
		IEnumerable<string>? googleMapsAddressTypes = null, string? googleMapsApiKey = null, IEnumerable<string>? openStreetMapProperties = null,
		string? locationIqApiKey = null, bool? hasPaidLicense = null, string? language = null, MissingReverseGeocodeAction missingReverseGeocodeAction = MissingReverseGeocodeAction.Continue,
		string? customDatabasePath = null)
	{
		// Required

		// Optional
		OutputPath = outputPath;
		InputPath = inputPath;
		IsDryRun = isDryRun;
		InvalidFileFormatAction = invalidFileFormatAction;
		NoPhotoTakenDateAction = noPhotoTakenDateAction;
		NoCoordinateAction = noCoordinateAction;
		ExpectedDayRange = expectedDayRange;
		AlbumType = albumType;
		AlbumNameNew = albumNameNew;
		AlbumIdUpdate = albumIdUpdate;
		AutoReverseGeocodeAlbum = autoReverseGeocodeAlbum;
		DeleteSource = deleteSource;

		// ReverseGeocode
		ReverseGeocodeProvider = reverseGeoCodeProvider;
		BigDataCloudApiKey = bigDataCloudApiKey;
		BigDataCloudAdminLevels = bigDataCloudAdminLevels ?? new List<int>();
		GoogleMapsAddressTypes = googleMapsAddressTypes ?? new List<string>();
		GoogleMapsApiKey = googleMapsApiKey;
		OpenStreetMapProperties = openStreetMapProperties ?? new List<string>();
		LocationIqApiKey = locationIqApiKey;
		HasPaidLicense = hasPaidLicense;
		Language = language;
		MissingReverseGeocodeAction = missingReverseGeocodeAction;
		CustomDatabasePath = customDatabasePath;
	}

	#region Required
	#endregion

	#region Optional

	[Option(OptionNames.OutputPathOptionNameShort, OptionNames.OutputPathOptionNameLong, HelpText = HelpTexts.OutputPathCopy)]
	public string OutputPath { get; }

	[Option(OptionNames.ArchivePathOptionNameShort, OptionNames.ArchivePathOptionNameLong, HelpText = HelpTexts.InputPath)]
	public string? InputPath { get; }

	[Option(OptionNames.IsDryRunOptionNameShort, OptionNames.IsDryRunOptionNameLong, HelpText = HelpTexts.IsDryRun)]
	public bool IsDryRun { get; }

	[Option(OptionNames.ArchiveInvalidFormatActionOptionNameShort, OptionNames.ArchiveInvalidFormatActionOptionNameLong, HelpText = HelpTexts.ArchiveInvalidFormatAction)]
	public ArchiveInvalidFormatAction InvalidFileFormatAction { get; }

	[Option(OptionNames.ArchiveNoPhotoDateTimeTakenActionOptionNameShort, OptionNames.ArchiveNoPhotoDateTimeTakenActionOptionNameLong, HelpText = HelpTexts.ArchiveNoPhotoTakenDateAction)]
	public ArchiveNoPhotoTakenDateAction NoPhotoTakenDateAction { get; }

	[Option(OptionNames.ArchiveNoCoordinateActionOptionNameShort, OptionNames.ArchiveNoCoordinateActionOptionNameLong, HelpText = HelpTexts.ArchiveNoCoordinateAction)]
	public ArchiveNoCoordinateAction NoCoordinateAction { get; }

	[Option(OptionNames.ArchiveExpectedDayRangeShort, OptionNames.ArchiveExpectedDayRangeLong, HelpText = HelpTexts.ExpectedDayRange)]
	public short? ExpectedDayRange { get; }

	[Option(OptionNames.AlbumTypeShort, OptionNames.AlbumTypeLong, HelpText = HelpTexts.AlbumType)]
	public ArchiveAlbumType? AlbumType { get; }

	[Option(OptionNames.AlbumNameNewShort, OptionNames.AlbumNameNewLong, HelpText = HelpTexts.AlbumNameNew)]
	public string? AlbumNameNew { get; }

	[Option(OptionNames.AlbumIdUpdateShort, OptionNames.AlbumIdUpdateLong, HelpText = HelpTexts.AlbumIdUpdate)]
	public int? AlbumIdUpdate { get; }

	[Option(OptionNames.AutoReverseGeocodeAlbumShort, OptionNames.AutoReverseGeocodeAlbumLong, HelpText = HelpTexts.AutoReverseGeocodeAlbum)]
	public bool AutoReverseGeocodeAlbum { get; }

	[Option(OptionNames.AlbumDeleteSourceShort, OptionNames.AlbumDeleteSourceLong, HelpText = HelpTexts.AlbumDeleteSource)]
	public bool DeleteSource { get; }

	#endregion

	#region Reverse Geocode

	[Option(OptionNames.ReverseGeocodeProvidersOptionNameShort, OptionNames.ReverseGeocodeProvidersOptionNameLong, HelpText = HelpTexts.ReverseGeocodeProvider)]
	public ReverseGeocodeProvider ReverseGeocodeProvider { get; }

	[Option(OptionNames.BigDataCloudApiKeyOptionNameShort, OptionNames.BigDataCloudApiKeyOptionNameLong, HelpText = HelpTexts.BigDataCloudApiKey)]
	public string? BigDataCloudApiKey { get; }

	[Option(OptionNames.BigDataCloudAdminLevelsOptionNameShort, OptionNames.BigDataCloudAdminLevelsOptionNameLong, HelpText = HelpTexts.BigDataCloudAdminLevels)]
	public IEnumerable<int> BigDataCloudAdminLevels { get; }

	[Option(OptionNames.GoogleMapsAddressTypesOptionNameShort, OptionNames.GoogleMapsAddressTypesOptionNameLong, HelpText = HelpTexts.GoogleMapsAddressTypes)]
	public IEnumerable<string> GoogleMapsAddressTypes { get; }

	[Option(OptionNames.GoogleMapsApiKeyOptionNameShort, OptionNames.GoogleMapsApiKeyOptionNameLong, HelpText = HelpTexts.GoogleMapsApiKey)]
	public string? GoogleMapsApiKey { get; }

	[Option(OptionNames.OpenStreetMapPropertiesOptionNameShort, OptionNames.OpenStreetMapPropertiesOptionNameLong, HelpText = HelpTexts.OpenStreetMapProperties)]
	public IEnumerable<string> OpenStreetMapProperties { get; }

	[Option(OptionNames.LocationIqApiKeyOptionNameShort, OptionNames.LocationIqApiKeyOptionNameLong, HelpText = HelpTexts.LocationIqApiKey)]
	public string? LocationIqApiKey { get; }

	[Option(OptionNames.HasPaidLicenseOptionNameShort, OptionNames.HasPaidLicenseOptionNameLong, HelpText = HelpTexts.HasPaidLicense)]
	public bool? HasPaidLicense { get; }

	[Option(OptionNames.LanguageOptionNameShort, OptionNames.LanguageOptionNameLong, HelpText = HelpTexts.Language)]
	public string? Language { get; }

	[Option(OptionNames.MissingReverseGeocodeActionShort, OptionNames.MissingReverseGeocodeActionLong, HelpText = HelpTexts.MissingReverseGeocodeAction)]
	public MissingReverseGeocodeAction MissingReverseGeocodeAction { get; }

	[Option(OptionNames.CustomDatabasePathShort, OptionNames.CustomDatabasePathLong, HelpText = HelpTexts.CustomDatabasePath)]
	public string? CustomDatabasePath { get; }

	#endregion
}
