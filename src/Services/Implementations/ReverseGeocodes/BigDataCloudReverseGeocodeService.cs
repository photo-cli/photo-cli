using System.Net.Http.Json;

namespace PhotoCli.Services.Implementations.ReverseGeocodes;

public class BigDataCloudReverseGeocodeService : IBigDataCloudReverseGeocodeService
{
	private readonly ApiKeyStore _apiKeyStore;
	private readonly HttpClient _httpClient;
	private readonly ILogger<BigDataCloudReverseGeocodeService> _logger;
	private readonly ICoordinateCache<BigDataCloudResponse> _coordinateCache;

	public BigDataCloudReverseGeocodeService(
		HttpClient httpClient,
		ILogger<BigDataCloudReverseGeocodeService> logger,
		ApiKeyStore apiKeyStore,
		ICoordinateCache<BigDataCloudResponse> coordinateCache)
	{
		_httpClient = httpClient;
		_logger = logger;
		_apiKeyStore = apiKeyStore;
		_coordinateCache = coordinateCache;
	}

	public async Task<IEnumerable<string>> Get(Coordinate coordinate, string? language, IEnumerable<int> adminLevels)
	{
		var bigDataCloudRequest = new ReverseGeocodeRequest(coordinate, language);
		var bigDataCloudResponse = await SerializeFullResponse(bigDataCloudRequest);
		var administratorLevels = bigDataCloudResponse?.LocalityInfo?.Administrative;
		if (administratorLevels == null)
		{
			_logger.LogCritical("Can't get {Type}", nameof(BigDataCloudAdministrative));
			return ArraySegment<string>.Empty;
		}

		var levelNames = new List<string>();
		foreach (var adminLevel in adminLevels)
		{
			var levelName = GetAdminLevelName(adminLevel, administratorLevels);
			if (levelName != null)
				levelNames.Add(levelName);
		}

		return levelNames;
	}

	public async Task<BigDataCloudResponse?> SerializeFullResponse(ReverseGeocodeRequest request)
	{
		try
		{
			var queryString = $"?latitude={request.Coordinate.Latitude}&longitude={request.Coordinate.Longitude}&key={_apiKeyStore.BigDataCloud}";

			if (request.Language != null)
				queryString += $"&localityLanguage={request.Language}";

			if (_coordinateCache.TryGet(request, out var cachedData))
				return cachedData;

			var bigDataCloudResponse = await _httpClient.GetFromJsonAsync<BigDataCloudResponse>(queryString, StaticOptions.JsonSerializerOptions);
			_coordinateCache.SetResponse(request, bigDataCloudResponse);

			return bigDataCloudResponse;
		}
		catch (Exception e)
		{
			_logger.LogCritical(e, "Can't get & serialize {Type}", nameof(BigDataCloudResponse));
			return null;
		}
	}

	public async Task<Dictionary<string, object>> AllAvailableReverseGeocodes(Coordinate coordinate, string? language)
	{
		var bigDataCloudRequest = new ReverseGeocodeRequest(coordinate, language);
		var bigDataCloudResponse = await SerializeFullResponse(bigDataCloudRequest);
		var administratorLevels = bigDataCloudResponse?.LocalityInfo?.Administrative;
		if (administratorLevels == null)
			throw new PhotoCliException($"Can't get {nameof(BigDataCloudAdministrative)}");

		var addressPropertyValueDict = new Dictionary<string, object>();
		foreach (var administratorLevel in administratorLevels)
		{
			if (administratorLevel.AdminLevel == null || administratorLevel.Name == null)
				continue;
			var key = $"AdminLevel{administratorLevel.AdminLevel.ToString()}";
			if (addressPropertyValueDict.TryGetValue(key, out var existingValue))
			{
				_logger.LogWarning("BigDataCloud returned inconsistent/duplicate data. Multiple admin level on {Level} found. Used value: {Existing}, duplicate value: {Duplicate}",
					administratorLevel.AdminLevel, existingValue, administratorLevel.Name);

				continue;
			}
			addressPropertyValueDict.Add(key, administratorLevel.Name);
		}

		return addressPropertyValueDict;
	}

	private string? GetAdminLevelName(int adminLevel, IEnumerable<BigDataCloudAdministrative> administratorLevels)
	{
		var administratorLevelList = administratorLevels.Where(s => s.AdminLevel == adminLevel).ToList();
		if (administratorLevelList.Count > 1)
			_logger.LogWarning("Multiple admin level {Level} result found in list {Levels}", adminLevel, administratorLevels);
		var administratorLevel = administratorLevelList.FirstOrDefault();
		if (administratorLevel == null)
			return null;
		return administratorLevel.Name ?? administratorLevel.IsoName ?? null;
	}
}
