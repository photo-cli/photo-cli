using CommandLine;

namespace PhotoCli.Options;

[Verb(OptionNames.InfoVerb, HelpText = "Creates a report (CSV file) listing all photo taken date and address (reverse geocode).")]
public class InfoOptions : IReverseGeocodeOptions
{
	// Notes: Constructor parameters and properties should be in same order for Immutable Options Type in CommandLineParser.
	// ref: https://github.com/commandlineparser/commandline/wiki/Immutable-Options-Type
	public InfoOptions(
		// Required
		string outputPath,
		// Optional
		string? inputPath = null, bool allFolders = false, InfoNoPhotoTakenDateAction noPhotoTakenDateAction = InfoNoPhotoTakenDateAction.Continue,
		InfoNoCoordinateAction noCoordinateAction = InfoNoCoordinateAction.Continue,
		// ReverseGeocode - Shared
		ReverseGeocodeProvider reverseGeoCodeProvider = ReverseGeocodeProvider.Disabled, string? bigDataCloudApiKey = null, IEnumerable<int>? bigDataCloudAdminLevels = null,
		IEnumerable<string>? googleMapsAddressTypes = null,
		string? googleMapsApiKey = null, IEnumerable<string>? openStreetMapProperties = null, string? mapQuestApiKey = null,
		string? locationIqApiKey = null, bool? hasPaidLicense = null, string? language = null)
	{
		// Required
		OutputPath = outputPath;

		// Optional
		InputPath = inputPath;
		AllFolders = allFolders;
		NoPhotoTakenDateAction = noPhotoTakenDateAction;
		NoCoordinateAction = noCoordinateAction;

		// ReverseGeocode
		ReverseGeocodeProvider = reverseGeoCodeProvider;
		BigDataCloudApiKey = bigDataCloudApiKey;
		BigDataCloudAdminLevels = bigDataCloudAdminLevels ?? new List<int>();
		GoogleMapsAddressTypes = googleMapsAddressTypes ?? new List<string>();
		GoogleMapsApiKey = googleMapsApiKey;
		OpenStreetMapProperties = openStreetMapProperties ?? new List<string>();
		MapQuestApiKey = mapQuestApiKey;
		LocationIqApiKey = locationIqApiKey;
		HasPaidLicense = hasPaidLicense;
		Language = language;
	}

	#region Required

	[Option(OptionNames.OutputPathOptionNameShort, OptionNames.OutputPathOptionNameLong, HelpText = HelpTexts.OutputPathInfo)]
	public string OutputPath { get; }

	#endregion

	#region Optional

	[Option(OptionNames.InputPathOptionNameShort, OptionNames.InputPathOptionNameLong, HelpText = HelpTexts.InputPath)]
	public string? InputPath { get; }

	[Option(OptionNames.AllFoldersOptionNameShort, OptionNames.AllFoldersOptionNameLong, HelpText = HelpTexts.AllFolders)]
	public bool AllFolders { get; }

	[Option(OptionNames.InfoNoPhotoDateTimeTakenActionOptionNameShort, OptionNames.InfoNoPhotoDateTimeTakenActionOptionNameLong, HelpText = HelpTexts.InfoNoPhotoTakenDateAction)]
	public InfoNoPhotoTakenDateAction NoPhotoTakenDateAction { get; }

	[Option(OptionNames.InfoNoCoordinateActionOptionNameShort, OptionNames.InfoNoCoordinateActionOptionNameLong, HelpText = HelpTexts.InfoNoCoordinateAction)]
	public InfoNoCoordinateAction NoCoordinateAction { get; }

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

	[Option(OptionNames.MapQuestApiKeyOptionNameShort, OptionNames.MapQuestApiKeyOptionNameLong, HelpText = HelpTexts.MapQuestApiKey)]
	public string? MapQuestApiKey { get; }

	[Option(OptionNames.LocationIqApiKeyOptionNameShort, OptionNames.LocationIqApiKeyOptionNameLong, HelpText = HelpTexts.LocationIqApiKey)]
	public string? LocationIqApiKey { get; }

	[Option(OptionNames.HasPaidLicenseOptionNameShort, OptionNames.HasPaidLicenseOptionNameLong, HelpText = HelpTexts.HasPaidLicense)]
	public bool? HasPaidLicense { get; }

	[Option(OptionNames.LanguageOptionNameShort, OptionNames.LanguageOptionNameLong, HelpText = HelpTexts.Language)]
	public string? Language { get; }

	#endregion
}
