using System.Net.Http.Json;

namespace PhotoCli.Services.Implementations.ReverseGeocodes;

public class BigDataCloudReverseGeocodeService : IBigDataCloudReverseGeocodeService
{
	private readonly ApiKeyStore _apiKeyStore;
	private readonly HttpClient _httpClient;
	private readonly ILogger<BigDataCloudReverseGeocodeService> _logger;

	public BigDataCloudReverseGeocodeService(HttpClient httpClient, ILogger<BigDataCloudReverseGeocodeService> logger, ApiKeyStore apiKeyStore)
	{
		_httpClient = httpClient;
		_logger = logger;
		_apiKeyStore = apiKeyStore;
	}

	public async Task<IEnumerable<string>> Get(Coordinate coordinate, string? language, IEnumerable<int> adminLevels)
	{
		var bigDataCloudResponse = await SerializeFullResponse(coordinate, language);
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

	public async Task<BigDataCloudResponse?> SerializeFullResponse(Coordinate coordinate, string? language)
	{
		try
		{
			var queryString = $"?latitude={coordinate.Latitude}&longitude={coordinate.Longitude}&key={_apiKeyStore.BigDataCloud}";
			if (language != null)
				queryString += $"&localityLanguage={language}";
			var bigDataCloudResponse = await _httpClient.GetFromJsonAsync<BigDataCloudResponse>(queryString, StaticOptions.JsonSerializerOptions);
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
		var bigDataCloudResponse = await SerializeFullResponse(coordinate, language);
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
