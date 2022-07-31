namespace PhotoCli.Options;

public interface IReverseGeocodeOptions
{
	ReverseGeocodeProvider ReverseGeocodeProvider { get; }
	string? BigDataCloudApiKey { get; }
	IEnumerable<int> BigDataCloudAdminLevels { get; }
	IEnumerable<string> GoogleMapsAddressTypes { get; }
	string? GoogleMapsApiKey { get; }
	IEnumerable<string> OpenStreetMapProperties { get; }
	string? MapQuestApiKey { get; }
	string? LocationIqApiKey { get; }
	bool? HasPaidLicense { get; }
	string? Language { get; }
}
