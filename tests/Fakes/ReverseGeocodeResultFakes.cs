namespace PhotoCli.Tests.Fakes;

public static class ReverseGeocodeResultFakes
{
	public static ReverseGeocodeAddressResult Valid()
	{
		return new ReverseGeocodeAddressResult(ReverseGeocodeFakes.Valid(), true);
	}

	public static ReverseGeocodeAddressResult WithCoordinate(Coordinate coordinate)
	{
		return WithCoordinate(coordinate.Latitude, coordinate.Longitude);
	}

	public static ReverseGeocodeAddressResult WithCoordinate(double latitude, double longitude)
	{
		return new ReverseGeocodeAddressResult(ReverseGeocodeFakes.WithCoordinate(latitude, longitude), true);
	}
}
