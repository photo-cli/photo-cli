using Moq.Protected;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace PhotoCli.Tests.UnitTests.Services.ReverseGeocodes;

public class BigDataCloudReverseGeocodeServiceUnitTests() : ReverseGeocodeServiceUnitTestBase<BigDataCloudResponse>(ReverseGeocodeProvider.BigDataCloud)
{
	public static TheoryData<List<int>, List<string>> WithGivenOptionsReverseGeocodeResultShouldBeGivenExpectedAddressData = new()
	{
		{ [2], ["Türkiye"] },
		{ [2, 4], ["Türkiye", "Ankara"] },
		{ [2, 4, 6], ["Türkiye", "Ankara", "Çankaya"] },
		{ [2, 4, 6, 8], ["Türkiye", "Ankara", "Çankaya", "Mebusevleri Mahallesi"] }
	};

	[Theory]
	[MemberData(nameof(WithGivenOptionsReverseGeocodeResultShouldBeGivenExpectedAddressData))]
	public async Task With_Given_Options_ReverseGeocode_Result_Should_Be_Given_Expected_Address_Data(List<int> adminLevels, List<string> expectedAddresses)
	{
		var sut = MockGeocodeService(BigDataCloudReverseGeocodeResponseFakes.Ankara());
		var reverseGeocodeResponseActual = await sut.Get(CoordinateFakes.Ankara(), PhotoFileValid, "tr", adminLevels);
		reverseGeocodeResponseActual.Should().BeEquivalentTo(new ReverseGeocodeAddressResult(expectedAddresses, true));
	}

	[Fact]
	public async Task Service_Error_Should_Give_Empty_List()
	{
		var mockHttpClient = MockHttpClient.WithError();
		var coordinateCacheMock = new Mock<IReverseGeocodeCache<BigDataCloudResponse>>();

		var sut = new BigDataCloudReverseGeocodeService(mockHttpClient, NullLogger<BigDataCloudReverseGeocodeService>.Instance, ApiKeyStoreFakes.BigDataCloudValid(),
			coordinateCacheMock.Object, StatisticsFakes.Empty());

		var reverseGeocodeResponseActual = await sut.Get(CoordinateFakes.Ankara(), PhotoFileValid, "tr", BigDataCloudAdminLevelsFakes.Valid());
		reverseGeocodeResponseActual.Should().BeEquivalentTo(new ReverseGeocodeAddressResult(ArraySegment<string>.Empty, false));
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
		var mockBigDataCloudResponseCache = ReverseGeocodeCacheMock(coordinate, cacheResponseFakeExpected);

		var sut = new BigDataCloudReverseGeocodeService(httpClientMock, NullLogger<BigDataCloudReverseGeocodeService>.Instance, ApiKeyStoreFakes.BigDataCloudValid(),
			mockBigDataCloudResponseCache, StatisticsFakes.Empty());

		var cacheResponseActual = await sut.SerializeFullResponse(new ReverseGeocodeRequest(coordinate));

		cacheResponseActual.Should().Be(cacheResponseFakeExpected);
		httpMessageHandlerMock.Protected().Verify("SendAsync", Times.Never(), ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>());
	}

	public static TheoryData<Coordinate, Coordinate[]> CacheMissData = new()
	{
		{
			new Coordinate(-89.1234567, -179.1234567),
			[
				new Coordinate(-89.1234568, -179.1234568)
			]
		},
		{
			new Coordinate(89.1234567, 179.1234567),
			[
				new Coordinate(89.1234566, 179.1234566),
				new Coordinate(45.1234566, 91.1234566)
			]
		},
	};

	[Theory]
	[MemberData(nameof(CacheMissData))]
	public async Task SerializeFullResponse_Should_Requested_From_HttpClient(Coordinate cacheCoordinate, Coordinate[] requestCoordinates)
	{
		var (httpMessageHandlerMock, httpClientMock) = MockHttpMessageHandler.WithResponse(BigDataCloudReverseGeocodeResponseFakes.Ankara());
		var mockBigDataCloudResponseCache = ReverseGeocodeCacheMock(cacheCoordinate, BigDataCloudFullResponseFakes.Valid(cacheCoordinate));

		var sut = new BigDataCloudReverseGeocodeService(httpClientMock, NullLogger<BigDataCloudReverseGeocodeService>.Instance, ApiKeyStoreFakes.BigDataCloudValid(),
			mockBigDataCloudResponseCache, StatisticsFakes.Empty());

		foreach (var requestCoordinate in requestCoordinates)
			_ = await sut.SerializeFullResponse(new ReverseGeocodeRequest(requestCoordinate));

		VerifyMockMessageHandlerSendRequestExactly(Times.Exactly(requestCoordinates.Length), httpMessageHandlerMock);
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
		var (sut, loggerMock) = MockServiceAndLoggerWithValidResponseNoCache(BigDataCloudReverseGeocodeResponseFakes.MultipleAdminLevel());
		await sut.AllAvailableReverseGeocodes(CoordinateFakes.Valid(), "tr");
		var logStatements = new[]
		{
			"BigDataCloud returned inconsistent/duplicate data. Multiple admin level on 3 found. Used value: First value on level3, duplicate value: Duplicate value on level3"
		};
		loggerMock.VerifyAllLogStatementsAtLeastOnce(LogLevel.Warning, true, logStatements);
	}

