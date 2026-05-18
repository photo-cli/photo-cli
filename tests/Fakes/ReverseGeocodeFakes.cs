namespace PhotoCli.Tests.Fakes;

public static class ReverseGeocodeFakes
{
	public static List<string> Valid()
	{
		return ["Country", "City", "Neighbourhood"];
	}

	public static List<string> Sample(int sampleId)
	{
		return ["Country", "City", "Neighbourhood", sampleId.ToString()];
	}

	public static string SampleSingleAddress(int sampleId)
	{
		return $"Neighbourhood - {sampleId}";
	}

	public static IEnumerable<string> Exact(params string[] reverseGeocodes)
	{
		return reverseGeocodes;
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
		return ["ReverseGeocode", latitude.ToString(CultureInfo.InvariantCulture), longitude.ToString(CultureInfo.InvariantCulture)];
	}
}
