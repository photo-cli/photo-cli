namespace PhotoCli.Tests.Fakes.ReverseGeocode;

public static class GoogleMapsPropertiesFakes
{
	public static IEnumerable<string> Valid()
	{
		return new List<string> { "country" };
	}
}
