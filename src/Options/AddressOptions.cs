using CommandLine;

namespace PhotoCli.Options;

[Verb(OptionNames.AddressVerb, HelpText = "Get address (reverse geocode) of single photo.")]
public class AddressOptions : IReverseGeocodeOptions
{
	public AddressOptions(
		// Required
		string inputPath, ReverseGeocodeProvider reverseGeoCodeProvider, AddressListType addressListType,
		// ReverseGeocode - Shared
		string? bigDataCloudApiKey = null, IEnumerable<int>? bigDataCloudAdminLevels = null, IEnumerable<string>? googleMapsAddressTypes = null, string? googleMapsApiKey = null,
		IEnumerable<string>? openStreetMapProperties = null, string? locationIqApiKey = null, bool? hasPaidLicense = null, string? language = null)
	{
		// Required
		InputPath = inputPath;
		ReverseGeocodeProvider = reverseGeoCodeProvider;
		AddressListType = addressListType;

		// ReverseGeocode
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

	[Option(OptionNames.InputPathOptionNameShort, OptionNames.InputPathOptionNameLong, HelpText = HelpTexts.InputPath)]
	public string InputPath { get; }

	[Option(OptionNames.ReverseGeocodeProvidersOptionNameShort, OptionNames.ReverseGeocodeProvidersOptionNameLong, HelpText = HelpTexts.ReverseGeocodeProvider)]
	public ReverseGeocodeProvider ReverseGeocodeProvider { get; }

	[Option(OptionNames.AddressListTypeTypeOptionNameShort, OptionNames.AddressListTypeOptionNameLong, HelpText = HelpTexts.AddressListType)]
	public AddressListType AddressListType { get; }

	#endregion

	#region Reverse Geocode

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
