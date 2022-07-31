namespace PhotoCli.Tests.IntegrationTests.HttpClientTests;

public class MapQuestReverseGeocodeServiceHttpClientTests : IClassFixture<SetEnvironmentVariablesFromLaunchSettingsFixture>
{
	[Fact]
	public async Task Service_Response_Valid()
	{
		const string apiKeyEnvironmentVariableName = "PHOTO_CLI_MAPQUEST_API_KEY";
		var apiKey = Environment.GetEnvironmentVariable(apiKeyEnvironmentVariableName);
		apiKey.Should().NotBeNull("{0} environment variable not set", apiKeyEnvironmentVariableName);
		var apiKeyStore = new ApiKeyStore { MapQuest = apiKey };
		var sut = new MapQuestReverseGeocodeService(CreateHttpClient(), NullLogger<MapQuestReverseGeocodeService>.Instance, apiKeyStore);
		var openStreetMapResponse = await sut.SerializeFullResponse(CoordinateFakes.Ankara());
		openStreetMapResponse.Verify(ReverseGeocodeProvider.MapQuest);
	}

	private static HttpClient CreateHttpClient()
	{
		Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
		return new HttpClient
		{
			BaseAddress = new Uri("https://open.mapquestapi.com/nominatim/v1/reverse.php/"),
			DefaultRequestHeaders =
			{
				UserAgent = { UserAgent.Instance() },
			}
		};
	}
}
