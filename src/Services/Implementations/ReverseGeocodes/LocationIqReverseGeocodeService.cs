namespace PhotoCli.Services.Implementations.ReverseGeocodes;

public class LocationIqReverseGeocodeService : OpenStreetMapReverseGeocodeServiceBase, ILocationIqReverseGeocodeService
{
	private readonly ApiKeyStore _apiKeyStore;

	public LocationIqReverseGeocodeService(HttpClient httpClient, ILogger<LocationIqReverseGeocodeService> logger, ApiKeyStore apiKeyStore, ICoordinateCache<OpenStreetMapResponse> coordinateCache)
		: base(httpClient, logger, coordinateCache)
	{
		_apiKeyStore = apiKeyStore;
	}

	protected override string RequestUri(ReverseGeocodeRequest request)
	{
		_ = _apiKeyStore.LocationIq ?? throw new PhotoCliException($"{nameof(CopyOptions.LocationIqApiKey)} must be exists");
		return $"{base.RequestUri(request)}&key={_apiKeyStore.LocationIq}";
	}
}
