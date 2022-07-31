namespace PhotoCli.Services.Implementations.ReverseGeocodes;

public class OpenStreetMapFoundationReverseGeocodeService : OpenStreetMapReverseGeocodeServiceBase, IOpenStreetMapFoundationReverseGeocodeService
{
	public OpenStreetMapFoundationReverseGeocodeService(HttpClient httpClient, ILogger<OpenStreetMapFoundationReverseGeocodeService> logger) : base(httpClient, logger)
	{
	}

	protected override string RequestUri(Coordinate coordinate)
	{
		return $"?format=json&lat={coordinate.Latitude}&lon={coordinate.Longitude}";
	}
}
