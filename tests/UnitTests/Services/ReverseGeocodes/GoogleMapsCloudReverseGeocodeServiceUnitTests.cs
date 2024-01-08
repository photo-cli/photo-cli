using Moq.Protected;

namespace PhotoCli.Tests.UnitTests.Services.ReverseGeocodes;

public class GoogleMapsCloudReverseGeocodeServiceUnitTests
{
	public static TheoryData<List<string>, List<string>> WithGivenOptionsReverseGeocodeResultShouldBeGivenExpectedAddressData = new()
	{
		{ new List<string> { "country" }, new List<string> { "Turkey" } },
		{ new List<string> { "administrative_area_level_1" }, new List<string> { "Ankara" } },
		{ new List<string> { "country", "administrative_area_level_1" }, new List<string> { "Turkey", "Ankara" } },
		{ new List<string> { "country", "administrative_area_level_1", "administrative_area_level_2" }, new List<string> { "Turkey", "Ankara", "Çankaya" } },
		{
			new List<string> { "country", "administrative_area_level_1", "administrative_area_level_2", "administrative_area_level_4" },
			new List<string> { "Turkey", "Ankara", "Çankaya", "Mebusevleri" }
		},
	};

	[Theory]
	[MemberData(nameof(WithGivenOptionsReverseGeocodeResultShouldBeGivenExpectedAddressData))]
	public async Task With_Given_Options_ReverseGeocode_Result_Should_Be_Given_Expected_Address_Data(List<string> properties, List<string> expectedAddresses)
	{
		var sut = MockServiceWithValidResponse();
		var actualReverseGeocode = await sut.Get(CoordinateFakes.Ankara(), "en", properties);
		actualReverseGeocode.Should().BeEquivalentTo(expectedAddresses);
	}

	[Fact]
	public async Task Response_Serialization_Verify()
	{
		var sut = MockServiceWithValidResponse();
		var googleMapsRequest = new ReverseGeocodeRequest(CoordinateFakes.Ankara(), "tr");
		var googleMapsResponse = await sut.SerializeFullResponse(googleMapsRequest);
		googleMapsResponse.Verify();
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
		var mockHttpMessageHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
		var mockHttpClient = new HttpClient(mockHttpMessageHandler.Object);

		var cacheResponseFakeExpected = GoogleMapsFullResponseFakes.Valid(coordinate);
		var mockGoogleMapsResponseCache = MockCoordinateCache(coordinate, cacheResponseFakeExpected);

		var sut = new GoogleMapsReverseGeocodeService(
			mockHttpClient,
			ApiKeyStoreFakes.GoogleMapsValid(),
			NullLogger<GoogleMapsReverseGeocodeService>.Instance,
			mockGoogleMapsResponseCache);

		var cacheResponseActual = await sut.SerializeFullResponse(new ReverseGeocodeRequest(coordinate));

		cacheResponseActual.Should().Be(cacheResponseFakeExpected);
		mockHttpMessageHandler.Protected().Verify("SendAsync", Times.Never(), ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>());
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
		var (mockHttpMessageHandler, mockHttpClient) = MockHttpMessageHandler.WithResponse(GoogleMapsReverseGeocodeResponseFakes.Ankara());

		var mockGoogleMapsResponseCache = MockCoordinateCache(cacheCoordinate, GoogleMapsFullResponseFakes.Valid(cacheCoordinate));

		var sut = new GoogleMapsReverseGeocodeService(
			mockHttpClient,
			ApiKeyStoreFakes.GoogleMapsValid(),
			NullLogger<GoogleMapsReverseGeocodeService>.Instance,
			mockGoogleMapsResponseCache);

		foreach (var requestCoordinate in requestCoordinates)
			_ = await sut.SerializeFullResponse(new ReverseGeocodeRequest(requestCoordinate));

		mockHttpMessageHandler.Protected().Verify("SendAsync", Times.Exactly(requestCoordinates.Length), ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>());
	}

	[Fact]
	public async Task Service_Error_Should_Give_Empty_List()
	{
		var mockHttpClient = MockHttpClient.WithError();
		var coordinateCacheMock = new Mock<CoordinateCache<GoogleMapsResponse>>();
		var sut = new GoogleMapsReverseGeocodeService(mockHttpClient, ApiKeyStoreFakes.GoogleMapsValid(), NullLogger<GoogleMapsReverseGeocodeService>.Instance, coordinateCacheMock.Object);
		var actualReverseGeocode = await sut.Get(CoordinateFakes.Ankara(), "en", GoogleMapsPropertiesFakes.Valid());
		actualReverseGeocode.Should().BeEquivalentTo(ArraySegment<string>.Empty);
	}

	public static TheoryData<Dictionary<string, object>> ExpectedAvailableReverseGeocodes = new()
	{
		new Dictionary<string, object>
		{
			{ "route", "Anıtkabir" },
			{ "administrative_area_level_4", "Mebusevleri" },
			{ "administrative_area_level_2", "Çankaya" },
			{ "administrative_area_level_1", "Ankara" },
			{ "country", "Turkey" },
			{ "postal_code", "06570" },
			{ "street_number", "108" },
			{ "plus_code", "WRGM+2W" },
		}
	};

	[Theory]
	[MemberData(nameof(ExpectedAvailableReverseGeocodes))]
	public async Task AllAvailableReverseGeocodes_Equivalent_To_Expected(Dictionary<string, object> expectedAllAvailableReverseGeocodes)
	{
		var sut = MockServiceWithValidResponse();
		var actualAllAvailableReverseGeocodes = await sut.AllAvailableReverseGeocodes(CoordinateFakes.Ankara(), "tr");
		actualAllAvailableReverseGeocodes.Should().BeEquivalentTo(expectedAllAvailableReverseGeocodes);
	}

	private GoogleMapsReverseGeocodeService MockServiceWithValidResponse()
	{
		var mockHttpClient = MockHttpClient.WithResponse(GoogleMapsReverseGeocodeResponseFakes.Ankara());
		var coordinateCacheMock = new Mock<CoordinateCache<GoogleMapsResponse>>();
		return new GoogleMapsReverseGeocodeService(mockHttpClient, ApiKeyStoreFakes.GoogleMapsValid(), NullLogger<GoogleMapsReverseGeocodeService>.Instance, coordinateCacheMock.Object);
	}

	private static ICoordinateCache<GoogleMapsResponse> MockCoordinateCache(Coordinate coordinateCacheKey, GoogleMapsResponse responseCacheResult)
	{
		var request =  new ReverseGeocodeRequest(coordinateCacheKey);

		var mockCoordinateCache = new Mock<ICoordinateCache<GoogleMapsResponse>>();
		mockCoordinateCache
			.Setup(x => x.TryGet(request, out It.Ref<GoogleMapsResponse?>.IsAny))
			.Returns((ReverseGeocodeRequest _, out GoogleMapsResponse? value) =>
			{
				value = responseCacheResult;
				return true;
			});

		return mockCoordinateCache.Object;
	}
}
