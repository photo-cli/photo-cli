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

	public static Coordinate NorthEastBoundary()
	{
		return new Coordinate(89.123456789012345, 179.123456789012345);
	}

	public static Coordinate SouthWestBoundary()
	{
		return new Coordinate(-89.123456789012345, -179.123456789012345);
	}

	public static Coordinate Center()
	{
		return new Coordinate(0, 0);
	}

	public static Coordinate Sample(int sampleId)
	{
		return sampleId switch
		{
			1 => new Coordinate(39.925054, 32.8347552),
			2 => new Coordinate(37.2173377,38.9103364),
			3 => new Coordinate(40.7579984, -73.9860462),
			4 => new Coordinate(-34.3218944,18.4438962),
			5 => new Coordinate(-33.8567799,151.2127218),
			6 => new Coordinate(41.9102088,12.3711925),
			_ => throw new NotImplementedException()
		};
	}
}
