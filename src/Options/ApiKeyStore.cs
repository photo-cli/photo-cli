namespace PhotoCli.Options;

public class ApiKeyStore
{
	internal const string BigDataCloudApiKeyEnvironmentKey = "PHOTO_CLI_BIG_DATA_CLOUD_API_KEY";
	internal const string GoogleMapsApiKeyEnvironmentKey = "PHOTO_CLI_GOOGLE_MAPS_API_KEY";
	internal const string LocationIqApiKeyEnvironmentKey = "PHOTO_CLI_LOCATIONIQ_API_KEY";
	public ReverseGeocodeProvider? ReverseGeocodeProvider { get; init; }
	public string? BigDataCloud { get; init; }
	public string? GoogleMaps { get; init; }
	public string? LocationIq { get; init; }

	public static ApiKeyStore Build(IConfiguration configuration, IReverseGeocodeOptions options)
	{
		var bigDataCloud = options.BigDataCloudApiKey;
		if (bigDataCloud.IsMissing())
			bigDataCloud = configuration.GetSection(nameof(ToolOptions.BigDataCloudApiKey)).Value;
		if (bigDataCloud.IsMissing())
			bigDataCloud = configuration.GetSection(BigDataCloudApiKeyEnvironmentKey).Value;

		var googleMaps = options.GoogleMapsApiKey;
		if (googleMaps.IsMissing())
			googleMaps = configuration.GetSection(nameof(ToolOptions.GoogleMapsApiKey)).Value;
		if (googleMaps.IsMissing())
			googleMaps = configuration.GetSection(GoogleMapsApiKeyEnvironmentKey).Value;

		var locationIq = options.LocationIqApiKey;
		if (locationIq.IsMissing())
			locationIq = configuration.GetSection(nameof(ToolOptions.LocationIqApiKey)).Value;
		if (locationIq.IsMissing())
			locationIq = configuration.GetSection(LocationIqApiKeyEnvironmentKey).Value;

		return new ApiKeyStore
		{
			ReverseGeocodeProvider = options.ReverseGeocodeProvider,
			BigDataCloud = bigDataCloud,
			GoogleMaps = googleMaps,
			LocationIq = locationIq,
		};
	}
}
