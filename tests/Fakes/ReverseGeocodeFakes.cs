namespace PhotoCli.Tests.Fakes;

public static class ReverseGeocodeFakes
{
	public static List<string> Valid()
	{
		return new List<string>
		{
			"Country", "City", "Neighbourhood"
		};
	}

	public static List<string> Sample(int sampleId)
	{
		return new List<string>
		{
			"Country", "City", "Neighbourhood", sampleId.ToString()
		};
	}

	public static string Format(IEnumerable<string> reverseGeocodes)
	{
		return string.Join("-", reverseGeocodes);
	}

	public static string FlatFormatSampleId(int sampleId)
	{
		return string.Join("-", Sample(sampleId));
	}

	public static string HierarchyFormatSampleId(int sampleId)
	{
		return string.Join(Path.DirectorySeparatorChar, Sample(sampleId));
	}

	public static List<string> WithCoordinate(double latitude, double longitude)
	{
		return new List<string>
		{
			"ReverseGeocode", latitude.ToString(CultureInfo.InvariantCulture), longitude.ToString(CultureInfo.InvariantCulture)
		};
	}

	public static IEnumerable<string> WithCoordinate(Coordinate coordinate)
	{
		return WithCoordinate(coordinate.Latitude, coordinate.Longitude);
	}
}
