namespace PhotoCli.Tests.Fakes.ReverseGeocode;

public static class ReverseGeocodeResponseFakes
{
	public static object WithCoordinate(Coordinate coordinate)
	{
		return GoogleMapsFullResponseFakes.Valid(coordinate);
	}
}
