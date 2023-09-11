namespace PhotoCli.Tests.IntegrationTests.HttpClientTests;

public class OpenStreetMapReverseGeocodeServiceHttpClientTests
{
	[Fact]
	public async Task Service_Response_Valid()
	{
		var coordinateCacheMock = new Mock<CoordinateCache<OpenStreetMapResponse>>();
		var sut = new OpenStreetMapFoundationReverseGeocodeService(OpenStreetMapRealHttpClient(), NullLogger<OpenStreetMapFoundationReverseGeocodeService>.Instance, coordinateCacheMock.Object);
		var openStreetMapRequest = new ReverseGeocodeRequest(CoordinateFakes.Ankara());
		var openStreetMapResponse = await sut.SerializeFullResponse(openStreetMapRequest);
		openStreetMapResponse.Verify(ReverseGeocodeProvider.OpenStreetMapFoundation);
	}

	private static HttpClient OpenStreetMapRealHttpClient()
	{
		Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
		return new HttpClient
		{
			BaseAddress = new Uri("https://nominatim.openstreetmap.org/reverse"),
			DefaultRequestHeaders = { UserAgent = { UserAgent.Instance() } }
		};
	}
}
