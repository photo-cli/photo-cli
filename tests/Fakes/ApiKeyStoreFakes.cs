namespace PhotoCli.Tests.Fakes;

public static class ApiKeyStoreFakes
{
	private const string ValidApiKey = "valid-api-key";

	public static ApiKeyStore Valid()
	{
		return new ApiKeyStore();
	}

	public static ApiKeyStore Invalid()
	{
		return new ApiKeyStore { ReverseGeocodeProvider = ReverseGeocodeProvider.MapQuest };
	}

	public static ApiKeyStore BigDataCloudValid()
	{
		return new ApiKeyStore { ReverseGeocodeProvider = ReverseGeocodeProvider.BigDataCloud, BigDataCloud = ValidApiKey };
	}

	public static ApiKeyStore GoogleMapsValid()
	{
		return new ApiKeyStore { ReverseGeocodeProvider = ReverseGeocodeProvider.GoogleMaps, GoogleMaps = ValidApiKey };
	}

	public static ApiKeyStore MapQuestValid()
	{
		return new ApiKeyStore { ReverseGeocodeProvider = ReverseGeocodeProvider.MapQuest, MapQuest = ValidApiKey };
	}

	public static ApiKeyStore LocationIqValid()
	{
		return new ApiKeyStore { ReverseGeocodeProvider = ReverseGeocodeProvider.LocationIq, LocationIq = ValidApiKey };
	}
}
