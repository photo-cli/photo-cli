using System.Net.Http.Json;
using PhotoCli.Utils.Logging;

namespace PhotoCli.Services.Implementations.ReverseGeocodes;

public class GoogleMapsReverseGeocodeService : IGoogleMapsReverseGeocodeService
{
	private const ReverseGeocodeProvider Provider = ReverseGeocodeProvider.GoogleMaps;

	private readonly ApiKeyStore _apiKeyStore;
	private readonly HttpClient _httpClient;
	private readonly ILogger<GoogleMapsReverseGeocodeService> _logger;
	private readonly IReverseGeocodeCache<GoogleMapsResponse> _reverseGeocodeCache;
	private readonly Statistics _statistics;

	public GoogleMapsReverseGeocodeService(HttpClient httpClient, ApiKeyStore apiKeyStore, ILogger<GoogleMapsReverseGeocodeService> logger,
		IReverseGeocodeCache<GoogleMapsResponse> reverseGeocodeCache, Statistics statistics)
	{
		_httpClient = httpClient;
		_apiKeyStore = apiKeyStore;
		_logger = logger;
		_reverseGeocodeCache = reverseGeocodeCache;
		_statistics = statistics;
	}

	public async Task<ReverseGeocodeAddressResult> Get(Coordinate coordinate, PhotoFile photoFile, string? language, IEnumerable<string> requestedAddressTypes)
	{
		var googleMapsRequest = new ReverseGeocodeRequest(coordinate, language);
		var googleMapsResponse = await SerializeFullResponse(googleMapsRequest);
		if (googleMapsResponse?.Results == null)
			return new ReverseGeocodeAddressResult(ArraySegment<string>.Empty, false);

		var namesByType = BuildNamesByType(googleMapsResponse.Results);
		var addressPropertyValueList = AppendMatchingTypeNames(namesByType, requestedAddressTypes, photoFile);
		return addressPropertyValueList;
	}

	public async Task<GoogleMapsResponse?> SerializeFullResponse(ReverseGeocodeRequest request)
	{
		try
		{
			var queryString = $"?latlng={request.Coordinate.Latitude},{request.Coordinate.Longitude}&key={_apiKeyStore.GoogleMaps}";

			if (request.Language != null)
				queryString += $"&language={request.Language}";

			var (cacheHit, cachedResponse) = await _reverseGeocodeCache.TryGet(request, Provider);
			if (cacheHit)
				return cachedResponse;

			var googleMapsResponse = await _httpClient.GetFromJsonAsync<GoogleMapsResponse>(queryString, StaticOptions.JsonSerializerOptions);
			++_statistics.ReserveGeocodeRequestSent;
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
			await _reverseGeocodeCache.SetResponse(request, Provider, googleMapsResponse);
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
				if (names.ContainsKey(type))
					continue;
				names.Add(type, new GoogleMapsNames(addressComponent.LongName, addressComponent.ShortName));
			}
		}

		return names;
	}

	private ReverseGeocodeAddressResult AppendMatchingTypeNames(IReadOnlyDictionary<string, GoogleMapsNames> namesByType, IEnumerable<string> requestedAddressTypes, PhotoFile photoFile)
	{
		var allPhotosHasReverseGeocodedAsRequested = true;
		var addressPropertyValueList = new List<string>();
		var addressIndex = 1;
		foreach (var requestedAddressTypesIndex in requestedAddressTypes)
		{
			_logger.LogDebug("Requesting Address index of {AddressIndex} with {GoogleRequestedAddressType}", addressIndex, requestedAddressTypesIndex);
			var typeListToCheckInOrder = requestedAddressTypesIndex.Split(",");

			string? typeValue = null;
			foreach (var typeToCheck in typeListToCheckInOrder)
			{
				var name = namesByType.GetValueOrDefault(typeToCheck);
				if (name == null)
				{
					_logger.LogWarning("Can't find requested address {GoogleRequestedAddressType}", typeToCheck);
					continue;
				}

				var value = name.LongName ?? name.ShortName;
				if (value == null || value.IsMissing())
				{
					_logger.LogWarning("Requested address {GoogleRequestedAddressType}: found in response but it's value is empty or missing", typeToCheck);
					continue;
				}

				typeValue = value;
				break;
			}

			if (typeValue.IsPresent())
			{
				addressPropertyValueList.Add(typeValue);
			}
			else
			{
				_logger.LogErrorWithPath("Requested address types: {GoogleRequestedAddressTypes} on index #{AddressIndex}, not found on Google's response. Try adding fallback types like : type1,type2,type3. Available types found: {AvailableGoogleTypesOnCoordinate}",
					photoFile.SourceFullPath, requestedAddressTypesIndex, addressIndex, namesByType);

				allPhotosHasReverseGeocodedAsRequested = false;
			}

			++addressIndex;
		}

		return new ReverseGeocodeAddressResult(addressPropertyValueList, allPhotosHasReverseGeocodedAsRequested);
	}
}
