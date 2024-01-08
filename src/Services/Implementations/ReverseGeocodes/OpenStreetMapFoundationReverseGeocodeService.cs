namespace PhotoCli.Services.Implementations.ReverseGeocodes;

public class OpenStreetMapFoundationReverseGeocodeService : OpenStreetMapReverseGeocodeServiceBase, IOpenStreetMapFoundationReverseGeocodeService
{
	public OpenStreetMapFoundationReverseGeocodeService(HttpClient httpClient, ILogger<OpenStreetMapFoundationReverseGeocodeService> logger, ICoordinateCache<OpenStreetMapResponse> coordinateCache)
		: base(httpClient, logger, coordinateCache)
	{
	}

	protected override string RequestUri(ReverseGeocodeRequest request)
	{
		return $"?format=json&lat={request.Coordinate.Latitude}&lon={request.Coordinate.Longitude}";
	}
}
