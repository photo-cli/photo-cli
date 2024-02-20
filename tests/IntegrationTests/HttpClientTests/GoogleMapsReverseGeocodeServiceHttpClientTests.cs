namespace PhotoCli.Tests.IntegrationTests.HttpClientTests;

public class GoogleMapsReverseGeocodeServiceHttpClientTests : IClassFixture<SetEnvironmentVariablesFromLaunchSettingsFixture>
{
	[Fact]
	public async Task Response_Serialization_Verify()
	{
		const string apiKeyEnvironmentVariableName = "PHOTO_CLI_GOOGLE_MAPS_API_KEY";
		var apiKey = Environment.GetEnvironmentVariable(apiKeyEnvironmentVariableName);
		apiKey.Should().NotBeNull("{0} environment variable not set", apiKeyEnvironmentVariableName);
		var apiKeyStore = new ApiKeyStore { GoogleMaps = apiKey };
		var coordinateCacheMock = new Mock<CoordinateCache<GoogleMapsResponse>>();
		var sut = new GoogleMapsReverseGeocodeService(CreateHttpClient(), apiKeyStore, NullLogger<GoogleMapsReverseGeocodeService>.Instance, coordinateCacheMock.Object);
		var googleMapsRequest = new ReverseGeocodeRequest(CoordinateFakes.Ankara(), "tr");
		var googleMapsResponse = await sut.SerializeFullResponse(googleMapsRequest);
		googleMapsResponse.Verify();
	}

	private static HttpClient CreateHttpClient()
	{
		Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
		return new HttpClient
		{
			BaseAddress = new Uri("https://maps.googleapis.com/maps/api/geocode/json"),
			DefaultRequestHeaders =
			{
				UserAgent = { UserAgent.Instance() },
			}
		};
	}
}
