using Moq.Protected;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace PhotoCli.Tests.UnitTests.Services.ReverseGeocodes;

public class BigDataCloudReverseGeocodeServiceUnitTests
{
	public static TheoryData<List<int>, List<string>> WithGivenOptionsReverseGeocodeResultShouldBeGivenExpectedAddressData = new()
	{
		{ new List<int> { 2 }, new List<string> { "Türkiye" } },
		{ new List<int> { 2, 4 }, new List<string> { "Türkiye", "Ankara" } },
		{ new List<int> { 2, 4, 6 }, new List<string> { "Türkiye", "Ankara", "Çankaya" } },
		{ new List<int> { 2, 4, 6, 8 }, new List<string> { "Türkiye", "Ankara", "Çankaya", "Mebusevleri Mahallesi" } }
	};

	[Theory]
	[MemberData(nameof(WithGivenOptionsReverseGeocodeResultShouldBeGivenExpectedAddressData))]
	public async Task With_Given_Options_ReverseGeocode_Result_Should_Be_Given_Expected_Address_Data(List<int> adminLevels, List<string> expectedAddresses)
	{
		var sut = MockGeocodeService(BigDataCloudReverseGeocodeResponseFakes.Ankara());
		var actualReverseGeocode = await sut.Get(CoordinateFakes.Ankara(), "tr", adminLevels);
		actualReverseGeocode.Should().BeEquivalentTo(expectedAddresses);
	}

	[Fact]
	public async Task Service_Error_Should_Give_Empty_List()
	{
		var mockHttpClient = MockHttpClient.WithError();
		var coordinateCacheMock = new Mock<CoordinateCache<BigDataCloudResponse>>();
		var sut = new BigDataCloudReverseGeocodeService(mockHttpClient, NullLogger<BigDataCloudReverseGeocodeService>.Instance, ApiKeyStoreFakes.BigDataCloudValid(), coordinateCacheMock.Object);
		var actualReverseGeocode = await sut.Get(CoordinateFakes.Ankara(), "tr", BigDataCloudAdminLevelsFakes.Valid());
		actualReverseGeocode.Should().BeEquivalentTo(ArraySegment<string>.Empty);
	}

	[Fact]
	public async Task Response_Serialization_Verify()
	{
		var sut = MockGeocodeService(BigDataCloudReverseGeocodeResponseFakes.Ankara());
		var request = new ReverseGeocodeRequest(CoordinateFakes.Ankara(), "tr");
		var bigDataCloudResponse = await sut.SerializeFullResponse(request);
		bigDataCloudResponse.Verify();
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

		var cacheResponseFakeExpected = BigDataCloudFullResponseFakes.Valid(coordinate);
		var mockBigDataCloudResponseCache = MockCoordinateCache(coordinate, cacheResponseFakeExpected);
		var sut = new BigDataCloudReverseGeocodeService(httpClientMock, NullLogger<BigDataCloudReverseGeocodeService>.Instance, ApiKeyStoreFakes.BigDataCloudValid(), mockBigDataCloudResponseCache);
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
		var (httpMessageHandlerMock, mockHttpClient) = MockHttpMessageHandler.WithResponse(BigDataCloudReverseGeocodeResponseFakes.Ankara());

		var mockBigDataCloudResponseCache = MockCoordinateCache(cacheCoordinate, BigDataCloudFullResponseFakes.Valid(cacheCoordinate));

		var sut = new BigDataCloudReverseGeocodeService(
			mockHttpClient,
			NullLogger<BigDataCloudReverseGeocodeService>.Instance,
			ApiKeyStoreFakes.BigDataCloudValid(),
			mockBigDataCloudResponseCache);

		foreach (var requestCoordinate in requestCoordinates)
			_ = await sut.SerializeFullResponse(new ReverseGeocodeRequest(requestCoordinate));

		httpMessageHandlerMock.Protected().Verify("SendAsync", Times.Exactly(requestCoordinates.Length), ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>());
	}

	public static TheoryData<Dictionary<string, object>> ExpectedAvailableReverseGeocodes = new()
	{
		new Dictionary<string, object>
		{
			{ "AdminLevel2", "Türkiye" },
			{ "AdminLevel3", "İç Anadolu Bölgesi" },
			{ "AdminLevel4", "Ankara" },
			{ "AdminLevel6", "Çankaya" },
			{ "AdminLevel8", "Mebusevleri Mahallesi" },
		}
	};

	[Theory]
	[MemberData(nameof(ExpectedAvailableReverseGeocodes))]
	public async Task AllAvailableReverseGeocodes_Equivalent_To_Expected(Dictionary<string, object> expectedAllAvailableReverseGeocodes)
	{
		var sut = MockGeocodeService(BigDataCloudReverseGeocodeResponseFakes.Ankara());
		var actualAllAvailableReverseGeocodes = await sut.AllAvailableReverseGeocodes(CoordinateFakes.Ankara(), "tr");
		actualAllAvailableReverseGeocodes.Should().BeEquivalentTo(expectedAllAvailableReverseGeocodes);
	}

	[Fact]
	public async Task AllAvailableReverseGeocodes_MultipleAdminLevelResponseFromService_ShouldLogWarningMessageOfInconsistentData()
	{
		var loggerMock = new Mock<ILogger<BigDataCloudReverseGeocodeService>>();
		var sut = MockGeocodeService(BigDataCloudReverseGeocodeResponseFakes.MultipleAdminLevel(), loggerMock.Object);
		await sut.AllAvailableReverseGeocodes(CoordinateFakes.Valid(), "tr");
		var logStatements = new[]
		{
			"BigDataCloud returned inconsistent/duplicate data. Multiple admin level on 3 found. Used value: First value on level3, duplicate value: Duplicate value on level3"
		};
		loggerMock.VerifyAllLogStatementsAtLeastOnce(LogLevel.Warning, logStatements);
	}

	private static BigDataCloudReverseGeocodeService MockGeocodeService(string response, ILogger<BigDataCloudReverseGeocodeService>? logger = null)
	{
		var mockHttpClient = MockHttpClient.WithResponse(response);
		logger ??= NullLogger<BigDataCloudReverseGeocodeService>.Instance;
		var coordinateCacheMock = new Mock<CoordinateCache<BigDataCloudResponse>>();
		return new BigDataCloudReverseGeocodeService(mockHttpClient, logger, ApiKeyStoreFakes.BigDataCloudValid(), coordinateCacheMock.Object);
	}

	private static ICoordinateCache<BigDataCloudResponse> MockCoordinateCache(Coordinate coordinateCacheKey, BigDataCloudResponse responseCacheResult)
	{
		var request =  new ReverseGeocodeRequest(coordinateCacheKey);

		var mockCoordinateCache = new Mock<ICoordinateCache<BigDataCloudResponse>>();
		mockCoordinateCache
			.Setup(x => x.TryGet(request, out It.Ref<BigDataCloudResponse?>.IsAny))
			.Returns((ReverseGeocodeRequest _, out BigDataCloudResponse? value) =>
			{
				value = responseCacheResult;
				return true;
			});

		return mockCoordinateCache.Object;
	}
}
