using System.Diagnostics;
using System.Net.Http.Json;
using PhotoCli.Utils.Logging;

namespace PhotoCli.Services.Implementations.ReverseGeocodes;

public class BigDataCloudReverseGeocodeService : IBigDataCloudReverseGeocodeService
{
	private const ReverseGeocodeProvider Provider = ReverseGeocodeProvider.BigDataCloud;
	private readonly ApiKeyStore _apiKeyStore;
	private readonly HttpClient _httpClient;
	private readonly ILogger<BigDataCloudReverseGeocodeService> _logger;
	private readonly IReverseGeocodeCache<BigDataCloudResponse> _reverseGeocodeCache;
	private readonly Statistics _statistics;

	public BigDataCloudReverseGeocodeService(
		HttpClient httpClient,
		ILogger<BigDataCloudReverseGeocodeService> logger,
		ApiKeyStore apiKeyStore,
		IReverseGeocodeCache<BigDataCloudResponse> reverseGeocodeCache,
		Statistics statistics)
	{
		_httpClient = httpClient;
		_logger = logger;
		_apiKeyStore = apiKeyStore;
		_reverseGeocodeCache = reverseGeocodeCache;
		_statistics = statistics;
	}

	public async Task<ReverseGeocodeAddressResult> Get(Coordinate coordinate, PhotoFile photoFile, string? language, IEnumerable<int> adminLevels)
	{
		var bigDataCloudRequest = new ReverseGeocodeRequest(coordinate, language);
		var bigDataCloudResponse = await SerializeFullResponse(bigDataCloudRequest);
		var administratorLevels = bigDataCloudResponse?.LocalityInfo?.Administrative;
		if (administratorLevels == null)
		{
			_logger.LogCritical("Can't get {Type}", nameof(BigDataCloudAdministrative));
			return new ReverseGeocodeAddressResult(ArraySegment<string>.Empty, false);
		}

		var levelNames = new List<string>();
		var allPhotosHasReverseGeocodedAsRequested = true;
		var addressIndex = 1;

		var namesByLevel = GetAdminLevels(administratorLevels);

		foreach (var adminLevelIndex in adminLevels)
		{
			_logger.LogDebug("Requesting Address index of {AddressIndex} with {BigDataCloudAdminLevel}", addressIndex, adminLevelIndex);
			if (namesByLevel.TryGetValue(adminLevelIndex, out var levelName))
			{
				levelNames.Add(levelName);
			}
			else
			{
				_logger.LogErrorWithPath("Requested address level: {BigDataCloudAdminLevel} on index #{AddressIndex}, not found on BigDataCloudAdmin's response. Available levels found: {AvailableLevels}",
					photoFile.SourceFullPath, adminLevelIndex, addressIndex, namesByLevel);

				allPhotosHasReverseGeocodedAsRequested = false;
			}
			++addressIndex;
		}

		return new ReverseGeocodeAddressResult(levelNames, allPhotosHasReverseGeocodedAsRequested);
	}

	public async Task<BigDataCloudResponse?> SerializeFullResponse(ReverseGeocodeRequest request)
	{
		try
		{
			var queryString = $"?latitude={request.Coordinate.Latitude}&longitude={request.Coordinate.Longitude}&key={_apiKeyStore.BigDataCloud}";

			if (request.Language != null)
				queryString += $"&localityLanguage={request.Language}";

			var (cacheHit, cachedResponse) = await _reverseGeocodeCache.TryGet(request, Provider);
			if (cacheHit)
				return cachedResponse;

			var bigDataCloudResponse = await _httpClient.GetFromJsonAsync<BigDataCloudResponse>(queryString, StaticOptions.JsonSerializerOptions);
			++_statistics.ReserveGeocodeRequestSent;
			await _reverseGeocodeCache.SetResponse(request, Provider, bigDataCloudResponse);
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

	private Dictionary<int, string> GetAdminLevels(IEnumerable<BigDataCloudAdministrative> administratorLevels)
	{
		var namesByLevel = new Dictionary<int, string>();
		foreach (var administratorLevel in administratorLevels.OrderByDescending(o => o.Order))
		{
			if (!administratorLevel.AdminLevel.HasValue)
			{
				_logger.LogError("Empty admin level found in BigDataCloudAdministrative response. Name: {Name}, IsoName: {IsoName}",
					administratorLevel.Name, administratorLevel.IsoName);

				continue;
			}

			var value = administratorLevel.Name ?? administratorLevel.IsoName;
			if (value.IsMissing())
			{
				_logger.LogError("Empty admin level name found in BigDataCloudAdministrative response. AdminLevel: {AdminLevel}, IsoName: {IsoName}",
					administratorLevel.AdminLevel, administratorLevel.IsoName);

				continue;
			}

			if (!namesByLevel.TryAdd(administratorLevel.AdminLevel.Value, value))
				_logger.LogWarning("Multiple admin level {Level} result found in list {Levels}", administratorLevel.AdminLevel, administratorLevels);
		}
		return namesByLevel;
	}
}
