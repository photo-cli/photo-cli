namespace PhotoCli.Tests.Fakes;

public static class ReverseGeocodeRequestFakes
{
	public static ReverseGeocodeRequest WithCoordinate(Coordinate coordinate)
	{
		return new(coordinate, null);
	}
}
