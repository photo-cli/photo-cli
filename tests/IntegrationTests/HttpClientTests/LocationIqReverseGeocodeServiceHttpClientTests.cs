namespace PhotoCli.Tests.IntegrationTests.HttpClientTests;

public class LocationIqReverseGeocodeServiceHttpClientTests : IClassFixture<SetEnvironmentVariablesFromLaunchSettingsFixture>
{
	[Fact]
	public async Task Service_Response_Valid()
	{
		const string apiKeyEnvironmentVariableName = "PHOTO_CLI_LOCATIONIQ_API_KEY";
		var apiKey = Environment.GetEnvironmentVariable(apiKeyEnvironmentVariableName);
		apiKey.Should().NotBeNull("{0} environment variable not set", apiKeyEnvironmentVariableName);
		var apiKeyStore = new ApiKeyStore { LocationIq = apiKey };
		var coordinateCacheMock = new Mock<CoordinateCache<OpenStreetMapResponse>>();
		var sut = new LocationIqReverseGeocodeService(CreateHttpClient(), NullLogger<LocationIqReverseGeocodeService>.Instance, apiKeyStore, coordinateCacheMock.Object);
		var coordinate = CoordinateFakes.Ankara();
		var openStreetMapRequest = new ReverseGeocodeRequest(coordinate);
		var openStreetMapResponse = await sut.SerializeFullResponse(openStreetMapRequest);
		openStreetMapResponse.Verify(ReverseGeocodeProvider.LocationIq);
	}

	private static HttpClient CreateHttpClient()
	{
		Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
		return new HttpClient
		{
			BaseAddress = new Uri("https://us1.locationiq.com/v1/reverse.php/"),
			DefaultRequestHeaders =
			{
				UserAgent = { UserAgent.Instance() },
			}
		};
	}
}
