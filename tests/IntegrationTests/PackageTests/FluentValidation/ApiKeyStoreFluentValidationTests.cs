namespace PhotoCli.Tests.IntegrationTests.PackageTests.FluentValidation;

public class ApiKeyStoreFluentValidationTests : BaseFluentValidationTests<ApiKeyStore, ApiKeyStoreValidator>
{
	#region Valid

	private const string ValidApiKey = "valid-api-key";

	[Fact]
	public void Valid_ApiKeyStore_With_BigDataCloud_Should_Have_NoError()
	{
		var apiKeyStore = new ApiKeyStore { BigDataCloud = ValidApiKey, ReverseGeocodeProvider = ReverseGeocodeProvider.BigDataCloud };
		ValidationShouldHaveNoError(apiKeyStore);
	}

	[Fact]
	public void Valid_ApiKeyStore_With_GoogleMaps_Should_Have_NoError()
	{
		var apiKeyStore = new ApiKeyStore { GoogleMaps = ValidApiKey, ReverseGeocodeProvider = ReverseGeocodeProvider.GoogleMaps };
		ValidationShouldHaveNoError(apiKeyStore);
	}

	[Fact]
	public void Valid_ApiKeyStore_With_MapQuest_Should_Have_NoError()
	{
		var apiKeyStore = new ApiKeyStore { MapQuest = ValidApiKey, ReverseGeocodeProvider = ReverseGeocodeProvider.MapQuest };
		ValidationShouldHaveNoError(apiKeyStore);
	}

	[Fact]
	public void Valid_ApiKeyStore_With_LocationIq_Should_Have_NoError()
	{
		var apiKeyStore = new ApiKeyStore { LocationIq = ValidApiKey, ReverseGeocodeProvider = ReverseGeocodeProvider.LocationIq };
		ValidationShouldHaveNoError(apiKeyStore);
	}

	#endregion

	#region Invalid

	[Fact]
	public void When_Using_BigDataCloud_Not_Using_BigDataCloudApiKey_Should_Give_NullValidator_And_Verify_Error_Message()
	{
		var apiKeyStore = new ApiKeyStore { ReverseGeocodeProvider = ReverseGeocodeProvider.BigDataCloud };
		var errorMessage = CantFindMessage(ReverseGeocodeProvider.BigDataCloud, "PHOTO_CLI_BIG_DATA_CLOUD_API_KEY", "bigdatacloud-key", 'b');
		CheckPropertyNotNull(apiKeyStore, nameof(ApiKeyStore.BigDataCloud), errorMessage);
	}

	[Fact]
	public void When_Using_GoogleMaps_Not_Using_GoogleMapsApiKey_Should_Give_NullValidator_And_Verify_Error_Message()
	{
		var apiKeyStore = new ApiKeyStore { ReverseGeocodeProvider = ReverseGeocodeProvider.GoogleMaps };
		var errorMessage = CantFindMessage(ReverseGeocodeProvider.GoogleMaps, "PHOTO_CLI_GOOGLE_MAPS_API_KEY", "googlemaps-key", 'k');
		CheckPropertyNotNull(apiKeyStore, nameof(ApiKeyStore.GoogleMaps), errorMessage);
	}

	[Fact]
	public void When_Using_MapQuest_Not_Using_MapQuestApiKey_Should_Give_NullValidator_And_Verify_Error_Message()
	{
		var apiKeyStore = new ApiKeyStore { ReverseGeocodeProvider = ReverseGeocodeProvider.MapQuest };
		var errorMessage = CantFindMessage(ReverseGeocodeProvider.MapQuest, "PHOTO_CLI_MAPQUEST_API_KEY", "mapquest-key", 'u');
		CheckPropertyNotNull(apiKeyStore, nameof(ApiKeyStore.MapQuest), errorMessage);
	}

	[Fact]
	public void When_Using_LocationIq_Not_Using_MapQuestApiKey_Should_Give_NullValidator_And_Verify_Error_Message()
	{
		var apiKeyStore = new ApiKeyStore { ReverseGeocodeProvider = ReverseGeocodeProvider.LocationIq };
		var errorMessage = CantFindMessage(ReverseGeocodeProvider.LocationIq, "PHOTO_CLI_LOCATIONIQ_API_KEY", "locationiq-key", 'q');
		CheckPropertyNotNull(apiKeyStore, nameof(ApiKeyStore.LocationIq), errorMessage);
	}


	private string CantFindMessage(ReverseGeocodeProvider reverseGeocodeProvider, string environmentVariableKey, string longOptionName, char shortOptionName)
	{
		return $"Can't find {reverseGeocodeProvider} API key at environment variable with key {environmentVariableKey} or application arguments -{longOptionName} or -{shortOptionName}";
	}

	#endregion
}
