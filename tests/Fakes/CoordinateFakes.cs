namespace PhotoCli.Tests.Fakes;

public static class CoordinateFakes
{
	public static Coordinate Valid()
	{
		return new Coordinate(1, 1);
	}

	public static Coordinate Ankara()
	{
		return new Coordinate(39.925054, 32.8347552);
	}
}
