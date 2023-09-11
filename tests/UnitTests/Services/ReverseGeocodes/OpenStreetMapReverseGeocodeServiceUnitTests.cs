using System.Reflection;
using Moq.Protected;

namespace PhotoCli.Tests.UnitTests.Services.ReverseGeocodes;

public class OpenStreetMapReverseGeocodeServiceUnitTests
{
	public static TheoryData<List<string>, List<string>> WithGivenOptionsReverseGeocodeResultShouldBeGivenExpectedAddressData = new()
	{
		{ new List<string> { "country" }, new List<string> { "Türkiye" } },
		{ new List<string> { "city" }, new List<string> { "Ankara" } },
		{ new List<string> { "country", "city" }, new List<string> { "Türkiye", "Ankara" } },
		{ new List<string> { "country", "city", "town" }, new List<string> { "Türkiye", "Ankara", "Çankaya" } },
		{ new List<string> { "country", "city", "town", "suburb" }, new List<string> { "Türkiye", "Ankara", "Çankaya", "Mebusevleri Mahallesi" } },
	};

	[Theory]
	[MemberData(nameof(WithGivenOptionsReverseGeocodeResultShouldBeGivenExpectedAddressData))]
	public async Task OpenStreetMapNominatim_With_Given_Options_ReverseGeocode_Result_Should_Be_Given_Expected_Address_Data(List<string> properties, List<string> expectedAddresses)
	{
		var coordinateCacheMock = new Mock<CoordinateCache<OpenStreetMapResponse>>();
		var sut = new OpenStreetMapFoundationReverseGeocodeService(CreateMockHttpClient(), NullLogger<OpenStreetMapFoundationReverseGeocodeService>.Instance, coordinateCacheMock.Object);
		var actualReverseGeocode = await sut.Get(CoordinateFakes.Ankara(), PrepareRequestedAddressProperties(properties));
		actualReverseGeocode.Should().BeEquivalentTo(expectedAddresses);
	}

	[Theory]
	[MemberData(nameof(WithGivenOptionsReverseGeocodeResultShouldBeGivenExpectedAddressData))]
	public async Task LocationIq_With_Given_Options_ReverseGeocode_Result_Should_Be_Expected_Address_Data(List<string> properties, List<string> expectedAddresses)
	{
		var coordinateCacheMock = new Mock<CoordinateCache<OpenStreetMapResponse>>();
		var sut = new LocationIqReverseGeocodeService(CreateMockHttpClient(), NullLogger<LocationIqReverseGeocodeService>.Instance, ApiKeyStoreFakes.LocationIqValid(), coordinateCacheMock.Object);
		var actualReverseGeocode = await sut.Get(CoordinateFakes.Ankara(), PrepareRequestedAddressProperties(properties));
		actualReverseGeocode.Should().BeEquivalentTo(expectedAddresses);
	}

	[Fact]
	public async Task LocationIq_Service_Error_Should_Give_Empty_List()
	{
		var coordinateCacheMock = new Mock<CoordinateCache<OpenStreetMapResponse>>();
		var sut = new LocationIqReverseGeocodeService(MockHttpClient.WithError(), NullLogger<LocationIqReverseGeocodeService>.Instance, ApiKeyStoreFakes.LocationIqValid(), coordinateCacheMock.Object);
		var actualReverseGeocode = await sut.Get(CoordinateFakes.Ankara(), OpenStreetMapAddressPropertiesFakes.ValidPropertyInfos());
		actualReverseGeocode.Should().BeEquivalentTo(ArraySegment<string>.Empty);
	}

	public static TheoryData<Dictionary<string, object>> ExpectedAvailableReverseGeocodes = new()
	{
		new Dictionary<string, object>
		{
			{ "CountryCode", "tr" },
			{ "Country", "Türkiye" },
			{ "Region", "İç Anadolu Bölgesi" },
			{ "Province", "Ankara" },
			{ "City", "Ankara" },
			{ "Town", "Çankaya" },
			{ "Postcode", "06580" },
			{ "Suburb", "Mebusevleri Mahallesi" },
			{ "Road", "Dumlupınar Yolu" },
			{ "Military", "Anıtkabir" },
		}
	};

