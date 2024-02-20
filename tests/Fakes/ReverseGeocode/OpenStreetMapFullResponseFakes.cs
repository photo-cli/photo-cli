namespace PhotoCli.Tests.Fakes.ReverseGeocode;

public static class OpenStreetMapFullResponseFakes
{
	public static OpenStreetMapResponse Valid(Coordinate coordinate)
	{
		return new OpenStreetMapResponse()
		{
			Lat = coordinate.Latitude,
			Lon = coordinate.Longitude
		};
	}
}