	public static TheoryData<List<int>, List<string>, string[]> RequestedAddressTypesSomeMissingWithValuesAndExpectedLogs = new()
	{
		{
			[100],
			[],
			[RequestedAddressLevelNotFoundLogStatement(100, 1)]
		},
		{
			[2, 101],
			["Türkiye"],
			[RequestedAddressLevelNotFoundLogStatement(101, 2)]
		},
		{
			[102, 4],
			["Ankara"],
			[RequestedAddressLevelNotFoundLogStatement(102, 1)]
		},
		{
			[2, 3, 4, 103, 6, 104, 8],
			["Türkiye", "İç Anadolu Bölgesi", "Ankara", "Çankaya", "Mebusevleri Mahallesi"],
			[
				RequestedAddressLevelNotFoundLogStatement(103, 4),
				RequestedAddressLevelNotFoundLogStatement(104, 6),
			]
		},
	};

	[Theory]
	[MemberData(nameof(RequestedAddressTypesSomeMissingWithValuesAndExpectedLogs))]
	public async Task Get_SomeRequestedAddressLevelsMissing_ShouldReturnAllPhotosHasReverseGeocodedAsRequestedAsFalseWithMatchWithAddressAndLogs(List<int> adminLevels,
		List<string> expectedAddresses, string[] expectedLogStatements)
	{
		await MockAndResponseShouldMatchWithExpectedReverseGeocodeAddressResultAndLogs(adminLevels, expectedAddresses, false, expectedLogStatements);
	}


	private BigDataCloudReverseGeocodeService MockGeocodeService(string responseMock)
	{
		var (sut, _) = MockServiceAndLoggerWithValidResponseNoCache(responseMock);
		return sut;
	}

	private async Task MockAndResponseShouldMatchWithExpectedReverseGeocodeAddressResultAndLogs(List<int> adminLevels, List<string> expectedAddresses,
		bool expectedAllPhotosHasReverseGeocodedAsRequested, string[]? expectedLogStatements = null)
	{
		var (sut, logger) = MockServiceAndLoggerWithValidResponseNoCache(BigDataCloudReverseGeocodeResponseFakes.Ankara());
		var reverseGeocodeResponseActual = await sut.Get(CoordinateFakes.Ankara(), PhotoFileValid, "en", adminLevels);

		using (new AssertionScope())
		{
			reverseGeocodeResponseActual.Should().BeEquivalentTo(new ReverseGeocodeAddressResult(expectedAddresses, expectedAllPhotosHasReverseGeocodedAsRequested));
			if (expectedLogStatements != null)
				logger.VerifyAllLogStatementsAtLeastOnce(LogLevel.Error, false, expectedLogStatements);
		}
	}

	private (BigDataCloudReverseGeocodeService, Mock<ILogger<BigDataCloudReverseGeocodeService>>) MockServiceAndLoggerWithValidResponseNoCache(string responseMock)
	{
		var httpClientMock = MockHttpClient.WithResponse(responseMock);
		var loggerMock = new Mock<ILogger<BigDataCloudReverseGeocodeService>>();

		var reverseGeocodeService = new BigDataCloudReverseGeocodeService(httpClientMock, loggerMock.Object, ApiKeyStoreFakes.BigDataCloudValid(),
			ReverseGeocodeCacheAlwaysMiss(), StatisticsFakes.Empty());

		return (reverseGeocodeService, loggerMock);
	}

	private static string RequestedAddressLevelNotFoundLogStatement(int adminLevel, int addressIndex)
	{
		return $"Requested address level: {adminLevel} on index #{addressIndex}, not found on BigDataCloudAdmin's response. " +
			   $"Available levels found: {BigDataCloudReverseGeocodeResponseFakes.AnkaraAdminLevelsLogOutput()}. " +
			   $"Path:<{PhotoPathValid}>";
	}
}