	[Theory]
	[MemberData(nameof(ExpectedAvailableReverseGeocodes))]
	public async Task AllAvailableReverseGeocodes_Equivalent_To_Expected(Dictionary<string, object> expectedAllAvailableReverseGeocodes)
	{
		var coordinateCacheMock = new Mock<CoordinateCache<OpenStreetMapResponse>>();
		var sut = new OpenStreetMapFoundationReverseGeocodeService(CreateMockHttpClient(), NullLogger<OpenStreetMapFoundationReverseGeocodeService>.Instance, coordinateCacheMock.Object);
		var actualAllAvailableReverseGeocodes = await sut.AllAvailableReverseGeocodes(CoordinateFakes.Ankara());
		actualAllAvailableReverseGeocodes.Should().BeEquivalentTo(expectedAllAvailableReverseGeocodes);
	}

	public static TheoryData<Coordinate> CacheHitData = new()
	{
		new Coordinate(-89.1234567, -179.1234567),
		new Coordinate(89.1234567, 179.1234567),
	};

	[Theory]
	[MemberData(nameof(CacheHitData))]
	public async Task SerializeFullResponse_Should_Return_From_Cache(Coordinate coordinate)
	{
		var httpMessageHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
		var httpClientMock = new HttpClient(httpMessageHandlerMock.Object);

		var cacheResponseFakeExpected = OpenStreetMapFullResponseFakes.Valid(coordinate);
		var mockOpenStreetMapResponseCache = MockCoordinateCache(coordinate, cacheResponseFakeExpected);
		var sut = new OpenStreetMapFoundationReverseGeocodeService(httpClientMock, NullLogger<OpenStreetMapFoundationReverseGeocodeService>.Instance, mockOpenStreetMapResponseCache);
		var cacheResponseActual = await sut.SerializeFullResponse(new ReverseGeocodeRequest(coordinate));

		cacheResponseActual.Should().Be(cacheResponseFakeExpected);
		httpMessageHandlerMock.Protected().Verify("SendAsync", Times.Never(), ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>());
	}

	public static TheoryData<Coordinate, Coordinate[]> CacheMissData = new()
	{
		{
			new Coordinate(-89.1234567, -179.1234567),
			new []
			{
				new Coordinate(-89.1234568, -179.1234568),
			}
		},
		{
			new Coordinate(89.1234567, 179.1234567),
			new []
			{
				new Coordinate(89.1234566, 179.1234566),
				new Coordinate(45.1234566, 91.1234566),
			}
		},
	};

	[Theory]
	[MemberData(nameof(CacheMissData))]
	public async Task SerializeFullResponse_Should_Requested_From_HttpClient(Coordinate cacheCoordinate, Coordinate[] requestCoordinates)
	{
		var (httpMessageHandlerMock, mockHttpClient) = MockHttpMessageHandler.WithResponse(OpenStreetMapReverseGeocodeResponseFakes.Ankara());

		var mockOpenStreetMapResponseCache = MockCoordinateCache(cacheCoordinate, OpenStreetMapFullResponseFakes.Valid(cacheCoordinate));

		var sut = new OpenStreetMapFoundationReverseGeocodeService(mockHttpClient, NullLogger<OpenStreetMapFoundationReverseGeocodeService>.Instance, mockOpenStreetMapResponseCache);

		foreach (var requestCoordinate in requestCoordinates)
			_ = await sut.SerializeFullResponse(new ReverseGeocodeRequest(requestCoordinate));

		httpMessageHandlerMock.Protected().Verify("SendAsync", Times.Exactly(requestCoordinates.Length), ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>());
	}

	private HttpClient CreateMockHttpClient()
	{
		return MockHttpClient.WithResponse(OpenStreetMapReverseGeocodeResponseFakes.Ankara());
	}

	private List<PropertyInfo> PrepareRequestedAddressProperties(List<string> properties)
	{
		var allLowerCaseRequestedProperties = properties.Select(s => s.ToLowerInvariant());
		var requestedAddressProperties = typeof(OpenStreetMapAddress).GetProperties().Where(w => allLowerCaseRequestedProperties.Contains(w.Name.ToLowerInvariant())).ToList();
		return requestedAddressProperties;
	}

	private static ICoordinateCache<OpenStreetMapResponse> MockCoordinateCache(Coordinate coordinateCacheKey, OpenStreetMapResponse responseCacheResult)
	{
		var request =  new ReverseGeocodeRequest(coordinateCacheKey);

		var mockCoordinateCache = new Mock<ICoordinateCache<OpenStreetMapResponse>>();
		mockCoordinateCache
			.Setup(x => x.TryGet(request, out It.Ref<OpenStreetMapResponse?>.IsAny))
			.Returns((ReverseGeocodeRequest _, out OpenStreetMapResponse? value) =>
			{
				value = responseCacheResult;
				return true;
			});

		return mockCoordinateCache.Object;
	}
}
