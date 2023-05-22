using CommandLine;

namespace PhotoCli.Options;

[Verb(OptionNames.CopyVerb, HelpText = "Copies photos into new folder hierarchy with given arguments using photograph's taken date and coordinate address (reverse geocode).")]
public class CopyOptions : IReverseGeocodeOptions
{
	// Notes: Constructor parameters and properties should be in same order for Immutable Options Type in CommandLineParser.
	// ref: https://github.com/commandlineparser/commandline/wiki/Immutable-Options-Type
	public CopyOptions(
		// Required
		string outputPath, NamingStyle namingStyle, FolderProcessType folderProcessType, NumberNamingTextStyle numberNamingTextStyle,
		// Optional
		CopyInvalidFormatAction invalidFileFormatAction, CopyNoPhotoTakenDateAction noPhotoTakenDateAction, CopyNoCoordinateAction noCoordinateAction,
		string? inputPath = null, bool isDryRun = false, GroupByFolderType? groupByFolderType = null, FolderAppendType? folderAppendType = null,
		FolderAppendLocationType? folderAppendLocationType = null, bool verify = false,
		// ReverseGeocode - Shared
		ReverseGeocodeProvider reverseGeoCodeProvider = ReverseGeocodeProvider.Disabled, string? bigDataCloudApiKey = null, IEnumerable<int>? bigDataCloudAdminLevels = null,
		IEnumerable<string>? googleMapsAddressTypes = null, string? googleMapsApiKey = null, IEnumerable<string>? openStreetMapProperties = null,
		string? locationIqApiKey = null, bool? hasPaidLicense = null, string? language = null)
	{
		// Required
		OutputPath = outputPath;
		NamingStyle = namingStyle;
		FolderProcessType = folderProcessType;
		NumberNamingTextStyle = numberNamingTextStyle;

		// Optional
		InvalidFileFormatAction = invalidFileFormatAction;
		NoPhotoTakenDateAction = noPhotoTakenDateAction;
		NoCoordinateAction = noCoordinateAction;
		InputPath = inputPath;
		IsDryRun = isDryRun;
		GroupByFolderType = groupByFolderType;
		FolderAppendType = folderAppendType;
		FolderAppendLocationType = folderAppendLocationType;
		Verify = verify;

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
	}

	#region Required

	[Option(OptionNames.OutputPathOptionNameShort, OptionNames.OutputPathOptionNameLong, HelpText = HelpTexts.OutputPathCopy)]
	public string OutputPath { get; }

	[Option(OptionNames.NamingStyleOptionNameShort, OptionNames.NamingStyleOptionNameLong, HelpText = HelpTexts.NamingStyle)]
	public NamingStyle NamingStyle { get; }

	[Option(OptionNames.FolderProcessTypeOptionNameShort, OptionNames.FolderProcessTypeOptionNameLong, HelpText = HelpTexts.FolderProcessType)]
	public FolderProcessType FolderProcessType { get; }

	[Option(OptionNames.NumberNamingTextStyleOptionNameShort, OptionNames.NumberNamingTextStyleOptionNameLong, HelpText = HelpTexts.NumberNamingTextStyle)]
	public NumberNamingTextStyle NumberNamingTextStyle { get; }

	[Option(OptionNames.CopyInvalidFormatActionOptionNameShort, OptionNames.CopyInvalidFormatActionOptionNameLong, HelpText = HelpTexts.CopyInvalidFormatAction)]
	public CopyInvalidFormatAction InvalidFileFormatAction { get; }

	[Option(OptionNames.CopyNoPhotoDateTimeTakenActionOptionNameShort, OptionNames.CopyNoPhotoDateTimeTakenActionOptionNameLong, HelpText = HelpTexts.CopyNoPhotoTakenDateAction)]
	public CopyNoPhotoTakenDateAction NoPhotoTakenDateAction { get; }

	[Option(OptionNames.CopyNoCoordinateActionOptionNameShort, OptionNames.CopyNoCoordinateActionOptionNameLong, HelpText = HelpTexts.CopyNoCoordinateAction)]
	public CopyNoCoordinateAction NoCoordinateAction { get; }

	#endregion

	#region Optional

	[Option(OptionNames.InputPathOptionNameShort, OptionNames.InputPathOptionNameLong, HelpText = HelpTexts.InputPath)]
	public string? InputPath { get; }

	[Option(OptionNames.IsDryRunOptionNameShort, OptionNames.IsDryRunOptionNameLong, HelpText = HelpTexts.IsDryRun)]
	public bool IsDryRun { get; }

	[Option(OptionNames.GroupByFolderTypeOptionNameShort, OptionNames.GroupByFolderTypeOptionNameLong, HelpText = HelpTexts.GroupByFolderType)]
	public GroupByFolderType? GroupByFolderType { get; }

	[Option(OptionNames.FolderAppendTypeOptionNameShort, OptionNames.FolderAppendTypeOptionNameLong, HelpText = HelpTexts.FolderAppendType)]
	public FolderAppendType? FolderAppendType { get; }

	[Option(OptionNames.FolderAppendLocationTypeOptionNameShort, OptionNames.FolderAppendLocationTypeOptionNameLong, HelpText = HelpTexts.FolderAppendLocationType)]
	public FolderAppendLocationType? FolderAppendLocationType { get; }

	[Option(OptionNames.VerifyOptionNameShort, OptionNames.VerifyOptionNameLong, HelpText = HelpTexts.Verify)]
	public bool Verify { get; }

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

	#endregion
}
