namespace PhotoCli.Services.Implementations.ReverseGeocodes;

public class OpenStreetMapFoundationReverseGeocodeService : OpenStreetMapReverseGeocodeServiceBase, IOpenStreetMapFoundationReverseGeocodeService
{
	public OpenStreetMapFoundationReverseGeocodeService(HttpClient httpClient, ILogger<OpenStreetMapFoundationReverseGeocodeService> logger, IReverseGeocodeCache<OpenStreetMapResponse>
			reverseGeocodeCache, Statistics statistics)
		: base(httpClient, logger, reverseGeocodeCache, statistics)
	{
	}

	protected override string RequestUri(ReverseGeocodeRequest request)
	{
		return $"?format=json&lat={request.Coordinate.Latitude}&lon={request.Coordinate.Longitude}";
	}
}
