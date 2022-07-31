using System.Net.Http.Json;

namespace PhotoCli.Services.Implementations.ReverseGeocodes;

public abstract class OpenStreetMapReverseGeocodeServiceBase : IOpenStreetMapReverseGeocodeServiceBase
{
	private readonly HttpClient _httpClient;
	private readonly ILogger<OpenStreetMapReverseGeocodeServiceBase> _logger;

	protected OpenStreetMapReverseGeocodeServiceBase(HttpClient httpClient, ILogger<OpenStreetMapReverseGeocodeServiceBase> logger)
	{
		_httpClient = httpClient;
		_logger = logger;
	}

	public async Task<IEnumerable<string>> Get(Coordinate coordinate, List<PropertyInfo> requestedAddressPropertyInfos)
	{
		var openStreetMapResponse = await SerializeFullResponse(coordinate);
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

	public async Task<OpenStreetMapResponse?> SerializeFullResponse(Coordinate coordinate)
	{
		try
		{
			var requestUri = RequestUri(coordinate);
			var openStreetMapResponse = await _httpClient.GetFromJsonAsync<OpenStreetMapResponse>(requestUri, StaticOptions.JsonSerializerOptions);
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
		var openStreetMapResponse = await SerializeFullResponse(coordinate);
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

	protected virtual string RequestUri(Coordinate coordinate)
	{
		return $"?format=json&lat={coordinate.Latitude}&lon={coordinate.Longitude}";
	}
}
