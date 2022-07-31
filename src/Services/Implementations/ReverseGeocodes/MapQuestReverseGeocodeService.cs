namespace PhotoCli.Services.Implementations.ReverseGeocodes;

public class MapQuestReverseGeocodeService : OpenStreetMapReverseGeocodeServiceBase, IMapQuestReverseGeocodeService
{
	private readonly ApiKeyStore _apiKeyStore;

	public MapQuestReverseGeocodeService(HttpClient httpClient, ILogger<MapQuestReverseGeocodeService> logger, ApiKeyStore apiKeyStore) : base(httpClient, logger)
	{
		_apiKeyStore = apiKeyStore;
	}

	protected override string RequestUri(Coordinate coordinate)
	{
		_ = _apiKeyStore.MapQuest ?? throw new PhotoCliException($"{nameof(CopyOptions.MapQuestApiKey)} must be exists");
		return $"{base.RequestUri(coordinate)}&key={_apiKeyStore.MapQuest}";
	}
}
