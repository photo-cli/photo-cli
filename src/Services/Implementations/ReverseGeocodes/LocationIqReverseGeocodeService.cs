namespace PhotoCli.Services.Implementations.ReverseGeocodes;

public class LocationIqReverseGeocodeService : OpenStreetMapReverseGeocodeServiceBase, ILocationIqReverseGeocodeService
{
	private readonly ApiKeyStore _apiKeyStore;

	public LocationIqReverseGeocodeService(HttpClient httpClient, ILogger<LocationIqReverseGeocodeService> logger, ApiKeyStore apiKeyStore) : base(httpClient, logger)
	{
		_apiKeyStore = apiKeyStore;
	}

	protected override string RequestUri(Coordinate coordinate)
	{
		_ = _apiKeyStore.LocationIq ?? throw new PhotoCliException($"{nameof(CopyOptions.LocationIqApiKey)} must be exists");
		return $"{base.RequestUri(coordinate)}&key={_apiKeyStore.LocationIq}";
	}
}
