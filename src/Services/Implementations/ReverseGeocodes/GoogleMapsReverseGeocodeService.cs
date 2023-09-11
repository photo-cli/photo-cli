using System.Net.Http.Json;

namespace PhotoCli.Services.Implementations.ReverseGeocodes;

public class GoogleMapsReverseGeocodeService : IGoogleMapsReverseGeocodeService
{
	private static readonly string[] ResultListTypeIgnore = { "locality" };

	private readonly ApiKeyStore _apiKeyStore;
	private readonly HttpClient _httpClient;
	private readonly ILogger<GoogleMapsReverseGeocodeService> _logger;
	private readonly ICoordinateCache<GoogleMapsResponse> _coordinateCache;

	public GoogleMapsReverseGeocodeService(HttpClient httpClient, ApiKeyStore apiKeyStore, ILogger<GoogleMapsReverseGeocodeService> logger, ICoordinateCache<GoogleMapsResponse> coordinateCache)
	{
		_httpClient = httpClient;
		_apiKeyStore = apiKeyStore;
		_logger = logger;
		_coordinateCache = coordinateCache;
	}

	public async Task<IEnumerable<string>> Get(Coordinate coordinate, string? language, IEnumerable<string> requestedAddressTypes)
	{
		var googleMapsRequest = new ReverseGeocodeRequest(coordinate, language);
		var googleMapsResponse = await SerializeFullResponse(googleMapsRequest);
		if (googleMapsResponse?.Results == null)
			return ArraySegment<string>.Empty;

		var namesByType = BuildNamesByType(googleMapsResponse.Results);
		var addressPropertyValueList = AppendMatchingTypeNames(namesByType, requestedAddressTypes);
		return addressPropertyValueList;
	}

	public async Task<GoogleMapsResponse?> SerializeFullResponse(ReverseGeocodeRequest request)
	{
		try
		{
			var queryString = $"?latlng={request.Coordinate.Latitude},{request.Coordinate.Longitude}&key={_apiKeyStore.GoogleMaps}";

			if (request.Language != null)
				queryString += $"&language={request.Language}";

			if (_coordinateCache.TryGet(request, out var cachedData))
				return cachedData;

			var googleMapsResponse = await _httpClient.GetFromJsonAsync<GoogleMapsResponse>(queryString, StaticOptions.JsonSerializerOptions);
			if (googleMapsResponse == null)
			{
				_logger.LogCritical("Can't get {Type}", nameof(GoogleMapsResult));
				return null;
			}

			if (googleMapsResponse.Status != "OK")
			{
				_logger.LogCritical("Response is not OK with value: {Status}", googleMapsResponse.Status);
				return null;
			}

			if (googleMapsResponse.Results == null)
			{
				_logger.LogCritical("{Type} not found on response", nameof(GoogleMapsResult));
				return null;
			}

			_coordinateCache.SetResponse(request, googleMapsResponse);

			return googleMapsResponse;
		}
		catch (Exception e)
		{
			_logger.LogCritical(e, "Can't get & serialize {Type}", nameof(GoogleMapsResponse));
			return null;
		}
	}

	public async Task<Dictionary<string, object>> AllAvailableReverseGeocodes(Coordinate coordinate, string? language)
	{
		var googleMapsRequest = new ReverseGeocodeRequest(coordinate, language);
		var googleMapsResponse = await SerializeFullResponse(googleMapsRequest);
		if (googleMapsResponse?.Results == null)
			throw new PhotoCliException($"Can't get {nameof(GoogleMapsResult)} list");
		var namesByType = BuildNamesByType(googleMapsResponse.Results);
		var addressPropertyValueDict = new Dictionary<string, object>();
		foreach (var (typeName, (longName, shortName)) in namesByType)
		{
			var name = longName ?? shortName;
			if (name == null)
				continue;
			addressPropertyValueDict.Add(typeName, name);
		}

		return addressPropertyValueDict;
	}

	private Dictionary<string, GoogleMapsNames> BuildNamesByType(List<GoogleMapsResult> googleMapsResults)
	{
		var names = new Dictionary<string, GoogleMapsNames>();
		foreach (var result in googleMapsResults)
		{
			if (result.AddressComponents == null)
				continue;
			foreach (var addressComponent in result.AddressComponents)
			{
				if (addressComponent.Types == null || addressComponent.Types.Count == 0)
					continue;
				var type = addressComponent.Types.First();
				if (ResultListTypeIgnore.Any(a => a == type))
					continue;
				if (names.ContainsKey(type))
					continue;
				names.Add(type, new GoogleMapsNames(addressComponent.LongName, addressComponent.ShortName));
			}
		}

		return names;
	}

	private IEnumerable<string> AppendMatchingTypeNames(IReadOnlyDictionary<string, GoogleMapsNames> namesByType, IEnumerable<string> requestedAddressTypes)
	{
		var addressPropertyValueList = new List<string>();
		foreach (var requestedAddressType in requestedAddressTypes)
		{
			var name = namesByType.GetValueOrDefault(requestedAddressType);
			if (name == null)
			{
				_logger.LogError("Can't find requested address type: {Type}", requestedAddressType);
				continue;
			}

			var value = name.LongName ?? name.ShortName;
			if (value == null)
				continue;
			addressPropertyValueList.Add(value);
		}

		return addressPropertyValueList;
	}
}
