namespace PhotoCli.Tests.Fakes;

public static class ReverseGeocodeProviderFakes
{
	public static ReverseGeocodeProvider NoWaitTime = ReverseGeocodeProvider.GoogleMaps;

	public static ReverseGeocodeProvider Valid()
	{
		return ReverseGeocodeProvider.BigDataCloud;
	}
}
