using System.Net.Http.Json;
using PhotoCli.Utils.Logging;

namespace PhotoCli.Services.Implementations.ReverseGeocodes;

public abstract class OpenStreetMapReverseGeocodeServiceBase : IOpenStreetMapReverseGeocodeServiceBase
{
	private const ReverseGeocodeProvider Provider = ReverseGeocodeProvider.OpenStreetMapFoundation;
	private readonly HttpClient _httpClient;
	private readonly ILogger<OpenStreetMapReverseGeocodeServiceBase> _logger;
	private readonly IReverseGeocodeCache<OpenStreetMapResponse> _reverseGeocodeCache;
	private readonly Statistics _statistics;

	protected OpenStreetMapReverseGeocodeServiceBase(
		HttpClient httpClient,
		ILogger<OpenStreetMapReverseGeocodeServiceBase> logger,
		IReverseGeocodeCache<OpenStreetMapResponse> reverseGeocodeCache,
		Statistics statistics)
	{
		_httpClient = httpClient;
		_logger = logger;
		_reverseGeocodeCache = reverseGeocodeCache;
		_statistics = statistics;
	}

	public async Task<ReverseGeocodeAddressResult> Get(Coordinate coordinate, PhotoFile photoFile, List<PropertyInfo> requestedAddressPropertyInfos)
	{
		var openStreetMapRequest = new ReverseGeocodeRequest(coordinate);
		var openStreetMapResponse = await SerializeFullResponse(openStreetMapRequest);
		if (openStreetMapResponse?.Address == null)
		{
			_logger.LogCritical("Can't get {Type}", nameof(OpenStreetMapAddress));
			return new ReverseGeocodeAddressResult(ArraySegment<string>.Empty, false);
		}

		var addressPropertyValueList = new List<string>();
		var allPhotosHasReverseGeocodedAsRequested = true;
		var addressIndex = 1;
		foreach (var requestedAddressIndex in requestedAddressPropertyInfos)
		{
			var addressPropertyValue = requestedAddressIndex.GetValue(openStreetMapResponse.Address)?.ToString();
			if (addressPropertyValue != null)
			{
				addressPropertyValueList.Add(addressPropertyValue);
			}
			else
			{
				_logger.LogErrorWithPath("Requested address types: {OpenStreetMapProperty} on index #{AddressIndex}, not found on OpenStreetMap's response. Available types found: {AvailableTypes}",
					photoFile.SourceFullPath, requestedAddressIndex.Name, addressIndex, openStreetMapResponse.Address);

				allPhotosHasReverseGeocodedAsRequested = false;
			}
			++addressIndex;
		}

		return new ReverseGeocodeAddressResult(addressPropertyValueList, allPhotosHasReverseGeocodedAsRequested);
	}

	public async Task<OpenStreetMapResponse?> SerializeFullResponse(ReverseGeocodeRequest request)
	{
		try
		{
			var requestUri = RequestUri(request);

			var (cacheHit, cachedResponse) = await _reverseGeocodeCache.TryGet(request, ReverseGeocodeProvider.OpenStreetMapFoundation);
			if (cacheHit)
				return cachedResponse;

			var openStreetMapResponse = await _httpClient.GetFromJsonAsync<OpenStreetMapResponse>(requestUri, StaticOptions.JsonSerializerOptions);
			++_statistics.ReserveGeocodeRequestSent;
			await _reverseGeocodeCache.SetResponse(request, Provider, openStreetMapResponse);
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
