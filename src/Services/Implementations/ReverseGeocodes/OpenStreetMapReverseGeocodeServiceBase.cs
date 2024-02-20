using System.Net.Http.Json;

namespace PhotoCli.Services.Implementations.ReverseGeocodes;

public abstract class OpenStreetMapReverseGeocodeServiceBase : IOpenStreetMapReverseGeocodeServiceBase
{
	private readonly HttpClient _httpClient;
	private readonly ILogger<OpenStreetMapReverseGeocodeServiceBase> _logger;
	private readonly ICoordinateCache<OpenStreetMapResponse> _coordinateCache;

	protected OpenStreetMapReverseGeocodeServiceBase(
		HttpClient httpClient,
		ILogger<OpenStreetMapReverseGeocodeServiceBase> logger,
		ICoordinateCache<OpenStreetMapResponse> coordinateCache)
	{
		_httpClient = httpClient;
		_logger = logger;
		_coordinateCache = coordinateCache;
	}

	public async Task<IEnumerable<string>> Get(Coordinate coordinate, List<PropertyInfo> requestedAddressPropertyInfos)
	{
		var openStreetMapRequest = new ReverseGeocodeRequest(coordinate);
		var openStreetMapResponse = await SerializeFullResponse(openStreetMapRequest);
		if (openStreetMapResponse?.Address == null)
		{
			_logger.LogCritical("Can't get {Type}", nameof(OpenStreetMapAddress));
			return ArraySegment<string>.Empty;
		}

		var addressPropertyValueList = new List<string>();
		foreach (var requestedAddressPropertyInfo in requestedAddressPropertyInfos)
		{
			var addressPropertyValue = requestedAddressPropertyInfo.GetValue(openStreetMapResponse.Address)?.ToString();
			if (addressPropertyValue != null)
				addressPropertyValueList.Add(addressPropertyValue);
		}

		return addressPropertyValueList;
	}

	public async Task<OpenStreetMapResponse?> SerializeFullResponse(ReverseGeocodeRequest request)
	{
		try
		{
			var requestUri = RequestUri(request);

			if (_coordinateCache.TryGet(request, out var cachedData))
				return cachedData;

			var openStreetMapResponse = await _httpClient.GetFromJsonAsync<OpenStreetMapResponse>(requestUri, StaticOptions.JsonSerializerOptions);
			_coordinateCache.SetResponse(request, openStreetMapResponse);

			return openStreetMapResponse;
		}
		catch (Exception e)
		{
			_logger.LogCritical(e, "Can't get & serialize {Type}", nameof(OpenStreetMapResponse));
			return null;
		}
	}

	public async Task<Dictionary<string, object>> AllAvailableReverseGeocodes(Coordinate coordinate)
	{
		var openStreetMapRequest = new ReverseGeocodeRequest(coordinate);
		var openStreetMapResponse = await SerializeFullResponse(openStreetMapRequest);
		if (openStreetMapResponse?.Address == null)
			throw new PhotoCliException($"Can't get {nameof(OpenStreetMapAddress)}");

		var addressPropertyValueDict = new Dictionary<string, object>();
		foreach (var property in typeof(OpenStreetMapAddress).GetProperties())
		{
			var addressPropertyValue = property.GetValue(openStreetMapResponse.Address)?.ToString();
			if (addressPropertyValue != null)
				addressPropertyValueDict.Add(property.Name, addressPropertyValue);
		}

		return addressPropertyValueDict;
	}

	protected virtual string RequestUri(ReverseGeocodeRequest request)
	{
		return $"?format=json&lat={request.Coordinate.Latitude}&lon={request.Coordinate.Longitude}";
	}
}
